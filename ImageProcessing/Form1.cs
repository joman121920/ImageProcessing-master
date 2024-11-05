using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using WebCamLib;
using ImageProcess2;
using AForge.Video.DirectShow;
using AForge.Video;


namespace ImageProcessing {
    public partial class Form1 : Form {
        Bitmap loaded, processed, imageA, imageB, resultImage;

        Device[] allDevice;

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap backgroundBitmap;


        public Form1() {
            InitializeComponent();
            InitializeVideo();
        }

        private void InitializeVideo() {
            
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            
            if (videoDevices.Count > 0) {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString); 
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
            } else {
                MessageBox.Show("No video sources found.");
            }
        }

        
private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
{
    
    Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
    
    pictureBox3.Image = frame; 
}


        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {
            loaded = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loaded;
        }

        private void pixelCopyToolStripMenuItem_Click(object sender, EventArgs e) {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            
            for (int i = 0; i < loaded.Width; i++) {
                for (int j = 0; j < loaded.Height; j++) {
                    pixel = loaded.GetPixel(i, j);
                    processed.SetPixel(i, j, pixel);
                }
            }
            pictureBox2.Image = processed;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e) {
            processed.Save(saveFileDialog1.FileName);
        }

        private void greyScalingToolStripMenuItem_Click(object sender, EventArgs e) {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            int avg;

            for (int i = 0; i < loaded.Width; i++) {
                for (int j = 0; j < loaded.Height; j++) {
                    pixel = loaded.GetPixel(i, j);
                    avg = (int) (pixel.R + pixel.G + pixel.B) / 3;
                    Color gray = Color.FromArgb(avg, avg, avg);
                    processed.SetPixel(i, j, gray);
                }
            }
            pictureBox2.Image = processed;
        }

        private void inversionToolStripMenuItem_Click(object sender, EventArgs e) {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
           

            for (int i = 0; i < loaded.Width; i++) {
                for (int j = 0; j < loaded.Height; j++) {
                    pixel = loaded.GetPixel(i, j);
                    
                    Color invert = Color.FromArgb(255-pixel.R, 255-pixel.G, 255-pixel.B);
                    processed.SetPixel(i, j, invert);
                }
            }
            pictureBox2.Image = processed;
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e) {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            int tr, tg, tb;

            for (int i = 0; i < loaded.Width; i++) {
                for (int j = 0; j < loaded.Height; j++) {
                    pixel = loaded.GetPixel(i, j);

                    
                    int r = pixel.R;
                    int g = pixel.G;
                    int b = pixel.B;

                   
                    tr = (int)(0.393 * r + 0.769 * g + 0.189 * b);
                    tg = (int)(0.349 * r + 0.686 * g + 0.168 * b);
                    tb = (int)(0.272 * r + 0.534 * g + 0.131 * b);

                    
                    if (tr > 255) tr = 255;
                    if (tg > 255) tg = 255;
                    if (tb > 255) tb = 255;

                    Color sepia = Color.FromArgb(tr, tg, tb);
                    processed.SetPixel(i, j, sepia);
                }
            }
            pictureBox2.Image = processed;
        }

        private void imageLoaderButton_Click(object sender, EventArgs e) {
            openFileDialog2.ShowDialog();
            pictureBox3.Image = imageB;
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e) {
            imageB = new Bitmap(openFileDialog2.FileName);
        }

        private void backgroundImageButton_Click(object sender, EventArgs e) {
            openFileDialog3.ShowDialog();
            pictureBox4.Image = imageA;
        }

        private void openFileDialog3_FileOk(object sender, CancelEventArgs e) {
            imageA = new Bitmap(openFileDialog3.FileName);
        }

        private void subtractImageButton_Click(object sender, EventArgs e) {
            timer1.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e) {
            allDevice = DeviceManager.GetAllDevices();
        }

        // Start the webcam
        private void onToolStripMenuItem_Click(object sender, EventArgs e) {
            videoSource.Start();
        }

        // Stop the webcam
        private void offToolStripMenuItem_Click(object sender, EventArgs e) {
            videoSource.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            
            Bitmap currentFrame = (Bitmap)pictureBox3.Image.Clone();
            resultImage = new Bitmap(currentFrame.Width, currentFrame.Height);

            
            Color mygreen = Color.FromArgb(0, 0, 255);
            int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;
            int threshold = 5;

            for (int x = 0; x < currentFrame.Width; x++) {
                for (int y = 0; y < currentFrame.Height; y++) {
                    Color pixel = currentFrame.GetPixel(x, y);
                    Color backpixel = imageA.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractValue = Math.Abs(grey - greygreen);

                    if (subtractValue > threshold) {
                        resultImage.SetPixel(x, y, pixel);
                    } else {
                        resultImage.SetPixel(x, y, backpixel);
                    }
                }
            }

            pictureBox5.Image = resultImage;
        }


        private void mirrorHorizToolStripMenuItem_Click(object sender, EventArgs e) {
            processed = new Bitmap(loaded.Width, loaded.Height);

            for (int i = 0; i < loaded.Width; i++) {
                for (int j = 0; j < loaded.Height; j++) {
                    
                    Color pixel = loaded.GetPixel(loaded.Width - 1 - i, j);
                    processed.SetPixel(i, j, pixel);
                }
            }

            pictureBox2.Image = processed;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e) {
            
            
            int[] histogram = new int[256];
            for (int i = 0; i < loaded.Width; i++) {
                for (int j = 0; j < loaded.Height; j++) {
                    int grayValue = loaded.GetPixel(i, j).R; 
                    histogram[grayValue]++;
                }
            }

          
            int maxFrequency = histogram.Max();
            Bitmap histogramImage = new Bitmap(256, 100);
            using (Graphics g = Graphics.FromImage(histogramImage)) {
                g.Clear(Color.White);
                for (int i = 0; i < 256; i++) {
                    int barHeight = (int)((histogram[i] / (float)maxFrequency) * 100);
                    g.DrawLine(Pens.Black, i, histogramImage.Height, i, histogramImage.Height - barHeight);
                }
            }

            
            pictureBox2.Image = histogramImage;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void mirrorVerticalToolStripMenuItem_Click(object sender, EventArgs e) {
            processed = new Bitmap(loaded.Width, loaded.Height);

            for (int i = 0; i < loaded.Width; i++) {
                for (int j = 0; j < loaded.Height; j++) {
                    
                    Color pixel = loaded.GetPixel(i, loaded.Height - 1 - j);
                    processed.SetPixel(i, j, pixel);
                }
            }

            pictureBox2.Image = processed;
        }


        

    }
}
