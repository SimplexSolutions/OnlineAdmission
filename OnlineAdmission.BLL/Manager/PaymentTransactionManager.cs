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
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;

        public PaymentTransactionManager(IPaymentTransactionRepository paymentTransactionRepository) : base(paymentTransactionRepository)
        {
            _paymentTransactionRepository = paymentTransactionRepository;
        }

        public async Task<PaymentTransaction> GetAdmissionTrByNuRoll(int nuRoll, int studentCategory)
        {
            return await _paymentTransactionRepository.GetAdmissionTrByNuRoll(nuRoll, studentCategory);
        }

        public async Task<List<PaymentTransaction>> GetAllPaymentTrancsactionByNuRoll(int nuRoll)
        {
            return await _paymentTransactionRepository.GetAllPaymentTrancsactionByNuRoll(nuRoll);
        }

        public async Task<PaymentTransaction> GetApplicationTransactionByNuRollAsync(int nuRoll, int studentCategory)
        {
            return await _paymentTransactionRepository.GetApplicationTransactionByNuRollAsync(nuRoll, studentCategory);
        }

        public async Task<PaymentTransaction> GetPaymentTransactionByTrId(string transactionId)
        {
            return await _paymentTransactionRepository.GetPaymentTransactionByTrId(transactionId.Trim().ToLower());
        }

        public async Task<bool> GetTransaction(List<PaymentTransaction> paymentTransactions)
        {
            return await _paymentTransactionRepository.GetTransaction(paymentTransactions);
        }

    }
}
