using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace INFOIBV
{
    public partial class INFOIBV : Form
    {
        private Bitmap InputImage;
        private Bitmap OutputImage;

        public INFOIBV()
        {
            InitializeComponent();
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("AAA");
           if (openImageDialog.ShowDialog() == DialogResult.OK)             // Open File Dialog
            {
                string file = openImageDialog.FileName;                     // Get the file name
                imageFileName.Text = file;                                  // Show file name
                if (InputImage != null) InputImage.Dispose();               // Reset image
                InputImage = new Bitmap(file);                              // Create new Bitmap from file
                if (InputImage.Size.Height <= 0 || InputImage.Size.Width <= 0 ||
                    InputImage.Size.Height > 512 || InputImage.Size.Width > 512) // Dimension check
                    MessageBox.Show("Error in image dimensions (have to be > 0 and <= 512)");
                else
                    pictureBox1.Image = (Image) InputImage;                 // Display input image
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (InputImage == null) return;                                 // Get out if no input image
            if (OutputImage != null) OutputImage.Dispose();                 // Reset output image
            OutputImage = new Bitmap(InputImage.Size.Width, InputImage.Size.Height); // Create new output image
            Color[,] Image = new Color[InputImage.Size.Width, InputImage.Size.Height]; // Create array to speed-up operations (Bitmap functions are very slow)
            int[,] houghImage = new int[InputImage.Size.Width, InputImage.Size.Height];

            // Setup progress bar
            progressBar.Visible = true;
            progressBar.Minimum = 1;
            progressBar.Maximum = InputImage.Size.Width * InputImage.Size.Height * 2;
            progressBar.Value = 1;
            progressBar.Step = 1;

            // Copy input Bitmap to array            
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    Image[x, y] = InputImage.GetPixel(x, y);                // Set pixel color in array at (x,y)
                }
            }

            //==========================================================================================
            int[,] grayscaleImage = new int[InputImage.Size.Width, InputImage.Size.Height];
            for (int i = 0; i < InputImage.Size.Width; i++)
            {
                for (int j = 0; j < InputImage.Size.Height; j++)
                {
                    grayscaleImage[i, j] = toGrayscale(Image[i, j]);
                }
            }
            if (houghTransformCheck.Checked)
            {
                houghImage = houghTransform(grayscaleImage);
            }
            //truncate and return grayscale image to actual image
            for (int i = 0; i < InputImage.Size.Width; i++)
            {
                for (int j = 0; j < InputImage.Size.Height; j++)
                {
                    int color = truncate(houghImage[i, j]);
                    Image[i, j] = Color.FromArgb(color, color, color);
                    progressBar.PerformStep();
                }
            }

            // Copy array to output Bitmap
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    OutputImage.SetPixel(x, y, Image[x, y]);               // Set the pixel color at coordinate (x,y)
                }
            }
            
            pictureBox2.Image = (Image)OutputImage;                         // Display output image
            progressBar.Visible = false;                                    // Hide progress bar
        }
        
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (OutputImage == null) return;                                // Get out if no output image
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
                OutputImage.Save(saveImageDialog.FileName);                 // Save the output image
        }
        
        private int[,] houghTransform(int[,] img)
        {
            getBinary(img);
            int[,] houghImage = new int[InputImage.Size.Width, InputImage.Size.Height];
            for (int y = 0; y < InputImage.Size.Height; y++)
            {
                for (int x = 0; x < InputImage.Size.Width; x++)
                {
                    if (img[x, y] == 255)
                    {
                        Debug.WriteLine(x + " " + y);
                        houghPerPixel(img, x, y, houghImage);
                    }
                }
            }
            return houghImage;
        }

        private void houghPerPixel(int[,] img, int xStart, int yStart, int[,] houghImage)
        {
            for (int y = yStart; y < InputImage.Size.Height; y++)
            {
                for (int x = xStart; x < InputImage.Size.Width; x++)
                {
                    if (img[x, y] == 255)
                    {
                        int[] line = getHoughLine(img, xStart, yStart, x, y);
                        houghImage[line[1], line[0]] = 255;
                    }
                }
            }
        }

        private int[] getHoughLine(int[,] img, int xStart, int yStart, int x, int y)
        {
            int xdif = x - xStart;
            int ydif = y - yStart;
            if (xdif == 0)
                return new int[] { 0, 0 };
            int a = ydif / xdif;
            int b = yStart - (a * xStart);
            Debug.WriteLine(a + " " + b);
            int[] ret = new int[] { a, b };
            return ret;
        }

        private int truncate(int value)
        {
            return Math.Max(Math.Min(value, 255), 0);
        }

        private int toGrayscale(Color pixelColor)
        {
            int grayscale = (int)((pixelColor.R * 0.3f) + (pixelColor.G * 0.59f) + (pixelColor.B * 0.11f));
            return grayscale;
        }

        private void getBinary(int[,] img)
        {
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    int pixelColor = img[x, y];
                    if (pixelColor < 128)
                        img[x, y] = 0;
                    else
                        img[x, y] = 255;
                }
            }
        }

    }
}
