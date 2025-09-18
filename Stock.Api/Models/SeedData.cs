using MassTransit;
using Microsoft.EntityFrameworkCore;
using static MassTransit.Logging.OperationName;

namespace Stock.Api.Models
{
    public static class SeedData
    {
        public static async Task AddSeedDataExt(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;


            if (!dbContext.Stocks.Any())
            {
                List<Stock> stocks =
                [
                    new ()
                    {
                        Id = 1,
                        Count = 100,
                        ProductId = 1
                    },
                    new ()
                    {
                        Id = 2,
                        Count = 100,
                        ProductId = 2
                    }



                ];


                dbContext.Stocks.AddRange(stocks);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
