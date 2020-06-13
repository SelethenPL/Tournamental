using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tournamental.Models;

namespace Tournamental.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified, ActivationCode, VerificationSendTime")] User user)
        {
            bool status = false;
            string message = null;

            // Model Validation
            if (ModelState.IsValid)
            {
                #region If email already exists
                bool exists = IsEmailExisting(user.EmailID);
                if (exists)
                {
                    ModelState.AddModelError("EmailExist", "Email already exists");

                }
                #endregion

                #region Generate activation code
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region Password hashing
                user.Password = Cryptography.Hash(user.Password);
                user.ConfirmPassword = Cryptography.Hash(user.ConfirmPassword);
                #endregion

                user.IsEmailVerified = false;
                #region Save to db
                using (TournamentsEntities dc = new TournamentsEntities())
                {
                    user.VerificationSendTime = DateTime.Now;
                    dc.User.Add(user);
                    dc.SaveChanges();

                    SendEmailVerificationLink(user.EmailID, user.ActivationCode.ToString());
                    message = "Registration successful. Account activation link has been sent.";
                    status = true;
                }
                #endregion
            }
            else
            {
                message = "Invalid request";
            }
            ViewBag.Status = status;
            ViewBag.Message = message;
            return View(user);
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool status = false;
            using (TournamentsEntities dc = new TournamentsEntities())
            {
                // To avoid confirm password not match issue
                dc.Configuration.ValidateOnSaveEnabled = false;
                var v = dc.User.Where(s => s.ActivationCode == new Guid(id)).FirstOrDefault();
                
                if (v != null)
                {
                    if (DateTime.Now <= v.VerificationSendTime.AddHours(24.0))
                    {
                        v.IsEmailVerified = true;
                        dc.SaveChanges();
                        status = true;
                    } 
                    else
                    {
                        ViewBag.Message = "Link expired, try again";
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid request";
                }
            }
            ViewBag.Status = status;
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string returnUrl="")
        {
            string message;
            using (TournamentsEntities dc = new TournamentsEntities())
            {
                var v = dc.User.Where(s => s.EmailID == login.EmailID).FirstOrDefault();
                if (v != null)
                {
                    if (string.Compare(Cryptography.Hash(login.Password), v.Password) == 0)
                    {
                        int timeout = login.RememberMe ? 420 : 20;
                        var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
                        string encrypt = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypt)
                        {
                            Expires = DateTime.Now.AddMinutes(timeout),
                            HttpOnly = true
                        };

                        Response.Cookies.Add(cookie);

                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                     } 
                    else
                    {
                        message = "Invalid credential";
                    }
                }
                else
                {
                    message = "Invalid credentials!";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }


        [NonAction]
        public bool IsEmailExisting(string emailID)
        {
            using (TournamentsEntities dc = new TournamentsEntities())
            {
                var v = dc.User.Where(s => s.EmailID == emailID).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public void SendEmailVerificationLink(string emailID, string activationCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("tournamentalservice@gmail.com", "Tournaments Service");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "2mWRQCbJdcP2zrH";
            string subject = "Your account in tournament service!";

            string body = "<br/>Click link below to verify account at our tournament service."
                + "<a href='" + link + "'>" + link + "</a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            smtp.Send(message);
        }
    
    
        // Forgot Password - part3

        public ActionResult ForgotPassword()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            string message = "";
            bool status = false;

            using (TournamentsEntities dc = new TournamentsEntities())
            {
                var account = dc.User.Where(s => s.EmailID == EmailID).FirstOrDefault();
                if (account != null)
                {
                    // Send email
                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            ViewBag.Status = status;
            return View();
        }
    }

}