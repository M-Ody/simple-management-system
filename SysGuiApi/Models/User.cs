using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SysGuiApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(50)]
        public string Hash { get; set; }

        [Required]
        public PermissionGroup PermissionGroup { get; set; }
    }
}
