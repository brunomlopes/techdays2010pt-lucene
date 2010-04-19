﻿using System.Web.Mvc;
using Common;
using Lucene.Net.Analysis.Br;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Util;

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

        public ActionResult Search(string query)
        {
            ViewData["Message"] = "query : " + query;

            var searcher = new IndexSearcher(Configuration.IndexDirectory);

            var fieldsToSearchIn = new[] {Configuration.Fields.Name, Configuration.Fields.Description};
            var queryanalizer = new MultiFieldQueryParser(Version.LUCENE_CURRENT,
                                                            fieldsToSearchIn,
                                                          new BrazilianAnalyzer());

            var numberOfResults = 10;
            var top10Results = searcher.Search(queryanalizer.Parse(query), numberOfResults);
            
            return View("Index");
        }
    }
}
