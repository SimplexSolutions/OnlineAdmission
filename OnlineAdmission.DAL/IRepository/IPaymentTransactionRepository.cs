using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.DAL.IRepository
{
    public interface IPaymentTransactionRepository : IRepository<PaymentTransaction>
    {
        Task<List<PaymentTransaction>> GetPaymentTransactionsAsync(int nuRoll, int studentCategoryId, int academicSessionId, int paymentTypeId);
        Task<bool> GetTransaction(List<PaymentTransaction> paymentTransactions);
        Task<PaymentTransaction> GetApplicationTransactionByNuRollAsync(int nuRoll, int studentCategory);
        Task<PaymentTransaction> GetAdmissionTrByNuRoll(int nuRoll, int studentCategory); 
        Task<PaymentTransaction> GetPaymentTransactionByTrId(string transactionId);
        Task<List<PaymentTransaction>> GetAllPaymentTrancsactionByNuRoll(int nuRoll);
    }
}
