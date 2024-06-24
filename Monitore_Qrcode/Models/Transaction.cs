namespace Monitore_Qrcode.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string GuId { get; set; }
        public string UserIdCustomer { get; set; }
        public string TransactionType { get; set; }
        public string AccountToCredit { get; set; }
        public string Amount { get; set; }
        public string Remarks { get; set; }
        public string IsGTBAccount { get; set; }
        public string IsOrangeMoney { get; set; }
        public string IsMoovMoney { get; set; }
        public string IsMTNMoney { get; set; }
        public string StatusQrCode { get; set; }
        public string InsertDate { get; set; }
    }
}
