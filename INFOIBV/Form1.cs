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
        private Bitmap houghImageBitmap;
        private int OutputWidth, OutputHeight;

        private int startX = -1, startY = -1, firstX = -1, firstY= -1;
        private int boundaryX, boundaryY, boundaryDirection;

        private int houghSize = 500;

        public INFOIBV()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
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
            if(houghImageBitmap != null) houghImageBitmap.Dispose();
            houghImageBitmap = new Bitmap(houghSize, houghSize);
            Color[,] Image = new Color[InputImage.Size.Width, InputImage.Size.Height]; // Create array to speed-up operations (Bitmap functions are very slow)
            float[,] houghImage = new float[houghSize+1, houghSize+1];

            // Setup progress bar
            progressBar.Visible = true;
            progressBar.Minimum = 1;
            progressBar.Maximum = (InputImage.Size.Width * InputImage.Size.Height)*2;
            progressBar.Value = 1;
            progressBar.Step = 1;

            // Copy input Bitmap to array            
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    Image[x, y] = InputImage.GetPixel(x, y);                // Set pixel color in array at (x,y)
                    progressBar.PerformStep();
                }
            }

            //==========================================================================================
            // TODO: include here your own code
            // example: create a negative image

            OutputWidth = InputImage.Size.Width;
            OutputHeight = InputImage.Size.Height;

            int[,] grayscaleImage = new int[InputImage.Size.Width, InputImage.Size.Height];
            for (int i = 0; i < InputImage.Size.Width; i++)
            {
                for (int j = 0; j < InputImage.Size.Height; j++)
                {
                    grayscaleImage[i, j] = toGrayscale(Image[i, j]);
                }
            }

            if (Complement.Checked)
            {
                complement(grayscaleImage);
            }

            if (BoundaryTrace.Checked)
            {
                getBoundary(grayscaleImage);
            }

            if(houghTransformCheckbox.Checked)
            {
                getHoughTransform(grayscaleImage, houghImage);
                for (int x = 0; x < houghSize - 1; x++)
                {
                    for (int y = 0; y < houghSize - 1; y++)
                    {
                        int val = truncate((int)houghImage[x, y]);
                        if (houghThresholdCheckbox.Checked)
                        {
                            int up = truncate((int)houghImage[Math.Max(Math.Min(x, houghSize - 1), 0), Math.Max(Math.Min(y + 1, houghSize - 1), 0)]);
                            int down = truncate((int)houghImage[Math.Max(Math.Min(x, houghSize - 1), 0), Math.Max(Math.Min(y - 1, houghSize - 1), 0)]);
                            int left = truncate((int)houghImage[Math.Max(Math.Min(x - 1, houghSize - 1), 0), Math.Max(Math.Min(y, houghSize - 1), 0)]);
                            int right = truncate((int)houghImage[Math.Max(Math.Min(x + 1, houghSize - 1), 0), Math.Max(Math.Min(y, houghSize - 1), 0)]);
                            if (up > val || down > val || right > val || left > val)
                            {
                                val = 0;
                            }
                            if (val < int.Parse(houghThresholdVal.Text))
                                val = 0;
                            else
                                val = 255;
                        }
                        Color color = Color.FromArgb(val,val,val);
                        houghImageBitmap.SetPixel(x, y, color);
                    }
                }
                houghImageOutput.Image = houghImageBitmap;
            }

            //truncate and return grayscale image to actual image
            for (int i = 0; i < OutputWidth; i++)
            {
                for (int j = 0; j < OutputHeight; j++)
                {
                    int color = truncate(grayscaleImage[i, j]);
                    Image[i, j] = Color.FromArgb(color, color, color);
                    progressBar.PerformStep();
                }
            }

            //==========================================================================================

            // Copy array to output Bitmap
            for (int x = 0; x < OutputWidth-1; x++)
            {
                for (int y = 0; y < OutputHeight-1; y++)
                {
                    OutputImage.SetPixel(x, y, Image[x, y]);               // Set the pixel color at coordinate (x,y)
                }
            }
            
            pictureBox2.Image = (Image)OutputImage;                         // Display output image
            progressBar.Visible = false;                                    // Hide progress bar
        }

        private void getHoughTransform(int[,] img, float[,] houghImage)
        {
            for (int y = 0; y < OutputHeight - 1; y++)
            {
                for (int x = 0; x < OutputWidth-1; x++)
                {
                    if (img[x, y] == 0) continue;
                    houghValues(x, y, houghImage, img);   
                }
            }
        }

        private void houghValues(int x, int y, float[,] houghImage, int[,] img)
        {
            float maxAngle = (float)(int.Parse(houghAngleMaxValue.Text)*Math.PI/180f);
            float minAngle = (float)(int.Parse(houghAngleMinValue.Text) * Math.PI / 180f);
            float m = Math.Max(OutputHeight,OutputWidth);
            float l = 0.78539791f;
            float max = (float)(m * Math.Cos(l) + m * Math.Sin(l));
            float min = -max;
            float a = houghSize / (max - min);
            float b = -(a * min);
            for (float o = minAngle; o <= maxAngle; o+=(maxAngle / houghSize))
            {
                float r = (float)((x-OutputWidth/2) * Math.Cos(o) + (y-OutputHeight/2) * Math.Sin(o));
                //Debug.WriteLine(max - min + " " + a + " " + b + " " + r + " " + a*r+b);
                houghImage[(int)(Math.Round(o * houghSize / maxAngle)), (int)(Math.Round(a*r+b))] += img[x,y]/255f;
            }
        }

        private void getBoundary(int[,] img)
        {
            List<int> shapeSizeList = new List<int>();
            List<List<int[]>> shapeBoundaries = new List<List<int[]>>();
            int[,] labelImg = new int[InputImage.Size.Width, InputImage.Size.Height];
            getBinaryLabel(img, labelImg);
            int label = 1;
            for (int y = 1; y < InputImage.Size.Height-1; y++)
            {
                for (int x = 1; x < InputImage.Size.Width-1; x++)
                {
                    if (labelImg[x - 1, y] == 0 && labelImg[x, y] == 1)
                    {
                        shapeSizeList.Add(0);
                        shapeBoundaries.Add(new List<int[]>());
                        label++;
                        startX = x;
                        startY = y;
                        boundaryX = x;
                        boundaryY = y;
                        boundaryDirection = 0;
                        firstX = -1;
                        firstY = -1;
                        while (traceContour(labelImg, label,true,shapeSizeList,shapeBoundaries)) ;
                    }
                    else if(labelImg[x-1,y] > 1 && labelImg[x,y] == 0)
                    {
                        startX = x;
                        startY = y;
                        boundaryX = x;
                        boundaryY = y;
                        boundaryDirection = 0;
                        firstX = -1;
                        firstY = -1;
                        while (traceContourInner(labelImg, labelImg[x - 1, y] ,false, shapeSizeList,shapeBoundaries)) ;
                    }
                    else if (labelImg[x - 1, y] > 1 && labelImg[x, y] == 1)
                    {
                        labelImg[x, y] = labelImg[x - 1, y];
                    }
                }
            }

            int biggestShapeSize = 0;
            int biggestShape = 0;

            if (!FullShapes.Checked)
            {
                for (int x = 0; x < InputImage.Size.Width; x++)
                {
                    for (int y = 0; y < InputImage.Size.Height; y++)
                    {
                        img[x, y] = 0;
                    }
                }
                if (BiggestShape.Checked)
                {
                    for (int i = 0; i < shapeSizeList.Count; i++)
                    {
                        if (shapeSizeList[i] > biggestShapeSize)
                        {
                            biggestShapeSize = shapeSizeList[i];
                            biggestShape = i;
                        }
                    }
                    foreach (int[] pos in shapeBoundaries[biggestShape])
                    {
                        int x = pos[0];
                        int y = pos[1];
                        img[x, y] = 255;
                    }
                }
                else
                {
                    foreach (List<int[]> lists in shapeBoundaries)
                    {
                        foreach (int[] pos in lists)
                        {
                            int x = pos[0];
                            int y = pos[1];
                            img[x, y] = 255;
                        }
                    }
                }
            }
            else
            {
                if (BiggestShape.Checked)
                {
                    for (int i = 0; i < shapeSizeList.Count; i++)
                    {
                        if (shapeSizeList[i] > biggestShapeSize)
                        {
                            biggestShapeSize = shapeSizeList[i];
                            biggestShape = i;
                        }
                    }
                }

                for (int x = 0; x < InputImage.Size.Width; x++)
                {
                    for (int y = 0; y < InputImage.Size.Height; y++)
                    {
                        if (labelImg[x, y] <= 0)
                            img[x, y] = 0;
                        if (BiggestShape.Checked)
                        {
                            if (biggestShape + 2 == labelImg[x, y])
                                img[x, y] = 255;
                            else
                                img[x, y] = 0;
                        }
                        else if (labelImg[x, y] > 1)
                            img[x, y] = 255;// - labelImg[x, y] * 20;
                    }
                }
            }
        }

        private bool traceContour(int[,] labelImg, int label, bool sizeCount, List<int> shapeSize, List<List<int[]>> shapeBoundaries)
        {
            labelPixels(labelImg, boundaryX, boundaryY, label);

            if (sizeCount)
                shapeSize[label-2]++;
            if(!FullShapes.Checked)
                shapeBoundaries[label - 2].Add(new int[2] {boundaryX,boundaryY});
            //Debug.WriteLine(startX + " " + startY + " " + firstX + " " + firstY + " " + label + " " + boundaryDirection + " " + boundaryX + " " + boundaryY);
            if (firstX == -1 && firstY == -1 && (boundaryX != startX || boundaryY != startY))
            {
                firstX = boundaryX;
                firstY = boundaryY;
            }
            for (int i = boundaryDirection; i < boundaryDirection+4; i++)
            {
                if (i%4 == 0)
                {
                    if (boundaryY > 0 && labelImg[boundaryX, boundaryY - 1] >= 1)
                    {
                        if (boundaryX == firstX && boundaryY - 1 == firstY && boundaryX == startX && boundaryY == startY)
                            return false;
                        boundaryDirection = (i + 3) % 4;
                        boundaryY = boundaryY - 1;
                        return true;
                    }
                    else
                        continue;
                }
                if (i % 4 == 1)
                {
                    if (boundaryX < OutputWidth-1 && labelImg[boundaryX+1, boundaryY] >= 1)
                    {
                        if (boundaryX + 1 == firstX && boundaryY == firstY && boundaryX == startX && boundaryY == startY)
                            return false;
                        boundaryDirection = (i + 3) % 4;
                        boundaryX = boundaryX+1;
                        return true;
                    }
                    else
                        continue;
                }
                if (i % 4 == 2)
                {
                    if (boundaryY < OutputHeight-1 && labelImg[boundaryX, boundaryY + 1] >= 1)
                    {
                        if (boundaryX == firstX && boundaryY + 1 == firstY && boundaryX == startX && boundaryY == startY)
                            return false;
                        boundaryDirection = (i + 3) % 4;
                        boundaryY = boundaryY + 1;
                        return true;
                    }
                    else
                        continue;
                }
                if (i % 4 == 3)
                {
                    if (boundaryX > 0 && labelImg[boundaryX - 1, boundaryY] >= 1)
                    {
                        if (boundaryX - 1 == firstX && boundaryY == firstY && boundaryX == startX && boundaryY == startY)
                            return false;
                        boundaryDirection = (i + 3) % 4;
                        boundaryX = boundaryX - 1;
                        return true;
                    }
                    else
                        continue;
                }
            }
            return false;
        }

        private bool traceContourInner(int[,] labelImg, int label, bool sizeCount, List<int> shapeSize, List<List<int[]>> shapeBoundaries)
        {
            labelPixels(labelImg, boundaryX, boundaryY, label);

            if (sizeCount)
                shapeSize[label - 2]++;
            if (!FullShapes.Checked)
                shapeBoundaries[label - 2].Add(new int[2] { boundaryX, boundaryY });
            if (firstX == -1 && firstY == -1 && (boundaryX != startX || boundaryY != startY))
            {
                firstX = boundaryX;
                firstY = boundaryY;
            }
            for (int i = boundaryDirection+4; i > boundaryDirection; i--)
            {
                if (i % 4 == 0)
                {
                    if (boundaryY > 0 && labelImg[boundaryX, boundaryY - 1] >= 1)
                    {
                        if (boundaryX == firstX && boundaryY - 1 == firstY && boundaryX == startX && boundaryY == startY)
                            return false;
                        boundaryDirection = i+1;
                        boundaryY = boundaryY - 1;
                        return true;
                    }
                    else
                        continue;
                }
                if (i % 4 == 1)
                {
                    if (boundaryX < OutputWidth - 1 && labelImg[boundaryX + 1, boundaryY] >= 1)
                    {
                        if (boundaryX + 1 == firstX && boundaryY == firstY && boundaryX == startX && boundaryY == startY)
                            return false;
                        boundaryDirection = i+1;
                        boundaryX = boundaryX + 1;
                        return true;
                    }
                    else
                        continue;
                }
                if (i % 4 == 2)
                {
                    if (boundaryY < OutputHeight - 1 && labelImg[boundaryX, boundaryY + 1] >= 1)
                    {
                        if (boundaryX == firstX && boundaryY + 1 == firstY && boundaryX == startX && boundaryY == startY)
                            return false;
                        boundaryDirection = i+1;
                        boundaryY = boundaryY + 1;
                        return true;
                    }
                    else
                        continue;
                }
                if (i % 4 == 3)
                {
                    if (boundaryX > 0 && labelImg[boundaryX - 1, boundaryY] >= 1)
                    {
                        if (boundaryX - 1 == firstX && boundaryY == firstY && boundaryX == startX && boundaryY == startY)
                            return false;
                        boundaryDirection = i+1;
                        boundaryX = boundaryX - 1;
                        return true;
                    }
                    else
                        continue;
                }
            }
            return false;
        }

        private void labelPixels(int[,] labelImg, int x, int y, int label)
        {
            labelImg[x, y] = label;
            if (x < OutputWidth-1 && labelImg[x + 1, y] == 0)
                labelImg[x + 1, y] = -1;
            if (y < OutputHeight-1 && labelImg[x, y + 1] == 0)
                labelImg[x, y + 1] = -1;

        }

        private void getBinaryLabel(int[,] img, int[,] labelImg) {
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    int pixelColor = img[x, y];
                    if (pixelColor < 128)
                        img[x, y] = 0;
                    else
                    {
                        labelImg[x, y] = 1;
                        img[x, y] = 255;
                    }
                }
            }
        }

        private void houghTransformCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (houghTransformCheckbox.Checked)
                BoundaryTrace.Checked = true;
        }

        private int[,] complement(int[,] img)
        {
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    int pixelColor = img[x, y];
                    img[x, y] = 255-pixelColor;
                }
            }
            return img;
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

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (OutputImage == null) return;                                // Get out if no output image
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
                OutputImage.Save(saveImageDialog.FileName);                 // Save the output image
        }

        private void BiggestShape_CheckedChanged(object sender, EventArgs e)
        {
            if (BiggestShape.Checked)
                BoundaryTrace.Checked = true;
        }

        private void FullShapes_CheckedChanged(object sender, EventArgs e)
        {
            if (FullShapes.Checked)
                BoundaryTrace.Checked = true;
        }

    }
}
