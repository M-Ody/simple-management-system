using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.Models
{
    public class PermissionGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string GroupName { get; set; }

        public int[] Permissions { get; set; }
    }
}
