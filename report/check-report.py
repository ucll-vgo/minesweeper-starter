import json
import sys
import os
import re


report_filename = 'report.json'

def abort(message):
    print(f'ERROR: {message}')
    sys.exit(-1)

def check_first_name(value):
    if not isinstance(value, str):
        abort('first-name should be string')

def check_last_name(value):
    if not isinstance(value, str):
        abort('last-name should be string')

def check_url(value):
    if not isinstance(value, str):
        abort('github-url should be string')
    if not re.fullmatch(r'https://github.com/.*', value):
        abort('github-url is invalid; it should start with https://github.com/')

def check_extensions(value):
    if not isinstance(value, list) or not all(isinstance(x, str) for x in value):
        abort('extensions should be array of strings')
    if len(value) < 3:
        print(f'Warning! You only have {len(value)} extensions!')


print(f"Looking for {report_filename}")
if not os.path.isfile(report_filename):
    abort(f"Failed to find {report_filename}")

print(f"Opening {report_filename}...")
with open(report_filename) as file:
    print(f"Parsing JSON...")
    try:
        data = json.load(file)
    except:
        abort(f"Failed to parse JSON file")

print("Checking contents...")

for key, checker in [ ('first-name', check_first_name), ('last-name', check_last_name), ('github-url', check_url), ('extensions', check_extensions) ]:
    if key not in data:
        abort(f'Missing {key}')
    value = data[key]
    checker(value)





print('SUCCESS')
print('You can upload your report now')