using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml.Serialization;

namespace WebProject.Models
{
    public class XML
    {

        public static void AddUser(User user)
        {
            string path = HostingEnvironment.MapPath($"~/App_Data/Users/{user.Username}.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(User));
            using (StreamWriter stream = new StreamWriter(path))
            {
                serializer.Serialize(stream, user);         
            }

        }

        public static void UpdateUser(string username, User user)
        {
            string oldPath = HostingEnvironment.MapPath($"~/App_Data/Users/{username}.xml");
            File.Delete(oldPath);              
            AddUser(user);
        }

        public static void AddGroupTraining(GroupTraining groupTraining)
        {
            string path = HostingEnvironment.MapPath($"~/App_Data/GroupTrainings/{groupTraining.TrainingId}.xml");
            
            XmlSerializer serializer = new XmlSerializer(typeof(GroupTraining));
            using (StreamWriter stream = new StreamWriter(path))
            {
                serializer.Serialize(stream, groupTraining);
            }
        }

        public static void UpdateTraining(string trainingId,GroupTraining groupTraining)
        {
            string oldPath = HostingEnvironment.MapPath($"~/App_Data/GroupTrainings/{trainingId}.xml");
            File.Delete(oldPath);
            AddGroupTraining(groupTraining);
        }

        public static void AddAndUpdateComment(CommentList comments)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CommentList));
            string path = HostingEnvironment.MapPath($"~/App_Data/comments.xml");
           
            using (StreamWriter stream = new StreamWriter(path))
            {
                serializer.Serialize(stream, comments);
            }
        }

        public static CommentList DeserializeComments()
        {
            CommentList comments = new CommentList();
            XmlSerializer serializer = new XmlSerializer(typeof(CommentList));
            string path = HostingEnvironment.MapPath($"~/App_Data/comments.xml");
            using (StreamReader stream = new StreamReader(path))
            {
                comments = (CommentList)serializer.Deserialize(stream);
            }
            return comments;

        }

        public static Dictionary<string,User> DeserializeUsers()          
        {
            string path = HostingEnvironment.MapPath($"~/App_Data/Users/");
            Dictionary<string, User> userDictionary = new Dictionary<string, User>();

            string[] users = Directory.GetFiles(path);            
            XmlSerializer serializer = new XmlSerializer(typeof(User));
            foreach (var str in users)
            {
                using (StreamReader stream = new StreamReader(str))
                {
                    User user = (User)serializer.Deserialize(stream);
                    userDictionary.Add(user.Username, user);
                }
            }

            return userDictionary;
        }

        public static Dictionary<string, GroupTraining> DeserializeGroupTrainings()          
        {
            string path = HostingEnvironment.MapPath($"~/App_Data/GroupTrainings/");
            Dictionary<string, GroupTraining> trainingDictionary = new Dictionary<string, GroupTraining>();

            string[] trainings = Directory.GetFiles(path);            
            XmlSerializer serializer = new XmlSerializer(typeof(GroupTraining));
            foreach (var str in trainings)
            {
                using (StreamReader stream = new StreamReader(str))
                {
                    GroupTraining training = (GroupTraining)serializer.Deserialize(stream);
                    trainingDictionary.Add(training.TrainingId, training);
                }
            }

            return trainingDictionary;
        }

    }
}