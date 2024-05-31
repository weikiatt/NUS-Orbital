using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NUS_Orbital.Models
{
    public class Post
    {
        public Module module;
        public int postId;
        public DateTime postTime;
        [Required(ErrorMessage="Field is empty")]
        public string description;
        public int upvotes;
        public Student student;
        public List<Comment> comments;
        public bool likedByCurrStud;
        public Post(Module module, int postId, DateTime postTime, string description, int upvotes, Student student, List<Comment> comments, bool likedByCurrStud)
        {
            this.module = module;
            this.postId = postId;
            this.postTime = postTime;
            this.description = description;
            this.upvotes = upvotes;
            this.student = student;
            this.comments = comments;
            this.likedByCurrStud = likedByCurrStud;
        }
        

        public string calculateDate(DateTime postDate)
        {

            DateTime now = DateTime.Now;

            TimeSpan diff = now - postDate;

            int seconds = (int)Math.Floor(diff.TotalSeconds);
            int minutes = (int)Math.Floor(diff.TotalMinutes);
            int hours = (int)Math.Floor(diff.TotalHours);
            int days = (int)Math.Floor(diff.TotalDays);

            if (days > 0)
            {
                return days == 1 ? "1 day ago" : days + " days ago";
            }
            else if (hours > 0)
            {
                return hours == 1 ? "1 hour ago" : hours + " hours ago";
            }
            else if (minutes > 0)
            {
                return minutes == 1 ? "1 minute ago" : minutes + " minutes ago";
            }
            else
            {
                return seconds <= 10 ? "Just now" : seconds + " seconds ago";
            }


        }
    }
}
