using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Drawing.Imaging;
using System.Threading;




namespace EscapeRoom
{
    public partial class Form1 : Form
    {
        //슬라이딩 메뉴의 최대, 최소 폭 크기
        const int MAX_SLIDING_WIDTH = 200;
        const int MIN_SLIDING_WIDTH = 50;
        //슬라이딩 메뉴가 보이는/접히는 속도 조절
        const int STEP_SLIDING = 10;
        //최초 슬라이딩 메뉴 크기
        int _posSliding = 200;



        SoundPlayer backgroundMusic;
        SoundPlayer laserSound;
        bool bgPlaying = false;




        int opacity = 240;
        Image original = null;
        PictureBox sourceImage = new PictureBox();
        bool[] hasItem = new bool[10];
        
        public class Item
        {
            public static List<PictureBox> items = new List<PictureBox>();
            
        }
        public Form1()
        {
            Item item = new Item();
            InitializeComponent(); this.MouseMove += new MouseEventHandler(Mouse_Move);

            mediaPlayer.URL = @"C:\Users\inven\Videos\2022-05-24 10-33-45.mkv";

            MXP.Ctlcontrols.play();
            MXP.URL = @"C:\Users\inven\OneDrive\바탕 화면\EscapeRoom\EscapeRoom\Resources\backgroundSound.mp3";
            MXP.settings.playCount = 9999; // repeating the music when it ends
            MXP.Ctlcontrols.stop();
            MXP.Visible = false;
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Item.items.Add(new PictureBox()); 
            pictureBox2.Visible = true;
            if (bgPlaying == true)
            {
                backgroundMusic.Stop();
                bgPlaying = false;
            }
           
            MXP.Ctlcontrols.play(); 

        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Point local = this.PointToClient(Cursor.Position);
            e.Graphics.DrawEllipse(Pens.Red, local.X - 25, local.Y - 25, 20, 20);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
            pictureBox2.Visible = true;
            playSimpleSound();

            sourceImage.ImageLocation = "../../Resources/1.jpg";
            sourceImage.SizeMode = PictureBoxSizeMode.StretchImage;
            sourceImage.Size = new Size(120, 120);
            sourceImage.Location = new Point(30, 50);
            sourceImage.Cursor = Cursors.Hand;
            sourceImage.Click += ItemClick;
            itemBox.Controls.Add(sourceImage);
        }
        

        public void ItemClick(object sender, EventArgs e)
        {
            sourceImage.Size = new Size(140, 140);
            sourceImage.Location = new Point(20, 40);
            sourceImage.ImageLocation = "../../Resources/2.jpg";
            SoundPlayer simpleSound = new SoundPlayer(@"c:\Windows\Media\chimes.wav");
            simpleSound.Play();
            
        }
        public void playSimpleSound()
        {
            SoundPlayer simpleSound = new SoundPlayer(@"C:\Users\inven\Music\backgroundSound.wav");
            simpleSound.Play();
        }


        private void Mouse_Move(object sender, EventArgs e)
        {

            //X-ray
            int x = Cursor.Position.X;
            //Y-ray
            int y = Cursor.Position.Y;
            this.textBox1.Text = x.ToString();
            this.textBox2.Text = y.ToString(); 
            mediaPlayer.StatusChange += mediaExit;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (!hasItem[0])
            {
                label2.Text = "아무일도 없습니다.";
                if (original == null) original = (Bitmap)pictureBox1.Image.Clone();
                timer1.Start();
               
            }

        }








        static Bitmap SetAlpha(Bitmap bmpIn, int alpha)
        {
            Bitmap bmpOut = new Bitmap(bmpIn.Width, bmpIn.Height);
            float a = alpha / 255f;
            Rectangle r = new Rectangle(0, 0, bmpIn.Width, bmpIn.Height);

                float[][] matrixItems = {
            new float[] {1, 0, 0, 0, 0},
            new float[] {0, 1, 0, 0, 0},
            new float[] {0, 0, 1, 0, 0},
            new float[] {0, 0, 0, a, 0},
            new float[] {0, 0, 0, 0, 1}};

            ColorMatrix colorMatrix = new ColorMatrix(matrixItems);

            ImageAttributes imageAtt = new ImageAttributes();
            imageAtt.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            using (Graphics g = Graphics.FromImage(bmpOut))
                g.DrawImage(bmpIn, r, r.X, r.Y, r.Width, r.Height, GraphicsUnit.Pixel, imageAtt);

            return bmpOut;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (opacity > 0.0)
            {

                opacity -= 30;
                pictureBox1.Image = SetAlpha((Bitmap)original, opacity);
                
            }
            else
            {
                
                timer1.Stop(); 
                
            }
        }

        private void PlayLaserSound(object sender, EventArgs e)
        {
            laserSound = new SoundPlayer(@"C:\Users\inven\OneDrive\바탕 화면\EscapeRoom\EscapeRoom\Resources\laserSound.wav");
            laserSound.Play();
        }

        private void PlayBackgroundSoundPlayer(object sender, EventArgs e)
        {
            backgroundMusic = new SoundPlayer(@"C:\Users\inven\OneDrive\바탕 화면\EscapeRoom\EscapeRoom\Resources\flashlight-click.wav");
            backgroundMusic.PlayLooping();
            //MXP.Ctlcontrols.stop(); // stop the media player
           // bgPlaying = true; // set playing to true
        }

        private void PlayMediaPlayer(object sender, EventArgs e)
        {
            if (bgPlaying == true)
            {
                backgroundMusic.Stop();
                bgPlaying = false;
            }
            
            MXP.Ctlcontrols.play();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void mediaPlayer_Enter(object sender, EventArgs e)
        {
            mediaPlayer.fullScreen = true;
            
            
        }
        public void mediaExit(object sender, EventArgs e)
        {
            mediaPlayer.Visible=false;
        }

        private void timerSliding_Tick(object sender, EventArgs e)
        {
            if (checkBoxHide.Checked == true)
            {
                //슬라이딩 메뉴를 숨기는 동작
                _posSliding -= STEP_SLIDING;
                if (_posSliding <= MIN_SLIDING_WIDTH)
                    timerSliding.Stop();
            }
            else
            {
                //슬라이딩 메뉴를 보이는 동작
                _posSliding += STEP_SLIDING;
                if (_posSliding >= MAX_SLIDING_WIDTH)
                    timerSliding.Stop();
            }

            itemBox.Width = _posSliding;
        }

        private void checkBoxHide_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHide.Checked == true)
            {
                //슬라이딩 메뉴가 접혔을 때, 메뉴 버튼의 표시
                checkBoxHide.Text = "<";
            }
            else
            {
                //슬라이딩 메뉴가 보였을 때, 메뉴 버튼의 표시

                checkBoxHide.Text = ">";
            }

            //타이머 시작
            timerSliding.Start();
        }
    }
}
