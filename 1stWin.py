import copy
import cv2
import numpy as np
import pyrealsense2 as rs
import sys
import yaml
from PyQt5 import QtCore, QtGui, QtWidgets
from tkinter import filedialog
from tkinter import *


class Ui_MainWindow(QtWidgets.QMainWindow):
    def __init__(self):
        super().__init__()
        self.setObjectName("MainWindow")
        self.resize(315, 389)
        self.centralwidget = QtWidgets.QWidget(self)
        self.centralwidget.setObjectName("centralwidget")
        self.makeNew = QtWidgets.QPushButton(self.centralwidget)
        self.makeNew.setGeometry(QtCore.QRect(70, 70, 161, 61))
        font = QtGui.QFont()
        font.setPointSize(18)
        self.makeNew.setFont(font)
        self.makeNew.setObjectName("makeNew")
        self.makeNew.clicked.connect(self.ClickNew)
        self.descText = QtWidgets.QLabel(self.centralwidget)
        self.descText.setGeometry(QtCore.QRect(0, 0, 241, 31))
        font = QtGui.QFont()
        font.setPointSize(15)
        self.descText.setFont(font)
        self.descText.setObjectName("descText")
        self.modOld = QtWidgets.QPushButton(self.centralwidget)
        self.modOld.setGeometry(QtCore.QRect(70, 175, 161, 61))
        self.modOld.clicked.connect(self.GetOld)
        font = QtGui.QFont()
        font.setPointSize(18)
        self.modOld.setFont(font)
        self.modOld.setObjectName("modOld")
        self.loadFam = QtWidgets.QPushButton(self.centralwidget)
        self.loadFam.setGeometry(QtCore.QRect(70, 280, 161, 61))
        font = QtGui.QFont()
        font.setPointSize(18)
        self.loadFam.setFont(font)
        self.loadFam.setObjectName("loadFam")
        self.setCentralWidget(self.centralwidget)
        self.loadFam.clicked.connect(self.getFam)
        self.chosen = "No"  # Changes based on what button the user presses
        self.quickLoad = False  # If true then the user wants to load a family

        self.retranslateUi(self)
        QtCore.QMetaObject.connectSlotsByName(self)

    def get_load(self):
        # Returns quickLoad
        return self.quickLoad

    def get_choice(self):
        # Returns chosen
        return self.chosen

    def ClickNew(self):
        # Closes window and sets chosen to New
        self.chosen = "New"
        self.close()

    def GetOld(self):
        # Gets the user to choose which recipe they want to edit
        root = Tk()
        self.chosen = filedialog.askopenfilename(initialdir="/", title="Select file",
                                                 filetypes=(("yaml files", "*.yaml"), ("all files", "*.*")))
        root.destroy()
        self.close()

    def getFam(self):
        # Loads only the screws from the destination
        self.quickLoad = True
        root = Tk()
        self.chosen = filedialog.askopenfilename(initialdir="/", title="Select file",
                                                 filetypes=(("yaml files", "*.yaml"), ("all files", "*.*")))
        root.destroy()
        self.close()

    def retranslateUi(self, MainWindow):
        _translate = QtCore.QCoreApplication.translate
        MainWindow.setWindowTitle(_translate("MainWindow", "MainWindow"))
        self.makeNew.setText(_translate("MainWindow", "Create New"))
        self.descText.setText(_translate("MainWindow", "Hard Drive Disassemble"))
        self.modOld.setText(_translate("MainWindow", "Modify Recipe"))
        self.loadFam.setText(_translate("MainWindow", "Load Family"))


class Edit_Window(QtWidgets.QMainWindow):
    def __init__(self):
        super().__init__()
        self.activeFrame = [None, None, None, None, None]  # RGB frames for every part of the hard drive
        self.activeDepth = [None, None, None, None, None]  # Depth frames for every part of the hard drive
        self.screws = [[], [], [], [], []]  # List of screws for each frame
        self.refFrame = [None, None, None, None, None]  # Active frame after zooming
        self.minBounds = [0, 0]  # The part of the image that's being shown
        self.scale = 1.0  # How zoomed in the picture is
        self.max = 0  # Maximum value of scrollbars
        self.zoomedImage = []  # The image being shown to the user
        self.intrinsics = {}  # The intrinsic values of the camera

        self.setObjectName("MainWindow")
        self.resize(1436, 763)
        self.centralwidget = QtWidgets.QWidget(self)
        self.centralwidget.setObjectName("centralwidget")
        self.imageLab = QtWidgets.QLabel(self.centralwidget)
        # self.imageLab.setGeometry(QtCore.QRect(140, 0, 640, 480))
        self.imageLab.setGeometry(QtCore.QRect(140, 0, 1280, 720))
        self.imageLab.setMouseTracking(True)
        self.imageLab.setText("")
        self.imageLab.setPixmap(QtGui.QPixmap("default_background.png"))
        self.imageLab.setScaledContents(True)
        self.imageLab.setObjectName("imageLab")
        self.tools = QtWidgets.QGroupBox(self.centralwidget)
        self.tools.setGeometry(QtCore.QRect(0, 10, 141, 611))
        font = QtGui.QFont()
        font.setPointSize(12)
        self.tools.setFont(font)
        self.tools.setObjectName("tools")
        self.title1 = QtWidgets.QLabel(self.tools)
        self.title1.setGeometry(QtCore.QRect(0, 70, 211, 16))
        self.title1.setObjectName("title1")
        self.title2 = QtWidgets.QLabel(self.tools)
        self.title2.setGeometry(QtCore.QRect(0, 160, 211, 16))
        self.title2.setObjectName("title2")
        self.screwBox = QtWidgets.QComboBox(self.tools)
        self.screwBox.setGeometry(QtCore.QRect(0, 180, 141, 81))
        self.screwBox.setObjectName("screwBox")
        self.addButton = QtWidgets.QPushButton(self.tools)
        self.addButton.setGeometry(QtCore.QRect(0, 350, 141, 31))
        self.addButton.setObjectName("addButton")
        self.title5 = QtWidgets.QLabel(self.tools)
        self.title5.setGeometry(QtCore.QRect(0, 440, 211, 16))
        self.title5.setObjectName("title5")
        self.refButton = QtWidgets.QPushButton(self.tools)
        self.refButton.setGeometry(QtCore.QRect(0, 30, 141, 31))
        self.refButton.setObjectName("refButton")
        self.xCoord = QtWidgets.QTextEdit(self.tools)
        self.xCoord.setGeometry(QtCore.QRect(0, 310, 61, 31))
        self.xCoord.setObjectName("xCoord")
        self.yCoord = QtWidgets.QTextEdit(self.tools)
        self.yCoord.setGeometry(QtCore.QRect(80, 310, 61, 31))
        self.yCoord.setObjectName("yCoord")
        self.title3 = QtWidgets.QLabel(self.tools)
        self.title3.setGeometry(QtCore.QRect(20, 280, 16, 16))
        self.title3.setObjectName("title3")
        self.title4 = QtWidgets.QLabel(self.tools)
        self.title4.setGeometry(QtCore.QRect(100, 280, 21, 16))
        self.title4.setObjectName("title4")
        self.delButton = QtWidgets.QPushButton(self.tools)
        self.delButton.setGeometry(QtCore.QRect(0, 380, 141, 31))
        self.delButton.setObjectName("delButton")
        self.insButton = QtWidgets.QPushButton(self.tools)
        self.insButton.setGeometry(QtCore.QRect(0, 570, 141, 31))
        self.insButton.setObjectName("insButton")
        self.partBox = QtWidgets.QComboBox(self.tools)
        self.partBox.setGeometry(QtCore.QRect(0, 100, 141, 51))
        self.partBox.setObjectName("partBox")
        self.partBox.addItem("")
        self.partBox.addItem("")
        self.partBox.addItem("")
        self.partBox.addItem("")
        self.partBox.addItem("")
        self.codeBox = QtWidgets.QTextEdit(self.tools)
        self.codeBox.setGeometry(QtCore.QRect(0, 460, 141, 31))
        self.codeBox.setObjectName("codeBox")
        self.saveButton = QtWidgets.QPushButton(self.tools)
        self.saveButton.setGeometry(QtCore.QRect(0, 530, 141, 31))
        self.saveButton.setObjectName("saveButton")
        self.loadButton = QtWidgets.QPushButton(self.tools)
        self.loadButton.setGeometry(QtCore.QRect(0, 500, 141, 31))
        self.loadButton.setObjectName("loadButton")
        self.errorLabel = QtWidgets.QLabel(self.centralwidget)
        self.errorLabel.setGeometry(QtCore.QRect(0, 740, 1401, 21))
        font = QtGui.QFont()
        font.setPointSize(12)
        self.errorLabel.setFont(font)
        self.errorLabel.setObjectName("errorLabel")
        self.horizontalScrollBar = QtWidgets.QScrollBar(self.centralwidget)
        self.horizontalScrollBar.setGeometry(QtCore.QRect(160, 720, 1281, 16))
        self.horizontalScrollBar.setOrientation(QtCore.Qt.Horizontal)
        self.horizontalScrollBar.setObjectName("horizontalScrollBar")
        self.horizontalScrollBar.setMaximum(self.max)
        self.verticalScrollBar = QtWidgets.QScrollBar(self.centralwidget)
        self.verticalScrollBar.setGeometry(QtCore.QRect(1420, 20, 16, 701))
        self.verticalScrollBar.setOrientation(QtCore.Qt.Vertical)
        self.verticalScrollBar.setObjectName("verticalScrollBar")
        self.verticalScrollBar.setMaximum(self.max)

        self.refButton.clicked.connect(self.refresh)
        self.insButton.clicked.connect(self.instructions)
        self.addButton.clicked.connect(self.addScrew)
        self.delButton.clicked.connect(self.delScrew)
        self.xCoord.textChanged.connect(self.map)
        self.yCoord.textChanged.connect(self.map)
        self.screwBox.highlighted[int].connect(self.boxMap)
        self.partBox.currentIndexChanged.connect(self.switchFrame)
        self.saveButton.clicked.connect(self.save)
        self.loadButton.clicked.connect(self.getFile)
        self.horizontalScrollBar.valueChanged.connect(self.frameZoom)
        self.verticalScrollBar.valueChanged.connect(self.frameZoom)

        self.setCentralWidget(self.centralwidget)
        self.retranslateUi(self)
        QtCore.QMetaObject.connectSlotsByName(self)

    def setImage(self):
        # Changes the main image
        if self.activeFrame[self.partBox.currentIndex()] is not None:
            self.zoomedImage = copy.deepcopy(self.activeFrame[self.partBox.currentIndex()])
            height, width, channel = self.activeFrame[self.partBox.currentIndex()].shape
            step = channel * width
            qImg = QtGui.QImage(self.activeFrame[self.partBox.currentIndex()].data, width, height, step,
                                QtGui.QImage.Format_RGB888)
            self.imageLab.setPixmap(QtGui.QPixmap.fromImage(qImg))
        else:
            self.refresh()

    def getFile(self):
        # Tries to get the file based on the bar code the user has entered. If it doesn't find the file,
        # the user is prompted to find it on their own
        try:
            open(self.codeBox.toPlainText(), 'r')
            self.load()
        except FileNotFoundError:
            root = Tk()
            path = filedialog.askopenfilename(initialdir="/", title="Select file",
                                              filetypes=(("yaml files", "*.yaml"), ("all files", "*.*")))
            if path != '':
                self.codeBox.setText(path)
                self.load()
            root.destroy()

    def load(self):
        # Loads all data from Yaml file
        with open(self.codeBox.toPlainText(), 'r') as file:
            yamlFile = yaml.load(file, Loader=yaml.Loader)
            self.codeBox.setText(yamlFile["barcode"])
            self.screws = yamlFile["screws"]
            self.intrinsics = {"fx": yamlFile["intrinsics"][0], "fy": yamlFile["intrinsics"][1], "ppx":
                yamlFile["intrinsics"][2], "ppy": yamlFile["intrinsics"][3]}
            # Convert real world points to pixel points
            for part in self.screws:
                for point in part:
                    point[0] = int((point[0] * self.intrinsics["fx"] / point[2]) + self.intrinsics["ppx"])
                    point[1] = int((point[1] * self.intrinsics["fy"] / point[2]) + self.intrinsics["ppy"])
            self.activeFrame = yamlFile["rgbs"]
            self.activeDepth = yamlFile["depth"]
            self.partBox.setCurrentIndex(2)
            self.errorLabel.setText("Message: Load Successful")

    def loadFamily(self):
        # Loads only the screw data from Yaml file
        with open(self.codeBox.toPlainText(), 'r') as file:
            yamlFile = yaml.load(file, Loader=yaml.Loader)
            self.screws = yamlFile["screws"]
            self.intrinsics = {"fx": yamlFile["intrinsics"][0], "fy": yamlFile["intrinsics"][1], "ppx":
                yamlFile["intrinsics"][2], "ppy": yamlFile["intrinsics"][3]}
            # Convert real world points to pixel points
            for part in self.screws:
                for point in part:
                    point[0] = int((point[0] * self.intrinsics["fx"] / point[2]) + self.intrinsics["ppx"])
                    point[1] = int((point[1] * self.intrinsics["fy"] / point[2]) + self.intrinsics["ppy"])
            self.codeBox.setText("")
            self.partBox.setCurrentIndex(2)
            self.errorLabel.setText("Message: Loaded Family")

    def save(self):
        # Checks that all required info is there then saves to a yaml file named after what is in the barcode box
        if self.codeBox.toPlainText() != "":
            # Convert pixel points to real world points
            if self.intrinsics is not None:
                realScrews = copy.deepcopy(self.screws)  # Don't want to alter the actual screw list
                for part in realScrews:
                    for point in part:
                        point[0] = (point[0] - self.intrinsics["ppx"]) * point[2] / self.intrinsics["fx"]
                        point[1] = (point[1] - self.intrinsics["ppy"]) * point[2] / self.intrinsics["fy"]
                # Save data
                tempInt = [self.intrinsics["fx"], self.intrinsics["fy"], self.intrinsics["ppx"], self.intrinsics["ppy"]]
                # Non dictionary version of intrinsics
                yamlFile = {"barcode": self.codeBox.toPlainText(),
                            "numScrews": ["Electronicboard Top Plate = " + str(len(self.screws[1])),
                                          "Plate Cover = " + str(len(self.screws[2])),
                                          "Motor Platter Cover = " + str(len(self.screws[3])),
                                          "VoiceRecorder Magnet Cover = " + str(len(self.screws[4]))],
                            "screws": realScrews, "rgbs": self.activeFrame, "depth": self.activeDepth,
                            "intrinsics": tempInt}
                with open(self.codeBox.toPlainText() + '.yaml', 'w') as file:
                    yaml.dump(yamlFile, file)
                self.errorLabel.setText("Message: Save successful")
        else:
            self.errorLabel.setText("Error Message: Enter the barcode into the barcode box")

    def map(self):
        # If the user clicks on the image, this puts a red and blue target there
        if self.activeFrame[self.partBox.currentIndex()] is not None:

            if self.xCoord.toPlainText() != '' and self.yCoord.toPlainText() != '':
                self.errorLabel.setText("Message:")
                try:
                    coords = (int((int(self.xCoord.toPlainText()) - self.minBounds[0]) * self.scale),
                              int((int(self.yCoord.toPlainText()) - self.minBounds[1]) * self.scale))
                    # coords stores coordinates of the screw on the zoomed image
                    tempimage = cv2.circle(copy.deepcopy(self.zoomedImage), coords, int(8 * self.scale),
                                           (0, 0, 255), 1)  # Image with the blue target on it
                    tempimage = cv2.circle(tempimage, coords, int(1 * self.scale), (255, 0, 0), -1)
                    # Convert the numpy array to a QImage
                    height, width, channel = tempimage.shape
                    step = channel * width
                    qImg = QtGui.QImage(tempimage.data, width, height, step, QtGui.QImage.Format_RGB888)
                    self.imageLab.setPixmap(QtGui.QPixmap.fromImage(qImg))
                except ValueError:
                    self.errorLabel.setText("Error Message: The coordinates should be positive integers")
        else:
            self.errorLabel.setText("Error Message: Press refresh first")

    def boxMap(self, index):
        # If the user highlights a screw from the combobox that is currently onscreen, this puts a green target there
        if index < len(self.screws[self.partBox.currentIndex()]):
            if self.minBounds[0] <= int(self.screws[self.partBox.currentIndex()][index][0]) <= self.minBounds[0] + \
                    1280 / self.scale and self.minBounds[1] <= int(self.screws[self.partBox.currentIndex()][index][1]) \
                    <= self.minBounds[1] + 720 / self.scale:
                coords = (int((int(self.screws[self.partBox.currentIndex()][index][0]) -
                               self.minBounds[0]) * self.scale),
                          int((int(self.screws[self.partBox.currentIndex()][index][1]) -
                               self.minBounds[1]) * self.scale))
                tempimage = cv2.circle(copy.deepcopy(self.zoomedImage), coords, int(8 * self.scale), (0, 255, 0), 1)
                tempimage = cv2.circle(tempimage, coords, int(1 * self.scale), (0, 255, 0), -1)
                height, width, channel = tempimage.shape
                step = channel * width
                qImg = QtGui.QImage(tempimage.data, width, height, step, QtGui.QImage.Format_RGB888)
                self.imageLab.setPixmap(QtGui.QPixmap.fromImage(qImg))
            else:
                height, width, channel = self.zoomedImage.shape
                step = channel * width
                qImg = QtGui.QImage(self.zoomedImage.data, width, height, step, QtGui.QImage.Format_RGB888)
                self.imageLab.setPixmap(QtGui.QPixmap.fromImage(qImg))

    def frameZoom(self):
        # Displays a 1280x720 part of the expanded image based on the position of the scrollbars
        frameMid = [-1, -1]  # Midpoint for the frame that will be displayed
        # If the scrollbar is at the end then the script will display the end otherwise there may be a row missing
        # due to float rounding
        if self.horizontalScrollBar.value() >= self.max:
            frameMid[0] = self.refFrame[self.partBox.currentIndex()].shape[1] - 640
        else:
            distApart = (self.refFrame[self.partBox.currentIndex()].shape[1] - 1280) / self.max
            # Distance between each zoomed frame. Never greater or equal to the length of a zoomed frame
            frameMid[0] = int(640 + distApart * self.horizontalScrollBar.value())
        if self.verticalScrollBar.value() >= self.max:
            frameMid[1] = self.refFrame[self.partBox.currentIndex()].shape[0] - 360
        else:
            distApart = (self.refFrame[self.partBox.currentIndex()].shape[0] - 720) / self.max
            frameMid[1] = int(360 + distApart * self.verticalScrollBar.value())
        self.zoomedImage = copy.deepcopy(self.refFrame[self.partBox.currentIndex()]
                                         [frameMid[1] - 360: frameMid[1] + 360, frameMid[0] - 640: frameMid[0] + 640])
        height, width, channel = self.zoomedImage.shape
        step = channel * width
        qImg = QtGui.QImage(self.zoomedImage.data, width, height, step,
                            QtGui.QImage.Format_RGB888)
        self.imageLab.setPixmap(QtGui.QPixmap.fromImage(qImg))
        self.minBounds = [(frameMid[0] - 640) / self.scale, (frameMid[1] - 360) / self.scale]

    def wheelEvent(self, event):
        # Zooms in if wheel moves forward, out if wheel moves backward, and adjusts the scroll bars based on the
        # quadrant the mouse was in when the wheel was moved
        try:
            scroll = event.angleDelta().y()  # Positive if wheel was moved forward
            center = (event.position().x(), event.position().y())  # Position of the mouse during wheel movement
            if scroll > 0:
                self.max = self.max + 1
                self.scale = self.scale * 1.25
                self.refFrame[self.partBox.currentIndex()] = cv2.resize(self.refFrame[self.partBox.currentIndex()],
                                                                        None, None, 1.25, 1.25, cv2.INTER_CUBIC)
            else:
                if self.scale == 1.0:
                    pass
                else:
                    self.max = self.max - 1
                    self.scale = self.scale * 0.8
                    self.refFrame[self.partBox.currentIndex()] = cv2.resize(self.refFrame[self.partBox.currentIndex()],
                                                                            None, None, 0.8, 0.8, cv2.INTER_AREA)
            self.horizontalScrollBar.setMaximum(self.max), self.verticalScrollBar.setMaximum(self.max)
            # Checks the quadrant and calls frameZoom
            if center[0] > 640:
                self.horizontalScrollBar.setValue(self.horizontalScrollBar.value() + 1)
                if center[1] > 360:
                    self.verticalScrollBar.setValue(self.verticalScrollBar.value() + 1)
                else:
                    self.frameZoom()

            else:
                if center[1] > 360:
                    self.verticalScrollBar.setValue(self.verticalScrollBar.value() + 1)
                else:
                    self.frameZoom()
        # If the user doesn't refresh there is no image to expand yet
        except TypeError:
            self.errorLabel.setText("Error Message: Refresh before zooming")

    def mousePressEvent(self, event):
        # If the user clicks on the image the place clicked is shown in the x/y boxes
        if event.x() > 140 and event.y() < 721:
            self.xCoord.setText(str(int((event.x() - 140) / self.scale + self.minBounds[0])))
            self.yCoord.setText(str(int(event.y() / self.scale + self.minBounds[1])))

    def instructions(self):
        # Replaces the current image with one that shows what that says how to use the gui
        self.imageLab.setPixmap(QtGui.QPixmap("default_background.png"))

    def refresh(self):
        # Replaces current image with a fresh one from the camera
        try:
            frames = pipeline.wait_for_frames()
            frames = align.process(frames)
            if self.partBox.currentIndex() != 0:
                self.activeDepth[self.partBox.currentIndex()] = hole_filling.process(frames.get_depth_frame())
                self.activeDepth[self.partBox.currentIndex()] = np.asanyarray(
                    self.activeDepth[self.partBox.currentIndex()].get_data())
                self.activeFrame[self.partBox.currentIndex()] = np.asanyarray(frames.get_color_frame().get_data())
                self.activeFrame[self.partBox.currentIndex()] = cv2.cvtColor(
                    self.activeFrame[self.partBox.currentIndex()],
                    cv2.COLOR_BGR2RGB)
                camIntrinsics = frames.get_profile().as_video_stream_profile().get_intrinsics()
                self.intrinsics = {"fx": camIntrinsics.fx, "fy": camIntrinsics.fy, "ppx":
                    camIntrinsics.ppx, "ppy": camIntrinsics.ppy}
                self.refFrame[self.partBox.currentIndex()] = copy.deepcopy(
                    self.activeFrame[self.partBox.currentIndex()])
                self.setImage()
                self.scale = 1.0
                self.minBounds = [0, 0]
                self.errorLabel.setText("Message:")
            else:
                tempImage = np.asanyarray(frames.get_color_frame().get_data())
                tempImage = cv2.putText(tempImage, "Please choose a part", (0, 50), cv2.FONT_HERSHEY_SIMPLEX, 1,
                                        (255, 0, 0), 3)
                height, width, channel = tempImage.shape
                step = channel * width
                qImg = QtGui.QImage(tempImage.data, width, height, step,
                                    QtGui.QImage.Format_RGB888)
                self.imageLab.setPixmap(QtGui.QPixmap.fromImage(qImg))
                self.errorLabel.setText("Error Message: Please select a part before refreshing")
        except RuntimeError:
            self.errorMessage.setText("Error Message: No frame came")

    def switchFrame(self):
        # Changes image and data to the one related to the part the user picks and puts green targets over the locations
        # of the screws already added to the list
        self.setImage()
        self.screwBox.clear()
        if self.partBox.currentIndex() != 0:
            tempimage = copy.deepcopy(self.activeFrame[self.partBox.currentIndex()])
            for screw in self.screws[self.partBox.currentIndex()]:
                self.screwBox.addItem(
                    'X: ' + str(screw[0]) + '\nY: ' + str(screw[1]) + '\nDist: ' + str(screw[2])[:4])
                tempimage = cv2.circle(tempimage, (screw[0], screw[1]), 8, (0, 255, 0), 1)
                tempimage = cv2.circle(tempimage, (screw[0], screw[1]), 1, (0, 255, 0), -1)
            height, width, channel = tempimage.shape
            step = channel * width
            qImg = QtGui.QImage(tempimage.data, width, height, step, QtGui.QImage.Format_RGB888)
            self.imageLab.setPixmap(QtGui.QPixmap.fromImage(qImg))
        self.scale = 1.0
        self.minBounds = [0, 0]

    def addScrew(self):
        # Adds the point at the x and y coordinates to the screw list as well as the distance from the camera to the
        # object at that point
        if self.activeFrame[self.partBox.currentIndex()] is not None and self.xCoord.toPlainText() != '' and \
                self.yCoord.toPlainText() != '':
            z = self.activeDepth[self.partBox.currentIndex()].data[int(self.yCoord.toPlainText()),
                                                                   int(self.xCoord.toPlainText())] * depthScale
            if [int(self.xCoord.toPlainText()), int(self.yCoord.toPlainText()), z] not in \
                    self.screws[self.partBox.currentIndex()]:
                self.screws[self.partBox.currentIndex()].append(
                    [int(self.xCoord.toPlainText()), int(self.yCoord.toPlainText()), z])
                self.screwBox.addItem(
                    'X: ' + self.xCoord.toPlainText() + '\nY: ' + self.yCoord.toPlainText() + '\nDist: ' + str(z)[:4])
                self.screwBox.setCurrentIndex(self.screwBox.__len__() - 1)
                self.errorLabel.setText("Message:")
            else:
                self.errorLabel.setText("Error Message: That screw has already been added")
        else:
            self.errorLabel.setText("Error Message: Double check the x and y coordinates")

    def delScrew(self):
        # Deletes the screw currently selected in the combobox
        if self.screws[self.partBox.currentIndex()]:
            self.screws[self.partBox.currentIndex()].pop(self.screwBox.currentIndex())
            self.screwBox.removeItem(self.screwBox.currentIndex())
        else:
            self.errorLabel.setText("error message: There is no screw to delete")

    def retranslateUi(self, MainWindow):
        _translate = QtCore.QCoreApplication.translate
        MainWindow.setWindowTitle(_translate("MainWindow", "MainWindow"))
        self.tools.setTitle(_translate("MainWindow", "Tools"))
        self.title1.setText(_translate("MainWindow", "Part"))
        self.title2.setText(_translate("MainWindow", "Screws"))
        self.addButton.setText(_translate("MainWindow", "Add New Screw"))
        self.title5.setText(_translate("MainWindow", "Load Barcode"))
        self.refButton.setText(_translate("MainWindow", "Refresh"))
        self.xCoord.setHtml(_translate("MainWindow",
                                       "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0//EN\" \"http://www.w3.org/TR/REC-html40/strict.dtd\">\n"
                                       "<html><head><meta name=\"qrichtext\" content=\"1\" /><style type=\"text/css\">\n"
                                       "p, li { white-space: pre-wrap; }\n"
                                       "</style></head><body style=\" font-family:\'MS Shell Dlg 2\'; font-size:12pt; font-weight:400; font-style:normal;\">\n"
                                       "<p style=\"-qt-paragraph-type:empty; margin-top:0px; margin-bottom:0px; margin-left:0px; margin-right:0px; -qt-block-indent:0; text-indent:0px;\"><br /></p></body></html>"))
        self.title3.setText(_translate("MainWindow", "X"))
        self.title4.setText(_translate("MainWindow", "Y"))
        self.delButton.setText(_translate("MainWindow", "Delete Screw"))
        self.insButton.setText(_translate("MainWindow", "What's this?"))
        self.partBox.setItemText(0, _translate("MainWindow", "Select the part\nin image"))
        self.partBox.setItemText(1, _translate("MainWindow", "Electronicboard\nTop Plate"))
        self.partBox.setItemText(2, _translate("MainWindow", "Plate Cover"))
        self.partBox.setItemText(3, _translate("MainWindow", "Motor Platter\nCover"))
        self.partBox.setItemText(4, _translate("MainWindow", "VoiceRecorder \nMagnet Cover"))
        self.saveButton.setText(_translate("MainWindow", "Save"))
        self.loadButton.setText(_translate("MainWindow", "Load"))
        self.errorLabel.setText(_translate("MainWindow", "Message:"))


if __name__ == "__main__":
    # Get camera ready
    pipeline = rs.pipeline()
    config = rs.config()
    # config.enable_stream(rs.stream.depth, 1280, 720, rs.format.z16, 30)
    # config.enable_stream(rs.stream.color, 1280, 720, rs.format.bgr8, 30)
    config.enable_stream(rs.stream.depth, 640, 480, rs.format.z16, 30)
    config.enable_stream(rs.stream.color, 640, 480, rs.format.bgr8, 30)
    profile = pipeline.start(config)
    # Getting the depth sensor's depth scale
    depth_sensor = profile.get_device().first_depth_sensor()
    depthScale = depth_sensor.get_depth_scale()
    hole_filling = rs.hole_filling_filter()
    align_to = rs.stream.color
    align = rs.align(align_to)
    # Configure main window
    choseCancel = True  # Used to loop if the user chooses cancel when loading a file
    q_application = QtWidgets.QApplication(sys.argv)
    app = q_application
    while choseCancel:
        ui = Ui_MainWindow()
        ui.show()
        app.exec_()
        if ui.get_choice() != "":
            choseCancel = False
    dir = ui.get_choice()
    if dir != "No":
        edit = Edit_Window()
        if dir != "New":
            edit.codeBox.setText(dir)
            if ui.get_load():
                edit.loadFamily()
            else:
                edit.load()
        edit.show()
        app.exec_()
    sys.exit()
