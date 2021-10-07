using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.BLL.IManager
{
    public interface IPaymentTransactionManager : IManager<PaymentTransaction>
    {
        Task<bool> GetTransaction(List<PaymentTransaction> paymentTransactions);
        Task<PaymentTransaction> GetTransactionByNuRollAsync(int nuRoll);
        Task<PaymentTransaction> GetAdmissionTrByNuRoll(int nuRoll);
    }
}
