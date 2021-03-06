using System;
using CommandLine;

namespace OpenFaaS.Hosting
{
    internal class RunnerOptions
    {
        [Option( 'p', "port", Default = 9000, HelpText = "Listening port" )]
        public int Port { get; set; }

        [Option( "config", Default = "config.json", HelpText = "Configuration file" )]
        public string Config { get; set; }

        [Option( "version", HelpText = "Display runner version" )]
        public bool Version { get; set; }
    }    
}
