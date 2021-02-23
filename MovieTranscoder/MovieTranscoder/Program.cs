using CommandDotNet;
using System;

namespace MovieTranscoder
{
    class Program
    {
        static int Main(string[] args)
        {
            return new AppRunner<MovieTranscoder>().Run(args);
        }
    }
}
