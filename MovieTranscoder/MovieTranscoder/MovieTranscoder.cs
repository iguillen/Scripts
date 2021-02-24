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
        // Compile command
        // dotnet publish -c Release --self-contained true -p:PublishTrimmed=true -p:PublishSingleFile=true -p:IncludeAllContentsForSelfExtract=true -p:IncludeNativeLibrariesForSelfExtract=true -r win-x64
        [Command(Name = "reconcile", Description = "Reconcile transcoded movies")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public void Reconcile()
        {
            var root = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"M:" : "/mnt/m";
            var separator = Path.DirectorySeparatorChar;
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

        [Command(Name = "transcode", Description = "Reconcile transcoded movies and perform transcoding")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public void Transcode()
        {
            var root = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                root = @"M:";
            }
            else
            {
                Console.WriteLine("This command is only valid on Windows");
                return;
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

        [Command(Name = "cleanup", Description = "Remove blu-ray rips that have been transcoded")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public void Cleanup()
        {
            Console.WriteLine("This command not completely implemented");
            return;

            var root = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"M:" : "/mnt/m";

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

            var matchingMovies = movieFiles.Intersect(transcodeFiles)
                .OrderBy(x => x)
                .ToList();

            Console.WriteLine($"{matchingMovies.Count} to be deleted\n");

            var moviesToTranscode = new List<string>();

            foreach (var movie in matchingMovies)
            {
                moviesToTranscode.Add(movie);
                Console.WriteLine($"{movie} to be deleted");
            }
        }

        [Command(Name = "convert", Description = "Convert mkv files to mp4")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark memebers as static", Justification = "<Pending>")]
        public void Convert()
        {
            var root = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"M:" : "/mnt/m";

            var movieTranscodesPath = Path.Combine(new string[] { root, "Movie Transcodes" });

            Console.WriteLine("Getting list of transcoded movies with .mkv extension...\n");

            var transcodedFiles = Directory.GetFiles(movieTranscodesPath, "*.mkv")
                .Where(x => x.EndsWith(".mkv"))
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(x => x)
                .ToList();

            Console.WriteLine($"{transcodedFiles.Count} transcoded movies with .mkv extension found\n");

            var moviesToConvert = new List<string>();

            foreach (var movie in transcodedFiles)
            {
                moviesToConvert.Add(movie);
                Console.WriteLine($"Saved {movie} to be converted");
            }

            foreach(var movie in moviesToConvert)
            {
                Console.WriteLine($"\n\nStarting conversion of {movie}");

                var movieName = $"{movie}.mkv".Replace(" ", "\\ ");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"convert-video",
                        Arguments = $"\"{Path.Combine(new string[] { movieTranscodesPath, $"{movie}.mkv\"" })}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = false,
                        WorkingDirectory = movieTranscodesPath
                    }
                };

                process.Start();
                var result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Console.WriteLine(result);
                Console.WriteLine($"Finished converting {movie}\n");
            }
        }
    }
}
