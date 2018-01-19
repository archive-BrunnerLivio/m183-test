using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using Pruefung_Praktisch_Musterloesung.Models;

namespace Pruefung_Praktisch_Musterloesung.Controllers
{
    public class Lab2Controller : Controller
    {

        /**
        * 
        * 1. Attacke:
        * Session Fixation
        * Angreifer 1 geht auf http://localhost:8080/lab2/index
        * Im <form action> kann er die generierte SID auslesen.
        * 
        * Wenn er nun die adresse  http://localhost:8080/lab2/login?sid={GENERIERTE_SID}
        * weiter schickt an Opfer 1, kann er nachdem es sich eingeloggt hat mit dieser
        * URL auf den Account zugreifen
        * 
        * 2. Attacke:
        * Cross-Site-Tracing
        * 
        * Man kann z.B. ein Bild posten auf einer Platform welches
        * z.B. die Bank eines Opfers referenziert. Wenn das Opfer
        * eine Session hat bei der Bank, könnte man Transaktion darüber
        * ausführen, nur wenn der Benutzer das Bild ansieht
        * 
        * 
        * */

        public ActionResult Index() {

            v

            return View();
        }

        [HttpPost]
        public ActionResult Login()
        {
            var username = Request["username"];
            var password = Request["password"];

            // hints:
            //var used_browser = Request.Browser.Platform;
            //var ip = Request.UserHostAddress;

            Lab2Userlogin model = new Lab2Userlogin();

            if (model.checkCredentials(username, password))
            {
                var sessionid = Request.QueryString["sid"];

                if (string.IsNullOrEmpty(sessionid))
                {
                    var hash = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
                    sessionid = string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
                }

                ViewBag.sessionid = sessionid;
                model.storeSessionInfos(username, password, sessionid);

                HttpCookie c = new HttpCookie("sid");
                c.Expires = DateTime.Now.AddMonths(2);
                c.Value = sessionid;
                Response.Cookies.Add(c);

                return RedirectToAction("Backend", "Lab2");
            }
            else
            {
                ViewBag.message = "Wrong Credentials";
                return View();
            }
        }

        public ActionResult Backend()
        {
            var sessionid = "";

            if (Request.Cookies.AllKeys.Contains("sid"))
            {
                sessionid = Request.Cookies["sid"].Value.ToString();
            }           

            if (!string.IsNullOrEmpty(Request.QueryString["sid"]))
            {
                sessionid = Request.QueryString["sid"];
            }
            
            // hints:
            //var used_browser = Request.Browser.Platform;
            //var ip = Request.UserHostAddress;

            Lab2Userlogin model = new Lab2Userlogin();

            if (model.checkSessionInfos(sessionid))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Lab2");
            }              
        }
    }
}