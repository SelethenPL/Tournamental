using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tournamental.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ResetCode { get; set; }
    }
}