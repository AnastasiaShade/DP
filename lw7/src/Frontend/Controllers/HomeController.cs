﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Net.Http;
using System.Text;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public async Task<IActionResult> TextDetails(string id) 
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://127.0.0.1:5000/api/values/" + id);

            string result = (response.IsSuccessStatusCode)
                ? await response.Content.ReadAsStringAsync()
                : response.StatusCode.ToString();

            ViewData["Message"] = result;
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(string data) 
        { 
            string id = null; 
            
            if (data != null)
            {
                var httpClient = new HttpClient();
                var response = await httpClient.PostAsync("http://127.0.0.1:5000/api/values", 
                new FormUrlEncodedContent(new[] {new KeyValuePair<string, string>("", data)}));

                id = await response.Content.ReadAsStringAsync();            
            }

            return new RedirectResult("http://127.0.0.1:5001/Home/TextDetails?=" + id);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
