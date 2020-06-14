using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PagedList;
using Tournamental.Models;

namespace Tournamental.Controllers
{
    public class TournamentController : Controller
    {
        private TournamentsEntities db = new TournamentsEntities();

        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            var tournaments = db.Tournament.ToList();
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            };
            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                tournaments = tournaments.Where(s => s.Name.Contains(searchString)).ToList();
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (TempData["shortMessage"] != null)
            {
                ViewBag.Message = TempData["shortMessage"];
                TempData.Remove("shortMessage");
            }
            return View(tournaments.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournament tournament = db.Tournament.Find(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Discipline,Time,Location,MaxParticipant,ApplicationDeadline,SponsorLogo,RankedPlayers")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                #region get user ID
                var v = User.Identity.Name;
                var user = db.User.Where(s => s.EmailID == v).FirstOrDefault();
                if (user != null)
                {
                    tournament.Organizer = user.UserID;
                } 
                else
                {
                    ViewBag.Message = "You must be logged in to create tournament!";
                    return View(tournament);
                }
                #endregion

                #region date comparison
                if (DateTime.Compare(tournament.Time, tournament.ApplicationDeadline) < 0)
                {
                    ViewBag.Message = "Deadline can't be after start date!";
                    return View(tournament);
                }
                #endregion

                #region date after today 
                if (DateTime.Compare(tournament.Time, DateTime.Now.Date) < 0)
                {
                    ViewBag.Message = "Date can't be in the past";
                    return View(tournament);
                }
                #endregion

                #region player numbers comparison
                if (tournament.RankedPlayers > tournament.MaxParticipant)
                {
                    ViewBag.Message = "Number of participant must be at least as big as ranked players." +
                                "(Max number of participants >= Number of ranked players)";
                    return View(tournament);
                }
                #endregion

                db.Tournament.Add(tournament);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tournament);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournament tournament = db.Tournament.Find(id);

            if (tournament == null)
            {
                return HttpNotFound();
            }

            #region check if user can edit
            var user = db.User.Where(s => s.UserID == tournament.Organizer).FirstOrDefault();
            
            if (user == null || !user.EmailID.Equals(User.Identity.Name))
            {
                TempData["shortMessage"] = "You need to be organizer to edit the tournament!";
                return RedirectToAction("Index");
            }
            #endregion

            return View(tournament);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Discipline,Organizer,Time,Location,MaxParticipant,ApplicationDeadline,SponsorLogo,RankedPlayers")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tournament).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tournament);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournament tournament = db.Tournament.Find(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tournament tournament = db.Tournament.Find(id);
            db.Tournament.Remove(tournament);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
