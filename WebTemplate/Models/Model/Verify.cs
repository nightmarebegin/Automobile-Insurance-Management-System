using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.Model
{
    public class Verify
    {
        [Required]
        
        public int otp { get; set; }
    }
}