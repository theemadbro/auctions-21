using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using beltfix.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace beltfix.Controllers
{
    public class LoginController : Controller
    {
        private beltfixContext _context;
        public LoginController(beltfixContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("testasdf")]
        public IActionResult Test()
        {
            return View("Test");
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            if(ret == null)
            {
                CurrentUser newcurr = new CurrentUser();
                newcurr.id = 0;
                ViewBag.CurrentUser = newcurr;
                List<object> temp = new List<object>();
                temp.Add(newcurr);
                HttpContext.Session.SetObjectAsJson("curr", temp);
                return View("./Login");
            }
            else if (ret.Count == 0)
            {
                ViewBag.CurrentUser = ret[0];
                return View("./Login");
            }
            else
            {
                ViewBag.CurrentUser = ret[0];
                return View("./Login");
            }
        }

        [HttpPost]
        [Route("login")]
        public IActionResult ProcessLogin(string inEmail, string inPass)
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            ViewBag.CurrentUser = ret[0];
            Users check = _context.users.SingleOrDefault(x => x.email == inEmail);
            if (check != null && inPass != null)
            {
                var hasher = new PasswordHasher<Users>();
                if(0 != hasher.VerifyHashedPassword(check, check.password, inPass))
                {
                    CurrentUser curr = new CurrentUser();
                    curr.id = check.id;
                    curr.name = check.first_name+" "+check.last_name;
                    curr.wallet = check.wallet;
                    ViewBag.CurrentUser = curr;
                    List<object> temp = new List<object>();
                    temp.Add(curr);
                    HttpContext.Session.SetObjectAsJson("curr", temp);
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    ViewBag.LogError = "The Email or Password was incorrect.";
                    return View("./Login");
                }
            }
            else 
            {
                ViewBag.LogError = "Missing Email or Password.";
                return View("Login");
            }
        }

        [HttpGet]
        [Route("register")]
        public IActionResult Register()
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            if(ret == null)
            {
                CurrentUser newcurr = new CurrentUser();
                newcurr.id = 0;
                ViewBag.CurrentUser = newcurr;
                List<object> temp = new List<object>();
                temp.Add(newcurr);
                HttpContext.Session.SetObjectAsJson("curr", temp);
                return View();
            }
            else if (ret.Count == 0)
            {
                ViewBag.CurrentUser = ret[0];
                return View();
            }
            else
            {
                ViewBag.CurrentUser = ret[0];
                return View();
            }
        }

        [HttpPost]
        [Route("register")]
        public IActionResult ProcessRegister(Users inp)
        {
            List<CurrentUser> ret = HttpContext.Session.GetObjectFromJson<List<CurrentUser>>("curr");
            ViewBag.CurrentUser = ret[0];
            if (ModelState.IsValid)
            {
                PasswordHasher<Users> Hasher = new PasswordHasher<Users>();
                inp.password = Hasher.HashPassword(inp, inp.password);
                inp.wallet = 1000;
                _context.Add(inp);
                _context.SaveChanges();
                Users check = _context.users.SingleOrDefault(x => x.email == inp.email);
                CurrentUser newcurr = new CurrentUser();
                newcurr.id = check.id;
                newcurr.name = check.first_name+" "+check.last_name;
                newcurr.wallet = check.wallet;
                List<object> temp = new List<object>();
                temp.Add(newcurr);
                HttpContext.Session.SetObjectAsJson("curr", temp);
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                return View("Register");
            }
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        




    }
}