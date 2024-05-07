using System.ComponentModel.DataAnnotations;

namespace NUS_Orbital.Models
{
    public class Student
    {
        public int StudentID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Invalid student name")]
        [Display(Name = "Student Name")]
        public string Name { get; set; }

        [Required]
        public string Course { get; set; }

        public string Photo { get; set; }

        [Required]
        [StringLength(3000, ErrorMessage = "Not more than 3000 characters")]
        public string Description { get; set; }

        [Display(Name = "E-Portfolio Link")]
        [StringLength(255, ErrorMessage = "Invalid URL")]
        public string ExternalLink { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        [ValidateEmailExists(ErrorMessage = "Email address already exists!")]
        public string EmailAddr { get; set; }

        public string Password { get; set; }

        [Required]
        [Display(Name = "Mentor")]
        public int MentorID { get; set; }

        public string SkillSets { get; set; }

        public IFormFile fileToUpload { get; set; }
    }
}
