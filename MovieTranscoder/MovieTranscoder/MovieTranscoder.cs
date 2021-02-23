using CommandDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace MovieTranscoder
{
    [Command(Description = "Transcode Movies for Plex")]
    public class MovieTranscoder
    {
        [Command(Description = "Reconcile transcoded movies")]
        public void Reconcile()
        {
            var root = string.Empty;
            var separator = Path.DirectorySeparatorChar;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                root = @"M:";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                root = "/mnt/m";
            }

            var moviesPath = Path.Combine(new string[] { root, "Movies" });
            var movieTranscodesPath = Path.Combine(new string[] { root, "Movie Transcodes" });

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

            var moviesToTranscode = new List<string>();

            foreach (var movie in nonMatchingMovies)
            {
                Console.WriteLine($"Detecting crop for {movie}...");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "detect-crop",
                        Arguments = $"\"{Path.Combine(new string[] { moviesPath, $"{movie}.mkv\"" })}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = false
                    }
                };

                process.Start();
                var result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Console.WriteLine(result);

                var transcodeCommand = result.Split(Environment.NewLine).ToList()[4];
                var transcodeCommandParts = transcodeCommand.Split(" ").ToList();

                if (!transcodeCommandParts[2].EndsWith(":0:0"))
                {
                    var parts = transcodeCommandParts[2].Split(":");
                    parts[2] = "0";
                    parts[3] = "0";
                    transcodeCommandParts[2] = string.Join(":", parts);
                }

                transcodeCommandParts.Insert(1, "--mp4");
                transcodeCommandParts.Insert(1, "--encoder nvenc_h264");
                transcodeCommand = string.Join(" ", transcodeCommandParts);

                moviesToTranscode.Add(transcodeCommand);

                File.AppendAllText("crops.sh", transcodeCommand);
                Console.WriteLine($"Saved {transcodeCommand}");
            }

            Console.WriteLine($"Saved {moviesToTranscode.Count} movies to transcode file");
        }

        public void Transcode()
        {
            var root = string.Empty;
            var separator = Path.DirectorySeparatorChar;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                root = @"M:";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                root = "/mnt/m";
            }

            var moviesPath = Path.Combine(new string[] { root, "Movies" });
            var movieTranscodesPath = Path.Combine(new string[] { root, "Movie Transcodes" });

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

            Console.WriteLine($"{movieFiles.Count} movies found\n");

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

            Console.WriteLine($"{transcodeFiles.Count} transcoded movies found\n");

            var nonMatchingMovies = movieFiles.Except(transcodeFiles)
                .OrderBy(x => x)
                .ToList();

            Console.WriteLine($"{nonMatchingMovies.Count} to be transcoded\n");

            var moviesToTranscode = new List<string>();

            foreach (var movie in nonMatchingMovies)
            {
                moviesToTranscode.Add(movie);
                Console.WriteLine($"Saved {movie} to be transcoded");
            }

            foreach (var movie in moviesToTranscode)
            {
                var movieName = $"{movie}.mkv".Replace(" ", "\\ ");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\Ruby27-x64\bin\other-transcode.bat",
                        Arguments = $"\"{Path.Combine(new string[] { moviesPath, $"{movie}.mkv" })}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = false,
                        WorkingDirectory = movieTranscodesPath
                    }
                };

                Console.WriteLine($"\n\n\nTranscoding {movie}...\n");

                process.Start();
                var result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Console.WriteLine(result);
                Console.WriteLine($"Finshed transcoding {movie}\n\n\n");
            }
        }
    }
}
