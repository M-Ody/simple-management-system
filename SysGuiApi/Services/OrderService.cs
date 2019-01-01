using Microsoft.EntityFrameworkCore;
using SysGuiApi.Control;
using SysGuiApi.Models;
using SysGuiApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.Services
{
    public class OrderService
    {
        public async Task<ServiceResponse> GetOrder(int id)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbOrder = await db.Order
                    .Where(x => x.Id == id)
                    .Select(x => new
                    {
                        x.Id,
                        client = new
                        {
                            x.Client.Name,
                            x.Client.Cpf,
                            x.Client.Address,
                            x.Client.Phone,
                            city = x.Client.City.Name
                        },
                        x.Description,
                        x.FirstPaymentPaid,
                        x.FirstPayment,
                        x.FirstPaymentFiscal,
                        x.DateFirstInstallmentPayment,
                        x.InstallmentPaymentDay,
                        x.InstallmentValues,
                        x.InstallmentValuesFiscal,
                        x.NumberInstallments,
                        x.NumberInstallmentsPaid,
                        x.DateCreation,
                        x.CloseningPayment,
                        x.CloseningPaymentFiscal,
                        x.CloseningPaid,
                        x.User.Username,
                        factory = x.Factory.ToString(),
                        paymentMode = x.PaymentMode.ToString()
                    })
                    .FirstOrDefaultAsync();

                if (dbOrder != null)
                {
                    response.Ok(dbOrder);
                }
                else
                {
                    response.BadRequest("A ordem não existe.");
                }
            }

            return response;
        }

        public async Task<ServiceResponse> GetPendingInstallmentPayments(int factory)
        {
            var response = new ServiceResponse();

            List<dynamic> expireds = new List<dynamic>();
            using (var db = new BrillDbContext())
            {
                var dbOrders = await db.Order
                    .Where(x => (int)x.Factory == factory)
                    .Where(x => IsActive(x.CloseningPaid))
                    .Where(x => HasPendingInstallments(
                        x.DateFirstInstallmentPayment,
                        x.NumberInstallmentsPaid,
                        x.NumberInstallments))
                    .Select(x => new
                    {
                        id = x.Id,
                        name = x.Client.Name,
                        amount = x.InstallmentValues[x.NumberInstallmentsPaid],
                        amountFiscal = x.InstallmentValuesFiscal[x.NumberInstallmentsPaid],
                        paymentMode = x.PaymentMode,
                        dateFirstPayment = x.DateFirstInstallmentPayment,
                        numberInstallmentsPaid = x.NumberInstallmentsPaid,
                        numberInstallments = x.NumberInstallments,
                        paymentDay = x.InstallmentPaymentDay
                    })
                    .ToListAsync();

                foreach (var dbOrder in dbOrders)
                {
                    ExpirementData data;
                    if (IsExpired(
                        dbOrder.dateFirstPayment,
                        dbOrder.numberInstallmentsPaid,
                        dbOrder.numberInstallments,
                        dbOrder.paymentDay,
                        dbOrder.paymentMode,
                        out data))
                    {
                        expireds.Add(new
                        {
                            dbOrder.id,
                            dbOrder.name,
                            dbOrder.amount,
                            dbOrder.amountFiscal,
                            data.expirementDate,
                            data.numberInstallmentsExpired,
                            paymentMode = dbOrder.paymentMode.ToString()
                        });
                    }
                }

                response.Ok(expireds);
            }

            return response;
        }

        public async Task<ServiceResponse> GetPendingFirstPayments(int factory)
        {
            var response = new ServiceResponse();

            using (var db = new BrillDbContext())
            {
                var dbOrders = await db.Order
                    .Where(x => (int)x.Factory == factory)
                    .Where(x => IsActive(x.CloseningPaid))
                    .Where(x => !x.FirstPaymentPaid)
                    .Select(x => new
                    {
                        id = x.Id,
                        name = x.Client.Name,
                        amount = x.FirstPayment,
                        amountFiscal = x.FirstPaymentFiscal,
                        paymentMode = x.PaymentMode.ToString(),
                        dateCreation = x.DateCreation,
                    })
                    .ToListAsync();

                response.Ok(dbOrders);
            }

            return response;
        }

        public async Task<ServiceResponse> GetPendingClosings(int factory)
        {
            var response = new ServiceResponse();

            using (var db = new BrillDbContext())
            {
                var dbOrders = await db.Order
                    .Where(x => (int)x.Factory == factory)
                    .Where(x => IsActive(x.CloseningPaid))
                    .Where(x => x.FirstPaymentPaid)
                    .Where(x => x.NumberInstallments == x.NumberInstallmentsPaid)
                    .Select(x => new
                    {
                        id = x.Id,
                        name = x.Client.Name,
                        paymentMode = x.PaymentMode.ToString(),
                        dateCreation = x.DateCreation,
                    })
                    .ToListAsync();

                response.Ok(dbOrders);
            }

            return response;
        }

        public async Task<ServiceResponse> GetPaymentModes()
        {
            var response = new ServiceResponse();
            var paymentModes = Enum.GetNames(typeof(PaymentMode));

            dynamic[] names = new dynamic[paymentModes.Length];

            for (int i = 0; i < paymentModes.Length; i++)
            {
                names[i] = new { id = i + 1, name = paymentModes[i] };
            }

            response.Ok(names);

            return response;
        }

        public async Task<ServiceResponse> SetFirstPaymentPaid(int id, string userSignature)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbOrder = await db.Order.FirstOrDefaultAsync(x => x.Id == id);
                if (dbOrder != null) // O pedido existe
                {
                    dbOrder.FirstPaymentPaid = true;

                    db.Order.Attach(dbOrder);
                    db.Entry(dbOrder).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    LogService.WriteLog(userSignature, "aceitou primeiro pagamento no pedido #" + id);
                    response.Ok(new { Id = dbOrder.Id });
                }
                else
                {
                    response.BadRequest("O cliente não existe.");
                }

                return response;
            }
        }

        public async Task<ServiceResponse> AddInstallmentPaid(int id, string userSignature)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbOrder = await db.Order.FirstOrDefaultAsync(x => x.Id == id);
                if (dbOrder != null) // O pedido existe
                {
                    dbOrder.NumberInstallmentsPaid ++;

                    db.Order.Attach(dbOrder);
                    db.Entry(dbOrder).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    LogService.WriteLog(userSignature, "pagou parcela " + dbOrder.NumberInstallmentsPaid-- + " do pedido #" + id);
                    response.Ok(new { Id = dbOrder.Id });
                }
                else
                {
                    response.BadRequest("O cliente não existe.");
                }

                return response;
            }
        }

        public async Task<ServiceResponse> CreateOrder(OrderViewModel model, int userId, string userSignature)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var client = await db.Client.FirstOrDefaultAsync(x => x.Id == model.ClientId);
                if (client != null) // O cliente existe
                {
                    Order order = new Order
                    {
                        Client = client,
                        Description = model.Description,
                        FirstPayment = model.FirstPayment,
                        FirstPaymentFiscal = model.FirstPaymentFiscal,
                        FirstPaymentPaid = model.Paid,
                        InstallmentValues = model.InstallmentsValues,
                        InstallmentValuesFiscal = model.InstallmentsValuesFiscal,
                        NumberInstallments = model.NumberInstallments,
                        InstallmentPaymentDay = model.PaymentDay,
                        DateFirstInstallmentPayment = model.FirstPaymentDateFormated,
                        PaymentMode = (PaymentMode)model.PaymentMode,
                        User = db.Users.FirstOrDefaultAsync(x => x.Id == userId).Result,
                        Factory = (Factory)model.Factory,
                        DateCreation = DateTime.Now,
                    };

                    await db.Order.AddAsync(order);
                    await db.SaveChangesAsync();

                    if (model.Paid)
                        LogService.WriteLog(userSignature, "criou o pedido #" + order.Id + " -> primeiro pagamento pago");
                    else
                        LogService.WriteLog(userSignature, "criou o pedido #" + order.Id + " -> primeiro pagamento não pago");

                    response.Ok(new { Id = order.Id });
                }
                else
                {
                    response.BadRequest("O cliente não existe.");
                }
            }

            return response;
        }

        public async Task<ServiceResponse> CloseOrder(CloseOrderViewModel model, string userSignature)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbOrder = await db.Order.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (dbOrder != null) // O pedido existe
                {
                    dbOrder.CloseningPayment = model.CloseValue;
                    dbOrder.CloseningPaymentFiscal = model.CloseValueFiscal;
                    dbOrder.CloseningPaid = true;

                    db.Order.Attach(dbOrder);
                    db.Entry(dbOrder).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    LogService.WriteLog(userSignature, "fechou o pedido #" + model.Id);
                    response.Ok(new { Id = dbOrder.Id });
                }
                else
                {
                    response.BadRequest("O pedido não existe.");
                }
            }

            return response;
        }

        /*
        public async Task<ServiceResponse> EditUser(int id, string username, string password, int permission)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbUser = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (dbUser != null) // O usuário ESTÁ registrado
                {
                    var permissionGroup = await db.PermissionGroup.FirstOrDefaultAsync(x => x.Id == permission);
                    if (permissionGroup != null) // A permissão existe
                    {
                        dbUser.Username = username;
                        dbUser.PermissionGroup = permissionGroup;

                        if (!string.IsNullOrEmpty(password)) //Vai mudar a senha também?
                        {
                            dbUser.Hash = AuthService.Md5Hash(username, password);
                        }

                        db.Users.Attach(dbUser);
                        db.Entry(dbUser).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        response.Ok(new { Id = dbUser.Id });
                    }
                    else
                    {
                        response.BadRequest("O grupo de usuários não existe.");
                    }
                }
                else
                {
                    response.BadRequest("O usuário não existe.");
                }
            }

            return response;
        }

        public async Task<ServiceResponse> DeleteUser(int id)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbUser = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (dbUser != null) // O usuário ESTÁ registrado
                {
                    db.Users.Remove(dbUser);
                    db.SaveChanges();
                    response.Ok("Ok");
                }
                else
                {
                    response.BadRequest("O usuário não existe.");
                }
            }

            return response;
        }*/

        private bool IsActive(bool closeningPaid)
        {
            return !closeningPaid;
        }

        private bool HasPendingInstallments(DateTime dateFirstPayment, int numberInstallmentsPaid, int numberInstallments)
        {
            return dateFirstPayment != new DateTime() && dateFirstPayment.Date < DateTime.Today.Date && numberInstallmentsPaid != numberInstallments;
        }

        private bool IsExpired(DateTime dateFirstPayment, int numberInstallmentsPaid, 
            int numberInstallments, int paymentDay, PaymentMode mode, out ExpirementData data)
        {
            data = new ExpirementData();
            DateTime expirementDate;

            if (dateFirstPayment.Date == DateTime.Today.Date && numberInstallmentsPaid == 0)
            {
                expirementDate = DateTime.Today.Date;
            }
            else if (dateFirstPayment.Date.AddMonths(numberInstallmentsPaid) < DateTime.Today.Date)
            {
                expirementDate = dateFirstPayment.Date.AddMonths(numberInstallmentsPaid).AddDays(paymentDay - 1);
            }
            else
            {
                return false;
            }

            if (mode == PaymentMode.BOLETO)
            {
                if (expirementDate.AddDays(2) <= DateTime.Today.Date)
                { 
                    data.expirementDate = expirementDate;
                    data.numberInstallmentsExpired = ((DateTime.Today.Year - expirementDate.Year) * 12) + DateTime.Today.Month - expirementDate.Month + 1;
                    return true;
                }
            }
            else if (expirementDate <= DateTime.Today.Date)
            {
                data.expirementDate = expirementDate;
                data.numberInstallmentsExpired = ((DateTime.Today.Year - expirementDate.Year) * 12) + DateTime.Today.Month - expirementDate.Month + 1;
                return true;
            }

            return false;
        }

        class ExpirementData
        {
            public DateTime expirementDate;
            public int numberInstallmentsExpired;
        }
    }
}
