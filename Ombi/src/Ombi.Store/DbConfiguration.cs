﻿#region Copyright
//  ***********************************************************************
//  Copyright (c) 2016 Jamie Rees
//  File: DbConfiguration.cs
//  Created By: Jamie Rees
// 
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
// 
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//  ***********************************************************************


#endregion

using System;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.PlatformAbstractions;


namespace Ombi.Store
{
    public class DbConfiguration : ISqliteConfiguration
    {
        //private static Logger Log = LogManager.GetCurrentClassLogger();
        public DbConfiguration(SqliteFactory provider)
        {
            Factory = provider;
        }

        private SqliteFactory Factory { get; }

        private string _currentPath;
        public string CurrentPath
        {
            get
            {

                if (!string.IsNullOrEmpty(_currentPath))
                {
                    return _currentPath;
                }

                if (File.Exists(OldPath))
                {
                    _currentPath = OldPath;
                    return _currentPath;
                }

                _currentPath = NewCurrentPath;

                return _currentPath;
            }
        }

        public string NewCurrentPath => Path.Combine(Path.GetDirectoryName(PlatformServices.Default.Application.ApplicationBasePath) ?? string.Empty, DbFile);
        public string OldPath => Path.Combine(Path.GetDirectoryName(PlatformServices.Default.Application.ApplicationBasePath) ?? string.Empty, OldDbFile);

        public virtual bool CheckDb()
        {
            //Log.Trace("Checking DB");
            Console.WriteLine("Location of the database: {0}",CurrentPath);
            if (File.Exists(CurrentPath))
            {
                return false;
            }
            //Log.Trace("DB doesn't exist, creating a new one");
            CreateDatabase();
            return true;
        }

        public const string OldDbFile = "PlexRequests.sqlite";
        public const string DbFile = "Ombi.sqlite";

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        /// <returns><see cref="IDbConnection"/></returns>
        /// <exception cref="System.Exception">Factory returned null</exception>
        public virtual IDbConnection DbConnection()
        {
            var fact = Factory.CreateConnection();
            if (fact == null)
            {
                throw new SqliteException("Factory returned null", -1);
            }
            fact.ConnectionString = "Data Source=" + CurrentPath;
            return fact;
        }

        /// <summary>
        /// Create the database file.
        /// </summary>
        public virtual void CreateDatabase()
        {
            try
            {
                using (File.Create(CurrentPath))
                {
                }
            }
            catch (Exception)
            {
                //Log.Error(e);
            }
        }


    }
}
