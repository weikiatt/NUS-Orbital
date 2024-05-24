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
        public int downvotes {  get; set; }
        [Required]
        public int postId { get; set; }
        [Required]
        public Student student {  get; set; }

        public Comment(int commentId, DateTime commentTime, string description, int upvotes, int downvotes, int postId, Student student)
        {
            this.commentId = commentId;
            this.commentTime = commentTime;
            this.description = description;
            this.upvotes = upvotes;
            this.downvotes = downvotes;
            this.postId = postId;
            this.student = student;
        }

    }
}
