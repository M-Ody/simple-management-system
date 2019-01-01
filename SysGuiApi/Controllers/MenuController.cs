using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SysGuiApi.Control;
using SysGuiApi.Models;
using SysGuiApi.Services;
using SysGuiApi.ViewModels;

namespace SysGuiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private MenuService _menu;

        public MenuController()
        {
            _menu = new MenuService();
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAll()
        {
            string signature = Request.Headers["Signature"];
            var permissions = SignatureManager.Instance.GetPermissions(signature);
            if (permissions != null)
            {
                return await _menu.GetMenu(permissions);
            }
            else
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }            
        }
    }
}