package com.zsh.mid_level;

import android.util.Log;

import com.zsh.zsh_android_serialport_lib.ChangeTool;
import com.zsh.zsh_android_serialport_lib.SerialPortUtils;

public class MidLevel {
    private boolean framePack = true;
    private SerialPortUtils serialPortUtils;
    private ChangeTool changeTool = new ChangeTool();
    private WriteThread writeThread;
    //帧头
    private byte frameHand = (byte)0x24;
    //数据长度
    private int dataLength = -1;
    //帧尾的数据长度，
    private int frameEndLength = 2;
    //帧尾
    private byte [] frameEndPack = {(byte)0x0D,(byte)0x0A};
    //异或结果
    private byte xorAns = (byte)0xff;
    //mid level 的tag
    private String TAG = "MidLevel";
    //
    private boolean reading = false;
    //0xC1 握手命令
    final private byte handshake = (byte) 0xC1;
    //0x61 执行成功
    final private byte successProcess = (byte) 0x61;
    //0x71 执行失败
    final private byte failProcess = (byte) 0x71;

    //0x71读动作状态反馈命令。自动清零。
    final private byte readStatusCmd = (byte) 0x71;
    //0x71读动作状态反馈命令。0：空闲状态，没有执行动作。
    final private byte returnFreeStatus = (byte) 0x0;
    //0x71读动作状态反馈命令。1：动作执行中
    final private byte returnRuningStatus = (byte) 0x1;
    //0x71读动作状态反馈命令。2：动作执行完成
    final private byte returnCompleteStatus = (byte) 0x2;

    //0x72 X轴Y轴移动行列数设置命令。
    final private byte xyMoveCmd = (byte) 0x72;
    //0x73 读取当前XY轴的值
    final private byte readCurrentXYValueCmd = (byte) 0x73;
    //0x74 X轴参数配置
    final private byte saveXValueCmd = (byte) 0x74;
    //0x75 X轴参数读取
    final private byte readXValueCmd = (byte) 0x75;
    //0x76 Y轴参数配置
    final private byte saveYValueCmd = (byte) 0x76;
    //0x77 Y轴参数读取
    final private byte readYValueCmd = (byte) 0x77;

    //单独动作控制
    final private byte singleControlCmd = (byte) 0x81;
    //组合动作控制
    final private byte combinationControlCmd = (byte) 0x82;
    //保存控制命令
    final private byte saveControl = (byte) 0x78;
    //保存成功
    final private byte saveOk = (byte)0x0;
    //保存失败
    final private byte saveFail = (byte) 0x1;

    //状态机的状态。
    //
    final private byte sendHandPackStatus = (byte) 0x0;
    //
    final private byte saveXValueStatus = (byte) 0x1;
    //
    final private byte readXValueStatus = (byte) 0x2;
    //
    final private byte saveYValueStatus = (byte) 0x3;
    //
    final private byte readYValueStatus = (byte) 0x4;
    //
    final private byte saveControlStatus = (byte) 0x5;
    //
    final public byte idleStatus = (byte) 0x6;
    //
    final private byte xyMoveStatus = (byte) 0x7;
    //
    final private byte readCurrentXYValueStatus = (byte) 0x8;
    //
    final private byte combinationStatus = (byte) 0x9;
    //
    final private byte gotoZeroPointStatus = (byte) 0xa;
    //
    private byte macStatus = sendHandPackStatus;


    //x轴上面有多少格
    private byte macMaxRow;
    //y轴上面有多少格
    private byte macMaxCol;
    //取x轴上面第几件货物
    private byte MacRow;
    //取y轴上面第几件货物
    private byte MacCol;
    //发送握手命令
    private byte sendHandPackCmd = (byte) 0x41;
    //决定查询状态，还是发组合命令状态。
    private boolean running = false;
    //组合命令的子命令。
    private byte combinationCount = (byte) 0x1;

    public MidLevel(int com,int baudrate){
        //
        serialPortUtils = new SerialPortUtils(com,baudrate);

        //串口数据监听事件
        serialPortUtils.setOnDataReceiveListener(new SerialPortUtils.OnDataReceiveListener() {
            @Override
            public void onDataReceive(byte[] buffer, int size) {
                //接收到的串口数据进行分析
                dataAnalysis(buffer,size);
            }
        });

        //写数据的线程
        writeThread = new WriteThread();

        if (serialPortUtils != null) {
            serialPortUtils.openSerialPort();
        }
    }

    public void StartWriteThread(){
        writeThread.start();
    }

    //接收到的串口数据进行分析
    private void dataAnalysis(byte [] buffer,int length) {
        //处理读逻辑
        Log.d(TAG,"正在处理读逻辑");
        reading = true;

        for (int i=0;i<length;i++){

            if ( (buffer[i] == (byte)frameHand) && (framePack) ){
                Log.d(TAG,"开始解包！！！出现帧头");
                framePack = false;
                dataLength = -1;
                xorAns = (byte)0xff;
                continue;
            }

            if ( (dataLength == -1) && (!framePack) ){
                dataLength = buffer[i];
                xorAns = (byte)dataLength;

                if (dataLength>=2) {
                    dataLength--;
                }else {
                    Log.d(TAG,"帧长度错误！！！采用丢包策略！！！");
                    framePack = true;
                    dataLength = -1;
                    xorAns = (byte)0xff;
                    return;
                }
                Log.d(TAG,"帧长度正常！！！继续解包！！！");
                continue;
            }

            if (i+dataLength+frameEndLength<=length){
                byte cmd = (byte) buffer[i];
                xorAns = (byte) (xorAns ^ cmd);
                dataLength--;
                i++;
                if (dataLength>0){
                    byte[] data = new byte[dataLength];
                    int cnt = 0;

                    while (dataLength>0){
                       data[cnt] = buffer[i];
                       xorAns = (byte) (xorAns ^ buffer[i]);
                       dataLength--;
                       i++;
                       cnt++;
                    }

                    if (xorAns != buffer[i]){
                        Log.d(TAG,"dataLeng>1,xor错误，导致解包失败！！！");
                        framePack = true;
                        dataLength = -1;
                        xorAns = (byte)0xff;
                        continue;
                    }else {
                        i++;
                        if ((buffer[i] != frameEndPack[0])||(buffer[i+1] != frameEndPack[1]))
                        {
                            Log.d(TAG,"dataLeng>1,没有帧尾，导致解包失败！！！");
                            framePack = true;
                            dataLength = -1;
                            xorAns = (byte)0xff;
                            continue;
                        }else {
                            Log.d(TAG,"unPack(cmd,buffer); 成功解包！！！");
                            //解包
                            UnPack(cmd,data);

                            //计算新的包;
                            i++;
                            framePack = true;
                            dataLength = -1;
                            xorAns = (byte)0xff;
                            continue;
                        }
                    }

                }else if (dataLength==0){
                    if (xorAns != buffer[i]){
                        Log.d(TAG,"dataLeng==1,xor错误，导致解包失败！！！");
                        framePack = true;
                        dataLength = -1;
                        xorAns = (byte)0xff;
                        continue;
                    }else {
                        i++;
                        if ((buffer[i] != frameEndPack[0])||(buffer[i+1] != frameEndPack[1]))
                        {
                            Log.d(TAG,"dataLeng==1,没有帧尾，导致解包失败！！！");
                            framePack = true;
                            dataLength = -1;
                            xorAns = (byte)0xff;
                            return;
                        }else {
                            Log.d(TAG,"unPack(cmd); 成功解包！！！");
                            //解包
                            UnPack(cmd);
                            //计算新的包;
                            i++;
                            framePack = true;
                            dataLength = -1;
                            xorAns = (byte)0xff;
                            continue;
                        }
                    }
                }
            }else {
                Log.d(TAG,"现在的偏移量+包长+帧尾 > buffer长度，导致解包错，采用丢包策略！！！");
                framePack = true;
                dataLength = -1;
                xorAns = (byte)0xff;
                continue;
            }
        }
        //读逻辑处理完
        Log.d(TAG,"读逻辑处理完");
        reading = false;
    }

    private void UnPack(byte cmd,byte [] arrBuffer){
        Log.d(TAG,"cmd :"+ changeTool.Byte2Hex(cmd)+", arrBuffer : " +changeTool.ByteArrToHex(arrBuffer) );
        switch (cmd) {
            case singleControlCmd:
                if (arrBuffer[0]==successProcess){
                    Log.d( TAG,"单独动作控制命令"+changeTool.Byte2Hex(cmd)+":处理成功");
                }else if (arrBuffer[0]==failProcess){
                    Log.d( TAG,"单独动作控制命令"+changeTool.Byte2Hex(cmd)+":处理失败");
                }else {
                    Log.d( TAG,"单独动作控制命令"+changeTool.Byte2Hex(cmd)+":未知状态");
                }
                break;

            case combinationControlCmd:
                if (arrBuffer[0]==successProcess){
                    Log.d( TAG,"组合动作控制命令"+changeTool.Byte2Hex(cmd)+":处理成功");
                    if (macStatus==combinationStatus){
                        running = true;
                    }

                    if (macStatus==gotoZeroPointStatus){
                        Log.d(TAG,"归原点，处理成功");
                        macStatus = idleStatus;
                    }
                }else if (arrBuffer[0]==failProcess){
                    Log.d( TAG,"组合动作控制命令"+changeTool.Byte2Hex(cmd)+":处理失败");
                }else {
                    Log.d( TAG,"组合动作控制命令"+changeTool.Byte2Hex(cmd)+":未知状态");
                }
                break;

            case readStatusCmd:
                if (arrBuffer[1]==returnFreeStatus){
                    Log.d( TAG,"读动作状态反馈命令,读取"+changeTool.Byte2Hex(arrBuffer[0]) +"状态，状态为空闲！！！");
                }else if (arrBuffer[1]==returnRuningStatus){
                    Log.d( TAG,"读动作状态反馈命令,读取"+changeTool.Byte2Hex(arrBuffer[0]) +"状态，状态为运行中！！！");
                }else if (arrBuffer[1]==returnCompleteStatus) {
                    Log.d(TAG, "读动作状态反馈命令,读取" + changeTool.Byte2Hex(arrBuffer[0]) + "状态，状态为完成！！！");

                    if (macStatus==combinationStatus){
                        running = false;

                        if(combinationCount==3){
                            combinationCount = 5;
                        }else if(combinationCount==5){
                            combinationCount = 7;
                        }else {
                            combinationCount++;
                        }

                        if (combinationCount>(byte) 0x8){
                            combinationCount = 0x1;
                            macStatus = gotoZeroPointStatus;
                            Log.d(TAG,"处理完成整个取货操作！！！");
                            onSendUIReport.OnSendUI("处理完成整个取货操作！！！");
                        }
                    }

                } else {
                    Log.d( TAG,"读动作状态反馈命令,读取" + changeTool.Byte2Hex(arrBuffer[0]) + "状态，状态为未知！！！");
                }
                break;

            case xyMoveCmd:
                if (arrBuffer[0]==successProcess){
                    Log.d( TAG,"X轴Y轴移动行列数设置命令"+changeTool.Byte2Hex(cmd)+":处理成功");
                    if (macStatus==xyMoveStatus){
                        macStatus = readCurrentXYValueStatus;
                    }
                }else if (arrBuffer[0]==failProcess){
                    Log.d( TAG,"X轴Y轴移动行列数设置命令"+changeTool.Byte2Hex(cmd)+":处理失败");
                }else {
                    Log.d( TAG,"X轴Y轴移动行列数设置命令"+changeTool.Byte2Hex(cmd)+":未知状态");
                }
                break;

            case readCurrentXYValueCmd:
                Log.d(TAG,"读当前XY值命令,读取X值为：" + changeTool.Byte2Hex(arrBuffer[0]) + ", 读取Y值为：" + changeTool.Byte2Hex(arrBuffer[1]) );
                if ((arrBuffer[0]== MacRow)&&(arrBuffer[1]== MacCol)){
                    Log.d(TAG,"读出来是要移动的值，正确无误！！！");
                    if (macStatus==readCurrentXYValueStatus){
                        macStatus = combinationStatus;
                    }
                }
                break;

            case saveXValueCmd:
                if (arrBuffer[1]==saveOk){
                    Log.d( TAG,"X轴参数配置命令:"+changeTool.Byte2Hex(cmd)+",参数:"+changeTool.Byte2Hex(arrBuffer[0])+":处理成功");

                    if (macStatus==saveXValueStatus){
                        macStatus = readXValueStatus;
                    }

                }else if (arrBuffer[1]==saveFail){
                    Log.d( TAG,"X轴参数配置命令:"+changeTool.Byte2Hex(cmd)+",参数:"+changeTool.Byte2Hex(arrBuffer[0])+":处理失败");
                }else {
                    Log.d( TAG,"X轴参数配置命令:"+changeTool.Byte2Hex(cmd)+",参数:"+changeTool.Byte2Hex(arrBuffer[0])+":未知状态");
                }
                break;

            case readXValueCmd:
                byte []dataReadXvalue = new byte[]{arrBuffer[1],arrBuffer[2],arrBuffer[3],arrBuffer[4]};
                int valueReadXvalue = changeTool.byteArrayToInt(dataReadXvalue);
                String strReadXvalue = Integer.toString(valueReadXvalue);
                Log.d(TAG,"读取X轴参数命令:"+changeTool.Byte2Hex(cmd)+",参数:"+changeTool.Byte2Hex(arrBuffer[0])+",参数值："+strReadXvalue);
                if ( (arrBuffer[0]==0) &&(arrBuffer[1]== macMaxRow)){
                    if (macStatus==readXValueStatus){
                        macStatus = saveYValueStatus;
                    }
                }
                break;

            case saveYValueCmd:
                if (arrBuffer[1]==saveOk){
                    Log.d( TAG,"Y轴参数配置命令:"+changeTool.Byte2Hex(cmd)+",参数:"+changeTool.Byte2Hex(arrBuffer[0])+":处理成功");
                    if (macStatus==saveYValueStatus) {
                        macStatus = readYValueStatus;
                    }
                }else if (arrBuffer[1]==saveFail){
                    Log.d( TAG,"Y轴参数配置命令:"+changeTool.Byte2Hex(cmd)+",参数:"+changeTool.Byte2Hex(arrBuffer[0])+":处理失败");
                }else {
                    Log.d( TAG,"Y轴参数配置命令:"+changeTool.Byte2Hex(cmd)+",参数:"+changeTool.Byte2Hex(arrBuffer[0])+":未知状态");
                }
                break;

            case readYValueCmd:
                byte []dataReadYvalue = new byte[]{arrBuffer[1],arrBuffer[2],arrBuffer[3],arrBuffer[4]};
                int valueReadYvalue = changeTool.byteArrayToInt(dataReadYvalue);
                String strReadYvalue = Integer.toString(valueReadYvalue);
                Log.d(TAG,"读取Y轴参数命令:"+changeTool.Byte2Hex(cmd)+",参数:"+changeTool.Byte2Hex(arrBuffer[0])+",参数值："+strReadYvalue);
                if ( (arrBuffer[0]==0) &&(arrBuffer[1]== macMaxCol)){
                    if (macStatus==readYValueStatus){
                        macStatus = saveControlStatus;
                    }
                }
                break;

            case saveControl:
                if (arrBuffer[0]==saveOk){
                    Log.d( TAG,"保存命令"+changeTool.Byte2Hex(cmd)+":保存成功");
                    if (macStatus==saveControlStatus){
                        macStatus = idleStatus;
                        onSendUIReport.OnSendUI("完成初始化！！！");
                    }
                }else if (arrBuffer[0]==saveFail){
                    Log.d( TAG,"保存命令"+changeTool.Byte2Hex(cmd)+":保存失败");
                }else {
                    Log.d( TAG,"保存命令"+changeTool.Byte2Hex(cmd)+":未知状态");
                }
                break;
            default:
                Log.d( TAG,"命令"+changeTool.Byte2Hex(cmd)+":未知命令");
                break;
        }
    }

    private void UnPack(byte cmd){
        Log.d(TAG,"cmd :"+ changeTool.Byte2Hex(cmd) );
        switch (cmd){
            case handshake:
                Log.d( TAG,"命令"+changeTool.Byte2Hex(cmd)+":握手成功");
                if (macStatus == sendHandPackStatus) {
                    macStatus = saveXValueStatus;
                }
                break;

            default:
                Log.d( TAG,"命令"+changeTool.Byte2Hex(cmd)+":未知命令");
                break;
        }
    }

    //发生命令数据，至单片机。命令本身不需要参数。
    public void SendPack(byte cmd){
       byte [] packData = Pack(cmd);

       Log.d(TAG,"发送命令帧："+changeTool.ByteArrToHex(packData));

       if (serialPortUtils != null){
           serialPortUtils.writeData(packData);
       }
    }

    //发生命令数据，至单片机。命令本身需要参数。
    public void SendPack(byte cmd,byte [] data){
        byte [] packData = Pack(cmd,data);

        Log.d(TAG,"发送命令帧："+changeTool.ByteArrToHex(packData));

        if (serialPortUtils != null){
            serialPortUtils.writeData(packData);
        }
    }

    private byte [] Pack(byte cmd){
        byte length = 2;
        int packLength = 1 + 1 +  1 + 0 + 1 + frameEndPack.length;
        byte xorAns = length;
        xorAns = xorAns = (byte) (xorAns ^ cmd);
        byte packData [] = new byte[packLength];

        packData[0] = frameHand;
        packData[1] = length;
        packData[2] = cmd;
        packData[3] = xorAns;
        packData[4] = frameEndPack[0];
        packData[5] = frameEndPack[1];

        return packData;
    }

    private byte [] Pack(byte cmd,byte [] data){
        byte length = (byte) (2 +data.length);
        int packLength = 1 + 1 +  1 + data.length + 1 + frameEndPack.length;
        byte xorAns = length;
        xorAns = xorAns = (byte) (xorAns ^ cmd);
        byte packData [] = new byte[packLength];

        packData[0] = frameHand;
        packData[1] = length;
        packData[2] = cmd;

        for (int i=0;i<data.length;i++){
            packData[3+i] = data[i];
            xorAns = (byte)(xorAns ^ packData[3+i]);
        }

        packData[3 + data.length] = xorAns;
        packData[4 + data.length] = frameEndPack[0];
        packData[5 + data.length] = frameEndPack[1];

        return packData;
    }


    //写数据的线程，
    //我这里就相当于一个状态机，进行状态切换。
    private class WriteThread extends Thread {
        @Override
        public void run() {
            super.run();
            while (true){
                try {
                    currentThread().sleep(200);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
                switch (macStatus){

                    case sendHandPackStatus:
                        //发送握手命令
                        SendPack(sendHandPackCmd);
                        break;

                    case saveXValueStatus:
                        //设置x上面有多多少个货物
                        byte data_1[] = new byte[] {(byte) 0x0, macMaxRow,(byte) 0x0,(byte) 0x0,(byte) 0x0};
                        SendPack(saveXValueCmd,data_1);
                        break;

                    case readXValueStatus:
                        //读取x上面有多少个货物
                        byte data_2[] = new byte[] {(byte) 0x0};
                        SendPack(readXValueCmd,data_2);
                        break;

                    case saveYValueStatus:
                        //设置y上面有多多少个货物
                        byte data_3[] = new byte[] {(byte) 0x0, macMaxCol,(byte) 0x0,(byte) 0x0,(byte) 0x0};
                        SendPack(saveYValueCmd,data_3);
                        break;

                    case readYValueStatus:
                        //读取y上面有多少个货物
                        byte data_4[] = new byte[] {(byte) 0x0};
                        SendPack(readYValueCmd,data_4);
                        break;

                    case saveControlStatus:
                        //保存
                        SendPack(saveControl);
                        break;

                    case idleStatus:
                        //空闲
                        break;

                    case xyMoveStatus:
                        //
                        byte data_7[] = new byte[] {MacRow, MacCol};
                        SendPack(xyMoveCmd,data_7);
                        break;

                    case readCurrentXYValueStatus:
                        //
                        SendPack(readCurrentXYValueCmd);
                        break;

                    case combinationStatus:
                        if (running){
                            //查询状态
                            SendPack(readStatusCmd);
                        }else {
                            //发送组合命令
                            byte data_9[] = new byte[] {combinationCount};
                            SendPack(combinationControlCmd,data_9);
                        }
                        break;

                    case gotoZeroPointStatus:
                        //利用组合命令去归原点。
                        byte data_10[] = new byte[] {combinationCount};
                        SendPack(combinationControlCmd,data_10);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //返回状态机的逻辑
    public byte getMacStatus(){
        return macStatus;
    }

    //启动写数据的命令
    public void setMacRunning(){
        if (macStatus==idleStatus){
            macStatus = xyMoveStatus;
            Log.d(TAG,"启动取货中！！！");
        }else {
            Log.d(TAG,"启动取货失败，非在空闲状态！！！");
        }
    }

    //取x轴上面，第几个货。从0开始。
    public void setMacRow(byte row){
        MacRow = row;
    }

    //取y轴上面，第几个货。从0开始。
    public void setMacCol(byte col) {
        MacCol = col;
    }

    //x轴上面有多少个格
    public byte getMaxRow(){
        return macMaxRow;
    }

    //设置，x轴上面有多少个格
    public void setMaxRow(byte row){
        macMaxRow = row;
    }

    //设置，y轴上面有多少个格
    public void setMaxCol(byte col){
        macMaxCol = col;
    }

    //y轴上面有多少个格
    public byte getMaxCol(){
        return macMaxCol;
    }

    public void Close(){
        if (serialPortUtils != null) {
            serialPortUtils.closeSerialPort();
        }
    }

    public OnSendUIReport onSendUIReport = null;
    public static interface OnSendUIReport {
        public void OnSendUI(String string);
        public void reportStatus(int value);
    }
    public void setOnSendUIReport(OnSendUIReport dataReceiveListener) {
        onSendUIReport = dataReceiveListener;
    }

}
