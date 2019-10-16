package com.zsh.Activity;

//import com.zsh.zsh_android_serialport_lib.*;

import  com.zsh.mid_level.MidLevel;
import com.zsh.zsh_android_serialport_lib.ChangeTool;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.os.Handler;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity {

    private MidLevel midLevel;
    private final String TAG = "MainActivity";

    private ChangeTool changeTool = new ChangeTool();

    private Button getSomething;
    private Button initMidLevel;
    private EditText rowSomething;
    private EditText colSomething;

    private String msgString;
    private Handler handler;
    int macStatus = 0;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        handler = new Handler();

        //串口，波特率
        midLevel = new MidLevel(2,9600);
        //x轴上面有多少货物
        midLevel.setMaxRow((byte)0x7);
        //y轴上面有多少货物
        midLevel.setMaxCol((byte) 0x7);

        initMidLevel = findViewById(R.id.init);
        initMidLevel.setOnClickListener(new View.OnClickListener(){
            public void onClick(View v) {
                midLevel.StartWriteThread();
                initMidLevel.setEnabled(false);
                initMidLevel.setVisibility(View.GONE);
                getSomething.setEnabled(true);
                getSomething.setVisibility(View.VISIBLE);
            }
        });

        getSomething = findViewById(R.id.getSomething_show);
        getSomething.setEnabled(false);
        getSomething.setVisibility(View.GONE);

        //绑定点击事件监听（这里用的是匿名内部类创建监听）
        getSomething.setOnClickListener(new View.OnClickListener() {
            int i = 0;

            public void onClick(View v) {
                byte status = midLevel.getMacStatus();
                if (status == midLevel.idleStatus) {
                    //取y轴上面第几个货物
                    midLevel.setMacCol((byte) 0x0);
                    //取x轴上面第几个货物
                    midLevel.setMacRow((byte) 0x0);
                    //取货
                    midLevel.setMacRunning();
                    Toast.makeText(getApplicationContext(), "正在取货！！！", Toast.LENGTH_SHORT).show();
                }
            }
        });

        //串口数据监听事件
        midLevel.setOnSendUIReport(new MidLevel.OnSendUIReport() {
            @Override
            public void OnSendUI(String string) {
                msgString = string;
                handler.post(runnable);
            }

            @Override
            //报告单片机的状态。
            public void reportStatus(int value) {
                macStatus = value;
                //handler.post(runnable);
            }
            //开线程更新UI
            Runnable runnable = new Runnable() {
                @Override
                public void run() {
                    Toast.makeText(getApplicationContext(), msgString, Toast.LENGTH_SHORT).show();
                }
            };
        });
    }

    @Override
    protected void onDestroy(){
        if (midLevel!=null){
            midLevel.Close();
        }
        super.onDestroy();
    }
}
