using System;
using System.Collections.Generic;
using Nancy;

namespace NinjaGold
{
    public class NinjaGoldModule : NancyModule
    {
        public Random random = new Random();
        public NinjaGoldModule()
        {
            Get("/", args => {
                if (Session["gold"] == null)
                {
                    Session["gold"] = 0;
                    Session["activities"] = new List<string>();
                }
                ViewBag["gold"] = Session["gold"];

                return View["index", Session["activities"]];
            });

            Post("/process_money", args => {
                string building = Request.Form.building;
                int gold = 0;

                switch (building)
                {
                    case "farm":
                        gold = new Random().Next(10, 21);
                        Session["gold"] = (int) Session["gold"] + gold;
                        break;
                    
                    case "cave":
                        gold = new Random().Next(5, 11);
                        Session["gold"] = (int) Session["gold"] + gold;
                        break;
                    
                    case "house":
                        gold = new Random().Next(2, 6);
                        Session["gold"] = (int) Session["gold"] + gold;
                        break;
                    
                    case "casino":
                        gold = new Random().Next(-50, 51);
                        Session["gold"] = (int) Session["gold"] + gold;
                        break;
                }

                var activity = Session["activities"] as List<string>;
                string time = DateTime.Now.ToString("yyyy/MM/dd hh:mm tt");

                if (gold >= 0)
                {
                    activity.Add($"Earned {gold} golds from the {building}! ({time})");
                }
                else
                {
                    activity.Add($"Entered a casino and lost {gold} golds...Ouch... ({time})");
                }
                return Response.AsRedirect("/");
            });
        }
    }
}