import io
import time
import picamera
from picamera.array import PiRGBArray
from videoStreaming.base_camera import BaseCamera


class Camera(BaseCamera):
    width = 1280
    height = 720

    def __init__(self, width=1280, height=720):
        """ initialize a PiCam with specified width and height """
        Camera.width = width
        Camera.height = height
        self.cameraReady = False
        try:
            with picamera.PiCamera() as camera:
                self.cameraReady = True
                super(Camera, self).__init__()
        except:
            print("Failed to initialize picamera")

    @staticmethod
    def frames():
        with picamera.PiCamera() as camera:
            # let camera warm up
            time.sleep(1)
            camera.resolution = (Camera.width, Camera.height)
            rawCapture = PiRGBArray(camera, size=(Camera.width, Camera.height))

            for _ in camera.capture_continuous(rawCapture, 'bgr', use_video_port=True):
                img = rawCapture.array
                yield img

                # reset rawCapture for next frame
                rawCapture.truncate(0)

    def isOpened(self):
        """ whether the camera is ready and availabe """
        return self.cameraReady