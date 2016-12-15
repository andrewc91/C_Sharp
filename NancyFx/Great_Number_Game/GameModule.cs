using System;
using Nancy;
namespace NumberGame
{
    public class GameModule : NancyModule
    {
        public GameModule()
        {
            Get("/", args => 
            {
                if (Session["RandomNum"] == null)
                {
                    Session["RandomNum"] = new Random().Next(1,101);
                    ViewBag.Correct = false;
                    ViewBag.result = "";
                }
                else {
                    ViewBag.RandomNum = Session["RandomNum"];
                }

                if (Session["RightNum"] != null)
                {
                    ViewBag.Correct = true;
                    ViewBag.result = Session["RightNum"];
                }

                else if (Session["WrongNum"] != null)
                {
                    ViewBag.Correct = false;
                    ViewBag.result = Session["WrongNum"];
                }
                return View["Game"];
            });

            Post("/submit", args => {
                if (Session["RandomNum"] != null) 
                {
                    int number = (int) Session["RandomNum"];
                    int guess = Request.Form.Number;

                    if (guess > number)
                    {
                        Session["WrongNum"] = "Too High!";
                    }

                    else if (guess < number)
                    {
                        Session["WrongNum"] = "Too Low!";
                    }
                    else 
                    {
                        Session["RightNum"] = String.Format("{0} was the number!", number);
                    }
                }
                return Response.AsRedirect("/");
            });

            Post("/reset", args => {
                Session.DeleteAll();
                return Response.AsRedirect("/");
            });
        }
    }
}