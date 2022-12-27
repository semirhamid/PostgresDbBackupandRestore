using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PostgresSQL.Backup.and.Restore
{
    public interface IBackupRestorePDb
    {
        PDbResult backupDatabase(string outFile, string host, string port, string database, string user, string password);
        Task<PDbResult> restoreDatabase(RestoreModel model);
    }
}