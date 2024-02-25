using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class FitnessCentre
    {
        public string CenterName { get; set; }
        public string Address { get; set; }
        public int OpeningYear { get; set; }
        public string OwnerUsername { get; set; }
        public int MonthlyMembership { get; set; }
        public int YearlyMembership { get; set; }
        public int TrainingCost { get; set; }
        public int GroupTrainingCost { get; set; }
        public int PersonalTrainingCost { get; set; }
        public bool Deleted { get; set; }

        public List<GroupTraining> FutureTrainings { get; set; }                     
        public List<Comment> Comments { get; set; }           
        public FitnessCentre(string CenterName, string Address, int OpeningYear, string OwnerUsername, int MonthlyMembership, int YearlyMembership, int TrainingCost, int GroupTrainingCost, int PersonalTrainingCost)
        {
            this.CenterName = CenterName;
            this.Address = Address;
            this.OpeningYear = OpeningYear;
            this.OwnerUsername = OwnerUsername;
            this.MonthlyMembership = MonthlyMembership;
            this.YearlyMembership = YearlyMembership;
            this.TrainingCost = TrainingCost;
            this.GroupTrainingCost = GroupTrainingCost;
            this.PersonalTrainingCost = PersonalTrainingCost;
            Deleted = false;
        }
        public FitnessCentre() { }


    }
}