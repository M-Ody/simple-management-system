using SysGuiApi.Control;
using SysGuiApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.ViewModels
{
    public class ClientViewModel
    {
        public int Id;
        public string Name;
        public string Cpf;
        public string Address;
        public int CityId;
        public string Phone;

        public bool IsValid(ref ServiceResponse result)
        {
            Cpf = Client.UnmaskCpf(Cpf);

            if (Name.Length < 5)
            {
                result.BadRequest("Verifique o nome do cliente");
                return false;
            }
            else if (Cpf.Length < 11)
            {
                result.BadRequest("Verifique o CPF/CNPJ");
                return false;
            }
            else if (Address.Length < 5)
            {
                result.BadRequest("Verifique o endereço");
                return false;
            }
            else if (CityId <= 0)
            {
                result.BadRequest("Verifique a cidade");
                return false;
            }
            else if (Phone.Length < 8)
            {
                result.BadRequest("Verifique o número de telefone");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
