using HouseholdBudgeterFrontEnd.Models;
using HouseholdBudgeterFrontEnd.Models.Domain;
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
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create( CreateHouseholdViewModel model)
        {
            var url = "http://localhost:50270/api/household/PostHousehold";
            var name = model.Name;
            var description = model.Description;

            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("name", name));
            parameters.Add(new KeyValuePair<string, string>("description", description));

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var cookie = Request.Cookies["HBFrontEnd"];

            if (cookie == null)
                return RedirectToAction("Login");

            var token = cookie.Value;
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = httpClient.PostAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return RedirectToAction(nameof(HouseholdController.GetHouseholds));
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErrorData>(data);

                return View();
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public ActionResult Edit (int id)
        {
            var url = $"http://localhost:50270/api/household/GetHousehold/{id}";

            var httpClient = new HttpClient();
            var cookie = Request.Cookies["HBFrontEnd"];

            if (cookie == null)
                return RedirectToAction("Login");

            var token = cookie.Value;
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");


            var response = httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<CreateHouseholdViewModel>(data);
                return View(result);
            }

            return View("GetHouseholds");
        }

        [HttpPost]
        public ActionResult Edit(int id, CreateHouseholdViewModel model)
        {
            var url = $"http://localhost:50270/api/household/PutHousehold/{id}";
            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(new KeyValuePair<string, string>("name", model.Name));
            parameters.Add(new KeyValuePair<string, string>("description", model.Description));

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var cookie = Request.Cookies["HBFrontEnd"];

            if (cookie == null)
                return RedirectToAction("Login");

            var token = cookie.Value;
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = httpClient.PutAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(HouseholdController.GetHouseholds));
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErrorData>(data);

                return View();
            }

            return View();
        }

        public ActionResult GetHouseholds()
        {
            var url = $"http://localhost:50270/api/household/GetAll";
            var httpClient = new HttpClient();
            var cookie = Request.Cookies["HBFrontEnd"];

            if (cookie == null)
                return RedirectToAction("Login");

            var token = cookie.Value;
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<List<Household>>(data);

                return View(result);
            }

            return View();
        }


        public ActionResult GetUsers(int id)
        {
            var url = $"http://localhost:50270/api/household/GetParticipants/{id}";

            var httpClient = new HttpClient();
            var cookie = Request.Cookies["HBFrontEnd"];

            if (cookie == null)
                return RedirectToAction("Login");

            var token = cookie.Value;
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");


            var response = httpClient.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<List<ParticipantViewModel>>(data);
                return View(result);
            }

            return View("GetHouseholds");
        }

        public ActionResult Invite(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Invite(int id, InviteViewModel model)
        {
            var url = $"http://localhost:50270/api/household/invite/{id}?email={model.Email}";
            var httpClient = new HttpClient();

            var cookie = Request.Cookies["HBFrontEnd"];

            if (cookie == null)
                return RedirectToAction("Login");

            var token = cookie.Value;
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = httpClient.PostAsync(url, null).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return RedirectToAction(nameof(HouseholdController.GetHouseholds));
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErrorData>(data);

                return View();
            }

            return View();
        }

        public ActionResult Join()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Join(int id)
        {
            var url = $"http://localhost:50270/api/household/postjoin/joiningHouseholdId={id}";
            var httpClient = new HttpClient();

            var cookie = Request.Cookies["HBFrontEnd"];

            if (cookie == null)
                return RedirectToAction("Login");

            var token = cookie.Value;
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = httpClient.PostAsync(url, null).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return RedirectToAction(nameof(HouseholdController.GetHouseholds));
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErrorData>(data);

                return View();
            }

            return View();
        }

        [HttpPost]
        public ActionResult Leave(int id)
        {
            var url = $"http://localhost:50270/api/household/postleave/{id}";
            var httpClient = new HttpClient();

            var cookie = Request.Cookies["HBFrontEnd"];

            if (cookie == null)
                return RedirectToAction("Login");

            var token = cookie.Value;
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = httpClient.PostAsync(url, null).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(HouseholdController.GetHouseholds));
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErrorData>(data);
                ModelState.AddModelError("", result.ToString());
                return GetHouseholds();
            }

            return RedirectToAction(nameof(HouseholdController.GetHouseholds));
        }

    }
}