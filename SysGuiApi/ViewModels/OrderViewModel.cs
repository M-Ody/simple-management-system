using SysGuiApi.Control;
using SysGuiApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.ViewModels
{
    public class OrderViewModel
    {
        public int ClientId;
        public string Description;
        public double FirstPayment;
        public double FirstPaymentFiscal;
        public bool Paid;
        public double []InstallmentsValues;
        public double []InstallmentsValuesFiscal;
        public int NumberInstallments;
        public int PaymentDay;
        public string FirstPaymentDate;
        public DateTime FirstPaymentDateFormated;
        public int PaymentMode;
        public int Factory;

        public bool IsValid(ref ServiceResponse result)
        {
            FirstPayment /= 100;
            FirstPaymentFiscal /= 100;
            for (int i = 0; i < InstallmentsValues.Length; i++)
            {
                InstallmentsValues[i] /= 100;
                InstallmentsValuesFiscal[i] /= 100;
            }

            if (ClientId <= 0)
            {
                result.BadRequest("Selecione um cliente");
                return false;
            }
            else if (Description.Length < 2)
            {
                result.BadRequest("Verifique a descrição");
                return false;
            }
            else if (FirstPayment <= 0.0)
            {
                result.BadRequest("Verifique o valor do pagamento");
                return false;
            }
            else if (FirstPaymentFiscal <= 0.0)
            {
                result.BadRequest("Verifique o valor do pagamento na nota");
                return false;
            }
            else if (PaymentMode <= 0)
            {
                result.BadRequest("Selecione um método de pagamento");
                return false;
            }

            if(InstallmentsValues.Any(x => x > 0.0) || InstallmentsValuesFiscal.Any(x => x > 0.0) || 
                NumberInstallments > 0 || PaymentDay > 0 || !string.IsNullOrEmpty(FirstPaymentDate))
            {
                if (InstallmentsValues.Any(x => x <= 0.0))
                {
                    result.BadRequest("Verifique o valor da parcela");
                    return false;
                }
                else if (InstallmentsValuesFiscal.Any(x => x <= 0.0))
                {
                    result.BadRequest("Verifique o valor da parcela na nota");
                    return false;
                }
                else if (NumberInstallments <= 0)
                {
                    result.BadRequest("Verifique o número de parcelas");
                    return false;
                }
                else if (PaymentDay <= 0)
                {
                    result.BadRequest("Verifique o dia do mês de vencimento do pagamento");
                    return false;
                }
                else
                { 
                    if (string.IsNullOrEmpty(FirstPaymentDate))
                    {
                        result.BadRequest("Verifique a data do primeiro pagamento");
                        return false;
                    }
                    FirstPaymentDate = "01/" + FirstPaymentDate;
                    if (DateTime.TryParseExact(FirstPaymentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out FirstPaymentDateFormated))
                    {
                        if (FirstPaymentDateFormated < DateTime.Today)
                        {
                            result.BadRequest("Verifique a data");
                            return false;
                        }
                    }
                    else
                    {
                        result.BadRequest("Verifique se o formato da data está dd/mm/aa");
                        return false;
                    }
                }
            }
            else
            {
                //FirstPaymentDateFormated = null;
            }

            return true;
        }
    }
}
