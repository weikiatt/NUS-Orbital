using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace NUS_Orbital.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(50, ErrorMessage = "Invalid student name")]
        [Display(Name = "Student Name")]
        public String Name { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [Display(Name = "Email Address")]
        [EmailAddress]
        //[ValidateEmailExists(ErrorMessage = "Email address already exists!")]
        public String Email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [Display(Name = "Course")]
        public String Course { get; set; }


        [Display(Name = "Description")]
        public String Description { get; set; }

        [NotMapped]
        public IFormFile? FileToUpload { get; set; }


        public byte[] ProfilePicture { get; set; }


        [Display(Name = "Photo")]
        public string Photo { get; set; }
        public bool verified { get; set; }


    }
}
