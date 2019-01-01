using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SysGuiApi.Control;

namespace SysGuiApi.ViewModels
{
    public class UserViewModel
    {
        public int Id;
        public string Username;
        public string Password;
        public int Permission;

        public bool IsValid(ref ServiceResponse result)
        {
            if (Username.Length < 4)
            {
                result.BadRequest("Nome de usuário muito curto");
                return false;
            }
            else if (Password.Length < 6)
            {
                result.BadRequest("Senha muito curta");
                return false;
            }
            else if (Permission == 0)
            {
                result.BadRequest("Escolha um grupo de permissões");
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsValidEdit(ref ServiceResponse result)
        {
            if (Username.Length < 4)
            {
                result.BadRequest("Nome de usuário muito curto");
                return false;
            }
            else if (Password.Length != 0 && Password.Length < 6)
            {
                result.BadRequest("Senha muito curta");
                return false;
            }
            else if (Permission == 0)
            {
                result.BadRequest("Escolha um grupo de permissões");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
