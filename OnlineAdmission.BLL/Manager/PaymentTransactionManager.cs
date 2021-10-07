using OnlineAdmission.BLL.IManager;
using OnlineAdmission.DAL.IRepository;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.BLL.Manager
{
    public class PaymentTransactionManager : Manager<PaymentTransaction>, IPaymentTransactionManager
    {
        private readonly IPaymentTransactionRepository paymentTransactionRepository;

        public PaymentTransactionManager(IPaymentTransactionRepository paymentTransactionRepository) : base(paymentTransactionRepository)
        {
            this.paymentTransactionRepository = paymentTransactionRepository;
        }

        public async Task<PaymentTransaction> GetTransactionByNuRollAsync(int nuRoll)
        {
            return await paymentTransactionRepository.GetTransactionByNuRollAsync(nuRoll);
        }

        public async Task<bool> GetTransaction(List<PaymentTransaction> paymentTransactions)
        {
            return await paymentTransactionRepository.GetTransaction(paymentTransactions);
        }

        public async Task<PaymentTransaction> GetAdmissionTrByNuRoll(int nuRoll)
        {
            return await paymentTransactionRepository.GetAdmissionTrByNuRoll(nuRoll);
        }
    }
}
