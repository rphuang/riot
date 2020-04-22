import os
import cv2
from videoStreaming.base_camera import BaseCamera


class Camera(BaseCamera):
    video_source = 0
    width = 1280
    height =720

    def __init__(self, width=1280, height=720):
        if os.environ.get('OPENCV_CAMERA_SOURCE'):
            Camera.set_video_source(int(os.environ['OPENCV_CAMERA_SOURCE']))
        Camera.width = width
        Camera.height = height
        super(Camera, self).__init__()

    @staticmethod
    def set_video_source(source):
        Camera.video_source = source

    @staticmethod
    def frames():
        camera = cv2.VideoCapture(Camera.video_source)
        if not camera.isOpened():
            raise RuntimeError('Could not start camera.')

        camera.set(cv2.CAP_PROP_FRAME_WIDTH, Camera.width)
        camera.set(cv2.CAP_PROP_FRAME_HEIGHT, Camera.height)
        while True:
            # read current frame
            _, img = camera.read()
            yield img
