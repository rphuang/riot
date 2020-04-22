#!/usr/bin/python3
# File name   : requestContext.py
# Description : request data object

from flask import Request as FlaskRequest

class RequestContext(FlaskRequest):
    """ the Request contains the context data for a request
    the following properties are avialable:
    rootPath: the root path for the request (the first part of the path).
    paths: a list of sub-paths that split by '/'
    """
    def setPaths(self, paths):
        """ initialize Request object with list of paths
        paths: a list of sub-paths that split by '/'
        """
        self.paths = paths
        self.rootPath = paths[0]





