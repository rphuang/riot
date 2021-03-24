#!/usr/bin/python3
# File name   : picarCamera.py
# Description : encapsulate the PI camera and video streaming with face tracking

import cv2
import base64
import time

from videoStreaming import camera_pi, faceTracking, videoStreamingService
from piServices.piUtils import *
from iotServerLib import piIotNode

class PiCarCamera(piIotNode.PiIotNode):
    """ encapsulates the PI camera and video streaming with face tracking """
    def __init__(self, name, parent, config, debug=False):
        """ construct a PicarCamera """
        super(PiCarCamera, self).__init__(name, parent)
        self.debug = debug
        width=config.getOrAddInt('camera.width', 640)
        height=config.getOrAddInt('camera.height', 480)
        enableFaceTracking = config.getOrAdd('camera.enableFaceTracking', 'true')
        drawCrosshair = config.getOrAdd('camera.drawCrosshair', 'true')
        self.camera = camera_pi.Camera(width=width, height=height, crosshair=drawCrosshair.startswith('true'))
        if enableFaceTracking.startswith('true'):
            filePath = config.getOrAdd('camera.classifier', '/home/pi/src/picarServer/data/haarcascade_frontalface_alt.xml')
            self.classifier = cv2.CascadeClassifier(filePath)
            #self.classifier = cv2.CascadeClassifier('/home/pi/adeept_picar-b/server/data/haarcascade_frontalface_alt.xml')
            self.faceTracker = faceTracking.FaceTracker(self.classifier, debug=self.debug)
            timePrint('created PicarCamera: ' + str(width) + ' x ' +str(height) + ' classifier: ' + filePath)
        else:
            self.faceTracker = None
            timePrint('created PicarCamera: ' + str(width) + ' x ' +str(height))

    def httpVideoStreaming(self, port):
        timePrint('starting httpVideoStreaming on port %d' %port)
        #videoStreamingService.runVideoStreaming(port, self.camera, tracker=self.faceTracker, debug=False, threaded=False, use_reloader=False, template_folder='/home/pi/adeept_picar-b/server/templates')
        runVideoStreaming(port, self.camera, tracker=self.faceTracker, debug=False, threaded=True)

from flask import Flask, render_template, Response
import sys

_app = Flask(__name__)

# the camera object (derived from BaseCamera) for video capture
_streamingCamera = None
# face tracking object (FaceTracker)
_faceTracker = None

def runVideoStreaming(port, camera, classifier=None, tracker=None, debug=False, threaded=True):
    """ run video streaming (flask app) as a web. calling parameters:
    port: the port number for the http web
    camera: a camera instance that is derived from base_camera.BaseCamera
    classifier: face tracking with FaceTracker using the specified classifier
    tracker: face tracking object (FaceTracker or instance of derived class)
    debug: whether to run the flask app under debug
    threaded: whether to run flask app threaded
    """
    global _streamingCamera, _faceTracker
    _streamingCamera = camera
    if tracker != None:
        _faceTracker = tracker
    elif classifier != None:
        _faceTracker = FaceTracker(classifier, debug=debug)
    _app.run(host='0.0.0.0', port=port, debug=debug, threaded=threaded, use_reloader=False)

@_app.route('/')
def index():
    """Video streaming home page."""
    return render_template('index.html')

def gen(camera):
    """Video streaming generator function."""
    while True:
        tracking = _faceTracker != None # and opencv_mode != 0
        img = camera.get_frame(tracking)

        # encode as a jpeg image and return it
        frame = cv2.imencode('.jpg', img)[1].tobytes()

        yield (b'--frame\r\n'
               b'Content-Type: image/jpeg\r\n\r\n' + frame + b'\r\n')

@_app.route('/video_feed')
def video_feed():
    """ Video streaming route. """
    _streamingCamera.start(_faceTracker)
    return Response(gen(_streamingCamera), mimetype='multipart/x-mixed-replace; boundary=frame')

if __name__ == '__main__':
    port = 8000
    if len(sys.argv) > 1:
        port = int(sys.argv[1])
    camera = Camera()
    classifier = cv2.CascadeClassifier('data/haarcascade_frontalface_alt.xml')
    runVideoStreaming(port, camera, classifier=classifier, debug=True, threaded=True)
