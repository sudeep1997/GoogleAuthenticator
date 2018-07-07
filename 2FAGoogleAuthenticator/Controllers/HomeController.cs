using _2FAGoogleAuthenticator.ViewModel;
using Google.Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _2FAGoogleAuthenticator.Controllers
{
    public class HomeController : Controller
    {
        private const string key = "qaz123!@@)(*";
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Login(LoginModel login)
        {
            string message = "";
            bool status = false;

            if(login.Username=="Admin" && login.Password=="Password1")
            {
                status = true;
                message = "2FA Verification";
                Session["Username"] = login.Username;

                TwoFactorAuthenticator tfa=new TwoFactorAuthenticator();
                string UserUniqueKey = login.Username + key;
                Session["UserUniqueKey"] = UserUniqueKey;
                var setupInfo = tfa.GenerateSetupCode("Security",login.Username,UserUniqueKey,300,300);
                ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                ViewBag.SetupCode = setupInfo.ManualEntryKey;
            }
            else
            {
                message = "Invalid Credential";
            }
            ViewBag.Message = message;
            ViewBag.Status = status;
            return View();
        }

        public ActionResult MyProfile()
        {
            if(Session["Username"] == null || Session["IsValid2FA"] == null || !(bool)Session["IsValid2FA"])
            {
                return RedirectToAction("Login");
            }
            ViewBag.Message = "Welcome" + Session["Username"].ToString();
            return View();
        }

        public ActionResult Verify2FA()
        {
            var token = Request["passcode"];
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            string UserUniqueKey = Session["UserUniqueKey"].ToString();
            bool isValid = tfa.ValidateTwoFactorPIN(UserUniqueKey, token);
            if(isValid)
            {
                Session["IsValid2FA"] = true;
                return RedirectToAction("MyProfile", "Home");
            }
            return RedirectToAction("Login", "Home");
        }
    }
}