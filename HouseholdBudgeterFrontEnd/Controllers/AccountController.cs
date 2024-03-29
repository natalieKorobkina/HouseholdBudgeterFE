﻿using HouseholdBudgeterFrontEnd.Models;
using HouseholdBudgeterFrontEnd.Models.ActionFilters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace HouseholdBudgeterFrontEnd.Controllers
{
    public class AccountController : Controller
    {
        private string url = "http://localhost:50270/api/account/";
        
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [CheckModelState]
        public ActionResult Login(LoginViewModel model)
        {
            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("username", model.Email));
            parameters.Add(new KeyValuePair<string, string>("password", model.Password));
            parameters.Add(new KeyValuePair<string, string>("grant_type", "password"));

            var encodedValues = new FormUrlEncodedContent(parameters);
            var response = httpClient.PostAsync("http://localhost:50270/token", encodedValues).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<LoginData>(data);
                var cookie = new HttpCookie("HBFrontEnd");
                cookie.Values.Add("token", result.AccessToken);
                cookie.Values.Add("username", model.Email);
                Response.Cookies.Add(cookie);

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            if (Request.Cookies["HBFrontEnd"] != null)
            {
                var cookie = new HttpCookie("HBFrontEnd");

                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            };

            return RedirectToAction("Login", "Account");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckModelState]
        public ActionResult Register(RegisterViewModel model)
        {
            var httpClient = new HttpClient();
            var parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(new KeyValuePair<string, string>("email", model.Email));
            parameters.Add(new KeyValuePair<string, string>("password", model.Password));
            parameters.Add(new KeyValuePair<string, string>("confirmPassword", model.ConfirmPassword));

            var encodedValues = new FormUrlEncodedContent(parameters);
            var response = httpClient.PostAsync(url+ "register", encodedValues).Result;

            return CheckError(response, "Login");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckModelState]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            var httpClient = new HttpClient();
            var frontUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("ResetPassword");
            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("email", model.Email));

            var encodedValues = new FormUrlEncodedContent(parameters);
            var response = httpClient.PostAsync(url + $"ForgotPassword?frontUrl={frontUrl}", encodedValues).Result;
 
            return CheckError(response, "ForgotPasswordConfirmation");
        }

        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public ActionResult ResetPassword(string code)
        {
            if (code == null)
            {
                ModelState.AddModelError(string.Empty, "Try again");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckModelState]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("email", model.Email));
            parameters.Add(new KeyValuePair<string, string>("password", model.Password));
            parameters.Add(new KeyValuePair<string, string>("confirmPassword", model.Password));
            parameters.Add(new KeyValuePair<string, string>("code", model.Code));

            var encodedValues = new FormUrlEncodedContent(parameters);
            var response = httpClient.PostAsync(url + "ResetPassword", encodedValues).Result;

            return CheckError(response, "Login");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckAutorization]
        [CheckModelState]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        { 
            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("oldPassword", model.OldPassword));
            parameters.Add(new KeyValuePair<string, string>("newPassword", model.NewPassword));
            parameters.Add(new KeyValuePair<string, string>("confirmPassword", model.ConfirmPassword));

            var encodedValues = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url + "ChangePassword", encodedValues).Result;

            return CheckError(response, "Login");
        }

        public ActionResult CheckError(HttpResponseMessage response, string actionName)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return RedirectToAction(actionName);

            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErrorData>(data);
                var modelErrors = result.ModelState.SingleOrDefault().Value;

                if(modelErrors != null && modelErrors.Any())
                    foreach (var error in modelErrors)
                        ModelState.AddModelError("", error);
                else
                {
                    string messageError = result.Message;
                    ViewBag.Error = messageError;
                }
                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("ErrorAccount");
            }

            return View("ErrorAccount");
        }
    }
}