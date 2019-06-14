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
    public class TransactionController : Controller
    {
        private string url = "http://localhost:50270/api/transaction/";
        private string urlCategory = "http://localhost:50270/api/category/";

        [CheckAutorization]
        public ActionResult GetTransactions(int id)
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.Error = TempData["Error"];

            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.GetAsync(url + $"GetTransactions/{id}").Result;
            var data = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<TransactionsViewModel>(data);
                result.BankAccountId = id;

                return View(result);
            }

            return CheckError(response, null);
        }

        [CheckAutorization]
        public ActionResult Create(int id)
        {
                var model = new CreateEditTransactionViewModel
                {
                    Categories = PopulateCategoriesList(id),
                    TransactionDate = DateTime.Now
                };

                if (model.Categories.Any())
                    model.Categories.ToList()[0].Selected = true;
                else
                {
                    var messageError = "There are no category in the household";
                    TempData["Error"] = messageError;

                    return RedirectToAction("GetHouseholds", "Household");
                }

                return View(model);
        }

        [HttpPost]
        [CheckModelState]
        [CheckAutorization]
        public ActionResult Create(int id, CreateEditTransactionViewModel model)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.PostAsync(url + $"PostTransaction/{id}", TransactionEncode(model)).Result;

            if (!response.IsSuccessStatusCode)
                model.Categories = PopulateCategoriesList(id);  

            return CheckStatusCode(response, id, model);
        }

        private IEnumerable<SelectListItem> PopulateCategoriesList(int id)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.GetAsync(urlCategory + $"GetCategoriesBA/{id}").Result;
            var data = response.Content.ReadAsStringAsync().Result;
            var categories = new List<SelectListItem>();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<CategoryViewModel>>(data);
                categories = result.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList();
            }

            return categories;
        }

        [CheckAutorization]
        public ActionResult Edit(int id, int bankAccountId)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.GetAsync(url + $"GetTransaction/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<CreateEditTransactionViewModel>(data);
                result.BankAccountId = id;

                return View(result);
            }

            return CheckError(response, null);
        }

        [HttpPost]
        [CheckAutorization]
        [CheckModelState]
        public ActionResult Edit(int id, CreateEditTransactionViewModel model)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.PutAsync(url + $"PutTransaction/{id}", TransactionEncode(model)).Result;

            return CheckStatusCode(response, model.BankAccountId, model);
        }

        private FormUrlEncodedContent TransactionEncode (CreateEditTransactionViewModel model)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(new KeyValuePair<string, string>("title", model.Title));
            parameters.Add(new KeyValuePair<string, string>("description", model.Description));
            parameters.Add(new KeyValuePair<string, string>("ammount", model.Ammount.ToString()));
            parameters.Add(new KeyValuePair<string, string>("transactionDate", model.TransactionDate.ToString()));
            parameters.Add(new KeyValuePair<string, string>("categoryId", model.CategoryId.ToString()));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            return encodedParameters;
        }

        [HttpPost]
        [CheckAutorization]
        public ActionResult Delete(int id, int bankAccountId)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.DeleteAsync(url + $"deletetransaction/{id}").Result;

            return CheckStatusCode(response, bankAccountId, null);
        }

        [HttpPost]
        [CheckAutorization]
        public ActionResult VoidTransaction(int id, int bankAccountId, string transactionName)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.PostAsync(url + $"PostVoidTransaction/{id}", null).Result;

            if (response.IsSuccessStatusCode)
                TempData["Message"] = $"The transaction '{transactionName}' has been successfully voided!";

            return CheckStatusCode(response, bankAccountId, null);
        }

        public ActionResult CheckStatusCode(HttpResponseMessage response, int bankAccountId, CreateEditTransactionViewModel model)
        {
            if (response.IsSuccessStatusCode)
                return RedirectToAction("GetTransactions", new { id = bankAccountId });
            else
                return CheckError(response, model);
        }

        public ActionResult CheckError(HttpResponseMessage response, CreateEditTransactionViewModel model)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErrorData>(data);

                if ((result.ModelState != null) && (model != null))
                {
                    var modelErrors = result.ModelState.SingleOrDefault().Value;

                    foreach (var error in modelErrors)
                        ModelState.AddModelError("", error);

                    return View(model);
                }
                else
                {
                    TempData["Error"] = result.Message;

                    return RedirectToAction("GetHouseholds", "Household");
                }
            }

            return View("Error");
        }
    }
}