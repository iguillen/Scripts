using CommandDotNet;
using System;

namespace MovieTranscoder
{
    class Program
    {
        static int Main(string[] args)
        {
            var appsettings = new AppSettings
            {
                IgnoreUnexpectedOperands = true
            };

            return new AppRunner<MovieTranscoder>(appsettings).Run(args);
        }
    }
}
