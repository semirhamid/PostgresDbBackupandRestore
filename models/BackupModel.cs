using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PostgresSQL.Backup.and.Restore
{
    public class BackupModel
    {
        public string OutputPath { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RestoreModel
    {
        public IFormFile BackupFile { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }


    public class PDbResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Errors { get; set; }
    }
}
