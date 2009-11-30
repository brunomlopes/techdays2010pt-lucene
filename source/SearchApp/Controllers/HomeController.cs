using System.IO;
using System.Web.Mvc;
using Common;
using Lucene.Net.Analysis.Br;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Util;
using SearchApp.Models;
using System.Collections.Generic;

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

            var searcher = new IndexSearcher(
                new Lucene.Net.Store.SimpleFSDirectory(new DirectoryInfo(Configuration.IndexDirectory)),
                readOnly: true);

            var fieldsToSearchIn = new[] {Configuration.Fields.Name, Configuration.Fields.Description};
            var queryanalizer = new MultiFieldQueryParser(Version.LUCENE_CURRENT,
                                                          fieldsToSearchIn,
                                                          new BrazilianAnalyzer());

            var numberOfResults = 10;
            var top10Results = searcher.Search(queryanalizer.Parse(query), numberOfResults);
            var docs = new List<DocumentViewModel>();
            foreach (var scoreDoc in top10Results.scoreDocs)
            {
                var document = searcher.Doc(scoreDoc.doc);
                var name = document.GetField(Configuration.Fields.Name).StringValue();
                var description = document.GetField(Configuration.Fields.Description).StringValue();
                var link = document.GetField(Configuration.Fields.Link).StringValue();
                docs.Add(new DocumentViewModel(name, description, link));
            }
            return View(new SearchViewModel(docs));
        }

       

        
    }
}
