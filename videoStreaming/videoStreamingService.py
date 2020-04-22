from flask import Flask, render_template, Response
import cv2
import sys
try:
    from videoStreaming.camera_opencv import Camera
    from videoStreaming.faceTracking import FaceTracker
except ImportError:
    from camera_opencv import Camera
    from faceTracking import FaceTracker

_app = Flask(__name__)

# the camera object (derived from BaseCamera) for video capture
_streamingCamera = None
# face tracking object (FaceTracker)
_faceTracker = None

def runVideoStreaming(port, camera, classifier=None, tracker=None, debug=False, threaded=True, use_reloader=False):
    """ run video streaming (flask app) as a web. calling parameters:
    port: the port number for the http web
    camera: a camera instance that is derived from base_camera.BaseCamera
    classifier: face tracking with FaceTracker using the specified classifier
    tracker: face tracking object (FaceTracker or instance of derived class)
    debug: whether to run the flask app under debug
    threaded: whether to run flask app threaded
    use_reloader: disable reload in case of "ValueError: signal only works in main thread"
    """
    global _streamingCamera, _faceTracker
    _streamingCamera = camera
    if tracker != None:
        _faceTracker = tracker
    elif classifier != None:
        _faceTracker = FaceTracker(classifier, debug=debug)
    _app.run(host='0.0.0.0', port=port, debug=debug, threaded=threaded, use_reloader=use_reloader)

@_app.route('/')
def index():
    """Video streaming home page."""
    return render_template('index.html')

@_app.route('/camera')
def camera():
    """Camera image home page."""
    return render_template('camera.html')

@_app.route('/tracking')
def tracking():
    """Video streaming home page."""
    return render_template('tracking.html')

def gen(camera, tracking=False):
    """Video streaming generator function."""
    while True:
        img = camera.get_frame(tracking)

        # encode as a jpeg image and return it
        frame = cv2.imencode('.jpg', img)[1].tobytes()

        yield (b'--frame\r\n'
               b'Content-Type: image/jpeg\r\n\r\n' + frame + b'\r\n')

@_app.route('/video_feed')
def video_feed():
    """ Video streaming route. """
    _streamingCamera.start()
    return Response(gen(_streamingCamera),
                    mimetype='multipart/x-mixed-replace; boundary=frame')

@_app.route('/tracking_feed')
def tracking_feed():
    """ Video streaming route. """
    _streamingCamera.start(_faceTracker)
    return Response(gen(_streamingCamera, tracking=True),
                    mimetype='multipart/x-mixed-replace; boundary=frame')

@_app.route('/camera_jpeg')
def camera_jpeg():
    """ camera image jpeg. """
    _streamingCamera.start()
    img = _streamingCamera.current_frame(tracking=False)
    frame = cv2.imencode('.jpg', img)[1].tobytes()
    return Response(frame, mimetype='image/jpeg')

if __name__ == '__main__':
    port = 8000
    if len(sys.argv) > 1:
        port = int(sys.argv[1])
    camera = Camera()
    classifier = cv2.CascadeClassifier('data/haarcascade_frontalface_alt.xml')
    runVideoStreaming(port, camera, classifier, debug=True, threaded=True)
