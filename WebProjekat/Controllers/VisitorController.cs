using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;

namespace WebProject.Controllers
{
    public class VisitorController : Controller
    {
        [HttpPost]
        public ActionResult SignUp(string trainingId)
        {
            Dictionary<string,GroupTraining> groupTrainings = HttpContext.Application["GroupTrainings"] as Dictionary<string, GroupTraining>;  

            GroupTraining training = groupTrainings[trainingId];         
            string username = Session["LoggedUser"] as string;

            Dictionary<string, User> users = HttpContext.Application["Users"] as Dictionary<string, User>;   

            if (users[username].GroupTrainings.Any(x=>x.TrainingId == trainingId))                   
            {
                HttpContext.Application["Error"] = "You have already signed up for this training session.";                    
                return RedirectToAction("DetailsView","Home");         
            }
            if (training.Visitors.Count() == training.AllowedVisitors)
            {
                HttpContext.Application["Error"] = "Maximum number of allowed visitors already signed up.";
                return RedirectToAction("DetailsView","Home");
            }
            groupTrainings[trainingId].Visitors.Add(username);                                
            XML.UpdateTraining(trainingId, groupTrainings[trainingId]);   
            foreach(var user in users.Values)
            {
                if(user.GroupTrainings.Any(x=>x.TrainingId == trainingId))                  
                {
                    user.GroupTrainings.RemoveAll(x => x.TrainingId == trainingId);
                    user.GroupTrainings.Add(groupTrainings[trainingId]);
                    XML.UpdateUser(user.Username, user);
                }
            }
            users[username].GroupTrainings.Add(groupTrainings[trainingId]);       
            XML.UpdateUser(username, users[username]);
            return RedirectToAction("Index","Home");
        }

        public ActionResult PastTrainings()    
        {
            string username = Session["LoggedUser"] as string;
            Dictionary<string, User> users = HttpContext.Application["Users"] as Dictionary<string, User>;
            List<GroupTraining> pastTrainings = users[username].GroupTrainings.Where(x => x.TimeOfTraining < DateTime.Now).ToList();
            return View(pastTrainings);
        }

        [HttpPost]
        public ActionResult Sort(string sortValue)   
        {
            string username = Session["LoggedUser"] as string;
            Dictionary<string, User> users = HttpContext.Application["Users"] as Dictionary<string, User>;
            List<GroupTraining> pastTrainings = users[username].GroupTrainings.Where(x => x.TimeOfTraining < DateTime.Now).ToList();
            if (sortValue.Equals("nameAscending"))
            {
                pastTrainings = pastTrainings.OrderBy(x => x.TrainingName).ToList();
                return View("PastTrainings", pastTrainings);
            }
            else if (sortValue.Equals("typeAscending"))
            {
                pastTrainings = pastTrainings.OrderByDescending(x => x.TrainingType).ToList();                         
                return View("PastTrainings", pastTrainings);
            }
            else if (sortValue.Equals("timeAscending"))
            {
                pastTrainings = pastTrainings.OrderBy(x => x.TimeOfTraining).ToList();
                return View("PastTrainings", pastTrainings);
            }
            else if (sortValue.Equals("nameDescending"))
            {
                pastTrainings = pastTrainings.OrderByDescending(x => x.TrainingName).ToList();
                return View("PastTrainings", pastTrainings);
            }
            else if (sortValue.Equals("typeDescending"))
            {
                pastTrainings = pastTrainings.OrderBy(x => x.TrainingType).ToList();                         
                return View("PastTrainings", pastTrainings);
            }
            else
            {
                pastTrainings = pastTrainings.OrderByDescending(x => x.TimeOfTraining).ToList();
                return View("PastTrainings", pastTrainings);
            }
        }

        [HttpPost]
        public ActionResult SearchParameters(string name, string trainingType, string centreName)
        {
            string username = Session["LoggedUser"] as string;
            Dictionary<string, User> users = HttpContext.Application["Users"] as Dictionary<string, User>;
            List<GroupTraining> pastTrainings = users[username].GroupTrainings.Where(x => x.TimeOfTraining < DateTime.Now).ToList();
            
            if (name != "")
            {
                foreach (var training in pastTrainings.ToList())
                {
                    if (!training.TrainingName.Contains(name))
                    {
                        pastTrainings.Remove(training);
                    }
                }
            }

            if (trainingType != "trainingType")
            {
                Enum.TryParse(trainingType, out TrainingType trainingTypeOut);
                foreach (var training in pastTrainings.ToList())
                {
                    if (training.TrainingType != trainingTypeOut)
                    {
                        pastTrainings.Remove(training);
                    }
                }

            }

            if (centreName != "")
            {
                foreach (var training in pastTrainings.ToList())
                {
                    if (!training.TrainingName.Contains(name))
                    {
                        pastTrainings.Remove(training);
                    }
                }
            }


            return View("PastTrainings", pastTrainings);
        }

        public ActionResult AddComment()
        {
            string username = Session["LoggedUser"] as string;
            Dictionary<string, User> users = HttpContext.Application["Users"] as Dictionary<string, User>;
            HashSet<string> centres = new HashSet<string>();                     
            foreach(var training in users[username].GroupTrainings.FindAll(x=>x.TimeOfTraining < DateTime.Now))        
            {
                centres.Add(training.FitnessCenter);
            }
            return View(centres.ToList());
        }
        [HttpPost]
        public ActionResult AddComment(Comment comment)
        {
            CommentList comments = HttpContext.Application["Comments"] as CommentList;                         
            Comment newComment = new Comment();                 
            newComment.FitnessCenter = comment.FitnessCenter;      
            newComment.Grade = comment.Grade;
            newComment.VisitorsComment = comment.VisitorsComment;
            string username = Session["LoggedUser"] as string;
            newComment.Visitor = username;
            comments.Comments.Add(newComment);
            XML.AddAndUpdateComment(comments);         
            return RedirectToAction("Index", "Home");
        }


    }
}