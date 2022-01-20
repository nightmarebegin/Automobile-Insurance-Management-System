using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.Model
{
    public class User
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public int RoleId { get; set; }
    }
}