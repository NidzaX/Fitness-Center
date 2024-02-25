using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;

namespace WebProject.Controllers
{
    public class OwnerController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RegisterTrainer()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public ActionResult RegisterTrainer(User user)                    
        {
            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];
            if (users.ContainsKey(user.Username))
            {
                ViewBag.Message = "Username taken. Try another.";
                return View();
            }
            if (DateTime.Parse(user.DateOfBirth) > DateTime.Now.AddYears(-18))
            {
                ViewBag.Message = "Minimum required age is 18.";
                return View();
            }
            user.GroupTrainings = new List<GroupTraining>();
            user.UserRole = UserRole.Trainer;    
            users.Add(user.Username, user);
            HttpContext.Application["Users"] = users;
            XML.AddUser(user);
            return View();
        }

        public ActionResult CreateFitnessCentre()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public ActionResult CreateFitnessCentre(FitnessCentre fitnessCentre)
        {
            List<FitnessCentre> centres = HttpContext.Application["FitnessCentresCopy"] as List<FitnessCentre>;         
            if (centres.Any(x => x.CenterName == fitnessCentre.CenterName))
            {
                ViewBag.Message = "Fitness centre with given name already exists. Try another name.";
                return View();
            }

            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];
            string username = Session["LoggedUser"] as string;
            fitnessCentre.OwnerUsername = username;
            users[username].FitnessCentres.Add(fitnessCentre);       
            HttpContext.Application["Users"] = users;

            centres.Add(fitnessCentre);              
            HttpContext.Application["FitnessCentresCopy"] = centres;

            XML.UpdateUser(username, users[username]);    
            return View();
        }

        [HttpPost]
        public ActionResult ModifyCentrePrepare(string centreName)          
        {

            List<FitnessCentre> centres = HttpContext.Application["FitnessCentresCopy"] as List<FitnessCentre>;
            FitnessCentre fitnessCentre = centres.Find(x => x.CenterName == centreName);
            HttpContext.Application["ModifyCentre"] = fitnessCentre;
            ViewBag.Message = "";
            return View("ModifyFitnessCentre");
        }

        [HttpPost]
        public ActionResult ModifyFitnessCentre(FitnessCentre fitnessCentre)
        {
            List<FitnessCentre> centres = HttpContext.Application["FitnessCentresCopy"] as List<FitnessCentre>;
            string username = Session["LoggedUser"] as string;
            fitnessCentre.OwnerUsername = username;                 
            FitnessCentre oldCentre = HttpContext.Application["ModifyCentre"] as FitnessCentre;           
            fitnessCentre.CenterName = oldCentre.CenterName;
            centres.RemoveAll(x => x.CenterName == fitnessCentre.CenterName);
            centres.Add(fitnessCentre);
            HttpContext.Application["FitnessCentresCopy"] = centres;

            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];

            users[username].FitnessCentres.RemoveAll(x => x.CenterName == fitnessCentre.CenterName);
            users[username].FitnessCentres.Add(fitnessCentre);

            HttpContext.Application["Users"] = users;

            XML.UpdateUser(username, users[username]);
            return RedirectToAction("MyCentres");
        }

        public ActionResult MyCentres()
        {
            ViewBag.Message = "";
            string username = Session["LoggedUser"] as string;
            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];


            return View(users[username].FitnessCentres.FindAll(x => x.Deleted == false));
        }


        [HttpPost]
        public ActionResult DeleteCentre(string centreName)
        {
            string username = Session["LoggedUser"] as string;
            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];


            Dictionary<string, GroupTraining> trainings = HttpContext.Application["GroupTrainings"] as Dictionary<string, GroupTraining>;
            foreach (var training in trainings.Values)
            {
                if (training.TimeOfTraining > DateTime.Now && training.FitnessCenter == centreName)
                {
                    ViewBag.Message = "Delete failed. Fitness centre has existing future trainings.";
                    return View("MyCentres", users[username].FitnessCentres.FindAll(x => x.Deleted == false)); 
                }
            }
            foreach (var fitnesCentre in users[username].FitnessCentres)        
            {
                if (fitnesCentre.CenterName == centreName)
                {

                    fitnesCentre.Deleted = true;
                    break;
                }
            }

            XML.UpdateUser(username, users[username]);       

            foreach (var trainer in users.Values)                    
            {
                if (trainer.FitnessCentre == centreName)
                {
                    trainer.LoginBlocked = true;
                    XML.UpdateUser(trainer.Username, trainer);
                }
            }

            HttpContext.Application["Users"] = users;    
            return RedirectToAction("MyCentres");
        }

        public ActionResult FitnessCentreTrainers()
        {
            string username = Session["LoggedUser"] as string;
            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];
            List<User> trainers = new List<User>();

            foreach (var centre in users[username].FitnessCentres)     
            {
                if (!centre.Deleted)     
                {
                    foreach (var trainer in users.Values)           
                    {
                        if (trainer.FitnessCentre == centre.CenterName)        
                            trainers.Add(trainer);
                    }
                }
            }


            return View(trainers);
        }

        [HttpPost]
        public ActionResult BlockTrainer(string username)
        {
            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];
            users[username].LoginBlocked = true;             
            XML.UpdateUser(username, users[username]);   
            HttpContext.Application["Users"] = users;    
            return RedirectToAction("FitnessCentreTrainers");
        }
        public ActionResult Comments()
        {
            CommentList comments = HttpContext.Application["Comments"] as CommentList;
            return View(comments.Comments);
        }

        [HttpPost]
        public ActionResult DeclineComment(string commentId)
        {
            CommentList comments = HttpContext.Application["Comments"] as CommentList;
            var index = comments.Comments.FindIndex(x => x.Id == commentId);
            comments.Comments[index].Declined = true;
            HttpContext.Application["Comments"] = comments;
            XML.AddAndUpdateComment(comments);           
            return RedirectToAction("Comments");
        }

        [HttpPost]
        public ActionResult ApproveComment(string commentId)        
        {
            CommentList comments = HttpContext.Application["Comments"] as CommentList;
            var index = comments.Comments.FindIndex(x => x.Id == commentId);
            comments.Comments[index].Approved = true;
            HttpContext.Application["Comments"] = comments;
            XML.AddAndUpdateComment(comments);           
            return RedirectToAction("Comments");
        }

      

    }
}