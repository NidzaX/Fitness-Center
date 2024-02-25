using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;

namespace WebProject.Controllers
{
    public class TrainerController : Controller
    {
       
        public ActionResult CreateTraining()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public ActionResult CreateTraining(GroupTraining training)
        {
            string username = Session["LoggedUser"] as string;      
            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];        

            training.FitnessCenter = users[username].FitnessCentre;        
            training.TrainingId = string.Format($"{training.FitnessCenter}.{training.TrainingName}");                     
            training.Visitors = new List<string>();   

            Dictionary<string, GroupTraining> groupTrainings = HttpContext.Application["GroupTrainings"] as Dictionary<string, GroupTraining>;
            if (groupTrainings.Values.Any(x=>x.TrainingId == training.TrainingId))       
            {
                ViewBag.Message = "Fitness centre already has training with given name. Try another";
                return View();
            }

            if(training.TimeOfTraining < DateTime.Now.AddDays(3))             
            {
                ViewBag.Message = "Training must be at least 3 days in advance.";
                return View();
            }
            

            users[username].GroupTrainings.Add(training);              
            HttpContext.Application["Users"] = users;
            XML.UpdateUser(username, users[username]);    

            groupTrainings.Add(training.TrainingId, training);           
            HttpContext.Application["GroupTrainings"] = groupTrainings;
            XML.AddGroupTraining(training);
            return View();
        }

        public ActionResult FutureTrainings()
        {
            ViewBag.Message = "";
            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];
            string username = Session["LoggedUser"] as string;
            return View(users[username].GroupTrainings.FindAll(x => x.TimeOfTraining > DateTime.Now && x.Deleted == false));                        
        }


        public ActionResult PrepareModify(string trainingId)
        {
            Dictionary<string,GroupTraining> groupTrainings = HttpContext.Application["GroupTrainings"] as Dictionary<string, GroupTraining>;
            GroupTraining training = groupTrainings[trainingId];
            ViewBag.Message = "";
            return View("ModifyTraining",training); 
            
        }

        [HttpPost]
        public ActionResult DeleteTraining(string trainingId)
        {
            Dictionary<string, GroupTraining> groupTrainings = HttpContext.Application["GroupTrainings"] as Dictionary<string, GroupTraining>;

            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];
            string username = Session["LoggedUser"] as string;

            if (groupTrainings[trainingId].Visitors.Count > 0)
            {
                ViewBag.Message = "Cannot delete training.";
                
                return View("FutureTrainings",users[username].GroupTrainings.FindAll(x => x.TimeOfTraining > DateTime.Now && x.Deleted == false));                      
            }
            groupTrainings[trainingId].Deleted = true;
            XML.UpdateTraining(trainingId, groupTrainings[trainingId]);
            users[username].GroupTrainings.RemoveAll(x => x.TrainingId == trainingId);
            users[username].GroupTrainings.Add(groupTrainings[trainingId]);            
            XML.UpdateUser(username, users[username]);
            HttpContext.Application["Users"] = users;


            return RedirectToAction("FutureTrainings");
        }

        [HttpPost]
        public ActionResult ModifyTraining(GroupTraining training)
        {
            Dictionary<string, GroupTraining> groupTrainings = HttpContext.Application["GroupTrainings"] as Dictionary<string, GroupTraining>;
            GroupTraining oldTraining = groupTrainings[training.TrainingId];
            
            

            if(training.TrainingName != oldTraining.TrainingName)                 
            {
                string trainingId = string.Format($"{training.FitnessCenter}.{training.TrainingName}");
                if (groupTrainings.Values.Any(x => x.TrainingId == trainingId))
                {
                    ViewBag.Message = "Fitness centre already has training with given name. Try another";
                    return View(oldTraining);
                }
                training.TrainingId = trainingId;                
            }

            
            string username = Session["LoggedUser"] as string;      
            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];        
            
            training.Visitors = oldTraining.Visitors;  


            foreach (var user in users.Values)                    
            {
                user.GroupTrainings.RemoveAll(x => x.TrainingId == oldTraining.TrainingId);                  
                user.GroupTrainings.Add(training);
                XML.UpdateUser(username, users[username]);
            }
            HttpContext.Application["Users"] = users;
            

            groupTrainings[oldTraining.TrainingId] = training;     
            HttpContext.Application["GroupTrainings"] = groupTrainings;
            XML.UpdateTraining(oldTraining.TrainingId,training);   
            return RedirectToAction("FutureTrainings");
        }

        public ActionResult PastTrainings()
        {
            string username = Session["LoggedUser"] as string;
            Dictionary<string, User> users = HttpContext.Application["Users"] as Dictionary<string, User>;
            List<GroupTraining> pastTrainings = users[username].GroupTrainings.Where(x => x.TimeOfTraining < DateTime.Now && x.Deleted == false).ToList();
            return View(pastTrainings);
        }

        [HttpPost]
        public ActionResult Sort(string sortValue)
        {
            string username = Session["LoggedUser"] as string;
            Dictionary<string, User> users = HttpContext.Application["Users"] as Dictionary<string, User>;
            List<GroupTraining> pastTrainings = users[username].GroupTrainings.Where(x => x.TimeOfTraining < DateTime.Now && x.Deleted == false).ToList();
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
        public ActionResult SearchParameters(string name, string trainingType, string minTime, string maxTime)                
        {
            string username = Session["LoggedUser"] as string;
            Dictionary<string, User> users = HttpContext.Application["Users"] as Dictionary<string, User>;
            List<GroupTraining> pastTrainings = users[username].GroupTrainings.Where(x => x.TimeOfTraining < DateTime.Now && x.Deleted == false).ToList();
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

            if (minTime != "")
            {
                DateTime min = DateTime.Parse(minTime);
                foreach(var training in pastTrainings.ToList())
                {
                    if(training.TimeOfTraining < min)       
                    {
                        pastTrainings.Remove(training);
                    }
                }
            }

            if (maxTime != "")
            {
                DateTime max = DateTime.Parse(maxTime);
                foreach (var training in pastTrainings.ToList())
                {
                    if (training.TimeOfTraining > max)
                    {
                        pastTrainings.Remove(training);
                    }
                }
            }


            return View("PastTrainings", pastTrainings);
        }

        [HttpPost]
        public ActionResult Visitors(string trainingId)
        {
            Dictionary<string, GroupTraining> trainings = HttpContext.Application["GroupTrainings"] as Dictionary<string, GroupTraining>;
            Dictionary<string, User> users = (Dictionary<string, User>)HttpContext.Application["Users"];
            List<User> visitors = new List<User>();
            GroupTraining training = trainings[trainingId];
            foreach(var username in training.Visitors)
            {
                visitors.Add(users[username]);
            }

            return View(visitors);
        }

    }
}