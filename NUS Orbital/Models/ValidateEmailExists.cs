using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using NUS_Orbital.DAL;

namespace NUS_Orbital.Models
{
    public class ValidateEmailExists : ValidationAttribute
    {
        private LecturerDAL lecturerContext = new LecturerDAL();
        public override bool IsValid(object value)
        {
            string email = Convert.ToString(value);
            if (lecturerContext.IsEmailExist(email))
                return false; // validation failed
            else
                return true; // validation passed
        }
    }
}
