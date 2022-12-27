
# PostgreSQL backup and restore tool

This package is used to backup and restore the postgresSQL database  

## Installation Instructions
Nuget package available (https://www.nuget.org/packages/Postgres.BackupandRestoreDatabase)
```
Install-Package Postgres.BackupandRestoreDatabase -Version 1.4.1
```
dotnet cli:
```
dotnet add package Postgres.BackupandRestoreDatabase --version 1.4.1
```
# Package usage
## 1. Register the service to Dependency Injection
```
services.AddPgBackupRestoreServices();
```
## 2. call the BackupDB method with path string(the storage location of the backup tar file) 
```
using PostgresSQL.Backup.and.Restore;
public class myClass
{
  private readonly IBackupRestorePDb _backupRestorePDb;
  
  public myClass(IBackupRestorePDb backupRestorePDb)
  {
    _backupRestorePDb = backupRestorePDb;
  }
}
```

## MIT License

The MIT License (MIT)

Copyright (c) <2022> Semir Hamid

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
