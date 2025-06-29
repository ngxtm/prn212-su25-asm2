using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NguyenTheMinhWPF.Utils
{
    public static class ConfigurationImport
    {
        private static readonly IConfigurationRoot _config;
        static ConfigurationImport()
        {
            _config = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .Build();
        }

        public static string AdminEmail => _config["AdminAccount:Email"];
        public static string AdminPassword => _config["AdminAccount:Password"];
    }
}
