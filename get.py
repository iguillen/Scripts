#!/usr/bin/env python3
import praw
import os.path
import random

print("Starting script")
reddit = praw.Reddit(client_id='ASU4EGxPUwfjig', client_secret='Fs82YAU0i0XK_GpdY5yy81ExKzM', password='LCpmrwJVH7X93P7yQaDE',
                     user_agent='User-Agent: macos:com.test.getsubreddits:v0.1 \(by /u/esced\)', username='esced')

print("Getting followed subreddits")
subscribed_subreddits = list(reddit.user.subreddits(limit=None))
print("Done getting followed subreddits")

print("Getting followed users")
friends = list(reddit.user.friends())
print("Done getting followed users")

subs = []

print("Fixing urls for subreddits")
for sub in subscribed_subreddits:
    if "u_" in sub.display_name:
        subs.append('https://reddit.com/user/' +
                    sub.display_name.replace('u_', '') + '\n')
    else:
        subs.append('https://reddit.com/r/' + sub.display_name + '\n')
print("Done fixing urls for subreddits")

print("Fixing urls for users")
for friend in friends:
    subs.append('https://reddit.com/user/' + friend.name + '\n')
print("Done fixing urls for users")

#print("Sorting list")
# subs.sort()
#print("Done sorting list")

print("Shuffling list")
random.shuffle(subs)
print("Done shuffling list")

print("Writing list to file")
with open("subreddits.txt", "w") as f:
    for item in subs:
        f.write("%s" % item)
print("Done writing list to file")

print("Finished script")
