#!/usr/bin/env python3

import re

f = open("instagram.html", "r")
contents = f.read()
f.close

f = open("instagram.txt", "w")

titles = re.findall('(title="[a-zA-Z0-9-_.]*")', contents)

for t in titles:
    quoted = t.replace('title=', '')
    noquotes = quoted.strip('"')
    if noquotes != 'Verified':
        f.write('https://instagram.com/' + noquotes + '\n')

f.close
