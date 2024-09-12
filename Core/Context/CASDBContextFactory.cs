using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace WCDS.WebFuncions.Core.Context
{
    public class CASDBContextFactory : IDesignTimeDbContextFactory<ApplicationDBContext>
    {
        public ApplicationDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDBContext>();
            var connectionString = Environment.GetEnvironmentVariable("casconnectionstring");
            optionsBuilder.UseOracle(connectionString);

            return new ApplicationDBContext(optionsBuilder.Options);
        }
    }
}