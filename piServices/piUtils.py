#!/usr/bin/python3
# File name   : piUtils.py
# Description : utility module for pi

from datetime import datetime

def timePrint(msg):
    """ print with time stamp """
    print(str(datetime.now()) + ' ' + msg)

def splitAndTrim(string, delimiter):
    """ split the string and remove all empty parts """
    parts = string.split(delimiter)
    results = []
    for item in parts:
        if len(item) == 0:
            continue
        results.append(item)
    return results

