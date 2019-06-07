using HouseholdBudgeterFrontEnd.Models;
using HouseholdBudgeterFrontEnd.Models.ActionFilters;
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
    public class CategoryController : Controller
    {
        private string url = "http://localhost:50270/api/category/";

        [CheckAutorization]
        public ActionResult Create(int id)
        {
            return View();
        }

        [HttpPost]
        [CheckModelState]
        [CheckAutorization]
        public ActionResult Create(int id, CreateEditCategoryViewModel model)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("name", model.Name));
            parameters.Add(new KeyValuePair<string, string>("description", model.Description));

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = httpClient.PostAsync(url + $"PostCategory/{id}", encodedParameters).Result;

            return CheckStatusCode(response, id);
        }

        [CheckAutorization]
        public ActionResult Edit(int id, int householdId)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;

            var response = httpClient.GetAsync(url + $"GetCategory/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<CreateEditCategoryViewModel>(data);

                return View(result);
            }

            return CheckError(response, householdId);
        }

        [HttpPost]
        [CheckAutorization]
        [CheckModelState]
        public ActionResult Edit(int id, CreateEditCategoryViewModel model)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(new KeyValuePair<string, string>("name", model.Name));
            parameters.Add(new KeyValuePair<string, string>("description", model.Description));

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = httpClient.PutAsync(url + $"PutCategory/{id}?householdId={model.CategoryHouseholdId}", encodedParameters).Result;

            return CheckStatusCode(response, model.CategoryHouseholdId);
        }

        [CheckAutorization]
        public ActionResult GetCategories(int id)
        {
            ViewBag.Error = TempData["Error"];

            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.GetAsync(url + $"GetCategories/{id}").Result;
            var data = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<CategoriesViewModel>(data);
                result.HouseholdId = id;
                
                return View(result);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var result = JsonConvert.DeserializeObject<APIErrorData>(data);
                string messageError = result.Message;
                TempData["Error"] = messageError;

               return RedirectToAction("GetHouseholds", "Household");
            }

            return View("Error");
        }

        [HttpPost]
        [CheckAutorization]
        public ActionResult Delete(int id, int householdId)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.DeleteAsync(url + $"deletecategory/{id}").Result;

            return CheckStatusCode(response, householdId);
        }

        public ActionResult CheckStatusCode(HttpResponseMessage response, int householdId)
        {
            if (response.IsSuccessStatusCode)
                return RedirectToAction("GetCategories", new { id = householdId });
            else
                return CheckError(response, householdId);
        }

        public ActionResult CheckError(HttpResponseMessage response, int householdId)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErrorData>(data);
                string messageError = result.Message;
                TempData["Error"] = messageError;

                return RedirectToAction("GetCategories", new { id = householdId });
            }

            return View("Error");
        }
    }
}