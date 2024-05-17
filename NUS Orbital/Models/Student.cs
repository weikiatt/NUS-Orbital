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
        [Required]
        public int studentId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Invalid student name")]
        [Display(Name = "Student Name")]
        public String name { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        [ValidateEmailExists(ErrorMessage = "Email address already exists!")]
        public String email { get; set; }

        public String password { get; set; }
        public String course { get; set; }
        public DateTime dob { get; set; }

        [StringLength(3000, ErrorMessage = "Not more than 3000 characters")]
        public String description { get; set; }

  
       
        public Student(String email, String name, String password)
        {
            this.email = email;
            this.name = name;
            this.password = password;
            this.course = "";
            this.description = "";
            this.studentId = 0;
        }

        public Student(int studentId, String name, String email, String course, DateTime dob, String description)
        {
            this.studentId = studentId;
            this.name = name;
            this.email = email;
            this.course = course;
            this.dob = dob;
            this.description = description;
        }

        public Student()
        {
            this.studentId = 0;
            this.name = "DNE";
            this.email = "DNE@gmail.com";
            this.course = "DNE";
            this.dob = DateTime.Now;
            this.description = "DNE";
        }

    }
}
