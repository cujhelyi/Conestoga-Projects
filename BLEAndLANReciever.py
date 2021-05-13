import pycom
import socket
import machine
import time
from machine import Pin
from network import WLAN
from network import Bluetooth

pycom.heartbeat(False)
pycom.rgbled(0x0F0000)
p_right = Pin('P6', mode=Pin.OUT)
p_right.value(0)
p_left = Pin('P5', mode=Pin.OUT)
p_left.value(0)
p_steering = Pin('P12', mode=Pin.OUT)
p_steering.value(0)
p_go = Pin('P8', mode=Pin.OUT)
p_go.value(0)
p_rev = Pin('P11', mode=Pin.OUT)
p_rev.value(0)
p_override = Pin('P9', mode=Pin.OUT)
p_override.value(1)
p_check = Pin('P7', mode=Pin.OUT)
p_check.value(0)
p_eStop = Pin('P4', mode=Pin.OUT)
p_eStop.value(0)
p_free = Pin('P3', mode=Pin.OUT)
p_free.value(0)

wlan = WLAN(mode=WLAN.AP, ssid = "Kidsquad_car")
contConn = False
first = True
html = b'''<!DOCTYPE html>
<html>
<head>
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>Change Color</title>
<style>
div {
  -webkit-user-select: none; /* Safari */
  -ms-user-select: none; /* IE 10 and IE 11 */
  user-select: none; /* Standard syntax */
}
</style>
<script>
window.onload = () => {
    const el = document.getElementById('up');
    el.click = evt => fetch('/' + "works");
};
function UpFn() {
  fetch('/' + "u");
}
function DownFn() {
  fetch('/' + "d");
}
function LeftFn() {
  fetch('/' + "l");
}
function RightFn() {
  fetch('/' + "r");
}
function Cease() {
  fetch('/' + "c");
}
function EStop() {
  fetch('/' + "e");
}
function FreeRd() {
  fetch('/' + "f");
}
</script>
</head>
<body>
<div>
<button id='up' onpointerdown="UpFn()" onpointerleave="Cease()">Up</button>
<button id='down' onpointerdown="DownFn()" onpointerleave="Cease()">Down</button>
<button id='left' onpointerdown="LeftFn()" onpointerleave="Cease()">left</button>
<button id='right' onpointerdown="RightFn()" onpointerleave="Cease()">right</button>
<button id='stop' onpointerdown="EStop()">stop</button>
<button id='free' onpointerdown="FreeRd()">Free Ride</button>
</div>
</body>
</html>'''
def conn_cb (bt_o):
    events = bt_o.events()
    if  events & Bluetooth.CLIENT_CONNECTED:
        print("Client connected")
        #pycom.rgbled(0x00F000)
        p_check.value(1)
    elif events & Bluetooth.CLIENT_DISCONNECTED:
        print("Client disconnected")
        #pycom.rgbled(0xF00000)
        p_check.value(0)
        if p_eStop() == 0:
            print("Stopped")
            p_override.value(0)
        elif contConn == False:
            if p_free() == 0:
                print("Stopped")
                p_override.value(0)


def char1_cb_handler(chr):
    if  chr.events() & Bluetooth.CHAR_WRITE_EVENT:
        print("Write request with value = {}".format(chr.value()))
        if chr.value() == b'e':
            if p_eStop() == 0:
                p_override.value(0)
            else:
                p_override.value(1)
            p_eStop.toggle()
        elif p_eStop() == 0:
            if chr.value() == b'1':
                p_left.value(1)
                p_right.value(0)
                p_steering.value(1)
                p_override.value(0)
            elif chr.value() == b'2':
                p_right.value(1)
                p_left.value(0)
                p_steering.value(1)
                p_override.value(0)
            elif chr.value() == b'f':
                p_free.toggle()
            else:
                p_left.value(0)
                p_right.value(0)
                p_steering.value(0)
                p_override.value(1)
    else:
        print('Read request on char 1')
def char2_cb_handler(chr):
    # The value is not used in this callback as the WRITE events are not processed.
    if  chr.events() & Bluetooth.CHAR_WRITE_EVENT:
        print("Write request with value = {}".format(chr.value()))
        if p_eStop() == 0:
            if chr.value() == b'3':
                p_go.value(1)
                p_rev.value(0)
                p_override.value(0)
            elif chr.value() == b'4':
                p_rev.value(1)
                p_go.value(0)
                p_override.value(0)
            else:
                p_rev.value(0)
                p_go.value(0)
                p_override.value(1)
    else:
        print('Read request on char 2')
bluetooth = Bluetooth()
bluetooth.set_advertisement(name='LoPy', service_uuid=b'1234567890123456')

srv1 = bluetooth.service(uuid=b'1234567890123456', isprimary=True)
chr1 = srv1.characteristic(uuid=b'ab34567890123456', value=5)
char1_cb = chr1.callback(trigger=Bluetooth.CHAR_WRITE_EVENT | Bluetooth.CHAR_READ_EVENT, handler=char1_cb_handler)

srv2 = bluetooth.service(uuid=1234, isprimary=True)
chr2 = srv2.characteristic(uuid=4567, value=0x1234)
char2_cb = chr2.callback(trigger=Bluetooth.CHAR_READ_EVENT | Bluetooth.CHAR_WRITE_EVENT, handler=char2_cb_handler)

bluetooth.callback(trigger=Bluetooth.CLIENT_CONNECTED | Bluetooth.CLIENT_DISCONNECTED, handler=conn_cb)
bluetooth.advertise(True)

#Wifi Part
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind(('0.0.0.0', 80))
s.listen()
s.settimeout(5) #Checks if it needs to stop car if not pinged every 5 seconds
while True:
    print("waiting")
    try:
        conn, addr = s.accept()
        contConn = True
        first = False
        try:
            request = conn.recv(1024)
            lines = request.split(b'\r\n')
            method, uri, version = lines[0].split(b' ')
            if uri == b'/':
                conn.send(b'HTTP/1.1 200 OK\r\n\r\n')
                conn.send(html)
            elif uri == b'/favicon.ico':
                conn.send(b'HTTP/1.1 404 Not Found\r\n\r\n')
            elif uri == b'/e':
                if p_eStop() == 0:
                    p_go.value(0)
                    p_rev.value(0)
                    p_left.value(0)
                    p_right.value(0)
                    p_steering.value(0)
                    p_override.value(0)
                else:
                    p_override.value(1)
                p_eStop.toggle()
                conn.send(b'HTTP/1.1 200 OK\r\n\r\n')
            elif uri == b'/c':
                p_go.value(0)
                p_rev.value(0)
                p_left.value(0)
                p_right.value(0)
                p_steering.value(0)
                p_override.value(1)
                conn.send(b'HTTP/1.1 200 OK\r\n\r\n')
            elif p_eStop() == 0:
                if uri == b'/u':
                    print("up")
                    p_go.value(1)
                    p_rev.value(0)
                    p_override.value(0)
                    conn.send(b'HTTP/1.1 200 OK\r\n\r\n')
                elif uri == b'/d':
                    print("down")
                    p_go.value(0)
                    p_rev.value(1)
                    p_override.value(0)
                    conn.send(b'HTTP/1.1 200 OK\r\n\r\n')
                elif uri == b'/l':
                    print("left")
                    p_left.value(1)
                    p_right.value(0)
                    p_steering.value(1)
                    p_override.value(0)
                    conn.send(b'HTTP/1.1 200 OK\r\n\r\n')
                elif uri == b'/r':
                    print("right")
                    p_left.value(0)
                    p_right.value(1)
                    p_steering.value(1)
                    p_override.value(0)
                    conn.send(b'HTTP/1.1 200 OK\r\n\r\n')
                elif uri == b'/f':
                    print("free")
                    p_free.toggle()
                    conn.send(b'HTTP/1.1 200 OK\r\n\r\n')
                elif uri == b'/ping':
                    print("pinged")
                else:
                    print("reset")
                    p_go.value(0)
                    p_rev.value(0)
                    p_left.value(0)
                    p_right.value(0)
                    p_steering.value(0)
                    p_override.value(1)
                    conn.send(b'HTTP/1.1 200 OK\r\n\r\n')

        except:
            conn.send(b'HTTP/1.1 500 Internal Server Error')
        conn.close()
    except:
        if first == False:
            contConn = False
            print('pcheck ')
            print(p_check())
            if p_eStop() == 1:
                print("eStopped")
                p_override.value(0)
            elif p_check() == 0:
                print('pcheck ')
                print(p_check())
                if p_free() == 0:
                    print("Stopped")
                    p_override.value(0)

bluetooth.advertise(False)
