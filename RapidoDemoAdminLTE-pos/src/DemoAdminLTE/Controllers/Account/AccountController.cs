using Newtonsoft.Json;
using NLog;
using DemoAdminLTE.CustomAuthentication;
using DemoAdminLTE.DAL;
using DemoAdminLTE.Models;
using DemoAdminLTE.Utils;
using DemoAdminLTE.Extensions;
using DemoAdminLTE.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using DemoAdminLTE.Extensions.Alerts;
using ChangePasswordViewStrings = DemoAdminLTE.Resources.Views.ChangePasswordViews.Messages;
using ProfileViewStrings = DemoAdminLTE.Resources.Views.ProfileViews.Messages;
using LoginViewStrings = DemoAdminLTE.Resources.Views.LoginViews.Messages;
using RegistrationViewStrings = DemoAdminLTE.Resources.Views.RegistrationViews.Messages;

namespace DemoAdminLTE.Controllers
{
    //[AllowAnonymous]
    public class AccountController : BaseController
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        [HttpGet]
        public ActionResult Login(string ReturnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                return LogOut();
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginView loginView, string ReturnUrl = "")
        {
            if (ModelState.IsValid)
            {
                var userByPhone = CustomMembership.ValidateUserByPhone(loginView.Username, loginView.Password);
                var userByEmail = CustomMembership.ValidateUserByEmail(loginView.Username, loginView.Password);
                var isValidUser = Membership.ValidateUser(loginView.Username, loginView.Password);

                if (userByPhone != null && !isValidUser)
                {
                    isValidUser = Membership.ValidateUser(userByPhone.Username, loginView.Password);
                    if (isValidUser)
                        loginView.Username = userByPhone.Username;
                }

                if (userByEmail != null && !isValidUser)
                {
                    isValidUser = Membership.ValidateUser(userByEmail.Username, loginView.Password);
                    if (isValidUser)
                        loginView.Username = userByEmail.Username;
                }

                if (isValidUser)
                {
                    var user = (CustomMembershipUser)Membership.GetUser(loginView.Username, false);

                    if (user != null)
                    {
                        if (user.IsApproved && !user.IsLockedOut)
                        {
                            CustomSerializeModel userModel = new CustomSerializeModel()
                            {
                                UserId = user.UserId,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                RoleName = user.Role.RoleName,
                                Phone = user.Phone,
                                Email = user.Email,
                                CreationDate = user.CreationDate,
                                PermissionString = user.Permissions.Select(p => p.ToString()).ToList(),
                            };

                            string userData = JsonConvert.SerializeObject(userModel);
                            DateTime issueDate = DateTime.Now;
                            DateTime expirDate = issueDate.AddMinutes(15);
                            if (loginView.RememberMe)
                            {
                                expirDate = issueDate.AddYears(1);
                            }
                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, loginView.Username, issueDate, expirDate, false, userData);

                            string enTicket = FormsAuthentication.Encrypt(authTicket);
                            HttpCookie faCookie = new HttpCookie(CONST.COOKIE_AUTHENTICATION, enTicket);
                            Response.Cookies.Add(faCookie);

                            // update last login date
                            using (DemoContext dbContext = new DemoContext())
                            {
                                var dbUser = dbContext.Users.Find(user.UserId);
                                if (dbUser != null)
                                {
                                    dbUser.LastLoginDate = DateTime.Now;
                                    dbUser.LastActivityDate = DateTime.Now;
                                    dbContext.Entry(dbUser).State = EntityState.Modified;
                                    dbContext.SaveChanges();
                                }
                            }

                            Log.ToDatabase(user.UserId, "Login", "User login");
                            Alerts.AddSuccess(LoginViewStrings.LoginSuccess);

                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return RedirectToDefault();
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", LoginViewStrings.LoginFailure);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", LoginViewStrings.LoginInvalid);
                }
            }

            return View(loginView);
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(RegistrationView registrationView)
        {
            if (ModelState.IsValid)
            {
                // Email Verification  
                MembershipUser membershipUser = Membership.GetUser(registrationView.Username);
                if (membershipUser != null)
                {
                    ModelState.AddModelError("Username", RegistrationViewStrings.UsernameExisted);
                    return View(registrationView);
                }

                registrationView.Phone = registrationView.Phone.Replace("-", "").Replace(" ", "");
                string userNameByPhone = CustomMembership.GetUserNameByPhonenumber(registrationView.Phone);
                if (!string.IsNullOrEmpty(userNameByPhone))
                {
                    ModelState.AddModelError("Phone", RegistrationViewStrings.PhoneExisted);
                    return View(registrationView);
                }

                string userName = Membership.GetUserNameByEmail(registrationView.Email);
                if (!string.IsNullOrEmpty(userName))
                {
                    ModelState.AddModelError("Email", RegistrationViewStrings.EmailExisted);
                    return View(registrationView);
                }

                if (!registrationView.AgreeTheTerms)
                {
                    ModelState.AddModelError("AgreeTheTerms", RegistrationViewStrings.MustAgreeTheTerms);
                    return View(registrationView);
                }

                //Save User Data   
                using (DemoContext dbContext = new DemoContext())
                {
                    var defaultRole = dbContext.Roles.FirstOrDefault(o => o.RoleName == "User");
                    var defaultRoleId = defaultRole != null ? defaultRole.Id : 2;

                    var user = new User()
                    {
                        Username = registrationView.Username,
                        FirstName = registrationView.FirstName,
                        LastName = registrationView.LastName,
                        Phone = registrationView.Phone,
                        Email = registrationView.Email,
                        PasswordHash = Crypto.HashPassword(registrationView.Password),
                        ActivationCode = Guid.NewGuid(),
                        IsApproved = false,
                        RoleId = defaultRoleId
                    };

                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();

                    //Verification Email  
                    //VerificationEmail(registrationView.Email, registrationView.ActivationCode.ToString());

                    Log.ToDatabase(user.Id, "Registration", "New Registration");

                    Alerts.AddSuccess(RegistrationViewStrings.RegistrationSuccessTitle, ALERTS.ALALWAYS_SHOW);
                    Alerts.AddInfo(RegistrationViewStrings.RegistrationSuccess, ALERTS.ALALWAYS_SHOW);

                    return RedirectToAction("Login");
                }
            }

            return View(registrationView);
        }

        [HttpGet]
        public ActionResult Activation(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToBadRequest();
            }

            using (DemoContext dbContext = new DemoContext())
            {
                var userAccount = dbContext.Users.Where(u => u.ActivationCode.ToString().Equals(id)).FirstOrDefault();

                if (userAccount != null)
                {
                    userAccount.IsApproved = true;
                    dbContext.SaveChanges();

                    Alerts.AddSuccess(RegistrationViewStrings.ActivateAccountSuccess, ALERTS.ALALWAYS_SHOW);
                }
                else
                {
                    Alerts.AddError(RegistrationViewStrings.ActivateAccountFailure, ALERTS.ALALWAYS_SHOW);
                }

            }
            return RedirectToAction("Login");
        }

        public ActionResult LogOut()
        {
            CustomPrincipal user = (CustomPrincipal)User;
            if (user != null)
            {
                Log.ToDatabase(user.UserId, "Logout", "User logout");
            }
            HttpCookie cookie = new HttpCookie(CONST.COOKIE_AUTHENTICATION, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", null);
        }

        //[NonAction]
        //public void VerificationEmail(string email, string activationCode)
        //{
        //    var url = string.Format("/Account/Activation/{0}", activationCode);
        //    var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url);

        //    var fromEmail = new MailAddress("mehdi.rami2012@gmail.com", "Activation Account - AKKA");
        //    var toEmail = new MailAddress(email);

        //    var fromEmailPassword = "******************";
        //    string subject = "Activation Account !";

        //    string body = "<br/> Please click on the following link in order to activate your account" + "<br/><a href='" + link + "'> Activation Account ! </a>";

        //    var smtp = new SmtpClient
        //    {
        //        Host = "smtp.gmail.com",
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
        //    };

        //    using (var message = new MailMessage(fromEmail, toEmail)
        //    {
        //        Subject = subject,
        //        Body = body,
        //        IsBodyHtml = true

        //    })

        //        smtp.Send(message);

        //}

        [CustomAuthorize]
        public ActionResult Profiles()
        {
            var user = (CustomPrincipal)User;
            if (user == null)
            {
                return RedirectToBadRequest();
            }

            using (DemoContext dbContext = new DemoContext())
            {
                var dbUser = dbContext.Users.Find(user.UserId);

                if (dbUser == null)
                {
                    return RedirectToBadRequest();
                }

                return View(dbUser);
            }
        }

        [CustomAuthorize]
        [HttpPost]
        public ActionResult UpdateProfiles(ProfileView view)
        {
            var user = (CustomPrincipal)User;
            if (user == null)
            {
                return RedirectToBadRequest();
            }

            using (DemoContext dbContext = new DemoContext())
            {
                var dbUser = dbContext.Users.Find(user.UserId);

                if (dbUser != null)
                {
                    dbUser.FirstName = view.FirstName;
                    dbUser.LastName = view.LastName;
                    //dbUser.Phone = view.Phone.Replace("-","");
                    //dbUser.Email = view.Email;
                    dbUser.Comment = view.Comment;

                    dbContext.Entry(dbUser).State = EntityState.Modified;
                    dbContext.SaveChanges();

                    Alerts.AddSuccess(ProfileViewStrings.UpdateProfileSuccess);
                    return RedirectToAction("Profiles");
                }
            }

            Alerts.AddError(ProfileViewStrings.UpdateProfileFailure);
            return RedirectToAction("Profiles");
        }

        [CustomAuthorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordView view)
        {
            var user = (CustomPrincipal)User;
            if (user == null)
            {
                return RedirectToBadRequest();
            }

            using (DemoContext dbContext = new DemoContext())
            {
                var dbUser = dbContext.Users.Find(user.UserId);

                if (dbUser != null)
                {
                    if (Crypto.VerifyHashedPassword(dbUser.PasswordHash, view.OldPassword))
                    {
                        dbUser.PasswordHash = Crypto.HashPassword(view.NewPassword);

                        dbContext.Entry(dbUser).State = EntityState.Modified;
                        dbContext.SaveChanges();

                        Alerts.AddSuccess(ChangePasswordViewStrings.ChangePasswordSuccess);
                        return RedirectToAction("Profiles");
                    }
                }
            }

            Alerts.AddError(ChangePasswordViewStrings.ChangePasswordFailure);
            return RedirectToAction("Profiles");
        }
    }
}
