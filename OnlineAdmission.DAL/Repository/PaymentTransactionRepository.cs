﻿using Microsoft.EntityFrameworkCore;
using OnlineAdmission.DAL.IRepository;
using OnlineAdmission.DB;
using OnlineAdmission.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.DAL.Repository
{
    public class PaymentTransactionRepository : Repository<PaymentTransaction>, IPaymentTransactionRepository
    {
        public PaymentTransactionRepository(OnlineAdmissionDbContext db) : base(db)
        {

        }

        public async Task<PaymentTransaction> GetTransactionByNuRollAsync(int nuRoll)
        {
            return await _context.PaymentTransactions.FirstOrDefaultAsync(p => p.ReferenceNo == nuRoll);
            //return await _context.PaymentTransactions.FirstOrDefaultAsync(p => p.ReferenceNo == nuRoll && p.PaymentType == 1);
        }

        public async Task<bool> GetTransaction(List<PaymentTransaction> paymentTransactions)
        {
            await _context.PaymentTransactions.AddRangeAsync(paymentTransactions);
            return await _context.SaveChangesAsync() > 0;
            
        }

        public async Task<PaymentTransaction> GetApplicationTransactionByNuRollAsync(int nuRoll, int studentCategory)
        {
            return await _context.PaymentTransactions.FirstOrDefaultAsync(p => p.ReferenceNo == nuRoll && p.StudentCategory == studentCategory && p.PaymentType == 1);
        }

        public async Task<PaymentTransaction> GetAdmissionTrByNuRoll(int nuRoll, int studentCategory)
        {
            return await _context.PaymentTransactions.FirstOrDefaultAsync(p => p.ReferenceNo == nuRoll && p.StudentCategory == studentCategory && p.PaymentType == 2);
        }

        public async Task<PaymentTransaction> GetPaymentTransactionByTrId(string transactionId)
        {
            var existingTransaction = await _context.PaymentTransactions.FirstOrDefaultAsync(t => t.TransactionId.Trim().ToLower() == transactionId);
            return existingTransaction;
        }
    }
}
