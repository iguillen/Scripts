#!/usr/bin/env python3
import os.path
import shutil

#path = '/Volumes/8TB/4K Stogram/'
path = "/Volumes/8TB/rips/"

list_subfolders = [f.name for f in os.scandir(path) if f.is_dir()]

# with open('/Users/ike/Desktop/follows.txt') as f:
with open('/Volumes/8TB/subreddits.txt') as f:
    follows = f.readlines()

follows = [x.strip() for x in follows]
follows = [x.replace('https://reddit.com/r/', 'reddit_sub_') for x in follows]
follows = [x.replace('https://reddit.com/user/', 'reddit_user_') for x in follows]

to_remove = []

print('list_subfolders count:' + str(len(list_subfolders)))
print('follows count: ' + str(len(follows)))

for folder in list_subfolders:
    for follow in follows:
        if folder in follow:
            #print('Folder: ' + folder + ' found in follows list')
            break
    else:
        print('Folder: ' + folder +
              ' not found in follows list. Deleting...')
        #shutil.rmtree(path + folder)
        # print(to_remove)

# for follow in follows:
#     for folder in list_subfolders:
#         if folder == follow:
#             #print('Follow: ' + follow + ' found in folders list')
#             break
#     else:
#         print('Follow: ' + follow +
#               ' not found in folders list')
# print("Starting script")
# reddit = praw.Reddit(client_id='ASU4EGxPUwfjig', client_secret='Fs82YAU0i0XK_GpdY5yy81ExKzM', password='LCpmrwJVH7X93P7yQaDE',
#                      user_agent='User-Agent: macos:com.test.getsubreddits:v0.1 \(by /u/esced\)', username='esced')

# print("Getting followed subreddits")
# subscribed_subreddits = list(reddit.user.subreddits(limit=None))
# print("Done getting followed subreddits")

# print("Getting followed users")
# friends = list(reddit.user.friends())
# print("Done getting followed users")

# subs = []

# print("Fixing urls for subreddits")
# for sub in subscribed_subreddits:
#     if "u_" in sub.display_name:
#         subs.append('https://reddit.com/user/' +
#                     sub.display_name.replace('u_', '') + '\n')
#     else:
#         subs.append('https://reddit.com/r/' + sub.display_name + '\n')
# print("Done fixing urls for subreddits")

# print("Fixing urls for users")
# for friend in friends:
#     subs.append('https://reddit.com/user/' + friend.name + '\n')
# print("Done fixing urls for users")

# #print("Sorting list")
# #subs.sort()
# #print("Done sorting list")

# print("Shuffling list")
# random.shuffle(subs)
# print("Done shuffling list")

# print("Writing list to file")
# with open("subreddits.txt", "w") as f:
#     for item in subs:
#         f.write("%s" % item)
# print("Done writing list to file")

# print("Finished script")
