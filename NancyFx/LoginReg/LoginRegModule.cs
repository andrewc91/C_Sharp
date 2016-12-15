using Nancy;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DbConnection;
using CryptoHelper;
namespace LoginReg
{
    public class LoginRegModule : NancyModule
    {
        public LoginRegModule()
        {
            Get("/", args => 
            {
                if (Session["user"] != null ) {
                    return Response.AsRedirect("/");
                }

                List<string> logErrors = null;

                if (Session["logErrors"] != null)
                {
                    logErrors = Session["logErrors"] as List<string>;
                    Session.Delete("logErrors");
                }

                if (logErrors != null && logErrors.Count > 0)
                {
                    ViewBag.logIsError = true;
                }

                return View["index", logErrors];
            });


            Get("/register", args =>
            {
                if( Session["user"] != null ) {
                    return Response.AsRedirect("/");
                }

                List<string> regErrors = null;
                if (Session["regErrors"] != null)
                {
                    regErrors = Session["regErrors"] as List<string>;
                    Session.Delete("regErrors"); 
                }
                
                if (regErrors != null && regErrors.Count > 0)
                {
                    ViewBag.regIsError = true;
                }

                return View["register", regErrors];
            });

            
            Get("/success", args => {
                if( Session["user"] == null ) {
                    return Response.AsRedirect("/");
                }
                return View["success"];
            });


            Post("/login", args =>
            {
                if (Session["user"] != null) {
                    return Response.AsRedirect("/");
                }

                string email = Request.Form.email;
                string password = Request.Form.password;

                string query = String.Format("SELECT id, password FROM users WHERE email='{0}'", email);
                var data = DbConnector.ExecuteQuery(query);

                List<string> logErrors = new List<string>();

                if (email == null || email.Length < 2)
                {
                    logErrors.Add("Please enter a email address");
                }

                if (password == null || password.Length < 8)
                {
                    logErrors.Add("Please enter a password");
                }

                else if (!Crypto.VerifyHashedPassword((string)data[0]["password"], password))
                {
                    logErrors.Add("Password is incorrect. Please try again");
                }

                Session.Delete("logErrors");
                Session.Delete("isError");


                if (logErrors.Count > 0)
                {
                    Session["isError"] = true;
                    Session["logErrors"] = logErrors;
                    Response.AsRedirect("/");
                }
                    
                else
                {
                    Session["user"] = data[0]["id"];
                    return Response.AsRedirect("/success");
                }

                return Response.AsRedirect("/");
            });

            Post("/register", args =>
            {
                if (Session["user"] != null)
                {
                    return Response.AsRedirect("/");
                } 

                string firstName = Request.Form.firstName;
                string lastName = Request.Form.lastName;
                string email = Request.Form.email;
                string password = Request.Form.password;
                string confirm = Request.Form.confirm;

                List<string> regErrors = new List<string>();

                if (firstName.Length < 2)
                {
                    regErrors.Add("First Name must be at least 2 characters");
                }

                else if (Regex.IsMatch(firstName, @"^[a-zA-Z]+$") == false)
                {
                    regErrors.Add("First Name must contain letters only");
                }

                if (lastName.Length < 2)
                {
                    regErrors.Add("Last Name must be at least 2 characters");
                }

                else if (Regex.IsMatch(lastName, @"^[a-zA-Z]+$") == false)
                {
                    regErrors.Add("Last Name must contain letters only");
                }

                if (email.Length < 2)
                {
                    regErrors.Add("Please provide a email address");
                }

                else if (Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$") == false)
                {
                    regErrors.Add("Email must have valid format");
                }

                if (password.Length < 8)
                {
                    regErrors.Add("Password must be at least 8 characters");
                }

                if (password != confirm)
                {
                    regErrors.Add("Passwords do not match you stupid idiot");
                }

                if (regErrors.Count > 0)
                {
                    Session["regErrors"] = regErrors;
                    Response.AsRedirect("/register");
                }

                else 
                {
                    string EncryptedString = Crypto.HashPassword(password);
                    string query = String.Format("INSERT INTO users (first_name, last_name, email, password) VALUES ('{0}', '{1}', '{2}', '{3}')", firstName, lastName, email, EncryptedString);
                    DbConnector.ExecuteQuery(query);
                    Response.AsRedirect("/success");
                }
                return Response.AsRedirect("/register");
            });


            Post("/logout", args => 
            {
                Session.Delete("user");
                return Response.AsRedirect("/");
            });
        }
    }
}