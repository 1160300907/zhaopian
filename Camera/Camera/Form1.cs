using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System.IO;

namespace Camera
{

    public partial class Test : Form
    {
        private bool flag = false;
        private bool using_flag = false;
        private bool camera_flag = false;
        private string[,] ke = new string[7, 8];
        private string[,] te = new string[7, 8];
        private FilterInfoCollection videoDevices;
        private int a = 0;
        private int d_zh = 0;
        private int d_ke = 0;
        private bool testing = false;
        private bool us = false;
        private string filepath11, filepath12, filepath21, filepath22, filepath31, filepath32;
        public Test()
        {
            InitializeComponent();
        }

        private void init()
        {

         }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // 枚举所有视频输入设备
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                foreach (FilterInfo device in videoDevices)
                {
                    tscbxCameras.Items.Add(device.Name);
                }

                tscbxCameras.SelectedIndex = 0;
            }
            catch (ApplicationException)
            {
                tscbxCameras.Items.Add("No local capture devices");
                videoDevices = null;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (us || testing)
            {
                return;
            }
            if (!using_flag)
            {
                testing = true;
            }
            else
            {
                a = 0;
                us = true;
                camera_flag = false;
                //todo
                int d = (int)DateTime.Now.DayOfWeek;
                if (d != (d_zh+1)%7)
                {
                    d_zh = d - 1;
                    if (d_zh == -1) d_zh = 6;
                    d_ke=0;
                }
                else
                {
                    d_ke++;
                    while (ke[d_zh, d_ke].Equals("无"))
                    {
                        d_ke--;
                    }
                }
                filepath11 = DateTime.Now.ToString("yyyy-MM-dd");
                filepath12 = te[d_zh, d_ke] + "老师第" + (d_ke+1) + "节" + ke[d_zh, d_ke];
                filepath21 = ke[d_zh, d_ke];
                filepath31 = te[d_zh, d_ke]+"老师";
                filepath22 = DateTime.Now.ToString("yyyy-MM-dd") + te[d_zh, d_ke] +"老师第" + (d_ke+1) + "节";
                filepath32 = DateTime.Now.ToString("yyyy-MM-dd") + "第" + (d_ke+1) + "节" + ke[d_zh, d_ke];
                if (d_ke == 0)
                {
                    label3.Text = "无";
                }
                else
                {
                    label3.Text = te[d_zh, d_ke-1] + "老师的" + ke[d_zh, d_ke-1] + "，第" + (d_ke) + "节";
                }
                label1.Text = te[d_zh, d_ke] + "老师的" + ke[d_zh, d_ke] + "，第" + (d_ke + 1) + "节";
                if (ke[d_zh, d_ke+1].Equals("无"))
                {
                    label2.Text = "无";
                }
                else
                {
                    label2.Text = te[d_zh, d_ke+1] + "老师的" + ke[d_zh, d_ke+1] + "，第" + (d_ke+2) + "节";
                }
                label1.Show();
                label2.Show();
                label3.Show();
                Console.WriteLine(filepath11 + "\\" + filepath12);
                if (!Directory.Exists(filepath11))
                {
                    Directory.CreateDirectory(filepath11);
                }
                if (!Directory.Exists(filepath11 + "\\" + filepath12))
                {
                    Directory.CreateDirectory(filepath11 + "\\" + filepath12);
                }
                if (!Directory.Exists(filepath21))
                {
                    Directory.CreateDirectory(filepath21);
                }
                if (!Directory.Exists(filepath21+"\\"+filepath22))
                {
                    Directory.CreateDirectory(filepath21+"\\"+filepath22);
                }
                if (!Directory.Exists(filepath31))
                {
                    Directory.CreateDirectory(filepath31);
                }
                if (!Directory.Exists(filepath31+"\\"+filepath32))
                {
                    Directory.CreateDirectory(filepath31+"\\"+filepath32);
                }
            }
            CameraConn();
        }

        //连接摄像头
        private void CameraConn()
        {
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[tscbxCameras.SelectedIndex].MonikerString);
            videoSource.DesiredFrameSize = new Size(1920, 1080);
            videoSource.DesiredFrameRate = 1;
            videPlayer.VideoSource = videoSource;
            videPlayer.Start();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (!camera_flag && Directory.Exists(filepath11+"\\"+filepath12) && !testing)
            {
                Directory.Delete(filepath11 + "\\" + filepath12, true);
                Directory.Delete(filepath21+"\\"+ filepath22, true);
                Directory.Delete(filepath31+"\\"+ filepath32, true);
                camera_flag = false;
            }
            testing = false;
            us = false;
            videPlayer.SignalToStop();
            videPlayer.WaitForStop();
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
            label1.Show();
            label2.Show();
            label3.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            toolStripButton2_Click(null, null);
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            System.Drawing.Image img = new Bitmap(videPlayer.Width, videPlayer.Height);
            videPlayer.DrawToBitmap((Bitmap)img, new Rectangle(0, 0, videPlayer.Width, videPlayer.Height));

            if(!using_flag)
            {
                ;
            }
            else
            {
                a++;
                us = true;
                string filename = filepath11 + "\\" + filepath12 + "\\" + a + ".bmp";
                img.Save(filename, ImageFormat.Bmp);
                filename = filepath21 + "\\" + filepath22 + "\\" + a + ".bmp";
                img.Save(filename, ImageFormat.Bmp);
                filename = filepath31 + "\\" + filepath32 + "\\" + a + ".bmp";
                img.Save(filename, ImageFormat.Bmp);
                camera_flag = true;
            }

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (a == 0)
            {
                MessageBox.Show("已经没有照片", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                File.Delete(filepath11 + "\\" + filepath12 + "\\" + a + ".bmp");
                File.Delete(filepath21 + "\\" + filepath22 + "\\" + a + ".bmp");
                File.Delete(filepath31 + "\\" + filepath32 + "\\" + a + ".bmp");
                a--;
                if (a == 0)
                {
                    camera_flag = false;
                }
            }
        }

        private void tscbxCameras_Click(object sender, EventArgs e)
        {

        }

        private void videPlayer_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label1_Click_2(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!us)
            {
                return;
            }
            if (a == 0)
            {
                d_ke--;
                if (d_ke == -1)
                {
                    MessageBox.Show("没有上一节课", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    d_ke++;
                }
                else
                {
                    label2.Text = label1.Text;
                    label1.Text = label3.Text;
                    if (d_ke == 0)
                        label3.Text = "无";
                    else
                        label3.Text = te[d_zh, d_ke-1] + "老师的" + ke[d_zh, d_ke-1] + "，第" + (d_ke) + "节";
                }
            }
            else
            {
                MessageBox.Show("不能在已经拍照后再更改课表", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            label1.Show();
            label2.Show();
            label3.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!us)
            {
                return;
            }
            if (a == 0)
            {
                d_ke++;
                if (ke[d_zh,d_ke].Equals("无"))
                {
                    MessageBox.Show("没有下一节课", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    d_ke--;
                }
                else
                {
                    label3.Text = label1.Text;
                    label1.Text = label2.Text;
                    if (ke[d_zh,d_ke+1] == "无")
                        label2.Text = "无";
                    else
                        label2.Text = te[d_zh, d_ke+1] + "老师的" + ke[d_zh, d_ke+1] + "，第" + (d_ke + 2) + "节";
                }
            }
            else
            {
                MessageBox.Show("不能在已经拍照后再更改课表", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            label1.Show();
            label2.Show();
            label3.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!testing)
                using_flag = true;
            string filepath = "课表.txt";
            StreamReader sr = new StreamReader(filepath, Encoding.Default);
            string nextLine = "";
            int i = 0, j = 0;
            while ((nextLine = sr.ReadLine()) != null)
            {
                if (nextLine.Equals(""))
                {
                    ke[i, j] = "无";
                    te[i, j] = "无";
                    i++;
                    j = 0;
                } 
                else
                {
                    int k = nextLine.IndexOf(" ");
                    ke[i, j] = nextLine.Substring(0, k);
                    te[i, j] = nextLine.Substring(k + 1);
                    j++;
                }
            }
            ke[i, j] = "无";
            te[i, j] = "无";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!us)
                using_flag = false;
            else
                MessageBox.Show("不能在程序使用时清除课表", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
