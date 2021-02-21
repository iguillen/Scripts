#!/usr/bin/env python3
import os
import shutil
import subprocess
import shlex

movies_path = "/mnt/m/Movies"
movie_transcodes_path = "/mnt/m/Movie Transcodes"

#print("Current working directory: " + os.getcwd())
# os.chdir(movie_transcodes_path)
#print("Changed current working directory to " + os.getcwd())

print("Getting list of movies...")

movie_files = os.listdir(movies_path)

if (".DS_Store" in movie_files):
    movie_files.remove(".DS_Store")

movie_files.sort()

movie_files = [x[:-4] for x in movie_files]

print("Getting list of transcoded movies...")

transcode_files = os.listdir(movie_transcodes_path)

if (".DS_Store" in transcode_files):
    transcode_files.remove(".DS_Store")

transcode_files.sort()

transcode_files = [x[:-4] for x in transcode_files]

non_matching_movies = list(set(movie_files) ^ set(transcode_files))
non_matching_movies.sort()

movies_to_transcode = []

f = open("crops.sh", "w")

for movie in non_matching_movies:
    print("Detecting crop for " + movie + "...")

    detect_crop = subprocess.run(shlex.split("detect-crop " + movies_path +
                                             "/\"" + movie + "\".mkv"), stdout=subprocess.PIPE, universal_newlines=True)

    transcode_command = detect_crop.stdout.splitlines()[4]
    transcode_command_parts = transcode_command.split()

    if transcode_command_parts[2].endswith(":0:0") == False:
        parts = transcode_command_parts[2].split(":")
        parts[2] = "0"
        parts[3] = "0"
        transcode_command_parts[2] = ":".join(parts)

    transcode_command_parts.insert(1, "--mp4")
    transcode_command_parts.insert(1, "--encoder nvenc_h264")
    transcode_command = ' '.join(transcode_command_parts)

    movies_to_transcode.append(transcode_command)
    f.write(transcode_command)
    print("Saved " + transcode_command)

print("Saved " + str(len(movies_to_transcode)) + " movies to transcode file")
