using SysGuiApi.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.ViewModels
{
    public class CloseOrderViewModel
    {
        public int Id;
        public double CloseValue;
        public double CloseValueFiscal;

        public bool IsValid(ref ServiceResponse result)
        {
            CloseValue /= 100;
            CloseValueFiscal /= 100;

            if (CloseValue < 0.0)
            {
                result.BadRequest("Verifique o valor do pagamento");
                return false;
            }
            else if (CloseValueFiscal < 0.0)
            {
                result.BadRequest("Verifique o valor do pagamento na nota");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
