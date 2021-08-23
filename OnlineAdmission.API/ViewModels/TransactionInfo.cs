using OnlineAdmission.Entity;

namespace OnlineAdmission.API.ViewModels
{
    public class TransactionInfo
    {
        
        public string Status { get; set; }





        public int NuRoll { get; set; }
        public double Amount { get; set; }
        public string Name { get; set; }
        public string SubjectName { get; set; }
        public PaymentTransaction PaymentTransaction { get; set; }
    }
}
