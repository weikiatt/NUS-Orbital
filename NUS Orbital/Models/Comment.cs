using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using NUS_Orbital.DAL;
using System.Runtime.ConstrainedExecution;

namespace NUS_Orbital.Models
{
    public class Comment
    {
        [Required]
        public int commentId { get; set; }
        [Required]
        public DateTime commentTime { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public int upvotes { get; set; }
        [Required]
        public int postId { get; set; }
        [Required]
        public Student student {  get; set; }
        [Required]
        public Boolean likedByCurrStud;

        public Comment(int commentId, DateTime commentTime, string description, int upvotes, int postId, Student student, Boolean likedByCurrStud)
        {
            this.commentId = commentId;
            this.commentTime = commentTime;
            this.description = description;
            this.upvotes = upvotes;
            this.postId = postId;
            this.student = student;
            this.likedByCurrStud = likedByCurrStud;
        }


        public string calculateDate(DateTime commentDate)
        {

            DateTime now = DateTime.Now;

            TimeSpan diff = now - commentDate;

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
