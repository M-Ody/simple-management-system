using Microsoft.EntityFrameworkCore;
using SysGuiApi.Control;
using SysGuiApi.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.Services
{
    public class MenuService
    {
        public async Task<ServiceResponse> GetMenu(List<int> permissions)
        {
            var response = new ServiceResponse();
            var menu = new List<MenuItem>();

            foreach (int permission in permissions)
            {
                switch (permission)
                {
                    case (int)Permission.USER_MANAGEMENT:
                        AddItem(ref menu, "Administração", "Cadastrar Usuário", "registerUser");
                        AddItem(ref menu, "Administração", "Editar Usuário", "editUser");
                        AddItem(ref menu, "Administração", "Deletar Usuário", "deleteUser");
                        break;
                    case (int)Permission.REGISTER_PAYMENT:
                        AddItem(ref menu, "Pedidos", "Registrar Pedido", "registerOrder");
                        AddItem(ref menu, "Pedidos", "Vencimentos", "nextPayments");
                        AddItem(ref menu, "Cliente", "Gerenciar Clientes", "searchClients");
                        break;

                    default:break;
                }
            }

            response.Ok(menu);

            return response;
        }

        private void AddItem(ref List<MenuItem> menu, string menuName, string item, string function)
        {
            if (!menu.Any(x => x.title == menuName))
            {
                menu.Add(new MenuItem(menuName));
            }
            menu.First(x => x.title == menuName).items.Add(new string[] { item, function });
        }
    }
    
    public class MenuItem
    {
        public MenuItem(string title)
        {
            this.title = title;
            items = new List<string[]>();
        }

        public string title;
        public List<string[]> items;
    }
}
