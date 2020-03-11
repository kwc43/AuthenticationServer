using System;
using System.Collections.Generic;
using System.Text;
using AuthenticationServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationServer.Infrastructure.Factory
{
    public class AppIdentityDbContextFactory : DesigntimeDbContextFactoryBase<AppIdentityDbContext>
    {
        protected override AppIdentityDbContext CreateNewInstance(DbContextOptions<AppIdentityDbContext> options)
        {
            return new AppIdentityDbContext(options);
        }
    }
}
