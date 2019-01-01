using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(15)]
        public string Cpf { get; set; }

        [Required]
        public User User { get; set; }

        public City City { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        public static string UnmaskCpf(string cpf)
        {
            string unmaskedCpf = cpf.Replace(".", "");
            unmaskedCpf = unmaskedCpf.Replace("-", "");
            unmaskedCpf = unmaskedCpf.Replace("/", "");
            return unmaskedCpf;
        }
    }
}
