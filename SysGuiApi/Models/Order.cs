using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Client Client { get; set; }

        [Required]
        public User User { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        //Data criação do pedido
        public DateTime DateCreation { get; set; }

        //Data do primeiro pagamento da primeira parcela (null se não for parcelado)
        public DateTime DateFirstInstallmentPayment { get; set; }

        //Numero parcelas
        public int NumberInstallments { get; set; }

        //Numero parcelas pagas
        public int NumberInstallmentsPaid { get; set; }

        //Valor parcela
        public double []InstallmentValues { get; set; }

        //Valor parcela na nota
        public double []InstallmentValuesFiscal { get; set; }

        //Dia do mês de pagamento da parcela
        public int InstallmentPaymentDay { get; set; }

        //Valor da entrada OU pagamento efetuado à vista
        public double FirstPayment { get; set; }

        //Valor da entrada OU pagamento efetuado à vista na nota
        public double FirstPaymentFiscal { get; set; }

        //A entrada ou pagamento à vista já foi pago?
        public bool FirstPaymentPaid { get; set; }

        //Valor do fechamento
        public double CloseningPayment { get; set; }

        //Valor do fechamento na nota
        public double CloseningPaymentFiscal { get; set; }

        //Fechamento pago
        public bool CloseningPaid { get; set; }

        //Modo de pagamento
        public PaymentMode PaymentMode { get; set; }

        //Esquadrias ou vidraçaria?
        public Factory Factory { get; set; }
    }

    public enum PaymentMode
    {
        DINHEIRO,
        CHEQUE,
        BOLETO,
        CARTAO,
        DEPOSITO
    }

    public enum Factory
    {
        ESQUADRIAS,
        VIDRAÇARIA
    }
}
