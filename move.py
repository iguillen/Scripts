#!/usr/bin/env python3
import os
import shutil

path = "/Volumes/5TB/tumbler"

print("Starting file move\n")

print("Getting list of folders...")
directories = os.listdir(path)
directories.pop(0)
print("Done getting list of folders\n\n")

count = len(directories)
total = count

for dir in directories:
    source = path + "/" + dir + "/media"
    print(source)
    destination = path + "/" + dir
    print(destination)

    print("Getting list of files...")
    try:
        files = os.listdir(source)
    except FileNotFoundError:
        print("Media folder doesn't exist")
        continue

    files.pop(0)
    print("Done getting list of files\n")

    filecount = len(files)
    filetotal = filecount

    for file in files:
        filesource = source + "/" + file
        dest = shutil.move(filesource, destination)

        filecount = filecount - 1
        print("Moved file [" + str(filecount) + "/" +
              str(filetotal) + "] " + filesource + " -> " + dest)

    print("Finished moving " + str(filetotal) + " files from " + source)
    count = count - 1
    print("Completed " + str(count) + " of " + str(total) + " folders\n\n")

print("Finished file move")
