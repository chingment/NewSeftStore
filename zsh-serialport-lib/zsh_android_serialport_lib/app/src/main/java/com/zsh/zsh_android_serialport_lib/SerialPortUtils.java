package com.zsh.zsh_android_serialport_lib;

import android.util.Log;

import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

import android_serialport_api.SerialPort;

public class SerialPortUtils {

    private SerialPort serialport = null;

    //串口名称
    private String com2Name = "/dev/ttymxc1";
    private String com3Name = "/dev/ttymxc2";
    private String com4Name = "/dev/ttymxc3";
    private String whichCom;

    //波特率
    private int baudrate = 0;
    //是否打开串口标志
    public boolean serialPortStatus = false;

    public boolean readStatus = true; //线程状态，为了安全终止线程
    public boolean writeStatus = true;
    public SerialPort serialPort = null;
    public InputStream inputStream = null;
    public OutputStream outputStream = null;
    public ChangeTool changeTool = new ChangeTool();

    private final String TAG = "SerialPortUtils";

    public SerialPortUtils(int com,int baudrate) {
        setComInfo(com,baudrate);
    }

    public boolean setComInfo(int com,int baudrate) {
        switch (com) {

            case 2 :
                whichCom = com2Name;
                break;

            case 3 :
                whichCom = com3Name;
                break;

            case 4 :
                whichCom = com4Name;
                break;

            default:
                whichCom = null;
                return false;
        }

        switch(baudrate) {
            case 0: this.baudrate = baudrate; break;
            case 50: this.baudrate = baudrate; break;
            case 75: this.baudrate = baudrate; break;
            case 110: this.baudrate = baudrate; break;
            case 134: this.baudrate = baudrate; break;
            case 150: this.baudrate = baudrate; break;
            case 200: this.baudrate = baudrate; break;
            case 300: this.baudrate = baudrate; break;
            case 600: this.baudrate = baudrate; break;
            case 1200: this.baudrate = baudrate; break;
            case 1800: this.baudrate = baudrate; break;
            case 2400: this.baudrate = baudrate; break;
            case 4800: this.baudrate = baudrate; break;
            case 9600: this.baudrate = baudrate; break;
            case 19200: this.baudrate = baudrate; break;
            case 38400: this.baudrate = baudrate; break;
            case 57600: this.baudrate = baudrate; break;
            case 115200: this.baudrate = baudrate; break;
            case 230400: this.baudrate = baudrate; break;
            case 460800: this.baudrate = baudrate; break;
            case 500000: this.baudrate = baudrate; break;
            case 576000: this.baudrate = baudrate; break;
            case 921600: this.baudrate = baudrate; break;
            case 1000000: this.baudrate = baudrate; break;
            case 1152000: this.baudrate = baudrate; break;
            case 1500000: this.baudrate = baudrate; break;
            case 2000000: this.baudrate = baudrate; break;
            case 2500000: this.baudrate = baudrate; break;
            case 3000000: this.baudrate = baudrate; break;
            case 3500000: this.baudrate = baudrate; break;
            case 4000000: this.baudrate = baudrate; break;
            default: this.baudrate = -1; return false;
        }

        return true;
    }

    public SerialPort openSerialPort() {
        try {
            serialport = new SerialPort(new File(whichCom),baudrate,0);
            this.serialPortStatus = true;

            readStatus = false; //线程状态
            writeStatus = false;

            //获取打开的串口中的输入输出流，以便于串口数据的收发
            inputStream = serialport.getInputStream();
            outputStream = serialport.getOutputStream();

            new ReadThread().start(); //开始线程监控是否有数据要接收

        } catch (IOException e) {
            Log.e(TAG, "openSerialPort: 打开串口异常：" + e.toString());
            return serialport;
        }
        Log.d(TAG, "openSerialPort: 打开串口");
        return serialport;
    }

    /**
     * 关闭串口
     */
    public void closeSerialPort(){
        try {
            inputStream.close();
            outputStream.close();

            this.serialPortStatus = false;
            this.readStatus = true; //线程状态
            this.writeStatus = true;
            serialPort.close();
        } catch (IOException e) {
            Log.e(TAG, "closeSerialPort: 关闭串口异常："+e.toString());
            return;
        }
        Log.d(TAG, "closeSerialPort: 关闭串口成功");
    }

    public void writeData(byte outputData[]){
        try {
            outputStream.write(outputData);
        } catch (IOException e) {
            Log.e(TAG, "run: 写数据异常：" +e.toString());
        }
    }

    /**
     * 单开一线程，来读数据
     */
    private class ReadThread extends Thread{
        @Override
        public void run() {
            super.run();
            //判断进程是否在运行，更安全的结束进程
            while (!readStatus){

                try {
                    currentThread().sleep(100);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }

                Log.d(TAG, "进入读线程run");
                //64   1024
                byte[] buffer = new byte[64];
                int size; //读取数据的大小
                try {
                    size = inputStream.read(buffer);
                    if (size > 0){
                        Log.d(TAG, "run: 接收到了数据：" + changeTool.ByteArrToHex(buffer,0,size));
                        Log.d(TAG, "run: 接收到了数据大小：" + String.valueOf(size));
                        onDataReceiveListener.onDataReceive(buffer,size);
                    }
                } catch (IOException e) {
                    Log.e(TAG, "run: 数据读取异常：" +e.toString());
                }
            }
        }
    }

    public OnDataReceiveListener onDataReceiveListener = null;
    public static interface OnDataReceiveListener {
        public void onDataReceive(byte[] buffer, int size);
    }
    public void setOnDataReceiveListener(OnDataReceiveListener dataReceiveListener) {
        onDataReceiveListener = dataReceiveListener;
    }

}
