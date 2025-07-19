using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAPIBusinessLayer.DTO.Auth
{
    public class AddRoleModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string RoleName { get; set; } = string.Empty;

    }
}
