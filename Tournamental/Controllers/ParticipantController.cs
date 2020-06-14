using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using Tournamental.Models;

namespace Tournamental.Controllers
{
    public class ParticipantController : Controller
    {
        private TournamentsEntities db = new TournamentsEntities();

        public ActionResult Index()
        {
            var participant = db.Participant.Include(p => p.Tournament1).Include(p => p.User1);
            return View(participant.ToList());
        }


        [Authorize]
        public ActionResult Create(int tournamentID)
        {
            ViewBag.ID = tournamentID;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LicenceNumber,CurrentRanking")] Participant participant, int tournamentID)
        {
            if (ModelState.IsValid)
            {
                #region check for user
                var user = db.User.Where(s => s.EmailID == User.Identity.Name).FirstOrDefault();

                if (user != null)
                {
                    participant.User = user.UserID;
                }
                else
                {
                    ViewBag.Message = "User not found";
                    ViewBag.ID = tournamentID;
                    return View(participant);
                }
                #endregion

                #region check for tournament
                var tournament = db.Tournament.Where(s => s.Id == tournamentID).FirstOrDefault();
                if (tournament != null)
                {
                    participant.Tournament = tournament.Id;
                }
                else
                {
                    ViewBag.Message = "Tournament not found";
                    ViewBag.ID = tournamentID;
                    return View(participant);
                }
                #endregion

                #region check if licence unique in tournament
                var licence = db.Participant.Where(s =>
                        (s.LicenceNumber == participant.LicenceNumber &&
                         s.Tournament == participant.Tournament)).FirstOrDefault();
                if (licence != null)
                {
                    ViewBag.Message = "Licence is not unique for this tournament";
                    ViewBag.ID = tournamentID;
                    return View(participant);
                }
                else
                {
                    // not found, licence is unique
                }
                #endregion

                #region check if current ranking is unique for this tournament
                var rank = db.Participant.Where(s =>
                        (s.CurrentRanking == participant.CurrentRanking &&
                         s.Tournament == participant.Tournament)).FirstOrDefault();
                if (rank != null)
                {
                    ViewBag.Message = "Current ranking is not unique for this tournament";
                    ViewBag.ID = tournamentID; 
                    return View(participant);
                }
                else
                {
                    // not found, rank is unique
                }
                #endregion

                #region check if not missed deadline
                if (DateTime.Compare(tournament.ApplicationDeadline, DateTime.Now.Date) < 0)
                {
                    ViewBag.Message = "You can't sign in, because application was closed!";
                    ViewBag.ID = tournamentID; 
                    return View(participant);
                }
                #endregion

                #region check if max participants not reached
                int participantCount = db.Participant.Count(s => s.Tournament == tournamentID);
                if (tournament.MaxParticipant <= participantCount)
                {
                    ViewBag.Message = "Maximum number of participants reached. Better luck next time!";
                    ViewBag.ID = tournamentID;
                    return View(participant);
                }
                #endregion

                db.Participant.Add(participant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID = tournamentID;
            return View(participant);
        }
        
        public ActionResult MyTournaments()
        {
            var participant = db.Participant.Include(p => p.Tournament1).Include(p => p.User1);

            participant = participant.Where(s => s.User1.EmailID == User.Identity.Name);
            
            return View(participant.ToList());
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
