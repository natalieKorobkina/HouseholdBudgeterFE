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

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var result = JsonConvert.DeserializeObject<APIErrorData>(data);
                string messageError = result.Message;
                TempData["Error"] = messageError;

                return RedirectToAction("GetBankAccounts", "BankAccount");
            }

            return View("Error");
        }

        [CheckAutorization]
        public ActionResult Create(int id)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.GetAsync(urlCategory + $"GetCategoriesBA/{id}").Result;
            var data = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<CategoryViewModel>>(data);
                var model = new CreateEditTransactionViewModel
                {
                    Categories = result.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList(),
                    TransactionDate = DateTime.Now
                };

                return View(model);
            }
            return View("Error");
        }

        [HttpPost]
        [CheckModelState]
        [CheckAutorization]
        public ActionResult Create(int id, CreateEditTransactionViewModel model)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.PostAsync(url + $"PostTransaction/{id}", TransactionEncode(model)).Result;

            return CheckStatusCode(response, id);
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

            return CheckError(response, bankAccountId);
        }

        [HttpPost]
        [CheckAutorization]
        [CheckModelState]
        public ActionResult Edit(int id, CreateEditTransactionViewModel model)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.PutAsync(url + $"PutTransaction/{id}", TransactionEncode(model)).Result;

            return CheckStatusCode(response, model.BankAccountId);
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

            return CheckStatusCode(response, bankAccountId);
        }

        [HttpPost]
        [CheckAutorization]
        public ActionResult VoidTransaction(int id, int bankAccountId)
        {
            var httpClient = HttpContext.Items["httpClient"] as HttpClient;
            var response = httpClient.PostAsync(url + $"PostVoidTransaction/{id}", null).Result;

            return CheckStatusCode(response, bankAccountId);
        }

        public ActionResult CheckStatusCode(HttpResponseMessage response, int bankAccountId)
        {
            if (response.IsSuccessStatusCode)
                return RedirectToAction("GetTransactions", new { id = bankAccountId });
            else
                return CheckError(response, bankAccountId);
        }

        public ActionResult CheckError(HttpResponseMessage response, int bankAccountId)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErrorData>(data);
                string messageError = result.Message;
                TempData["Error"] = messageError;

                return RedirectToAction("GetBankAccounts", new { id = bankAccountId });
            }

            return View("Error");
        }
    }
}