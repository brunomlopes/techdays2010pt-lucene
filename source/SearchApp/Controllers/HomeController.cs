﻿using System.Web.Mvc;

namespace SearchApp.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Search(string query)
        {
            ViewData["Message"] = "query : "+query;
            return View("Index");
        }

    }
}
