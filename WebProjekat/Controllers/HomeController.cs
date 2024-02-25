using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;

namespace WebProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<User> owners = ((Dictionary<string,User>)HttpContext.Application["Users"]).Values.ToList(); 
            owners = owners.FindAll(x => x.UserRole == UserRole.Owner);   

            List<FitnessCentre> centres = FitnessCentresRetriever(owners);     

            centres = centres.FindAll(x => x.Deleted == false);      

            Dictionary<string, GroupTraining> groupTrainings = HttpContext.Application["GroupTrainings"] as Dictionary<string, GroupTraining>;     
            CommentList comments = HttpContext.Application["Comments"] as CommentList;  
            foreach (var centre in centres)
            {
                List<GroupTraining> centresTrainings = groupTrainings.Values.Where(x => x.FitnessCenter == centre.CenterName && x.TimeOfTraining > DateTime.Now && x.Deleted == false).ToList();
                List<Comment> centreComments = comments.Comments.FindAll(x => x.FitnessCenter == centre.CenterName && x.Approved);         
                centre.FutureTrainings = centresTrainings;
                centre.Comments = centreComments;
            }
           

            HttpContext.Application["FitnessCentres"] = centres;
            HttpContext.Application["FitnessCentresCopy"] = centres;     
            return View();
        }

        public ActionResult DetailsView()               
        {
            ViewBag.Message = HttpContext.Application["Error"] as string;
            return View("Details");
        }

        public ActionResult Details(string centreName)
        {
            ViewBag.Message = "";

            List<FitnessCentre> centres = HttpContext.Application["FitnessCentres"] as List<FitnessCentre>;           
            var centre = centres.Find(x => x.CenterName == centreName);
            HttpContext.Application["CentreDetails"] = centre;           

            return View(); 
        }

        [HttpPost]
        public ActionResult Sort(string sortValue)
        {
            List<FitnessCentre> centres = HttpContext.Application["FitnessCentresCopy"] as List<FitnessCentre>;
            
            if(sortValue.Equals("nameAscending"))
            {
                
                HttpContext.Application["FitnessCentres"] = centres.OrderBy(x => x.CenterName).ToList();
                return View("Index");
            }
            else if(sortValue.Equals("addressAscending"))
            {
                HttpContext.Application["FitnessCentres"] = centres.OrderBy(x => x.Address).ToList();
                return View("Index");
            }
            else if (sortValue.Equals("yearAscending"))
            {
                HttpContext.Application["FitnessCentres"] = centres.OrderBy(x => x.OpeningYear).ToList();
                return View("Index");
            }
            else if (sortValue.Equals("nameDescending"))
            {
                HttpContext.Application["FitnessCentres"] = centres.OrderByDescending(x => x.CenterName).ToList();
                return View("Index");
            }
            else if (sortValue.Equals("addressDescending"))
            {
                HttpContext.Application["FitnessCentres"] = centres.OrderByDescending(x => x.Address).ToList();
                return View("Index");
            }
            else
            {
                HttpContext.Application["FitnessCentres"] = centres.OrderByDescending(x => x.OpeningYear).ToList();
                return View("Index");
            }
        }

        [HttpPost]
        public ActionResult SearchParameters(string name,string address, int? minYear, int? maxYear)                
        {
            List<FitnessCentre> centres = HttpContext.Application["FitnessCentresCopy"] as List<FitnessCentre>;
            if (name != "")
            {
                centres = centres.FindAll(x => x.CenterName.Contains(name));
            }
            if(address != "")
            {
                centres = centres.FindAll(x => x.Address.Contains(address));
            }
            if(minYear.HasValue)
            {
                centres = centres.FindAll(x => x.OpeningYear >= minYear);
            }

            if (maxYear.HasValue)
            {
                centres = centres.FindAll(x => x.OpeningYear <= maxYear);
            }

            HttpContext.Application["FitnessCentres"] = centres;
            return View("Index");
        }


        #region AddedFunctions

        public static void FirstStart()                  
        {
            FitnessCentre fitnessCentre1 = new FitnessCentre("BestFit", "Ive Andrica 5, Novi Sad, 21100", 2004, "MikeOwner", 3200, 32000, 500, 700, 950);
            FitnessCentre fitnessCentre2 = new FitnessCentre("NewFit", "Bulevar Oslobodjenja 55, Novi Sad, 21100", 2018, "MikeOwner", 3200, 32000, 400, 600, 950);
            User user = new User("MikeOwner", "Milos", "Jankovic", "1234", "08-03-1995", "milosjankovic@gmail.com", Gender.Male) { UserRole = UserRole.Owner };
            user.FitnessCentres.Add(fitnessCentre1);
            user.FitnessCentres.Add(fitnessCentre2);
            XML.AddUser(user);
        }

        public static List<FitnessCentre> FitnessCentresRetriever(List<User> owners)
        {
           

            List<FitnessCentre> fitnessCentres = new List<FitnessCentre>();
            foreach(var owner in owners)
            {
                fitnessCentres.AddRange(owner.FitnessCentres);
            }

            return fitnessCentres;
        }

        #endregion
    }
}
    
