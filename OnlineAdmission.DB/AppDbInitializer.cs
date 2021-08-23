using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OnlineAdmission.Entity;

namespace OnlineAdmission.DB
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicaitonBuilder)
        {
            using (var serviceScope = applicaitonBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<OnlineAdmissionDbContext>();
                if (!context.MeritStudents.Any())
                {
                    context.MeritStudents.AddRange(
                        new MeritStudent() { 
                            NUAdmissionRoll = 678134,
                            HSCRoll =113497,
                            SubjectCode = 1107,
                            MeritPosition = 112,
                            Comments = "Eligible for English",
                            PaymentStatus = false,
                            //PaidAmaount = 0.00
                        },
                        new MeritStudent()
                        {
                            NUAdmissionRoll = 113497,
                            HSCRoll = 121232,
                            SubjectCode = 1108,
                            MeritPosition = 345,
                            Comments = "Eligible for Bangla",
                            PaymentStatus = false,
                            //PaidAmaount = 0.00
                        });
                    context.SaveChanges();
                }
            }
        }
    }
}
