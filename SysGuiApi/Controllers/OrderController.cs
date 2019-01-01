using Microsoft.AspNetCore.Mvc;
using SysGuiApi.Control;
using SysGuiApi.Models;
using SysGuiApi.Services;
using SysGuiApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private AuthService _auth;
        private OrderService _order;

        public OrderController()
        {
            _auth = new AuthService();
            _order = new OrderService();
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

            return await _order.GetOrder(id);
        }

        [HttpGet]
        [Route("payment-modes")]
        public async Task<ActionResult<object>> GetPaymentModes()
        {
            if (!_auth.HasPermission(Permission.ORDER_REGISTER, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            return await _order.GetPaymentModes();
        }

        [HttpGet]
        [Route("get-expired-installments/{idFactory}")]
        public async Task<ActionResult<object>> GetExpiredInstallments(int idFactory)
        {
            if (!_auth.HasPermission(Permission.ORDER_REGISTER, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            return await _order.GetPendingInstallmentPayments(idFactory);
        }

        [HttpGet]
        [Route("get-expired-first-payments/{idFactory}")]
        public async Task<ActionResult<object>> GetExpiredFirstPayments(int idFactory)
        {
            if (!_auth.HasPermission(Permission.ORDER_REGISTER, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            return await _order.GetPendingFirstPayments(idFactory);
        }

        [HttpGet]
        [Route("get-pending-closings/{idFactory}")]
        public async Task<ActionResult<object>> GetPendingClosings(int idFactory)
        {
            if (!_auth.HasPermission(Permission.ORDER_REGISTER, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            return await _order.GetPendingClosings(idFactory);
        }

        [HttpGet]
        [Route("pay-first-payment/{id}")]
        public async Task<ActionResult<object>> PayFirstPayment(int id)
        {
            if (!_auth.HasPermission(Permission.ORDER_REGISTER, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            return await _order.SetFirstPaymentPaid(id, Request.Headers["Signature"]);
        }

        [HttpGet]
        [Route("pay-installment/{id}")]
        public async Task<ActionResult<object>> PayInstallment(int id)
        {
            if (!_auth.HasPermission(Permission.ORDER_REGISTER, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            return await _order.AddInstallmentPaid(id, Request.Headers["Signature"]);
        }

        [HttpPost]
        public async Task<ActionResult<object>> Create(OrderViewModel model)
        {
            if (!_auth.HasPermission(Permission.ORDER_REGISTER, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            ServiceResponse result = new ServiceResponse();
            if (model.IsValid(ref result))
                result = await _order.CreateOrder(model, SignatureManager.Instance.GetUserId(Request.Headers["Signature"]), Request.Headers["Signature"]);
            Response.StatusCode = result.statusCode;

            return await Task.FromResult(result.message);
        }

        [HttpPost]
        [Route("close")]
        public async Task<ActionResult<object>> Close(CloseOrderViewModel model)
        {
            if (!_auth.HasPermission(Permission.ORDER_REGISTER, Request.Headers["Signature"]))
            {
                Response.StatusCode = 403; //FORBIDDEN
                return await Task.FromResult("Erro");
            }

            ServiceResponse result = new ServiceResponse();
            if (model.IsValid(ref result))
                result = await _order.CloseOrder(model, Request.Headers["Signature"]);
            Response.StatusCode = result.statusCode;

            return await Task.FromResult(result.message);
        }
    }
}
