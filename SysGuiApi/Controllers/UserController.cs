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
    public class UserController : ControllerBase
    {
        private AuthService _auth;
        private UserService _user;

        public UserController()
        {
            _auth = new AuthService();
            _user = new UserService();
        }

        [HttpGet]
        [Route("Check")]
        public async Task<ActionResult<object>> Check()
        {
            if (SignatureManager.Instance.HasSignature(Request.Headers["Signature"]))
                Response.StatusCode = 200;
            else
                Response.StatusCode = 403;

            return await Task.FromResult("");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<object>> Login(LoginViewModel model)
        {
            var encoding = Encoding.GetEncoding("iso-8859-1");
            model.Username = encoding.GetString(Convert.FromBase64String(model.Username));
            model.Password = encoding.GetString(Convert.FromBase64String(model.Password));

            var result = _auth.Login(model.Username, model.Password).Result;
            Response.StatusCode = result.statusCode;

            return await Task.FromResult(result.message);
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<ActionResult<object>> Logout(LogoutViewModel model)
        {
            var encoding = Encoding.GetEncoding("iso-8859-1");

            var result = _auth.Logout(model.Signature).Result;
            Response.StatusCode = result.statusCode;

            return await Task.FromResult(result.message);
        }

        [HttpPost]
        public async Task<ActionResult<object>> Create(UserViewModel model)
        {
            if (!_auth.HasPermission(Permission.USER_MANAGEMENT, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro"); 
            }

            var encoding = Encoding.GetEncoding("iso-8859-1");
            model.Username = encoding.GetString(Convert.FromBase64String(model.Username));
            model.Password = encoding.GetString(Convert.FromBase64String(model.Password));

            ServiceResponse result = new ServiceResponse();
            if (model.IsValid(ref result))
                result = await _user.CreateUser(model.Username, model.Password, model.Permission);
            Response.StatusCode = result.statusCode;

            return await Task.FromResult(result.message);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<object>> Index(int id)
        {
            if (!_auth.HasPermission(Permission.USER_MANAGEMENT, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            return await _user.GetUser(id);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAll()
        {
            if (!_auth.HasPermission(Permission.USER_MANAGEMENT, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            return await _user.GetUsers();
        }

        [HttpGet]
        [Route("Groups")]
        public async Task<ActionResult<object>> GetGroups()
        {
            if (!_auth.HasPermission(Permission.USER_MANAGEMENT, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            return await _user.GetGroups();
        }

        [HttpPut]
        public async Task<ActionResult<object>> Edit(UserViewModel model)
        {
            if (!_auth.HasPermission(Permission.USER_MANAGEMENT, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            var encoding = Encoding.GetEncoding("iso-8859-1");
            model.Username = encoding.GetString(Convert.FromBase64String(model.Username));
            if (!string.IsNullOrEmpty(model.Password))
                model.Password = encoding.GetString(Convert.FromBase64String(model.Password));

            ServiceResponse result = new ServiceResponse();
            if (model.IsValidEdit(ref result))
                result = await _user.EditUser(model.Id, model.Username, model.Password, model.Permission);
            Response.StatusCode = result.statusCode;

            return await Task.FromResult(result.message);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<object>> Delete(int id)
        {
            if (!_auth.HasPermission(Permission.USER_MANAGEMENT, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            if(SignatureManager.Instance.GetUserId(Request.Headers["Signature"]) == id) //Deletar a si mesmo
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            var result = _user.DeleteUser(id).Result;
            Response.StatusCode = result.statusCode;

            return await Task.FromResult(result.message);
        }
    }
}