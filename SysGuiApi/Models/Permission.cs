using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.Models
{
    public enum Permission
    {
        NONE,
        USER_MANAGEMENT,
        REGISTER_PAYMENT,
        ORDER_REGISTER
    }
}
