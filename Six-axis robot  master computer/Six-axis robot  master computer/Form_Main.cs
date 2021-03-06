﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Six_axis_robot__master_computer
{

    public partial class Form_Main : Form
    {
        /////////////////////////////////////////////////////public static
        Double X_pianyi = 0;
        Double Y_pianyi = 0;
        Double Z_pianyi = 0;
        Double E0_pianyi = 0;
        Double E1_pianyi = 0;
        Double Jia_pianyi = 0;
        Double X_shangci= 0;
        Double Y_shangci = 0;
        Double Z_shangci = 0;
        Double E0_shangci = 0;
        Double E1_shangci = 0;
        Double Tingzhi = 0;

        Double Xianwei_X_max = 30;  //X最高软限位
        Double Xianwei_Y_max = 800;  //Y最高软限位
        Double Xianwei_Z_max =10;  //Z最高软限位
        Double Xianwei_E0_max = 2.5;  //E0最高软限位
        Double Xianwei_E1_max = 8.0;  //E1最高软限位
        Double Xianwei_X_min = -30;  //X最低软限位
        Double Xianwei_Y_min = -900;  //Y最低软限位
        Double Xianwei_Z_min = -13;  //Z最低软限位
        Double Xianwei_E0_min = -2.5;  //E0最低软限位
        Double Xianwei_E1_min = -7.2;  //E1最低软限位
        /////////////////////////////////////////////////////


        public Form_Main()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(1000);

            chuankou_chushihua();
            backgroundWorker1.RunWorkerAsync();


        }

        private void button70_Click(object sender, EventArgs e)
        {
            textBox_Daochu.Clear();//导出框清空
        }

        private void button73_Click(object sender, EventArgs e)
        {
            textBox_Daoru.Clear();//导入框清空
        }

        //导出控制文件
        private void button71_Click(object sender, EventArgs e)
        {
            Double X_jieshu = -Convert.ToDouble(label_Weizhi_X.Text);
            Double Y_jieshu = -Convert.ToDouble(label_Weizhi_Y.Text);
            Double Z_jieshu = -Convert.ToDouble(label_Weizhi_Z.Text);
            this.textBox_Daochu.AppendText(";[G228#" + P + "]");
            string Weizhi = "G90\r\n" + "G1" + " " + "X" + X_jieshu + " " + "Y" + Y_jieshu + " " + "Z" + Z_jieshu + " " + "F" + "1000";
            textBox_Daochu.AppendText(Weizhi);
            textBox_Daochu.AppendText(Environment.NewLine);



            var save = new SaveFileDialog();
            save.Filter = "输出.G228文件 (*.G228)|*.G228";
            save.FileName = "输出_" + DateTime.Now.ToString("yyyyMMddHHmmss");//年月日时分秒
            if (save.ShowDialog() == DialogResult.OK && save.FileName != "")
            {
                var sw = new StreamWriter(save.FileName);
                for (var i = 0; i < textBox_Daochu.Lines.Length; i++)
                {
                    sw.WriteLine(textBox_Daochu.Lines.GetValue(i).ToString());
                }
                sw.Close();
            }
            MessageBox.Show("控制文件保存成功");
        }

        //导入控制文件
        private void button72_Click(object sender, EventArgs e)
        {
            string file = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择.G228文件";
            dialog.Filter = ".G228文件(*.G228*)|*.G228*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file = dialog.FileName;
            }
            Read(file);
            MessageBox.Show("控制文件导入成功");
        }

        //导入控制文件中Read的调用
        public void Read(string path)
        {
            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line.ToString());
                    textBox_Daoru.AppendText(line);
                    textBox_Daoru.AppendText(Environment.NewLine);                 
                }
            }
            catch
            {
                MessageBox.Show("取消控制文件导入");
            }
        }




        //扫描控制串口
        private void button20_Click(object sender, EventArgs e)
        {
            //Saomiao(serialPort1, comboBox1);

            chuankou_chushihua();
        }
        //扫描串口
        private void Saomiao(SerialPort MyPort, ComboBox MyBox)
        {
            string[] MyString = new string[20];
            string Buffer;
            MyBox.Items.Clear();
            for (int i = 1; i < 20; i++)
            {
                try
                {
                    Buffer = "COM" + i.ToString();
                    MyPort.PortName = Buffer;
                    MyPort.Open();
                    MyString[i - 1] = Buffer;
                    MyBox.Items.Add(Buffer);
                    MyPort.Close();
                }
                catch
                {

                }
            }
            MyBox.Text = MyString[0];
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Close();
                }
                catch { }
                button21.Text = "打开串口";
                button20.Enabled = true;
                label_CK_Xinxi.Text = "COM串口已关闭";

            }
            else
            {
                try
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.Open();
                    //注册事件。
                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(this.serialPort1_DataReceived);

                    button21.Text = "关闭串口";
                    button20.Enabled = false;
                    label_CK_Xinxi.Text = comboBox1.Text + "串口已打开";
                }
                catch
                {
                    MessageBox.Show("串口打开失败", "错误");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (!serialPort1.IsOpen) //如果没打开
            {
                MessageBox.Show("请先打开串口！", "Error");
                return;
            }
            else
            {
                //serialPort1.WriteLine(textBox_Send.Text);    //写入数据，此数据没经过实时位置反馈，需走输出程序检测
                serialPort1_shuchu(textBox_Send.Text);

                this.textBox_zhukong.Text += textBox_Send.Text + "\r\n";
            }
        }
        //串口初始化
        private void chuankou_chushihua()
        {
            //清空comboBox1下的所有可以选择数据项
            comboBox1.Items.Clear();
            //检查是否含有串口
            string[] str = SerialPort.GetPortNames();
            if (str == null)
            {
                MessageBox.Show("本机没有串口！", "Error");
                return;
            }

            //添加串口项目
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {//获取有多少个COM口
                //System.Diagnostics.Debug.WriteLine(s);
                comboBox1.Items.Add(s);
            }

            //串口设置默认选择项
            comboBox1.SelectedIndex = 1;         //note：获得COM9口，但别忘修改
                                                 //cbBaudRate.SelectedIndex = 5;
                                                 // cbDataBits.SelectedIndex = 3;
                                                 // cbStop.SelectedIndex = 0;
                                                 //  cbParity.SelectedIndex = 0;


            Control.CheckForIllegalCrossThreadCalls = false;    //这个类中我们不检查跨线程的调用是否合法(因为.net 2.0以后加强了安全机制,，不允许在winform中直接跨线程访问控件的属性)
            //serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
            //sp1.ReceivedBytesThreshold = 1;

            //准备就绪              
            serialPort1.DtrEnable = true;
            serialPort1.RtsEnable = true;
            //设置数据读取超时为1秒
            serialPort1.ReadTimeout = 1000;

            serialPort1.Close();

        }

        //后台调用串口
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            /*
             char[] c;
             while (true)
             {
                 try
                 {
                    if (serialPort1.IsOpen)
                     {

                         c = new char[serialPort1.BytesToRead];
                         serialPort1.Read(c, 0, c.Length);
                         if (c.Length > 0)
                         {
                            SetText(new string(c));                          
                         }
                        string data = serialPort1.ReadExisting();
                        if (data != string.Empty)
                        {
                             //this.txtb_receive.Text = data;这种方法因为线程不同步，只能接收一次数据,连续发送时程序崩溃
                             SetText(data);
                        }
                          }
                    }
                catch (Exception) { }
            }*/

        }
        /*
        //打印串口数据
        delegate void SetTextCallback(string text);
        private void SetText(string text)
        {
            try
            {
                if (this.textBox_zhukong.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetText);
                    this.Invoke(d, new object[] { text });
                }
                else
                {
                    this.textBox_zhukong.AppendText(text);
                    textBox_zhukong.AppendText(Environment.NewLine);
                    textBox_zhukong.SelectionStart = textBox_zhukong.Text.Length; //设定光标位置
                    textBox_zhukong.ScrollToCaret(); //滚动到光标处 
                }
            }
            catch (Exception)
            {
            }
        }*/

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //接收数据事件
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            try
            {
                string data = serialPort1.ReadExisting();
                if (data != string.Empty)
                {
                    //this.txtb_receive.Text = data;这种方法因为线程不同步，只能接收一次数据,连续发送时程序崩溃
                    SetData(data);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }


        delegate void SetTextCallback(string text);

        string serialPort1_shuchu_jieshou_text =" ";

        private void SetData(string text)
        {
            if (this.textBox_zhukong.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetData);
                this.Invoke(d, new object[] { text + Environment.NewLine });
            }
            else
            {
                //this.txtb_receive.Text += text+Environment.NewLine;//这种方法会多出空行
                this.textBox_zhukong.AppendText(text);
                serialPort1_shuchu_jieshou_text = text;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////



        private void textBox_Daochu_TextChanged(object sender, EventArgs e)
        {

        }

        //位置改变指令0.1,1,10,100
        private void Zhilin_Weizhigaibian(string XYZ, Double Sudu, Double Zhi)
        {
            if (!serialPort1.IsOpen) //如果没打开
            {
                MessageBox.Show("请先打开串口！", "Error");
                return;
            }
            else
            {
                string XYZ_O = "G91\r\n" + "G0 " + XYZ + Zhi + " " + "F" + Sudu + "\r\n" + "G90\r\n" + "M114\r\n";

                //serialPort1.WriteLine(XYZ_O);    //串口写入数据     
                serialPort1_shuchu(XYZ_O);

                this.textBox_zhukong.AppendText(XYZ_O);
                if (XYZ == "X")
                {
                    Double NUM = Convert.ToDouble(label_Weizhi_X.Text);
                    NUM += Zhi;
                    label_Weizhi_X.Text = NUM.ToString();

                }
                else if (XYZ == "Y")
                {
                    Double NUM = Convert.ToDouble(label_Weizhi_Y.Text);
                    NUM += Zhi;
                    label_Weizhi_Y.Text = NUM.ToString();
                }
                else if (XYZ == "Z")
                {
                    Double NUM = Convert.ToDouble(label_Weizhi_Z.Text);
                    NUM += Zhi;
                    label_Weizhi_Z.Text = NUM.ToString();
                }
            }
        }
        //x,0.1
        private void button26_Click(object sender, EventArgs e)
        {
            /*
            string XYZ = "X";
            Double Sudu = Convert.ToDouble(label_Sudu.Text);
            Double Zhi = 0.1;
            Zhilin_Weizhigaibian(XYZ,Sudu,Zhi);*/

            Zhilin_Weizhigaibian("X", Bianliang.X_Sudu, 0.1);
        }
        //x,1 
        private void button25_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("X", Bianliang.X_Sudu, 1);
        }
        //x,10
        private void button24_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("X", Bianliang.X_Sudu, 10);
        }
        //x,100
        private void button23_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("X", Bianliang.X_Sudu, 100);
        }
        //x,-0.1
        private void button27_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("X", Bianliang.X_Sudu, -0.1);
        }
        //x,-1
        private void button28_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("X", Bianliang.X_Sudu, -1);
        }
        //x,-10
        private void button29_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("X", Bianliang.X_Sudu, -10);
        }
        //x,-100
        private void button30_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("X", Bianliang.X_Sudu, -100);
        }
        //Y,0.1
        private void button37_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Y", Bianliang.Y_Sudu, 0.1);
        }
        //Y,1
        private void button38_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Y", Bianliang.Y_Sudu, 1);
        }
        //Y,10
        private void button39_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Y", Bianliang.Y_Sudu, 10);
        }
        //Y,100
        private void button40_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Y", Bianliang.Y_Sudu, 100);
        }
        //Y,-0.1
        private void button36_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Y", Bianliang.Y_Sudu, -0.1);
        }
        //Y,-1
        private void button35_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Y", Bianliang.Y_Sudu, -1);
        }
        //Y,-10
        private void button34_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Y", Bianliang.Y_Sudu, -10);
        }
        //Y,-100
        private void button33_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Y", Bianliang.Y_Sudu, -100);
        }
        //Z,0.1
        private void button46_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Z", Bianliang.Z_Sudu, 0.1);
        }
        //Z,1
        private void button47_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Z", Bianliang.Z_Sudu, 1);
        }
        //Z,10
        private void button48_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Z", Bianliang.Z_Sudu, 10);
        }
        //Z,100
        private void button49_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Z", Bianliang.Z_Sudu, 100);
        }
        //Z,-0.1
        private void button45_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Z", Bianliang.Z_Sudu, -0.1);
        }
        //Z,-1
        private void button44_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Z", Bianliang.Z_Sudu, -1);
        }
        //Z,-10
        private void button43_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Z", Bianliang.Z_Sudu, -10);
        }
        //Z,-100
        private void button42_Click(object sender, EventArgs e)
        {
            Zhilin_Weizhigaibian("Z", Bianliang.Z_Sudu, -100);
        }


        //位置回家指令
        private void Zhilin_Home(string XYZ)
        {
            if (!serialPort1.IsOpen) //如果没打开
            {
                MessageBox.Show("请先打开串口！", "Error");
                return;
            }
            else
            {
                string XYZ_O = "G28" + " " + XYZ + "0" + "\r\n" + "M114\r\n";

                //serialPort1.WriteLine(XYZ_O);    //串口写入数据         
                serialPort1_shuchu(XYZ_O);

                this.textBox_zhukong.AppendText(XYZ_O);
            }
        }
        //X_Home
        private void button31_Click(object sender, EventArgs e)
        {
            Zhilin_Home("X");
        }
        //Y_Home
        private void button32_Click(object sender, EventArgs e)
        {
            Zhilin_Home("Y");
        }
        //Z_Home
        private void button41_Click(object sender, EventArgs e)
        {
            Zhilin_Home("Z");
        }

        private void label25_Click(object sender, EventArgs e)
        {

        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            textBox_zhukong.Clear();//清空
        }

        //T0T1位置改变指令0.1,1,10,100
        private void Zhilin_T0T1_Weizhigaibian(string T0T1, Double Sudu, Double Zhi)
        {
            if (!serialPort1.IsOpen) //如果没打开
            {
                MessageBox.Show("请先打开串口！", "Error");
                return;
            }
            else
            {
                string T0T1_0 = T0T1 + "\r\n" + "G91\r\n" + "G1 " + "E" + Zhi + " " + "F" + Sudu + "\r\n" + "G90\r\n" + "M114\r\n";

                //serialPort1.WriteLine(T0T1_0);    //串口写入数据    
                serialPort1_shuchu(T0T1_0);

                this.textBox_zhukong.AppendText(T0T1_0);
                if (T0T1 == "T0")
                {
                    Double NUM = Convert.ToDouble(label_Weizhi_E0.Text);
                    NUM += Zhi;
                    label_Weizhi_E0.Text = NUM.ToString();

                }
                else if (T0T1 == "T1")
                {
                    Double NUM = Convert.ToDouble(label_Weizhi_E1.Text);
                    NUM += Zhi;
                    label_Weizhi_E1.Text = NUM.ToString();
                }
            }
        }

        //T0,1
        private void button56_Click(object sender, EventArgs e)
        {
            Zhilin_T0T1_Weizhigaibian("T0", Bianliang.E0_Sudu, 1);
        }
        //T0,0.1
        private void button55_Click(object sender, EventArgs e)
        {
            Zhilin_T0T1_Weizhigaibian("T0", Bianliang.E0_Sudu, 0.1);
        }
        //T0,-0.1
        private void button54_Click(object sender, EventArgs e)
        {
            Zhilin_T0T1_Weizhigaibian("T0", Bianliang.E0_Sudu, -0.1);
        }
        //T0,-1
        private void button53_Click(object sender, EventArgs e)
        {
            Zhilin_T0T1_Weizhigaibian("T0", Bianliang.E0_Sudu, -1);
        }
        //T1,1
        private void button65_Click(object sender, EventArgs e)
        {
            Zhilin_T0T1_Weizhigaibian("T1", Bianliang.E1_Sudu, 1);
        }
        //T1,0.1
        private void button64_Click(object sender, EventArgs e)
        {
            Zhilin_T0T1_Weizhigaibian("T1", Bianliang.E1_Sudu, 0.1);
        }
        //T1,-0.1
        private void button63_Click(object sender, EventArgs e)
        {
            Zhilin_T0T1_Weizhigaibian("T1", Bianliang.E1_Sudu, -0.1);
        }
        //T1,-1
        private void button62_Click(object sender, EventArgs e)
        {
            Zhilin_T0T1_Weizhigaibian("T1", Bianliang.E1_Sudu, -1);
        }

        //夹取命令
        private void button22_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen) //如果没打开
            {
                MessageBox.Show("请先打开串口！", "Error");
                return;
            }
            else
            {
                string Jia = "M280 P0 S"+textBox_Jiaoduzhi.Text+"\r\n";

                //serialPort1.WriteLine(Jia);    //串口写入数据  
                serialPort1_shuchu(Jia);

                this.textBox_zhukong.AppendText(Jia);
                label_Jiaqu.Text = "夹取中";
                Jia_pianyi = 1;
            }
        }
        //放开命令
        private void button_Fang_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen) //如果没打开
            {
                MessageBox.Show("请先打开串口！", "Error");
                return;
            }
            else
            {
                string Fang = "M280 P0 S0\r\n";

                //serialPort1.WriteLine(Fang);    //串口写入数据 
                serialPort1_shuchu(Fang);

                this.textBox_zhukong.AppendText(Fang);
                label_Jiaqu.Text = "未夹取";
                Jia_pianyi = 0;
            }
        }
        //STOP
        private void button68_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen) //如果没打开
            {
                MessageBox.Show("请先打开串口！", "Error");
                return;
            }
            else
            {
                Tingzhi = 1;
                string STOP = "M18\r\n"+ "stop\r\n" + "stop\r\n" + "stop\r\n";

                //serialPort1.WriteLine(STOP);    //串口写入数据  
                serialPort1_shuchu(STOP);

                this.textBox_zhukong.AppendText(STOP);
                label_huifu.Visible = true;

            }
        }

        private void button19_Click(object sender, EventArgs e)
        {

        }


        
        //位置直接改变指令
        public void Zhilin_Weizhizhijiegaibian(string XYZ, Double Sudu, Double Zhi)
        {


            if (!serialPort1.IsOpen) //如果没打开
            {
                MessageBox.Show("请先打开串口！", "Error");
                return;
            }
            else
            {
                string XYZ_O = "G91\r\n" + "G0 " + XYZ + Zhi + " " + "F" + Sudu + "\r\n" + "G90\r\n" + "M114\r\n";

                //serialPort1.WriteLine(XYZ_O);    //串口写入数据        
                serialPort1_shuchu(XYZ_O);

                this.textBox_zhukong.AppendText(XYZ_O);
                if (XYZ == "X")
                {
                    label_Weizhi_X.Text = Zhi.ToString();

                }
                else if (XYZ == "Y")
                {
                    label_Weizhi_Y.Text = Zhi.ToString();
                }
                else if (XYZ == "Z")
                {
                    label_Weizhi_Z.Text = Zhi.ToString();
                }
            }
        }




        //T0T1位置直接改变指令
        //private 
        public void Zhilin_T0T1_Weizhizhijiegaibian(string T0T1, Double Sudu, Double Zhi)
        {
            if (!serialPort1.IsOpen) //如果没打开
            {
                MessageBox.Show("请先打开串口！", "Error");
                return;
            }
            else
            {
                string T0T1_0 = T0T1 + "\r\n" + "G91\r\n" + "G1 " + "E" + Zhi + " " + "F" + Sudu + "\r\n" + "G90\r\n" + "M114\r\n";

                //serialPort1.WriteLine(T0T1_0);    //串口写入数据
                serialPort1_shuchu(T0T1_0);

                this.textBox_zhukong.AppendText(T0T1_0);
                if (T0T1 == "T0")
                {
                    label_Weizhi_E0.Text = Zhi.ToString();
                }
                else if (T0T1 == "T1")
                {
                    label_Weizhi_E1.Text = Zhi.ToString();
                }
            }
        }

        //记录机械臂位置
        private void button69_Click(object sender, EventArgs e)
        {
            JiluXYZ();
            JiluE0E1();

        }
        //记录夹具状态
        private void button22_Click_1(object sender, EventArgs e)
        {
            Jilu_Jiaju();
        }

        int P = 0;
        private void JiluXYZ()
        {
            P += 1;
            X_pianyi = Convert.ToDouble(label_Weizhi_X.Text) - X_shangci;
            Y_pianyi = Convert.ToDouble(label_Weizhi_Y.Text) - Y_shangci;
            Z_pianyi = Convert.ToDouble(label_Weizhi_Z.Text) - Z_shangci;
            
            this.textBox_Daochu.AppendText(";[G228#"+P+"]");
            string Weizhi = "G91\r\n" + "G1" + " " + "X" +X_pianyi + " " + "Y" + Y_pianyi + " " + "Z" + Z_pianyi + " " + "F" + "3000";
            textBox_Daochu.AppendText(Weizhi);
            textBox_Daochu.AppendText(Environment.NewLine);

            X_shangci = Convert.ToDouble(label_Weizhi_X.Text);
            Y_shangci = Convert.ToDouble(label_Weizhi_Y.Text);
            Z_shangci = Convert.ToDouble(label_Weizhi_Z.Text);
        }

        private void JiluE0E1()
        {
            P += 1;
            E0_pianyi = Convert.ToDouble(label_Weizhi_E0.Text) - E0_shangci;
            E1_pianyi = Convert.ToDouble(label_Weizhi_E1.Text) - E1_shangci;

            this.textBox_Daochu.AppendText(";[G228#" + P + "]");
            textBox_Daochu.AppendText(Environment.NewLine);
            string Weizhi = "T0\r\n" + "G91\r\n" + "G1 " + "E" + E0_pianyi + " " + "F" + "1000";
            textBox_Daochu.AppendText(Weizhi);
            textBox_Daochu.AppendText(Environment.NewLine);
            Weizhi = "T1\r\n" + "G91\r\n" + "G1 " + "E" + E1_pianyi + " " + "F" + "1000";
            textBox_Daochu.AppendText(Weizhi);
            textBox_Daochu.AppendText(Environment.NewLine);

            E0_shangci = Convert.ToDouble(label_Weizhi_E0.Text);
            E1_shangci = Convert.ToDouble(label_Weizhi_E1.Text);
        }

        private void Jilu_Jiaju()
        {
            P += 1;
            this.textBox_Daochu.AppendText(";[G228#" + P + "]");
            textBox_Daochu.AppendText(Environment.NewLine);
            if (Jia_pianyi ==0)
            {
                textBox_Daochu.AppendText("M280 P0 S0\r\n"); 
            }
            else
            {
                textBox_Daochu.AppendText("M280 P0 S80\r\n"); 
            }
        }


        private void button23_Click_1(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen) //如果没打开
            {
                MessageBox.Show("请先打开串口！", "Error");
                return;
            }
            else
            {
                Zhixing();
            }           
        }

        private void Zhixing()
        {
            
           /* foreach (string s in textBox_Daoru.Lines)
            {
 
                serialPort1.WriteLine(s);    //写入数据
                this.textBox_zhukong.Text += s + "\r\n";
                //System.Threading.Thread.Sleep(3000);
                
            }*/


            //string Content= textBox_Daoru.Text;//读取所有导入执行信息
            //string[] ContentLines = Content.Split(new string[] { "\r\n" }, StringSplitOptions.None);//不忽略空行
            //string[] ContentLines = Content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);//忽略空行

            string[] str = new string[textBox_Daoru.Lines.Length];

            progressBar_fasong.Maximum = textBox_Daoru.Lines.Length;//progressBar_fasong设置最大长度值
            progressBar_fasong.Value = 0;//progressBar_fasong设置当前值
            progressBar_fasong.Step = 1;//progressBar_fasong设置没次增长多少

            for (int i = 0; i < textBox_Daoru.Lines.Length; i++)
            {

                str[i] = textBox_Daoru.Lines[i];
                serialPort1_shuchu(str[i]);
                for (int n = 0; n == 1; )
                {
                    if (serialPort1_shuchu_jieshou_text.Contains("ok"))
                    {
                        n = 1;
                    }
                }
                this.textBox_zhukong.Text += str[i] + "\r\n";
                progressBar_fasong.Value += progressBar_fasong.Step; //让进度条增加一次
            }
                


        }

        private void serialPort1_shuchu(string shuchu)  //串口1输出数据的检测以及发送，用于软限位的控制
        {
            if (Convert.ToDouble(label_Weizhi_X.Text) < Xianwei_X_max && Convert.ToDouble(label_Weizhi_Y.Text) < Xianwei_Y_max && Convert.ToDouble(label_Weizhi_Z.Text) < Xianwei_Z_max && Convert.ToDouble(label_Weizhi_E0.Text) < Xianwei_E0_max && Convert.ToDouble(label_Weizhi_E1.Text) < Xianwei_E1_max && Convert.ToDouble(label_Weizhi_X.Text) > Xianwei_X_min && Convert.ToDouble(label_Weizhi_Y.Text) > Xianwei_Y_min && Convert.ToDouble(label_Weizhi_Z.Text) > Xianwei_Z_min && Convert.ToDouble(label_Weizhi_E0.Text) > Xianwei_E0_min && Convert.ToDouble(label_Weizhi_E1.Text) > Xianwei_E1_min)
            {
                serialPort1.WriteLine(shuchu);
            }
            else
            {
                MessageBox.Show("超出限位！", "Error");
            }


        }

        private void GroupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {

        }

        private void button16_Click(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)
        {

        }

        private void Button_shuxing_Click(object sender, EventArgs e)
        {
            Form_Shuxing f = new Form_Shuxing();
            f.Owner = this; //设置查找窗体的父窗体为本窗体
            f.ShowDialog();
        }
        //查询更新位置
        private void Button_weizhigengxin_Click(object sender, EventArgs e)
        {
            
            if (!serialPort1.IsOpen) //如果没打开
            {
                MessageBox.Show("请先打开串口！", "Error");
                return;
            }
            else
            {
                string Chaxun = "M114\r\n";

                //serialPort1.WriteLine(Chaxun);    //串口写入数据   
                serialPort1_shuchu(Chaxun);
                this.textBox_zhukong.AppendText(Chaxun);

                Weizhi_jiancegengxin();
            }
        }

        //输入位置检测，用于位置更新
        private void Weizhi_jiancegengxin()
        {
            string str = serialPort1_shuchu_jieshou_text;//输入文本

            //MessageBox.Show(str);//测试，调试

            byte[] byte_A = Encoding.ASCII.GetBytes(str);//转换为数组byte型

            //MessageBox.Show(System.Text.Encoding.ASCII.GetString(byte_A));//测试，调试
           
            int Out = 0;
            for (int i = 0; i < byte_A.Length && Out==0 ; i++)
            {
                int A = byte_A[i];

                //MessageBox.Show("c");//测试，调试

                if (A == 'C'|| A == 'o' || A == 'u'|| A == 'n' || A == 't')
                {
                    //MessageBox.Show("out");//测试，调试
                    Out = 1;
                }
                else
                {
                    
                    if (A == 'X')
                    {
                        //MessageBox.Show("X成功");//测试，调试
                        int Out_2 = 0;
                        for (int n=i; n < byte_A.Length && Out_2 == 0; n++)
                        {
                            //MessageBox.Show("shadiao1");//测试，调试

                            int B = byte_A[n];
                            if (B == ' ' )
                            {
                                //MessageBox.Show("shadiao2");//测试，调试

                                byte[] a2 = new byte[10];
                                for (int k = 0; k < 10 && k < (n-i); k++)
                                {
                                    a2[k] = byte_A[i+k+2];
                                }                               
                                string X_sudu_str = System.Text.Encoding.ASCII.GetString(a2);
                                this.textBox_zhukong.AppendText(X_sudu_str);
                                Out_2 = 1;
                                //MessageBox.Show(X_sudu_str);//测试，调试
                            }

                        }
                    }
                    if (A == 'Y')
                    {
                        int Out_2 = 0;
                        for (int n = i; n < byte_A.Length && Out_2 == 0; n++)
                        {
                            int B = byte_A[n];
                            if (B == ' ')
                            {
                                byte[] a2 = new byte[10];
                                for (int k = 0; k < 10 && k < (n - i); k++)
                                {
                                    a2[k] = byte_A[i + k + 2];
                                }
                                string X_sudu_str = System.Text.Encoding.ASCII.GetString(a2);
                                this.textBox_zhukong.AppendText(X_sudu_str);
                                Out_2 = 1;
                            }

                        }
                    }
                    if (A == 'Z')
                    {
                        int Out_2 = 0;
                        for (int n = i; n < byte_A.Length && Out_2 == 0; n++)
                        {
                            int B = byte_A[n];
                            if (B == ' ')
                            {
                                byte[] a2 = new byte[10];
                                for (int k = 0; k < 10 && k < (n - i); k++)
                                {
                                    a2[k] = byte_A[i + k + 2];
                                }
                                string X_sudu_str = System.Text.Encoding.ASCII.GetString(a2);
                                this.textBox_zhukong.AppendText(X_sudu_str);
                                Out_2 = 1;
                            }

                        }
                    }
                }

            }
        }


        private void 导入控制文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择.G228文件";
            dialog.Filter = ".G228文件(*.G228*)|*.G228*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file = dialog.FileName;
            }
            Read(file);
            MessageBox.Show("控制文件导入成功");
        }

        private void 导出控制文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Double X_jieshu = -Convert.ToDouble(label_Weizhi_X.Text);
            Double Y_jieshu = -Convert.ToDouble(label_Weizhi_Y.Text);
            Double Z_jieshu = -Convert.ToDouble(label_Weizhi_Z.Text);
            this.textBox_Daochu.AppendText(";[G228#" + P + "]");
            string Weizhi = "G90\r\n" + "G1" + " " + "X" + X_jieshu + " " + "Y" + Y_jieshu + " " + "Z" + Z_jieshu + " " + "F" + "1000";
            textBox_Daochu.AppendText(Weizhi);
            textBox_Daochu.AppendText(Environment.NewLine);



            var save = new SaveFileDialog();
            save.Filter = "输出.G228文件 (*.G228)|*.G228";
            save.FileName = "输出_" + DateTime.Now.ToString("yyyyMMddHHmmss");//年月日时分秒
            if (save.ShowDialog() == DialogResult.OK && save.FileName != "")
            {
                var sw = new StreamWriter(save.FileName);
                for (var i = 0; i < textBox_Daochu.Lines.Length; i++)
                {
                    sw.WriteLine(textBox_Daochu.Lines.GetValue(i).ToString());
                }
                sw.Close();
            }
            MessageBox.Show("控制文件保存成功");
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 属性ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 属性修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_Shuxing f = new Form_Shuxing();
            f.Owner = this; //设置查找窗体的父窗体为本窗体
            f.ShowDialog();
        }

        private void 关于ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form_About w = new Form_About();
            w.ShowDialog();
        }

        private void GroupBox9_Enter(object sender, EventArgs e)
        {

        }

        private void Label_Jiaqu_Click(object sender, EventArgs e)
        {

        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_Help w = new Form_Help();
            w.ShowDialog();
        }
    }


}
