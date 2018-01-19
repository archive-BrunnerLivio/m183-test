﻿using System;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using Pruefung_Praktisch_Musterloesung.Models;

namespace Pruefung_Praktisch_Musterloesung.Controllers
{
    public class Lab3Controller : Controller
    {

        /**
        * 1. Attacke:
        * 
        * HTML Injection
        * 
        * Man könnte z.B. ein JavaScript einbetten, welches
        * die eingaben loggt und an einen externen Server schickt
        * 
        * 2. Attacke:
        * Brute force
        * 
        * Script laufen lassen welches beim Login username und
        * Login submitted, bis man den Account hat.
        * */

        public ActionResult Index()
        {

            Lab3Postcomments model = new Lab3Postcomments();

            return View(model.getAllData());
        }

        public ActionResult Backend()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Comment()
        {
            var comment = Request["comment"];
            var postid = Int32.Parse(Request["postid"]);

            Lab3Postcomments model = new Lab3Postcomments();

            if (model.storeComment(postid, comment))
            {
                return RedirectToAction("Index", "Lab3");
            }
            else
            {
                ViewBag.message = "Failed to Store Comment";
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login()
        {
            var username = Request["username"];
            var password = Request["password"];

            Lab3User model = new Lab3User();

            // Pseudocode für Anti-Bruteforce

            this.db.Add(new UserLog()
            {
                username = username,
                ip = Request.UserHostAddress,
                time = DateTime.Now
            })
            this.db.SaveChanges();

            int attemptInLast5Min = this.db.UserLogs.Where(u => u.time <= DateTime.Now && u.time > DateTime.Now.AddMinutes(-5)).Count

                if( attemptInLast5Min > 5)
            {
                throw new Exception("Too many attempts");
            }

            if (model.checkCredentials(username, password))
            {
                return RedirectToAction("Backend", "Lab3");
            }
            else
            {
                ViewBag.message = "Wrong Credentials";
                return View();
            }
        }
    }
}