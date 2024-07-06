using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using NUS_Orbital.DAL;

namespace NUS_Orbital.Models
{
    public class Module
    {
        [Required(ErrorMessage = "Please enter a module code")]
        [Display(Name = "Module Code")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Length of module code should be 5")]
        [ValidateModuleExist(ErrorMessage = "Module code already exists")]
        public String moduleCode { get; set; }

        [Required]
        public String moduleName { get; set; }
        [Required]
        public String description { get; set; }
        public bool hidden { get; set; }
        public Module(String moduleCode, String moduleName, String description, bool hidden)
        {
            this.moduleCode = moduleCode;
            this.moduleName = moduleName;
            this.description = description;
            this.hidden = hidden;
        }

    }
}
