using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace NUS_Orbital.Models
{
    public class Student
    {
        //[Required]
        public int studentId { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(50, ErrorMessage = "Invalid student name")]
        [Display(Name = "Student Name")]
        public String name { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [Display(Name = "Email Address")]
        [EmailAddress]
        //[ValidateEmailExists(ErrorMessage = "Email address already exists!")]
        public String email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public String password { get; set; }

        [Display(Name = "Course")]
        public String course { get; set; }

        [Required(ErrorMessage = "Please enter your date of birth")]
        [Display(Name = "Date of Birth")]
        public DateTime dob { get; set; }

        [Display(Name = "Description")]
        public String description { get; set; }

        public IFormFile? fileToUpload { get; set; }

        [Display(Name = "Photo")]
        public string photo { get; set; }

        public Student(String email, String name, String password)
        {
            this.email = email;
            this.name = name;
            this.password = password;
            this.course = "";
            this.description = "";
            this.studentId = 0;
            this.photo = "user.png";
        }

        public Student(int studentId, String name, String email, String course, DateTime dob, String description, String photo)
        {
            this.studentId = studentId;
            this.name = name;
            this.email = email;
            this.course = course;
            this.dob = dob;
            this.description = description;
            this.photo = photo;
        }

        public Student()
        {
            this.studentId = 0;
            this.name = "DNE";
            this.email = "DNE@gmail.com";
            this.course = "DNE";
            this.dob = DateTime.Now;
            this.description = "DNE";
            this.photo = "user.png";
        }

    }
}
