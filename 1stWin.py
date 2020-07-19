# -*- coding: utf-8 -*-

# Form implementation generated from reading ui file '1stWin.ui'
#
# Created by: PyQt5 UI code generator 5.13.2

import pyrealsense2 as rs
import numpy as np
import cv2
import sys
import copy
import csv
from PyQt5 import QtCore, QtGui, QtWidgets
from tkinter import filedialog
from tkinter import *


class Ui_MainWindow(QtWidgets.QMainWindow):
    def __init__(self, screwList=[]):
        super().__init__()
        self.setObjectName("MainWindow")
        self.resize(315, 318)
        self.centralwidget = QtWidgets.QWidget(self)
        self.centralwidget.setObjectName("centralwidget")
        self.makeNew = QtWidgets.QPushButton(self.centralwidget)
        self.makeNew.setGeometry(QtCore.QRect(70, 70, 161, 61))
        font = QtGui.QFont()
        font.setPointSize(20)
        self.makeNew.setFont(font)
        self.makeNew.setObjectName("makeNew")
        self.makeNew.clicked.connect(self.ClickNew)
        # self.makeNew.clicked.connect(lambda: self.close())
        self.descText = QtWidgets.QLabel(self.centralwidget)
        self.descText.setGeometry(QtCore.QRect(0, 0, 241, 31))
        font = QtGui.QFont()
        font.setPointSize(15)
        self.descText.setFont(font)
        self.descText.setObjectName("descText")
        self.loadOld = QtWidgets.QPushButton(self.centralwidget)
        self.loadOld.setGeometry(QtCore.QRect(70, 175, 161, 61))
        self.loadOld.clicked.connect(self.GetOld)
        font = QtGui.QFont()
        font.setPointSize(20)
        self.loadOld.setFont(font)
        self.loadOld.setObjectName("loadOld")
        self.setCentralWidget(self.centralwidget)
        self.chosen = "No"
        self.screwList = screwList

        self.retranslateUi(self)
        QtCore.QMetaObject.connectSlotsByName(self)

    def get_choice(self):
        return self.chosen

    def ClickNew(self):
        self.chosen = "New"
        self.close()

    def GetOld(self):
        root = Tk()
        self.chosen = filedialog.askopenfilename(initialdir="/", title="Select file",
                                                 filetypes=(("csv files", "*.csv"), ("all files", "*.*")))
        root.destroy()
        self.close()

    def retranslateUi(self, MainWindow):
        _translate = QtCore.QCoreApplication.translate
        MainWindow.setWindowTitle(_translate("MainWindow", "MainWindow"))
        self.makeNew.setText(_translate("MainWindow", "Create New"))
        self.descText.setText(_translate("MainWindow", "Hard Drive Disassemble"))
        self.loadOld.setText(_translate("MainWindow", "Modify/Load"))


class Edit_Window(QtWidgets.QMainWindow):
    def __init__(self):
        super().__init__()
        self.activeFrame = None
        self.activeDepth = None
        self.frames = None
        # self.screwList = []
        self.setObjectName("MainWindow")
        self.resize(853, 480)
        self.centralwidget = QtWidgets.QWidget(self)
        self.centralwidget.setObjectName("centralwidget")
        self.imageLab = QtWidgets.QLabel(self.centralwidget)
        self.imageLab.setGeometry(QtCore.QRect(213, 0, 640, 480))
        self.imageLab.setMouseTracking(True)
        self.imageLab.setText("")
        self.imageLab.setPixmap(QtGui.QPixmap("default_background.png"))
        self.imageLab.setScaledContents(True)
        self.imageLab.setObjectName("imageLab")
        self.tools = QtWidgets.QGroupBox(self.centralwidget)
        self.tools.setGeometry(QtCore.QRect(0, 0, 213, 480))
        font = QtGui.QFont()
        font.setPointSize(12)
        self.tools.setFont(font)
        self.tools.setObjectName("tools")
        self.title1 = QtWidgets.QLabel(self.tools)
        self.title1.setGeometry(QtCore.QRect(0, 60, 211, 16))
        self.title1.setObjectName("title1")
        self.title2 = QtWidgets.QLabel(self.tools)
        self.title2.setGeometry(QtCore.QRect(0, 130, 211, 16))
        self.title2.setObjectName("title2")
        self.screwBox = QtWidgets.QComboBox(self.tools)
        self.screwBox.setGeometry(QtCore.QRect(0, 150, 213, 31))
        self.screwBox.setObjectName("screwBox")
        self.addButton = QtWidgets.QPushButton(self.tools)
        self.addButton.setGeometry(QtCore.QRect(0, 250, 213, 31))
        self.addButton.setObjectName("addButton")
        self.title5 = QtWidgets.QLabel(self.tools)
        self.title5.setGeometry(QtCore.QRect(0, 330, 211, 16))
        self.title5.setObjectName("title5")
        self.refButton = QtWidgets.QPushButton(self.tools)
        self.refButton.setGeometry(QtCore.QRect(0, 20, 213, 31))
        self.refButton.setObjectName("refButton")
        self.xCoord = QtWidgets.QTextEdit(self.tools)
        self.xCoord.setGeometry(QtCore.QRect(10, 210, 71, 31))
        self.xCoord.setObjectName("xCoord")
        self.yCoord = QtWidgets.QTextEdit(self.tools)
        self.yCoord.setGeometry(QtCore.QRect(130, 210, 71, 31))
        self.yCoord.setObjectName("yCoord")
        self.title3 = QtWidgets.QLabel(self.tools)
        self.title3.setGeometry(QtCore.QRect(40, 190, 16, 16))
        self.title3.setObjectName("title3")
        self.title4 = QtWidgets.QLabel(self.tools)
        self.title4.setGeometry(QtCore.QRect(160, 190, 21, 16))
        self.title4.setObjectName("title4")
        self.delButton = QtWidgets.QPushButton(self.tools)
        self.delButton.setGeometry(QtCore.QRect(0, 280, 213, 31))
        self.delButton.setObjectName("delButton")
        self.insButton = QtWidgets.QPushButton(self.tools)
        self.insButton.setGeometry(QtCore.QRect(0, 440, 213, 31))
        self.insButton.setObjectName("insButton")
        self.partBox = QtWidgets.QComboBox(self.tools)
        self.partBox.setGeometry(QtCore.QRect(0, 80, 213, 31))
        self.partBox.setObjectName("partBox")
        self.partBox.addItem("")
        self.partBox.addItem("")
        self.partBox.addItem("")
        self.partBox.addItem("")
        self.titleBox = QtWidgets.QTextEdit(self.tools)
        self.titleBox.setGeometry(QtCore.QRect(0, 350, 213, 31))
        self.titleBox.setObjectName("titleBox")
        self.saveButton = QtWidgets.QPushButton(self.tools)
        self.saveButton.setGeometry(QtCore.QRect(0, 410, 213, 31))
        self.saveButton.setObjectName("saveButton")
        self.loadButton = QtWidgets.QPushButton(self.tools)
        self.loadButton.setGeometry(QtCore.QRect(0, 380, 213, 31))
        self.loadButton.setObjectName("loadButton")

        self.refButton.clicked.connect(self.refresh)
        self.insButton.clicked.connect(self.instructions)
        self.addButton.clicked.connect(self.addScrew)
        self.delButton.clicked.connect(self.delScrew)
        self.xCoord.textChanged.connect(self.map)
        self.yCoord.textChanged.connect(self.map)
        self.screwBox.highlighted[int].connect(self.boxMap)

        self.setCentralWidget(self.centralwidget)
        self.retranslateUi(self)
        QtCore.QMetaObject.connectSlotsByName(self)

    def map(self):
        if self.activeFrame is not None and self.xCoord.toPlainText() != '' and self.yCoord.toPlainText() != '':
            tempimage = cv2.circle(copy.deepcopy(self.activeFrame), (int(self.xCoord.toPlainText()),
                                                                     int(self.yCoord.toPlainText())), 8, (0, 0, 255), 1)
            tempimage = cv2.circle(tempimage, (int(self.xCoord.toPlainText()), int(self.yCoord.toPlainText())), 1,
                                   (0, 0, 255), -1)
            height, width, channel = tempimage.shape
            step = channel * width
            qImg = QtGui.QImage(tempimage.data, width, height, step, QtGui.QImage.Format_RGB888)
            self.imageLab.setPixmap(QtGui.QPixmap.fromImage(qImg))

    def boxMap(self, index):
        tempimage = cv2.circle(copy.deepcopy(self.activeFrame), (screwList[index][0],
                               screwList[index][1]), 8, (0, 255, 0), 1)
        tempimage = cv2.circle(tempimage, (screwList[index][0], screwList[index][1]), 1,
                               (0, 255, 0), -1)
        height, width, channel = tempimage.shape
        step = channel * width
        qImg = QtGui.QImage(tempimage.data, width, height, step, QtGui.QImage.Format_RGB888)
        self.imageLab.setPixmap(QtGui.QPixmap.fromImage(qImg))

    def mousePressEvent(self, event: QtGui.QMouseEvent) -> None:
        if event.x() > 213:
            self.xCoord.setText(str(event.x() - 213))
            self.yCoord.setText(str(event.y()))

    def instructions(self):
        self.imageLab.setPixmap(QtGui.QPixmap("default_background.png"))

    def refresh(self):
        self.frames = pipeline.wait_for_frames()
        aligned_frames = align.process(self.frames)
        self.activeDepth = np.asanyarray(aligned_frames.get_depth_frame().get_data())
        self.activeFrame = np.asanyarray(aligned_frames.get_color_frame().get_data())
        self.activeFrame = cv2.cvtColor(self.activeFrame, cv2.COLOR_BGR2RGB)
        height, width, channel = self.activeFrame.shape
        step = channel * width
        qImg = QtGui.QImage(self.activeFrame.data, width, height, step, QtGui.QImage.Format_RGB888)
        self.imageLab.setPixmap(QtGui.QPixmap.fromImage(qImg))

    def addScrew(self):
        if self.activeFrame is not None and self.xCoord.toPlainText() != '' and self.yCoord.toPlainText() != '':
            screwList.append([int(self.xCoord.toPlainText()), int(self.yCoord.toPlainText()),
                              self.activeDepth.data[int(self.yCoord.toPlainText()), int(self.xCoord.toPlainText())] *
                              depthScale])
            self.screwBox.addItem('X: ' + self.xCoord.toPlainText() + ' Y: ' + self.yCoord.toPlainText() + ' Dist: ' +
                                  str(self.activeDepth.data[
                                          int(self.yCoord.toPlainText()), int(self.xCoord.toPlainText())] *
                                      depthScale)[:4])
            self.screwBox.setCurrentIndex(self.screwBox.__len__() - 1)

    def delScrew(self):
        self.screwBox.removeItem(self.screwBox.currentIndex())

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
        self.insButton.setText(_translate("MainWindow", "How Does This Work?"))
        self.partBox.setItemText(0, _translate("MainWindow", "Select the part in image"))
        self.partBox.setItemText(1, _translate("MainWindow", "Electronicboard"))
        self.partBox.setItemText(2, _translate("MainWindow", "Plate Cover"))
        self.partBox.setItemText(3, _translate("MainWindow", "Motor"))
        self.saveButton.setText(_translate("MainWindow", "Save"))
        self.loadButton.setText(_translate("MainWindow", "Load"))


if __name__ == "__main__":
    # Get camera ready
    pipeline = rs.pipeline()
    config = rs.config()
    config.enable_stream(rs.stream.depth, 640, 480, rs.format.z16, 30)
    config.enable_stream(rs.stream.color, 640, 480, rs.format.bgr8, 30)
    profile = pipeline.start(config)
    # Getting the depth sensor's depth scale
    depth_sensor = profile.get_device().first_depth_sensor()
    depthScale = depth_sensor.get_depth_scale()
    align_to = rs.stream.color
    align = rs.align(align_to)
    # Configure main window
    q_application = QtWidgets.QApplication(sys.argv)
    app = q_application
    ui = Ui_MainWindow()
    ui.show()
    app.exec_()
    if ui.get_choice() != "No":
        screwList = []
        edit = Edit_Window()
        edit.show()
        app.exec_()
    sys.exit()
