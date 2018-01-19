using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Web.Mvc;
using System.Linq;

namespace Pruefung_Praktisch_Musterloesung.Controllers
{
    public class Lab1Controller : Controller
    {
        /**
         * 
         * Overview:
         * 
         * Beim "type" parameter-query-string kann man eigene Pfade eingeben und somit
         * sensible daten anfordern
         * 
         * Beispielurl:
         * http://localhost:8080/lab1/index/?type=../../App_Data/
         * 
         * Es würde den path /Content/images/../../App_Data mit
         * Directory.GetFiles()-auflisten. Alle files werden im img-src
         * angezeigt.
         * 
         * ------
         * 
         * Detail:
         * Hier ebenfalls kann man den Ordner und sogar noch 
         * das file bestimmen mit dem type und file query parameter
         * 
         * Beispielurl:
         * http://localhost:8080/lab1/index/?type=../../App_Data&file=lab3.mdf
         * 
         * Es würde den Path Content/images/../../App_Data/lab3.mdf auflösen
         * welches die url der Datebank in den img-src anzeigen würde
         * 
         * */
        List<string> types = new List<string> { "bears", "elephants", "lions" };

        public ActionResult Index()
        {
            var type = Request.QueryString["type"].ToLower();
            

            if (string.IsNullOrEmpty(type))
            {
                type = "lions";
            }

            if (!types.Contains(type))
            {
                throw new ApplicationException("Sie dürfen diesen Typ nicht angeben");
            }

            var path = "~/Content/images/" + type;

            List<List<string>> fileUriList = new List<List<string>>();

            if (Directory.Exists(Server.MapPath(path)))
            {
                var scheme = Request.Url.Scheme;
                var host = Request.Url.Host;
                var port = Request.Url.Port;

                string[] fileEntries = Directory.GetFiles(Server.MapPath(path));
                foreach (var filepath in fileEntries)
                {
                    var filename = Path.GetFileName(filepath);
                    var imageuri = scheme + "://" + host + ":" + port + path.Replace("~", "") + "/" + filename;

                    var urilistelement = new List<string>();
                    urilistelement.Add(filename);
                    urilistelement.Add(imageuri);
                    urilistelement.Add(type);

                    fileUriList.Add(urilistelement);
                }
            }

            return View(fileUriList);
        }

        public ActionResult Detail()
        {
            var file = Request.QueryString["file"];
            var type = Request.QueryString["type"].ToLower();

            if (string.IsNullOrEmpty(file))
            {
                file = "Lion1.jpg";
            }
            if (string.IsNullOrEmpty(type))
            {
                file = "lions";
            }

            if (!types.Contains(type))
            {
                throw new ApplicationException("Sie dürfen diesen Typ nicht angeben");
            }

            var folderpath = "/Content/images/" + type;

            if(!Directory.GetFiles(folderpath).Contains(file))
            {
                throw new ApplicationException("Das File existiert nicht");
            }

            var relpath = "~/Content/images/" + type + "/" + file;

            List<List<string>> fileUriItem = new List<List<string>>();
            var path = Server.MapPath(relpath);

            if (System.IO.File.Exists(path))
            {
                var scheme = Request.Url.Scheme;
                var host = Request.Url.Host;
                var port = Request.Url.Port;
                var absolutepath = Request.Url.AbsolutePath;

                var filename = Path.GetFileName(file);
                var imageuri = scheme + "://" + host + ":" + port + "/Content/images/" + type + "/" + filename;

                var urilistelement = new List<string>();
                urilistelement.Add(filename);
                urilistelement.Add(imageuri);
                urilistelement.Add(type);

                fileUriItem.Add(urilistelement);
            }

            return View(fileUriItem);
        }
    }
}