using CommandDotNet;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace MovieTranscoder
{
    [Command(Description = "Transcode Movies for Plex")]
    public class MovieTranscoder
    {
        [Command(Description = "Reconcile transcoded movies")]
        public void Reconcile()
        {
            var moviesPath = @"M:\Movies";
            var movieTranscodesPath = @"M:\Movie Transcodes";

            Console.WriteLine("Getting list of movies...");

            var movieFiles = Directory.GetFiles(moviesPath)
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(x => x)
                .ToList();

            if (movieFiles.Contains(string.Empty))
            {
                var index = movieFiles.IndexOf(string.Empty);
                movieFiles.RemoveAt(index);
            }

            // Console.WriteLine(string.Join(Environment.NewLine, movieFiles));

            Console.WriteLine("Getting list of transcoded movies...");

            var transcodeFiles = Directory.GetFiles(movieTranscodesPath)
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(x => x)
                .ToList();

            if (transcodeFiles.Contains(string.Empty))
            {
                var index = transcodeFiles.IndexOf(string.Empty);
                transcodeFiles.RemoveAt(index);
            }

            var nonMatchingMovies = movieFiles.Except(transcodeFiles)
                .OrderBy(x => x)
                .ToList();

            foreach (var movie in nonMatchingMovies)
            {
                Console.WriteLine($"Detecting crop for {movie}...");

                var process = new Process();
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = "detect-crop";
                startInfo.Arguments($"{moviesPath}")
            }
        }
    }
}
