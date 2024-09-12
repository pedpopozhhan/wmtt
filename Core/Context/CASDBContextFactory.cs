using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace WCDS.WebFuncions.Core.Context
{
    public class CASDBContextFactory : IDesignTimeDbContextFactory<CASDBContext>
    {
        public CASDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CASDBContext>();
            var connectionString = Environment.GetEnvironmentVariable("casconnectionstring");
            optionsBuilder.UseOracle(connectionString);

            return new CASDBContext(optionsBuilder.Options);
        }
    }
}