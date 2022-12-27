using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PostgresSQL.Backup.and.Restore
{
    public class BackupRestorePDb : IBackupRestorePDb
    {

        PDbResult IBackupRestorePDb.backupDatabase(string outFile, string host, string port, string database, string user, string password)
        {
            string pgDumpPath = Path.GetFullPath("Programs/pg_dump.exe");
            String dumpCommand = "\"" + pgDumpPath + "\"" + " -Fc" + " -h " + host + " -p " + port + " -d " + database + " -U " + user + "";
            String passFileContent = "" + host + ":" + port + ":" + database + ":" + user + ":" + password + "";

            String batFilePath = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString() + ".bat");

            String passFilePath = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString() + ".conf");

            try
            {
                String batchContent = "";
                batchContent += "@" + "set PGPASSFILE=" + passFilePath + "\n";
                batchContent += "@" + dumpCommand + "  > " + "\"" + outFile + "\"" + "\n";

                System.IO.File.WriteAllText(
                    batFilePath,
                    batchContent,
                    Encoding.ASCII);

                System.IO.File.WriteAllText(
                    passFilePath,
                    passFileContent,
                    Encoding.ASCII);

                if (System.IO.File.Exists(outFile))
                    System.IO.File.Delete(outFile);

                ProcessStartInfo oInfo = new ProcessStartInfo(batFilePath);
                oInfo.UseShellExecute = false;
                oInfo.CreateNoWindow = true;

                using (Process proc = System.Diagnostics.Process.Start(oInfo))
                {
                    proc.WaitForExit();
                    proc.Close();
                }
                return new PDbResult()
                {
                    Success = true,
                    Message = "The database has been successfully backuped",
                    Errors = ""
                };
            }
            catch (Exception e)
            {
                return new PDbResult()
                {
                    Success = false,
                    Message = "Couldnot backup the database",
                    Errors = e.Message
                };
            }
            finally
            {
                if (System.IO.File.Exists(batFilePath))
                    System.IO.File.Delete(batFilePath);

                if (System.IO.File.Exists(passFilePath))
                    System.IO.File.Delete(passFilePath);
            }
        }

        private Task Execute(string dumpCommand)
        {
            return Task.Run(() =>
            {

                string batFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}." + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bat" : "sh"));
                try
                {
                    string batchContent = "";
                    batchContent += $"{dumpCommand}";

                    System.IO.File.WriteAllText(batFilePath, batchContent, Encoding.ASCII);

                    ProcessStartInfo info = ProcessInfoByOS(batFilePath);

                    using System.Diagnostics.Process proc = System.Diagnostics.Process.Start(info);


                    proc.WaitForExit();
                    var exit = proc.ExitCode;





                    proc.Close();
                }
                catch (Exception e)
                {
                    // Your exception handler here.

                }
                finally
                {
                    if (System.IO.File.Exists(batFilePath)) System.IO.File.Delete(batFilePath);
                }
            });
        }

        private static ProcessStartInfo ProcessInfoByOS(string batFilePath)
        {
            ProcessStartInfo info;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                info = new ProcessStartInfo(batFilePath)
                {
                };
            }
            else
            {
                info = new ProcessStartInfo("sh")
                {
                    Arguments = $"{batFilePath}"
                };
            }

            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            info.RedirectStandardError = true;

            return info;
        }

        public async Task<PDbResult> restoreDatabase(RestoreModel model)
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            var location = await this.UploadFile(model.BackupFile);
            startInfo.FileName = model.Database;
            string tempLocation = Path.GetFullPath("Temp");
            string inputFile = Path.Combine(tempLocation, location);
            string host = model.Host;
            string port = model.Port;
            string database = model.Database;
            string user = model.Username;
            string password = "root";


            string dumpCommand = $"set PGPASSWORD={password}\n" +
                                $"psql -h {host} -p {port} -U {user} -d {database} -c \"select pg_terminate_backend(pid) from pg_stat_activity where datname = '{database}'\"\n" +
                                $"dropdb -h " + host + " -p " + port + " -U " + user + $" {database}\n" +
                                $"createdb -h " + host + " -p " + port + " -U " + user + $" {database}\n" +
                                "pg_restore -h " + host + " -p " + port + " -d " + database + " -U " + user + "";

            //psql command disconnect database
            //dropdb and createdb  remove database and create.
            //pg_restore restore database with file create with pg_dump command
            dumpCommand = $"{dumpCommand} {inputFile}";

            try
            {
                await Execute(dumpCommand);
                return new PDbResult()
                {
                    Success = true,
                    Message = "Backup restored successfully"
                };


            }
            catch (Exception ex)
            {
                return new PDbResult()
                {
                    Success = false,
                    Errors = ex.Message,
                    Message = "Couldnot restore the Backup file"
                };
            }
            finally
            {
                this.DeleteFile(location);
            }
        }

        public async Task<string> UploadFile(IFormFile mediaModel)
        {
            string uniqueFileName = "";
            if (mediaModel != null)
            {
                string unTrimmedFileName;
                unTrimmedFileName = Guid.NewGuid().ToString() + "_" + mediaModel.FileName;
                uniqueFileName = unTrimmedFileName.Replace(' ', '-');
                var fileExt = Path.GetExtension(mediaModel.FileName).Substring(1);
                uniqueFileName = uniqueFileName.Replace('+', '-');
                string filePath = Path.Combine("Temp", uniqueFileName);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await mediaModel.CopyToAsync(fileStream);
                }
                return uniqueFileName;
            }
            return uniqueFileName;

        }

        public bool DeleteFile(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {

                var parentpath = Path.GetFullPath("StaticFiles/Materials");
                string fullpath = Path.Combine(parentpath, filename);
                File.Delete(fullpath);
                return true;
            }
            return false;
        }
    }

}
