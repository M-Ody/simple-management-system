﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [StringLength(20)]
        public string Name { get; set; }
    }
}
