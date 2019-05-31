using HouseholdBudgeterFrontEnd.Models;
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
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            var url = "http://localhost:50270/token";

            var userName = model.Email;
            var password = model.Password;
            var grantType = "password";

            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("username", userName));
            parameters.Add(new KeyValuePair<string, string>("password", password));
            parameters.Add(new KeyValuePair<string, string>("grant_type", grantType));

            var encodedValues = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedValues).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<LoginData>(data);
                var cookie = new HttpCookie("HBFrontEnd", result.AccessToken);

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
            Response.Cookies.Remove("HBFrontEnd");
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var url = "http://localhost:50270/api/account/register";

            var email = model.Email;
            var password = model.Password;
            var confirmPassword = model.ConfirmPassword;

            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("email", email));
            parameters.Add(new KeyValuePair<string, string>("password", password));
            parameters.Add(new KeyValuePair<string, string>("confirmPassword", confirmPassword));

            var encodedValues = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedValues).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return RedirectToAction(nameof(AccountController.Login));

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var url = "http://localhost:50270/api/account/ForgotPassword";

            var email = model.Email;

            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("email", email));

            var encodedValues = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedValues).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return View("ResetPassword"); 

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ResetPassword()
        {    
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var url = "http://localhost:50270/api/account/ResetPassword";

            var email = model.Email;
            var password = model.Password;
            var confirmPassword = model.Password;
            var code = model.Code;

            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("email", email));
            parameters.Add(new KeyValuePair<string, string>("password", password));
            parameters.Add(new KeyValuePair<string, string>("confirmPassword", confirmPassword));
            parameters.Add(new KeyValuePair<string, string>("code", code));

            var encodedValues = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedValues).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return View("Login");

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var url = "http://localhost:50270/api/account/ChangePassword";

            var oldPassword = model.OldPassword;
            var newPassword = model.NewPassword;
            var confirmPassword = model.ConfirmPassword;

            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("oldPassword", oldPassword));
            parameters.Add(new KeyValuePair<string, string>("newPassword", newPassword));
            parameters.Add(new KeyValuePair<string, string>("confirmPassword", confirmPassword));

            var encodedValues = new FormUrlEncodedContent(parameters);
            var cookie = Request.Cookies["HBFrontEnd"];

            if (cookie == null)
                return RedirectToAction("Login");

            var token = cookie.Value;
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = httpClient.PostAsync(url, encodedValues).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return View("Login");

            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }
}