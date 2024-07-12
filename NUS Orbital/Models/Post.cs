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
        public Module module { get; set; }
        public int postId { get; set; }
        public DateTime postTime { get; set; }
        [Required(ErrorMessage = "Field is empty")]
        public string title { get; set; }
        [Required(ErrorMessage="Field is empty")]
        public string description { get; set; }
        public int upvotes { get; set; }
        public Student student { get; set; }
        public List<Comment> comments { get; set; }
        public bool likedByCurrStud { get; set; }
        public List<Tag> tags { get; set; }
        public bool edited { get; set; }
        public bool deleted { get; set; }
        public FileDataModel file { get; set; }
        public Post(Module module, int postId, DateTime postTime, string title, string description, int upvotes, Student student, List<Comment> comments, bool likedByCurrStud, List<Tag> tags, bool edited, bool deleted)
        {
            this.module = module;
            this.postId = postId;
            this.postTime = postTime;
            this.title = title;
            this.description = description;
            this.upvotes = upvotes;
            this.student = student;
            this.comments = comments;
            this.likedByCurrStud = likedByCurrStud;
            this.tags = tags;
            this.edited = edited;
            this.deleted = deleted;
        }

        public Post(Module module, int postId, DateTime postTime, string title, string description, int upvotes, Student student, List<Comment> comments, bool likedByCurrStud, List<Tag> tags, bool edited, bool deleted, FileDataModel file)
        {
            this.module = module;
            this.postId = postId;
            this.postTime = postTime;
            this.title = title;
            this.description = description;
            this.upvotes = upvotes;
            this.student = student;
            this.comments = comments;
            this.likedByCurrStud = likedByCurrStud;
            this.tags = tags;
            this.edited = edited;
            this.deleted = deleted;
            this.file = file;
        }

        public bool TagExistInPost(List<int> filterTagIds)
        {
            foreach (Tag t in tags)
            {
                foreach(int tagId in filterTagIds)
                {
                    if (t.tagId == tagId)
                    {
                        return true;
                    }
                }
            }
            return false;
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
