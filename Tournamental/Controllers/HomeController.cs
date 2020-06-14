using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Tournamental.Models;

namespace Tournamental.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            var cities = new List<City>();
            cities.Add(new City() { Title = "Paris", Lat = 48.855901, Lng = 2.349272 });
            cities.Add(new City() { Title = "Berlin", Lat = 52.520413, Lng = 13.402794 });
            cities.Add(new City() { Title = "Rome", Lat = 41.907074, Lng = 12.498474 });
            return View(cities);
        }


        [HttpPost]
        public JsonResult GetAnswer(string question)
        {
            int index = _rnd.Next(_db.Count);
            var answer = _db[index];
            return Json(answer);
        }

        private static Random _rnd = new Random();

        private static List<string> _db = new List<string> { "Yes", "No", "Definitely, yes", "I don't know", "Looks like, yes" };

        public ActionResult About()
        {
            ViewBag.Message = "Your about page.";

            return View();
        }
    }
}