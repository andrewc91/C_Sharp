using System;
using Nancy;
using DbConnection;
namespace QuotingDojo
{
    public class QuotingDojoModule : NancyModule
    {
        public QuotingDojoModule()
        {
            Get("/", args => 
            {
                if (Session["error"] != null)
                {
                    ViewBag.IsError = true;
                    ViewBag.error = Session["error"];
                }
                return View["index"];
            });

            Get("/quotes", args =>
            {
                string query = "SELECT name, quote, DATE_FORMAT(created_at, '%r %M %D, %Y') AS created_at FROM userquotes";
                var data = DbConnector.ExecuteQuery(query);

                return View["quotes", data];
            });

            Post("/submit", args =>
            {
                string name = Request.Form.name;
                string quote = Request.Form.quote;

                if (name.Length == 0 || quote.Length == 0)
                {
                    Session["error"] = "Please do not leave either fields blank!";
                    return Response.AsRedirect("/");
                }

                string query = String.Format("INSERT INTO userquotes (name, quote, created_at) VALUES ('{0}', '{1}', NOW())", name, quote);
                DbConnector.ExecuteQuery(query);

                return Response.AsRedirect("/quotes");
            });
        }
    }
}