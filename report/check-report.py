import json
import sys
import os
import re


report_filename = 'report.json'



checkers = []


def checker(field):
    def receiver(func):
        checkers.append((field, func))
        return func
    return receiver


def abort(message):
    print(f'ERROR: {message}')
    sys.exit(-1)


@checker('first-name')
def check_first_name(value):
    if not isinstance(value, str):
        abort('first-name should be string')


@checker('last-name')
def check_last_name(value):
    if not isinstance(value, str):
        abort('last-name should be string')


@checker('github-url')
def check_url(value):
    if not isinstance(value, str):
        abort('github-url should be string')
    if not re.fullmatch(r'https://github.com/ucll-vgo2122/minesweeper-[^./]*', value):
        abort('github-url is invalid; it should have the form https://github.com/ucll-vgo2122/minesweeper-LOGIN')


@checker('extensions')
def check_extensions(value):
    if not isinstance(value, list) or not all(isinstance(x, str) for x in value):
        abort('extensions should be array of strings')
    if len(value) < 3:
        print(f'Warning! You only have {len(value)} extensions!')


@checker('framework')
def check_framework(value):
    if value.lower() not in [ 'wpf', 'avalonia' ]:
        abort("framework should be either WPF or Avalonia")


print(f"Looking for {report_filename}")
if not os.path.isfile(report_filename):
    abort(f"Failed to find {report_filename}")

print(f"Opening {report_filename}...")
with open(report_filename, encoding='utf-8-sig') as file:
    print(f"Parsing JSON...")
    try:
        data = json.load(file)
    except:
        abort(f"Failed to parse JSON file")


print("Checking contents...")

for key, checker in checkers:
    print(f"Checking {key}")
    if key not in data:
        abort(f'Missing {key}')
    value = data[key]
    checker(value)


def idiot_check():
    abort('Godverdomme Cooreman! Leert uw naam schrijven!')


data['first-name'] == 'Siebe' and data['last-name'] == 'Coorman' and idiot_check()



print('SUCCESS')
print('You can upload your report now')
