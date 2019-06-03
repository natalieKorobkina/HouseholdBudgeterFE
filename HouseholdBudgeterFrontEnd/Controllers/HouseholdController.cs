using HouseholdBudgeterFrontEnd.Models;
using HouseholdBudgeterFrontEnd.Models.ActionFilters;
using HouseholdBudgeterFrontEnd.Models.ViewModel;
using HouseholdBudgeterFrontEnd.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace HouseholdBudgeterFrontEnd.Controllers
{
    public class HouseholdController : Controller
    {
        private string url = "http://localhost:50270/api/household/";

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [CheckModelState]
        [CheckAutorization]
        public ActionResult Create(CreateHouseholdViewModel model)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("name", model.Name));
            parameters.Add(new KeyValuePair<string, string>("description", model.Description));

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = httpClient.PostAsync(url + "PostHousehold", encodedParameters).Result;

            return CheckStatusCode(response, "GetHouseholds");
        }

        [CheckAutorization]
        public ActionResult Edit(int id)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;

            var response = httpClient.GetAsync(url + $"GetHousehold/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<CreateHouseholdViewModel>(data);
                return View(result);
            }

            return CheckError(response);
        }

        [HttpPost]
        [CheckAutorization]
        [CheckModelState]
        public ActionResult Edit(int id, CreateHouseholdViewModel model)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient; 
            var parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(new KeyValuePair<string, string>("name", model.Name));
            parameters.Add(new KeyValuePair<string, string>("description", model.Description));

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = httpClient.PutAsync(url + $"PutHousehold/{id}", encodedParameters).Result;

            return CheckStatusCode(response, "Edit");
        }

        [CheckAutorization]
        public ActionResult GetHouseholds()
        {
            ViewBag.Error = TempData["Error"];

            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.GetAsync(url + "GetAll").Result;

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<List<HouseholdViewModel>>(data);

                return View(result);
            }

            return CheckError(response);
        }

        [HttpPost]
        [CheckAutorization]
        public ActionResult Leave(int id)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.PostAsync(url + $"postleave/{id}", null).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetHouseholds");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErrorData>(data);
                string messageError = result.Message;
                TempData["Error"] = messageError;

                return RedirectToAction("GetHouseholds");
            }

            return View("Error");
        }


        [CheckAutorization]
        public ActionResult GetUsers(int id, string householdName)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.GetAsync(url + $"GetParticipants/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<List<ParticipantViewModel>>(data);
                return View(result);
            }

            return CheckError(response);
        }

        public ActionResult Invite(int id, string householdName)
        {
            if (TempData["Message"] != null)
                ViewBag.Message = TempData["Message"].ToString();

            return View();
        }

        [HttpPost]
        [CheckAutorization]
        [CheckModelState]
        public ActionResult Invite(int id, string householdName, InviteViewModel model)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.PostAsync(url + $"invite/{id}?email={model.Email}", null).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = $"The invitation was sent to user with email {model.Email}";

                return RedirectToAction("Invite", new { id=id, householdName=householdName});
            }

            return CheckError(response);
        }

        [CheckAutorization]
        public ActionResult InvitingHouseholds()
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.GetAsync(url + "GetInvitingHouseholds").Result;

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<List<HouseholdViewModel>>(data);

                return View(result);
            }

            return CheckError(response);
        }

        [HttpPost]
        [CheckAutorization]
        public ActionResult InvitingHouseholds(int id)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.PostAsync(url + $"postjoin?joiningHouseholdId={id}", null).Result;

            return CheckError(response);
        }

        public ActionResult CheckStatusCode(HttpResponseMessage response, string actionName)
        {
            if (response.IsSuccessStatusCode)
                return RedirectToAction(actionName);
            else
                return CheckError(response);
        }

        public ActionResult CheckError(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErrorData>(data);
                string messageError = result.Message;

                ViewBag.Error = messageError;
                  
                return View();
            }

            return View("Error");
        }
    }
}