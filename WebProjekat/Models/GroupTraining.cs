using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    
    public enum TrainingType { Yoga, LesMillsTone, BodyPump }
    public class GroupTraining
    {
        public string TrainingName { get; set; }
        public string TrainingId { get; set; }
        public string FitnessCenter { get; set; }
        public TrainingType TrainingType { get; set; }
        public int Length { get; set; }
        public DateTime TimeOfTraining { get; set; }
        public int AllowedVisitors { get; set; }
        public List<string> Visitors { get; set; }
        public bool Deleted { get; set; }

        public GroupTraining(string TrainingName, string FitnessCenter, TrainingType TrainingType, int Length, DateTime TimeOfTraining, int AllowedVisitors)
        {
            this.TrainingName = TrainingName;
            TrainingId = string.Format($"{FitnessCenter}.{TrainingName}");
            this.FitnessCenter = FitnessCenter;
            this.TrainingType = TrainingType;
            this.Length = Length;
            this.TimeOfTraining = TimeOfTraining;
            this.AllowedVisitors = AllowedVisitors;
            Visitors = new List<string>();
            Deleted = false;
        
        }
        public GroupTraining() { }

    }
}