using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NUS_Orbital.Models
{
    public class Student
    {
        public int id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Invalid student name")]
        [Display(Name = "Student Name")]
        public String name { get; set; }

        public String course { get; set; }

        //public String photo { get; set; }

        [Required]
        [StringLength(3000, ErrorMessage = "Not more than 3000 characters")]
        public String description { get; set; }

  
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        [ValidateEmailExists(ErrorMessage = "Email address already exists!")]
        public String email { get; set; }

        public String password { get; set; }

    }
}
