#!/usr/bin/python3
# File name   : flaskUtil.py
# Description : utilities for flask

from flask import jsonify

def makeJsonResponse(status, responseData):
    """ create Json Response with status """
    response = jsonify(responseData)
    response.status_code = status
    return response
