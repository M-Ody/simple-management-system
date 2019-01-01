using Microsoft.AspNetCore.Mvc;
using SysGuiApi.Control;
using SysGuiApi.Models;
using SysGuiApi.Services;
using SysGuiApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGuiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private AuthService _auth;
        private ClientService _client;

        public ClientController()
        {
            _auth = new AuthService();
            _client = new ClientService();
        }

        [HttpGet]
        [Route("cities")]
        public async Task<ActionResult<object>> GetCities()
        {
            if (!_auth.HasPermission(Permission.ORDER_REGISTER, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            return await _client.GetCities();
        }

        [HttpGet]
        public async Task<ActionResult<object>> Get()
        {
            if (!_auth.HasPermission(Permission.ORDER_REGISTER, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            string queryArgName = Request.QueryString.Value
                .Split('=')[0]
                .Replace("?", "");
            string queryArg = Request.QueryString.Value
                .Split('=')[1];

            queryArg = Uri.UnescapeDataString(queryArg);

            if (queryArgName == "input")
            {
                return await _client.GetSearchResults(queryArg);
            }
            else if (queryArgName == "name")
            {
                return await _client. GetClientByName(queryArg);
            }
            else if (queryArgName == "cpf")
            {
                return await _client.GetClientByCpf(queryArg);
            }
            else
            {
                var response = new ServiceResponse();
                response.BadRequest("Parâmetro não identificado");
                return response;
            }
        }

        [HttpGet]
        [Route("fullClient")]
        public async Task<ActionResult<object>> GetFull()
        {
            if (!_auth.HasPermission(Permission.ORDER_REGISTER, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            string queryArgName = Request.QueryString.Value
                .Split('=')[0]
                .Replace("?", "");
            string queryArg = Request.QueryString.Value
                .Split('=')[1];

            queryArg = Uri.UnescapeDataString(queryArg);

            if (queryArgName == "name")
            {
                return await _client.GetFullClientByName(queryArg);
            }
            else if (queryArgName == "cpf")
            {
                return await _client.GetFullClientByCpf(queryArg);
            }
            else
            {
                var response = new ServiceResponse();
                response.BadRequest("Parâmetro não identificado");
                return response;
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<object>> Index(int id)
        {
            if (!_auth.HasPermission(Permission.ORDER_REGISTER, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            return await _client.GetClient(id);
        }

        [HttpPost]
        public async Task<ActionResult<object>> Create(ClientViewModel model)
        {
            if (!_auth.HasPermission(Permission.ORDER_REGISTER, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            int userId = SignatureManager.Instance.GetUserId(Request.Headers["Signature"]);

            ServiceResponse result = new ServiceResponse();
            if (model.IsValid(ref result))
                result = await _client.
                    CreateClient(model.Name, model.Cpf, userId, model.Address, model.CityId, model.Phone, Request.Headers["Signature"]);
            Response.StatusCode = result.statusCode;

            return await Task.FromResult(result.message);
        }

        [HttpPut]
        public async Task<ActionResult<object>> Edit(ClientViewModel model)
        {
            if (!_auth.HasPermission(Permission.ORDER_REGISTER, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            ServiceResponse result = new ServiceResponse();
            if (model.IsValid(ref result))
                result = await _client.EditClient(model, Request.Headers["Signature"]);
            Response.StatusCode = result.statusCode;

            return await Task.FromResult(result.message);
        }
    }
}
