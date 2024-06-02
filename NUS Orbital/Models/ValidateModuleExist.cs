using NUS_Orbital.DAL;
using System.ComponentModel.DataAnnotations;

namespace NUS_Orbital.Models
{
    public class ValidateModuleExist : ValidationAttribute
    {
        private ModuleDAL moduleContext = new ModuleDAL();
        public override bool IsValid(object value)
        {
            string moduleCode = Convert.ToString(value);
            if (moduleContext.DoesModuleExist(moduleCode))
                return false; // validation failed
            else
                return true; // validation passed
        }
    }
}
