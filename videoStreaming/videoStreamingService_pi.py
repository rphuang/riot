import cv2
import sys
import videoStreamingService
from videoStreaming.camera_pi import Camera

if __name__ == '__main__':
    port = 8000
    if len(sys.argv) > 1:
        port = int(sys.argv[1])

    camera = Camera(width=1280, height=720)
    classifier = cv2.CascadeClassifier('data/haarcascade_frontalface_alt.xml')
    videoStreamingService.runVideoStreaming(port, camera, classifier, debug=True, threaded=True)
