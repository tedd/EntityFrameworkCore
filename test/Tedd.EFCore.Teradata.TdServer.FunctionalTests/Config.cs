using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Teradata.Client.Provider;

namespace Microsoft.EntityFrameworkCore
{
    public class Config
    {
        public static readonly Config Current;
        public string DbUserId = "";
        public string DbPassword = "";
        public string DbDataSource = "";
        public string DbAuthenticationMechanism = "LDAP";

        private Config() { }

        static Config()
        {
            var filename = @"C:\Temp\TeradataTestConnectionInfo.json";
            if (File.Exists(filename))
                Current = JsonConvert.DeserializeObject<Config>(File.ReadAllText(filename));
            else
                throw new Exception("Missing config file: " + filename);
        }

        public static string GetConnectionString()
        {
            return new TdConnectionStringBuilder()
            {
                UserId = Current.DbUserId,
                Password = Current.DbPassword,
                DataSource = Current.DbDataSource,
                AuthenticationMechanism = Current.DbAuthenticationMechanism
            }.ConnectionString;

        }
    }
}
