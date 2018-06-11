using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Frontend.Controllers
{
    public class StatisticsController : Controller
    {
        public async Task<IActionResult> Statistics() 
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://127.0.0.1:5000/api/values/statistics");

            string result = (response.IsSuccessStatusCode)
                ? await response.Content.ReadAsStringAsync()
                : response.StatusCode.ToString();
            
            var splitedResult = Regex.Split(result, ":");
            ViewData["TextNum"] = "Общее количество всех обработанных текстов: " + splitedResult[0];
            ViewData["HighRankPart"] = "Количество текстов с высокой оценкой (выше 0.5) : " + splitedResult[1];
            ViewData["AvgRank"] = "Средняя оценка: " + splitedResult[2];
            return View();
        }
    }
}
