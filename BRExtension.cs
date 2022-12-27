using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PostgresSQL.Backup.and.Restore
{
    public static class BRExtensions
    {
        public static IServiceCollection AddPgBackupRestoreServices(
            this IServiceCollection services)
        {
            services.AddScoped<IBackupRestorePDb, BackupRestorePDb>();

            return services;
        }
    }
}