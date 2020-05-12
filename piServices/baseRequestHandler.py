#!/usr/bin/python3
# File name   : baseRequestHandler.py
# Description : the base class for handling request

import traceback
from flask import jsonify, Response
from threading import RLock
from piServices.piUtils import timePrint
from piServices.flaskUtil import makeJsonResponse

class BaseRequestHandler(object):
    """ the base class for handling request """
    # registered instances of RequestHandler key: rootPath, value: RequestHandler
    _handlers = {}
    _lock = RLock()

    def __init__(self, name, rootPaths, authorizationList=None):
        """ constructor for BaseRequestHandler 
        name: the name for the handler
        rootPaths: list of root paths that will be handled by the instance
        authorizationList: a list of strings for authorized users
        """
        self.name = name
        if authorizationList == None or len(authorizationList) == 0:
            self.authorizationList = None
        else:
            self.authorizationList = authorizationList
        # register the handler
        with BaseRequestHandler._lock:
            for key in rootPaths:
                BaseRequestHandler._handlers[key.lower()] = self
        timePrint('Registered RequestHandler: %s rootPaths: %s' %(self.name, str(rootPaths)))

    def doGet(self, request):
        """ must be implemented by derived class to handle GET request 
        request: the RequestContext data object 
        must return a Flask.Response object
        """
        return makeJsonResponse(400, {'response': 'InvalidRequest'})

    def doPost(self, request):
        """ must be implemented by derived class to handle POST request 
        request: the RequestContext data object 
        must return a Flask.Response object
        """
        return makeJsonResponse(400, {'response': 'InvalidRequest'})

    def doPut(self, request):
        """ implemented by derived class to handle PUT request. 
        the default implementation is to call self.doPost()
        request: the RequestContext data object 
        must return a Flask.Response object
        """
        return self.doPost(request)

    def doDelete(self, request):
        """ must be implemented by derived class to handle Delete request 
        request: the RequestContext data object 
        must return a Flask.Response object
        """
        return makeJsonResponse(400, {'response': 'InvalidRequest'})

    def checkAuth(self, request):
        """ check authorization against self.authorizationList
        returns True if authorized.
        Override this method to implement custom authorization.
        """
        if self.authorizationList == None:
            return True
        if 'Authorization' in request.headers:
            auth = request.headers['Authorization']
            parts = auth.split(' ')
            authValue = parts[1]
            if authValue in self.authorizationList:
                return True
            else:
                return False
        else:
            return False

    @classmethod
    def processRequest(cls, request):
        """ process a request by finding the request handler """
        method = request.method
        key = request.rootPath.lower()
        if key in BaseRequestHandler._handlers:
            handler = BaseRequestHandler._handlers[key]
            timePrint('%s %s client: %s handler: %s' %(method, request.url, request.remote_addr, handler.name))
            try:
                if handler.checkAuth(request):
                    if method == 'GET':
                        response = handler.doGet(request)
                    elif method == 'POST':
                        response = handler.doPost(request)
                    elif method == 'PUT':
                        response = handler.doPut(request)
                    elif method == 'DELETE':
                        response = handler.doDelete(request)
                    else:
                        response = makeJsonResponse(403, {'response': 'InvalidMethod'})
                else:
                    response = makeJsonResponse(401, {'response': 'UnAuthorized'})
            except Exception as e:
                timePrint('Exception handling request: ' + str(e))
                traceback.print_exc()
                response = makeJsonResponse(500, { KeyResponse: 'Exception: ' + str(e) })
        elif len(key) == 0:
            # send available discovery paths if rootPath is not specified
            result = []
            for key in BaseRequestHandler._handlers:
                handler = BaseRequestHandler._handlers[key]
                result.append({'name': handler.name, 'path': key, 'url': request.host_url + key})
            response = makeJsonResponse(200, result)
        else:
            timePrint('%s %s client: %s handler: N.A.' %(method, request.url, request.remote_addr))
            response = makeJsonResponse(403, {'response': 'InvalidPath'})

        timePrint('%s %s client: %s status: %s' %(method, request.url, request.remote_addr, response.status_code))
        return response



