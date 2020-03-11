﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationServer.Infrastructure.Factory
{
    public class PersistedGrantDbContextFactory : DesigntimeDbContextFactoryBase<PersistedGrantDbContext>
    {
        protected override PersistedGrantDbContext CreateNewInstance(DbContextOptions<PersistedGrantDbContext> options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
            optionsBuilder.UseSqlServer(ConnectionString, sql => sql.MigrationsAssembly(this.GetType().GetTypeInfo().Assembly.GetName().Name));
            return new PersistedGrantDbContext(optionsBuilder.Options, new OperationalStoreOptions());
        }
    }
}
