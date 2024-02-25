using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;

namespace WebProject.Controllers
{
    public class LoginController : Controller
    {
        
        public ActionResult Login()
        {
            ViewBag.Message = "";       
            return View();   
        }
        [HttpPost]
        public ActionResult Login(string username, string password)     
        {
            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];

            if (!users.ContainsKey(username))         
            {
                ViewBag.Message = "Unknown username. Try again";
                return View();
            }
            if(users[username].Password != password)      
            {
                ViewBag.Message = "Incorrect password. Try again";
                return View();
            }

            if (users[username].LoginBlocked)       
            {
                ViewBag.Message = "User is blocked.";
                return View();
            }

            Session["LoggedUser"] = username;
            return RedirectToAction("Index", "Home");       
        }

        public ActionResult Register()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];
            if(users.ContainsKey(user.Username))
            {
                ViewBag.Message = "Username taken. Try another."; 
                return View();
            }
            if(DateTime.Parse(user.DateOfBirth) > DateTime.Now.AddYears(-15))           
            {
                ViewBag.Message = "Minimum required age is 15.";
                return View();
            }
            user.GroupTrainings = new List<GroupTraining>();     
            users.Add(user.Username, user);
            HttpContext.Application["Users"] = users;           
            XML.AddUser(user);

            Session["LoggedUser"] = user.Username;            
            return RedirectToAction("Index","Home");       
        }

        public ActionResult LogOut()
        {
            Session["LoggedUser"] = null;    
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ModifyUser()       
        {
           
            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];
            string username = Session["LoggedUser"] as string;
            ViewBag.Message = "";

            return View(users[username]);                   
        }

        [HttpPost]                
        public ActionResult ModifyUser(User user)
        {
            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];
            string username = Session["LoggedUser"] as string;

            if (DateTime.Parse(user.DateOfBirth) > DateTime.Now.AddYears(-15))           
            {
                ViewBag.Message = "Minimum required age is 15.";
                return View(users[username]);
            }

            User oldUser = users[username];                  
            user.Username = username;                    
            user.GroupTrainings = oldUser.GroupTrainings;
            user.FitnessCentre = oldUser.FitnessCentre;
            user.FitnessCentres = oldUser.FitnessCentres;
            user.LoginBlocked = false;          
            user.UserRole = oldUser.UserRole;

            XML.UpdateUser(username, user);
            users[username] = user;       
            HttpContext.Application["Users"] = users;

            ViewBag.Message = "";
            return View(users[username]);       
        }

    }
}