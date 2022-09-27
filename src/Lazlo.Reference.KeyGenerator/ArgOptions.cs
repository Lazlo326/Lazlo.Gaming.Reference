using CommandLine.Text;
using CommandLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazlo.Reference.KeyGenerator
{
    class ArgOptions
    {
        [Option("outputpath",
            Default = @"C:\\temp",
            HelpText = "Output path.")]
        public string OutputPath { get; set; }

        [Option("pwd",
            Default = null,
            HelpText = "Password for private key.")]
        public string Pwd { get; set; }

        [Option("pwdconfirm",
            Default = null,
            HelpText = "ConfirmPassword for private key.")]
        public string PwdConfirm { get; set; }
    }
}
