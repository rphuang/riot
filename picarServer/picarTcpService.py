#!/usr/bin/python3
# File name   : picarTcpService.py
# Description : handles commands for PiCar

import os
import socket
import time
import threading
import argparse
from collections import deque
import cv2
import base64
import zmq

from piServices.piUtils import timePrint
import picarCommandHandler
import picarConst

def tcpVideoWorker(car, videoSocket, tracking=True):         # OpenCV and FPV video
    """ control the picar to follow an object """
    #car.tcpVideo(car, videoSocket, tracking=True)
    font = cv2.FONT_HERSHEY_SIMPLEX
    timePrint('Started TCP Video')
    time.sleep(1)
    picam = car.head.camera
    picam.camera.start(faceTracker=picam.faceTracker)
    while True:
        image = picam.camera.get_frame(tracking=tracking)
        #timePrint('processing image')

        cv2.line(image, (300, 240), (340, 240), (128, 255, 128), 1)
        cv2.line(image, (320, 220), (320, 260), (128, 255, 128), 1)

        dis = car.distance
        if dis < 8:
            cv2.putText(image,'%s m'%str(round(dis, 2)), (40,40), font, 0.5, (255,255,255), 1, cv2.LINE_AA)

        encoded, buffer = cv2.imencode('.jpg', image)
        jpg_as_text = base64.b64encode(buffer)
        videoSocket.send(jpg_as_text)

def ap_thread():             #Set up an AP-Hotspot
    os.system("sudo create_ap wlan0 eth0 AdeeptCar 12345678")

def scan(commandHandler, starth, startv, endh, endv, inch, incv):                  #Ultrasonic Scanning
    dis_dir=['list']         #Make a mark so that the client would know it is a list
    values, posh, posv = commandHandler.head.scan(starth, startv, endh, endv, inch, incv)
    for val in values:
        dis_dir.append(str(val))
    return dis_dir

ap_status = 1       # 0 to enable hotspot
wifi_status = 0     # wifi status
data = ''           # data buffer received from socket

def run(commandHandler, config):               #Main loop for tcp service
    global ap_status, wifi_status, data

    # load configuration
    motor_speed = config.getOrAddInt('tcpserver.motorSpeed', 100)
    spd_ad = config.getOrAddInt('tcpserver.motorSpeedAdjustment', 1)
    cam_turn_increment = config.getOrAddInt('tcpserver.camTurnIncrement', 4)

    HOST = ''
    PORT = config.getOrAddInt('tcpserver.tcpPort', 10223)       #Define port serial 
    BUFSIZ = 1024                             #Define buffer size
    ADDR = (HOST, PORT)

    tcpSerSock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    tcpSerSock.setsockopt(socket.SOL_SOCKET,socket.SO_REUSEADDR,1)
    tcpSerSock.bind(ADDR)
    tcpSerSock.listen(5)                      #Start server,waiting for client

    colorLower = (24, 100, 100)               #The color that openCV find
    colorUpper = (44, 255, 255)               #USE HSV value NOT RGB

    ap = argparse.ArgumentParser()            #OpenCV initialization
    ap.add_argument("-b", "--buffer", type=int, default=64,
        help="max buffer size")
    args = vars(ap.parse_args())
    pts = deque(maxlen=args["buffer"])
    time.sleep(0.1)

    # Process arguments
    parser = argparse.ArgumentParser()
    parser.add_argument('-c', '--clear', action='store_true', help='clear the display on exit')
    args = parser.parse_args()


    while True:              #Connection
        try:
            s =socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
            s.connect(("1.1.1.1",80))
            ipaddr_check=s.getsockname()[0]
            s.close()
            #print(ipaddr_check)
            wifi_status=1
        except:
            if ap_status == 0:
                ap_threading=threading.Thread(target=ap_thread)   #Define a thread for data receiving
                ap_threading.setDaemon(True)                          #'True' means it is a front thread,it would close when the mainloop() closes
                ap_threading.start()                                  #Thread starts
                commandHandler.drive.setLedsRGB(picarConst.YELLOW)
                time.sleep(2)
                wifi_status = 0
            
        if wifi_status == 1:
            timePrint('waiting for connection IP: ' + str(ipaddr_check))
            commandHandler.drive.setLedsRGB(picarConst.RED)
            tcpCliSock, addr = tcpSerSock.accept()#Determine whether to connect
            commandHandler.drive.setLedsRGB(picarConst.GREEN)
            print('...connected from :', addr)
            #time.sleep(1)
            #tcpCliSock.send(('SET %s'%vtr_mid+' %s'%hoz_mid+' %s'%left_spd+' %s'%right_spd+' %s'%look_up_max+' %s'%look_down_max).encode())
            #print('SET %s'%vtr_mid+' %s'%hoz_mid+' %s'%left_spd+' %s'%right_spd+' %s'%left+' %s'%right)
            break
        else:
            commandHandler.drive.setLedsRGB(picarConst.BLUE)
            timePrint('waiting for connection IP: ' + str(ipaddr_check))
            tcpCliSock, addr = tcpSerSock.accept()#Determine whether to connect
            commandHandler.drive.setLedsRGB(picarConst.GREEN)
            print('...connected from :', addr)
            #time.sleep(1)
            #tcpCliSock.send(('SET %s'%vtr_mid+' %s'%hoz_mid+' %s'%left_spd+' %s'%right_spd+' %s'%look_up_max+' %s'%look_down_max).encode())
            #print('SET %s'%vtr_mid+' %s'%hoz_mid+' %s'%left_spd+' %s'%right_spd+' %s'%left+' %s'%right)
            ap_status = 1
            break


    #FPV initialization
    videoPort = config.getOrAddInt('tcpserver.videoPort', 5555)
    context = zmq.Context()
    footage_socket = context.socket(zmq.PUB)
    footage_socket.connect('tcp://%s:'%addr[0] + str(videoPort))
    #Threads start
    timePrint('starting video thread port: %s client: '%videoPort + str(addr[0]))
    video_threading=threading.Thread(target=tcpVideoWorker, args=(commandHandler.car, footage_socket))      #Define a thread for FPV and OpenCV
    video_threading.setDaemon(True)                             #'True' means it is a front thread,it would close when the mainloop() closes
    video_threading.start()                                     #Thread starts


    while True: 
        data = ''
        data = tcpCliSock.recv(BUFSIZ).decode()
        if not data:
            continue
        elif 'exit' in data:
            tcpCliSock.send('shutting down PiCar'.encode())
            os.system("sudo shutdown -h now\n")

        elif 'reboot' in data:
            tcpCliSock.send('rebooting PiCar'.encode())
            os.system("sudo reboot\n")

        elif 'spdset' in data:
            spd_ad=float((str(data))[7:])      #Speed Adjustment
            config.set('tcpserver.motorSpeedAdjustment', spd_ad)
            tcpCliSock.send(('speed set to ' + spd_ad).encode())

        elif 'scan' in data:
            dis_can=scan(commandHandler, starth=-60, startv=0, endh=60, endv=0, inch=2, incv=2)                     #Start Scanning
            str_list_1=dis_can                 #Divide the list to make it samller to send 
            str_index=' '                      #Separate the values by space
            str_send_1=str_index.join(str_list_1)+' '
            tcpCliSock.sendall((str(str_send_1)).encode())   #Send Data
            tcpCliSock.send('finished'.encode())        #Sending 'finished' tell the client to stop receiving the list of dis_can

        elif 'scan_rev' in data:
            dis_can=scan(commandHandler, starth=60, startv=0, endh=-60, endv=0, inch=2, incv=2)                     #Start Scanning
            str_list_1=dis_can                 #Divide the list to make it samller to send 
            str_index=' '                      #Separate the values by space
            str_send_1=str_index.join(str_list_1)+' '
            tcpCliSock.sendall((str(str_send_1)).encode())   #Send Data
            tcpCliSock.send('finished'.encode())        #Sending 'finished' tell the client to stop receiving the list of dis_can

        elif 'EC1set' in data:                 # vertical servo center
            new_EC1=int((str(data))[7:])
            commandHandler.head.moveVertical(new_EC1)
            config.set('verticalServo.centerAngle', new_EC1)
            tcpCliSock.send('EC1set'.encode())

        elif 'EC2set' in data:                 # horizontal servo center
            new_EC2=int((str(data))[7:])
            commandHandler.head.moveHorizontal(new_EC2)
            config.set('horizontalServo.centerAngle', new_EC2)
            tcpCliSock.send('EC2set'.encode())

        elif 'EM1set' in data:                 # Motor Speed
            new_EM1=int((str(data))[7:])
            config.set('tcpserver.motorSpeed', new_EM1)
            tcpCliSock.send('EM1set'.encode())

        elif 'EM2set' in data:                 # Motor Speed
            new_EM2=int((str(data))[7:])
            config.set('tcpserver.motorSpeed', new_EM2)
            tcpCliSock.send('EM2set'.encode())

        elif 'LUMset' in data:                 # todo: look_up_max
            new_ET1=int((str(data))[7:])
            #replace_num('look_up_max:',new_ET1)
            #turn.camera_turn(new_ET1)
            tcpCliSock.send('LUMset'.encode())

        elif 'LDMset' in data:                 # todo: look_down_max
            new_ET2=int((str(data))[7:])
            #replace_num('look_down_max:',new_ET2)
            #turn.camera_turn(new_ET2)
            tcpCliSock.send('LDMset'.encode())

        elif 'stop' in data:                   #When server receive "stop" from client,car stops moving
            tcpCliSock.send('9 - stopping moving'.encode())
            commandHandler.drive.stop()
            continue
        
        elif 'lightsON' in data:               #Turn on the LEDs
            commandHandler.drive.setLedsRGB(picarConst.RGBON)
            tcpCliSock.send('lightsON'.encode())

        elif 'ledRainbowCycle' in data:        #Turn on rainbowCycle
            tcpCliSock.send('RainbowCycle'.encode())
            led_lightshow = 1

        elif 'ledTheaterChaseRainbow' in data:        #Turn on theaterChaseRainbow
            tcpCliSock.send('TheaterChaseRainbow'.encode())
            led_lightshow = 2

        elif 'ledColor' in data:        #Turn on leds with color - format: ledColor: rrr, ggg, bbb
            red=int((str(data))[10:13])
            green=int((str(data))[15:18])
            blue=int((str(data))[20:])
            colorWipe(strip, Color(red, green, blue))
            tcpCliSock.send(('ledColor: ' + str(red) + str(green) + str(blue)).encode())
            led_lightshow = 0

        elif 'lightsOFF'in data:               #Turn off the LEDs
            commandHandler.drive.setLedsRGB(picarConst.RGBOFF)
            tcpCliSock.send('lightsOFF'.encode())

        elif 'middle' in data:                 #Go straight
            commandHandler.drive.turnStraight()
            tcpCliSock.send('Going straight'.encode())
        
        elif 'Left' in data:                   #Turn left
            commandHandler.drive.turnLeft(45)
            tcpCliSock.send('3 - turning left'.encode())
        
        elif 'Right' in data:                  #Turn right
            commandHandler.drive.turnRight(45)
            tcpCliSock.send('4 - turning right'.encode())
        
        elif 'backward' in data:               #When server receive "backward" from client,car moves backward
            tcpCliSock.send('2 - going backward'.encode())
            commandHandler.drive.backward(motor_speed*spd_ad);

        elif 'forward' in data:                #When server receive "forward" from client,car moves forward
            tcpCliSock.send('1 - going forward'.encode())
            commandHandler.drive.forward(motor_speed*spd_ad)

        elif 'l_up' in data:                   #Camera look up
            angle = commandHandler.head.servoV.angle
            if angle < commandHandler.head.maxUpAngle:
                angle += cam_turn_increment
            commandHandler.head.moveVertical(angle)
            tcpCliSock.send('5 - move camera up'.encode())

        elif 'l_do' in data:                   #Camera look down
            angle = commandHandler.head.servoV.angle
            if angle > commandHandler.head.maxDownAngle:
                angle -= cam_turn_increment
            commandHandler.head.moveVertical(angle)
            tcpCliSock.send('6 - move camera down'.encode())

        elif 'l_le' in data:                   #Camera look left
            angle = commandHandler.head.servoH.angle
            if angle > commandHandler.head.maxLeftAngle:
                angle -= cam_turn_increment
            commandHandler.head.moveHorizontal(angle)
            msg = '7 - move camera left: ' + str(angle)
            timePrint(msg)
            tcpCliSock.send(msg.encode())

        elif 'l_ri' in data:                   #Camera look right
            angle = commandHandler.head.servoH.angle
            if angle < commandHandler.head.maxRightAngle:
                angle += cam_turn_increment
            commandHandler.head.moveHorizontal(angle)
            msg = '8 - move camera right: ' + str(angle)
            timePrint(msg)
            tcpCliSock.send(msg.encode())

        elif 'ahead' in data:                  #Camera look ahead
            commandHandler.head.lookStraight()
            tcpCliSock.send('recenter camera'.encode())

        elif 'Stop' in data:                   #When server receive "Stop" from client,Auto Mode switches off
            tcpCliSock.send('auto_status_off'.encode())
            commandHandler.car.setOperationMode(picarConst.ManualMode)
        
        elif 'auto' in data:                   #When server receive "auto" from client,start Auto Mode
            if commandHandler.car.setOperationMode(picarConst.FollowDistanceMode):
                tcpCliSock.send('0 - starting auto mode'.encode())
            continue

        elif 'wander' in data:                 #When server receive "wander" from client,start wander Mode
            if commandHandler.car.setOperationMode(picarConst.AutoWanderMode):
                tcpCliSock.send('wandering'.encode())
            continue

        elif 'findline' in data:               #Find line mode start
            if commandHandler.car.setOperationMode(picarConst.FollowLineMode):
                tcpCliSock.send('findline'.encode())
            continue

        elif 'voice_3' in data:                #Speech recognition mode start
            #if set_mode_speech():
            #    tcpCliSock.send('voice_3'.encode())
            continue

if __name__ == '__main__':

    try:
        run()
    except KeyboardInterrupt:
        if ap_status == 1:
            os.system("sudo shutdown -h now\n")
            time.sleep(5)
            print('shutdown')
        destroy()

