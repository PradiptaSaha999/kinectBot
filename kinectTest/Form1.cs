using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Kinect;
using Kinect.Toolbox;
using Coding4Fun.Kinect.WinForm;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO.Ports;

namespace kinectTest
{
    public partial class Form1 : Form
    {
        public HaarCascade haar;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        // private Image<Gray, byte> nextframe;
        // Image<Bgr, Byte> My_image_copy;
        static SerialPort sp = new SerialPort();
        Boolean connectSp;
        Capture capture;
        Image<Bgr, Byte> cImage;
        Image<Gray, Byte> gray_image1;
        Image<Bgr, Byte> My_Image;
        KinectSensor sensor = null;
        Image<Gray, byte> gray_image;
        Boolean running,disT;
        DepthImageFrame imageFrame;
        string value;
        string[] ArrayComPortsNames = null;
        int index = -1;
        int PosX;
        string ComPortName = null;
        private int baudR = 0;
        private string stopB = "";
        private string COMno = "";

        int posY;


        public Form1()
        {
            InitializeComponent();
            settings();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            haar = new HaarCascade("haarcascade_frontalface_alt2.xml");
            sensor = KinectSensor.KinectSensors[0];
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            sensor.SkeletonStream.Enable();
            running = false;
            PosX = 320;
            disT = false;
            try
            {
                sensor.Start();
            }
            catch {
                MessageBox.Show("Sensor not star");
            }
            try
            {
                sensor.ColorFrameReady += FrameReady;
            }
            catch
            {
                MessageBox.Show("colour frame not Found");
            }
            try
            {
                sensor.DepthFrameReady += DepthFrameReady;
            }
            catch
            {
                MessageBox.Show("Defth frame not Found");
            }
            posY = 0;
            sensor.ElevationAngle= 0;
            connectSp = false;
            DisconnectButton.Hide();
           

            Application.Idle += new EventHandler(take1);

            
        }


        void FramesReady(object sender,
                  AllFramesReadyEventArgs e)
        {
            ColorImageFrame VFrame =
                           e.OpenColorImageFrame();
            if (VFrame == null) return;
            Bitmap bmap = ImageToBitmap(VFrame);
            //pictureBox1.Image = bmap;
        }

        


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            sensor.Stop();
        }


       
        Bitmap ByteToBitmap(Byte[] pixeldata,
                                int w, int h)
        {
            Bitmap bmap = new Bitmap(w, h,
                          PixelFormat.Format32bppRgb);
            BitmapData bmapdata = bmap.LockBits(
              new System.Drawing.Rectangle(0, 0, w, h),
              ImageLockMode.WriteOnly,
              bmap.PixelFormat);
            IntPtr ptr = bmapdata.Scan0;
            Marshal.Copy(pixeldata, 0, ptr,
                          pixeldata.Length);
            bmap.UnlockBits(bmapdata);
            return bmap;
        }
        void DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            imageFrame = e.OpenDepthImageFrame();
            if (imageFrame != null)
            {
                disT = true;
                pictureBox2.Image = imageFrame.ToBitmap();
                // label1.Text = imageFrame.GetDistance(imageFrame.Width / 2, imageFrame.Height / 2).ToString();
            }
            else {
                disT = false;
            }
        }

        Bitmap IntToBitmap(int[] array, int w, int h)
        {
            Bitmap bmap = new Bitmap( w,h,PixelFormat.Format32bppRgb);
            BitmapData bmapdata = bmap.LockBits(new System.Drawing.Rectangle(0, 0, w, h),ImageLockMode.WriteOnly,bmap.PixelFormat);
            IntPtr ptr = bmapdata.Scan0;
            Marshal.Copy(array, 0,ptr,array.Length);
            bmap.UnlockBits(bmapdata);
            return bmap;
        }

        int getValue(DepthImageFrame imageFrame, int x, int y)
        {
            short[] pixelData = new short[imageFrame.PixelDataLength];
            imageFrame.CopyPixelDataTo(pixelData);
            return ((ushort)
              pixelData[x + y * imageFrame.Width]) >> 3;
        }

        void FrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            ColorImageFrame imageFrameC =
                           e.OpenColorImageFrame();
            if (imageFrameC != null)
            {
                //Bitmap bmap = ImageToBitmap(imageFrame);
                //Image<Bgr, Byte> img = bmap.
                Bitmap bmap = imageFrameC.ToBitmap();
                running = true;

                My_Image = new Image<Bgr, Byte>(bmap);
                //pictureBox1.Image = My_Image.ToBitmap();
            }
            else {
                running = false;
            }
        }


        Bitmap DepthToBitmap(DepthImageFrame imageFrame)
        {
            short[] pixelData = new short[imageFrame.PixelDataLength];
            imageFrame.CopyPixelDataTo(pixelData);
            //for (int i = 0; i < imageFrame.PixelDataLength; i++)
            //{
            //    pixelData[i] = (short)(((ushort)pixelData[i]) >> 3);
            //}
            Bitmap bmap = new Bitmap(
            imageFrame.Width,
            imageFrame.Height,
            PixelFormat.Format16bppRgb555);

            BitmapData bmapdata = bmap.LockBits(
             new System.Drawing.Rectangle(0, 0, imageFrame.Width,
                                    imageFrame.Height),
             ImageLockMode.WriteOnly,
             bmap.PixelFormat);
            IntPtr ptr = bmapdata.Scan0;
            Marshal.Copy(pixelData,
             0,
             ptr,
             imageFrame.Width *
               imageFrame.Height);
            bmap.UnlockBits(bmapdata);
            return bmap;
        }



       
        Bitmap ImageToBitmap(ColorImageFrame Image)
        {
            byte[] pixeldata =
                     new byte[Image.PixelDataLength];
            Image.CopyPixelDataTo(pixeldata);
            Bitmap bmap = new Bitmap(
                   Image.Width,
                   Image.Height,
                   PixelFormat.Format32bppRgb);
            BitmapData bmapdata = bmap.LockBits(
              new System.Drawing.Rectangle(0, 0,
                         Image.Width, Image.Height),
              ImageLockMode.WriteOnly,
              bmap.PixelFormat);
            IntPtr ptr = bmapdata.Scan0;
            Marshal.Copy(pixeldata, 0, ptr,
                       Image.PixelDataLength);
            bmap.UnlockBits(bmapdata);
            return bmap;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(
             object sender, EventArgs e)
        {
            if(sensor.ElevationAngle<25)
            sensor.ElevationAngle += 4;
        }


      

        private void button2_Click_1(object sender, EventArgs e)
        {
          //  if (sensor.ElevationAngle <- 25)
            sensor.ElevationAngle -= 4;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }

        private void imageBox2_Click(object sender, EventArgs e)
        {

        }

        public void take1(object sender, EventArgs e)
        {
            if (running)
            {
                
                gray_image = My_Image.Convert<Gray, byte>();
                //imageBox1.Image = gray_image;
                //label2.Text = gray_image[100, 100].ToString();
                //cImage = capture.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);//capture immage
                //gray_image1 = cImage.Convert<Gray, byte>();//convert yo gray
                HaarCascade HaarCascadeXML = new HaarCascade("haarcascade_frontalface_alt2.xml");
                //var faces = HaarCascadeXML.Detect(gray_image1);
                MCvAvgComp[][] faces = (gray_image.DetectHaarCascade(
                   HaarCascadeXML,
                   1.3,
                   5,
                   Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                   new Size(20, 20)));
                int cnt=0;
                int dist = 0;
                int distA = 0;
                int dist1 = 0;
                int i = 0;
                Boolean fc = false;
                if (disT)
                {
                    
                    foreach (MCvAvgComp face in faces[0])
                    {

                       dist = imageFrame.GetDistance((face.rect.X + (face.rect.Width / 2)) / 2, (face.rect.Y + (face.rect.Height / 2)) / 2);
                       if (i == 0)
                       {
                           dist1 = dist;
                           cnt = i;
                       }
                       else
                       {
                           if (dist1 > dist)
                           {
                               dist1 = dist;
                               cnt = i;
                           }
                       }
                        i++;
                    }
                }
                i=0;
                foreach (MCvAvgComp face in faces[0])
                {
                    fc = true;

                    if (disT && cnt == i)
                    {
                        PointF center = new PointF((face.rect.X + (face.rect.Width / 2)), face.rect.Y + (face.rect.Height / 2));
                        posY=(face.rect.Y + (face.rect.Height / 2));
                        distA = imageFrame.GetDistance((face.rect.X + (face.rect.Width / 2)) / 2, (face.rect.Y + (face.rect.Height / 2)) / 2);
                        label1.Text = distA.ToString();
                        CircleF circle = new CircleF(center, 5);
                        My_Image.Draw(circle, new Bgr(255, double.MaxValue, 0), 1);
                        My_Image.Draw(face.rect, new Bgr(0, double.MaxValue, 255), 3);
                        PosX = (face.rect.X + (face.rect.Width / 2));
                        //label2.Text = PosX.ToString();
                    }
                    else {
                        My_Image.Draw(face.rect, new Bgr(0, double.MaxValue, 0), 3);
                    }
                    
                    i++;
                }
                if (fc == false)
                {
                    posY = 160;
                    PosX = 320;
                    if (connectSp)
                    {
                        sp.WriteLine("s");
                        label2.Text = "s";
                    }
                    if (sensor.ElevationAngle != 0)
                    {
                        try
                        {
                            sensor.ElevationAngle = 0;
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("faild to initiate");

                        }
                    }
                }
                else
                {
                    if (sensor.ElevationAngle <= 25 && posY <= 130)
                    {


                        try
                        {
                            sensor.ElevationAngle += 2;
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("faild to UP");

                        }

                    }

                    else if (sensor.ElevationAngle >= (-25) && posY >= 190)
                    {

                        try
                        {
                            sensor.ElevationAngle -= 2;
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("faild to DOWN");

                        }
                    }

                    else
                    {
                        if (connectSp && PosX >= 360)
                        {
                            sp.WriteLine("l");
                            label2.Text = "L";

                        }

                        else if (connectSp && PosX <= 280)
                        {
                            sp.WriteLine("r");
                            label2.Text = "R";
                        }
                        else if (connectSp && PosX < 360 && PosX > 280)
                        {
                            if (distA > 0)
                            {
                                sp.WriteLine("f");
                                label2.Text = "F";
                            }
                            else
                            {
                                sp.WriteLine("s");
                                label2.Text = "S";
                            }
                        }
                    }
                }
                i = 0;
               // label1.Text = imageFrame.GetDistance(imageFrame.Width / 2, imageFrame.Height / 2).ToString();
                imageBox1.Image = My_Image;

                // imageBox2.Image = cImage;
            }
        }

        private void comboBoxBAUDrate_SelectedIndexChanged(object sender, EventArgs e)
        {
            bRate = Convert.ToInt32(comboBoxBAUDrate.Text);
        }
        public int bRate
        {
            get
            {
                return baudR;
            }
            set
            {
                baudR = value;
            }
        }

        public string sBits
        {
            get
            {
                return stopB;
            }
            set
            {
                stopB = value;
            }
        }

        public string comNO
        {
            get
            {
                return COMno;
            }
            set
            {
                COMno = value;
            }
        }




        public void settings()
        {
            comboBoxBAUDrate.Items.Clear();
            comboBoxCOM.Items.Clear();
            index = -1;
            ArrayComPortsNames = SerialPort.GetPortNames();


            do
            {
                index += 1;
                try
                {
                    comboBoxCOM.Items.Add(ArrayComPortsNames[index]);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show("no com port connected plz connect");
                    //this.Close();
                    break;
                }
            }

            while (!((ArrayComPortsNames[index] == ComPortName)
              || (index == ArrayComPortsNames.GetUpperBound(0))));
            Array.Sort(ArrayComPortsNames);
            comboBoxBAUDrate.Items.Add("1200");
            comboBoxBAUDrate.Items.Add("9600");
            comboBoxBAUDrate.Items.Add("38400");
            comboBoxBAUDrate.Items.Add("115200");
        }

        private void comboBoxCOM_SelectedIndexChanged(object sender, EventArgs e)
        {
            comNO = comboBoxCOM.Text;
        }
        public void PortSettings()
        {
            
            try
            {
                sp.PortName = comNO;
                sp.BaudRate = bRate;
                sp.Parity = Parity.None;
                sp.StopBits = StopBits.One;
                sp.DataBits = 8;
                sp.Handshake = Handshake.None;
                sp.RtsEnable = true;


            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            
           

        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            PortSettings();
            try
            {
                //sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                //open serial port
                sp.Open();

                //set read time out to 500 ms
                // sp.ReadTimeout = 100;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (sp.IsOpen)
            {
                MessageBox.Show("serial port has open");
                connectSp = true;
                ConnectButton.Hide();
                DisconnectButton.Show();
                RefreshButton.Hide();
            }
            else
            {
                MessageBox.Show("faild to open serial port");
                connectSp = false;
                ConnectButton.Show();
                DisconnectButton.Hide();
                RefreshButton.Show();
            }
            
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            settings();
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                connectSp = false;
                sp.Close();
                ConnectButton.Show();
                DisconnectButton.Hide();
                RefreshButton.Show();
            }
            catch (System.Exception ex)
            {
                connectSp = true;
                MessageBox.Show("faild to close serial port");
                ConnectButton.Hide();
                DisconnectButton.Show();
                RefreshButton.Hide();
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            if (connectSp)
            {
                try
                {
                    sp.Close();

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            try
            {
                sensor.Stop();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                Application.Exit();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
           
        }

    }
}
