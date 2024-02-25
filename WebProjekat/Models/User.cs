using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public enum UserRole { Visitor, Owner, Trainer}
    public enum Gender { Male, Female, Other}

    public class User
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Password { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public UserRole UserRole { get; set; }
        public bool LoginBlocked { get; set; }                  

        public List<GroupTraining> GroupTrainings{ get; set; }         
        public string FitnessCentre { get; set; }        

        public List<FitnessCentre> FitnessCentres { get; set; }     
        

        public User(string Username, string Name, string Lastname, string Password, string DateOfBirth, string Email, Gender Gender)
        {
            this.Username = Username;
            this.Name = Name;
            this.Lastname = Lastname;
            this.Password = Password;
            this.DateOfBirth = DateOfBirth;
            this.Email = Email;
            this.Gender = Gender;
            UserRole = UserRole.Visitor;
            GroupTrainings = new List<GroupTraining>();
            FitnessCentres = new List<FitnessCentre>();
    
        }
        public User() { }

        

}
}