using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class Comment
    {
        public string Visitor { get; set; }
        public string FitnessCenter { get; set; }
        public int Grade { get; set; }
        public string VisitorsComment { get; set; }
        public bool Approved { get; set; }
        public bool Declined { get; set; }
        public string Id { get; set; }

        public Comment(string Visitor, string FitnessCenter, int Grade, string VisitorsComment)
        {
            this.Visitor = Visitor;
            this.FitnessCenter = FitnessCenter;
            this.Grade = Grade;
            this.VisitorsComment = VisitorsComment;
            Id = Guid.NewGuid().ToString();
            Approved = false;
            Declined = false;
        }

        public Comment() {
            Id = Guid.NewGuid().ToString();
            Approved = false;
            Declined = false;
        }
    }

    public class CommentList                    
    {
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public CommentList() { }     
    }
}