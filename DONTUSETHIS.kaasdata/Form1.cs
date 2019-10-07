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
        private Bitmap InputImage2;
        private Bitmap OutputImage;
        private int OutputWidth, OutputHeight;

        private int startX = -1, startY = -1, firstX = -1, firstY = -1;
        private int boundaryX, boundaryY, boundaryDirection;

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
                    pictureBox1.Image = (Image)InputImage;                 // Display input image
            }
        }

        private void LoadImageButton2_Click(object sender, EventArgs e)
        {
            if (openImageDialog2.ShowDialog() == DialogResult.OK)             // Open File Dialog
            {
                string file = openImageDialog2.FileName;                     // Get the file name
                imageFileName2.Text = file;                                  // Show file name
                if (InputImage2 != null) InputImage2.Dispose();               // Reset image
                InputImage2 = new Bitmap(file);                              // Create new Bitmap from file
                if (InputImage2.Size.Height <= 0 || InputImage2.Size.Width <= 0 ||
                    InputImage2.Size.Height > 512 || InputImage2.Size.Width > 512) // Dimension check
                    MessageBox.Show("Error in image dimensions (have to be > 0 and <= 512)");
                else
                    pictureBox3.Image = (Image)InputImage2;                 // Display input image
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (InputImage == null) return;                                 // Get out if no input image
            if (OutputImage != null) OutputImage.Dispose();                 // Reset output image
            OutputImage = new Bitmap(InputImage.Size.Width, InputImage.Size.Height); // Create new output image
            Color[,] Image = new Color[InputImage.Size.Width, InputImage.Size.Height]; // Create array to speed-up operations (Bitmap functions are very slow)
            Color[,] Image2 = null;
            if (openImageDialog2.FileName != "")
                Image2 = new Color[InputImage2.Size.Width, InputImage2.Size.Height]; // Create array to speed-up operations (Bitmap functions are very slow)

            if (String.IsNullOrEmpty(SESize.Text))
                SESize.Text = "1";

            int[,] SE = StructureEl(SEShape.Text, int.Parse(SESize.Text));

            // Setup progress bar
            progressBar.Visible = true;
            progressBar.Minimum = 1;
            progressBar.Maximum = (InputImage.Size.Width * InputImage.Size.Height) * 2;
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

            if (filter.Text == "dilation" || filter.Text == "erosion")
                grayscaleImage = applyKernel(grayscaleImage, SE, filter.Text);

            if (filter.Text == "opening")
            {
                grayscaleImage = applyKernel(grayscaleImage, SE, "erosion");
                grayscaleImage = applyKernel(grayscaleImage, SE, "dilation");
            }

            if (filter.Text == "closing")
            {
                grayscaleImage = applyKernel(grayscaleImage, SE, "dilation");
                grayscaleImage = applyKernel(grayscaleImage, SE, "erosion");
            }

            if (ANDOR.Checked && imageFileName2.Text != "")
            {
                int[,] grayscaleImage2 = new int[InputImage2.Size.Width, InputImage2.Size.Height];
                for (int i = 0; i < InputImage2.Size.Width; i++)
                {
                    for (int j = 0; j < InputImage2.Size.Height; j++)
                    {
                        grayscaleImage2[i, j] = toGrayscale(InputImage2.GetPixel(i, j));
                    }
                }
                if (ANDORDropdown.Text == "AND")
                {
                    grayscaleImage = ANDfunction(grayscaleImage, grayscaleImage2);
                }
                if (ANDORDropdown.Text == "OR")
                {
                    grayscaleImage = ORfunction(grayscaleImage, grayscaleImage2);
                    OutputWidth = grayscaleImage.GetLength(0);
                    OutputHeight = grayscaleImage.GetLength(1);
                    Image = new Color[OutputWidth, OutputHeight];
                    OutputImage = new Bitmap(OutputWidth, OutputHeight);
                }
            }

            if (BoundaryTrace.Checked)
            {
                getBoundary(grayscaleImage);
            }

            valueCounting(grayscaleImage);

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
            for (int x = 0; x < OutputWidth; x++)
            {
                for (int y = 0; y < OutputHeight; y++)
                {
                    OutputImage.SetPixel(x, y, Image[x, y]);               // Set the pixel color at coordinate (x,y)
                }
            }

            pictureBox2.Image = (Image)OutputImage;                         // Display output image
            progressBar.Visible = false;                                    // Hide progress bar
        }

        private void valueCounting(int[,] grayscaleImage)
        {
            int[] pixelCount = new int[256];
            for (int i = 0; i < OutputWidth; i++)
            {
                for (int j = 0; j < OutputHeight; j++)
                {
                    int val = truncate(grayscaleImage[i, j]);
                    pixelCount[val]++;
                }
            }

            pixelCountText.Text = " ";
            pixelCountChart.Series[0].Points.Clear();
            int count = 0;
            for (int i = 0; i < 256; i++)
            {
                if (pixelCount[i] > 0)
                {
                    count++;
                    pixelCountText.Text += (i + 1) + " ";
                }
                pixelCountChart.Series[0].Points.AddXY(i + 1, pixelCount[i]);
            }
            pixelCountText.Text += " - Total values => " + count;
        }

        private void getBoundary(int[,] img)
        {
            List<int> shapeSizeList = new List<int>();
            List<List<int[]>> shapeBoundaries = new List<List<int[]>>();
            int[,] labelImg = new int[InputImage.Size.Width, InputImage.Size.Height];
            getBinaryLabel(img, labelImg);
            int label = 1;
            for (int y = 1; y < InputImage.Size.Height - 1; y++)
            {
                for (int x = 1; x < InputImage.Size.Width - 1; x++)
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
                        while (traceContour(labelImg, label, true, shapeSizeList, shapeBoundaries)) ;
                    }
                    else if (labelImg[x - 1, y] > 1 && labelImg[x, y] == 0)
                    {
                        startX = x;
                        startY = y;
                        boundaryX = x;
                        boundaryY = y;
                        boundaryDirection = 0;
                        firstX = -1;
                        firstY = -1;
                        while (traceContourInner(labelImg, labelImg[x - 1, y], false, shapeSizeList, shapeBoundaries)) ;
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
                            img[x, y] = 255 - labelImg[x, y] * 20;
                    }
                }
            }
        }

        private bool traceContour(int[,] labelImg, int label, bool sizeCount, List<int> shapeSize, List<List<int[]>> shapeBoundaries)
        {
            labelPixels(labelImg, boundaryX, boundaryY, label);

            if (sizeCount)
                shapeSize[label - 2]++;
            if (!FullShapes.Checked)
                shapeBoundaries[label - 2].Add(new int[2] { boundaryX, boundaryY });
            Debug.WriteLine(startX + " " + startY + " " + firstX + " " + firstY + " " + label + " " + boundaryDirection + " " + boundaryX + " " + boundaryY);
            if (firstX == -1 && firstY == -1 && (boundaryX != startX || boundaryY != startY))
            {
                firstX = boundaryX;
                firstY = boundaryY;
            }
            for (int i = boundaryDirection; i < boundaryDirection + 4; i++)
            {
                if (i % 4 == 0)
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
                    if (boundaryX < OutputWidth - 1 && labelImg[boundaryX + 1, boundaryY] >= 1)
                    {
                        if (boundaryX + 1 == firstX && boundaryY == firstY && boundaryX == startX && boundaryY == startY)
                            return false;
                        boundaryDirection = (i + 3) % 4;
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
            for (int i = boundaryDirection + 4; i > boundaryDirection; i--)
            {
                if (i % 4 == 0)
                {
                    if (boundaryY > 0 && labelImg[boundaryX, boundaryY - 1] >= 1)
                    {
                        if (boundaryX == firstX && boundaryY - 1 == firstY && boundaryX == startX && boundaryY == startY)
                            return false;
                        boundaryDirection = i + 1;
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
                        boundaryDirection = i + 1;
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
                        boundaryDirection = i + 1;
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
                        boundaryDirection = i + 1;
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
            if (x < OutputWidth - 1 && labelImg[x + 1, y] == 0)
                labelImg[x + 1, y] = -1;
            if (y < OutputHeight - 1 && labelImg[x, y + 1] == 0)
                labelImg[x, y + 1] = -1;

        }

        private void getBinaryLabel(int[,] img, int[,] labelImg)
        {
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

        private int[,] complement(int[,] img)
        {
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    int pixelColor = img[x, y];
                    img[x, y] = 255 - pixelColor;
                }
            }
            return img;
        }

        private int[,] ANDfunction(int[,] img1, int[,] img2)
        {
            OutputWidth = Math.Min(InputImage.Size.Width, InputImage2.Size.Width);
            OutputHeight = Math.Min(InputImage.Size.Height, InputImage2.Size.Height);
            int[,] OutputImage = new int[OutputWidth, OutputHeight];
            for (int i = 0; i < OutputWidth; i++)
            {
                for (int j = 0; j < OutputHeight; j++)
                {
                    if (toGrayscale(InputImage.GetPixel(i, j)) > 128 && toGrayscale(InputImage2.GetPixel(i, j)) > 128)
                    {
                        OutputImage[i, j] = 255;
                    }
                    else
                    {
                        OutputImage[i, j] = 0;
                    }
                }
            }
            return OutputImage;
        }

        private int[,] ORfunction(int[,] img1, int[,] img2)
        {
            OutputWidth = Math.Max(InputImage.Size.Width, InputImage2.Size.Width);
            OutputHeight = Math.Max(InputImage.Size.Height, InputImage2.Size.Height);
            int[,] OutputImage = new int[OutputWidth, OutputHeight];

            for (int i = 0; i < OutputWidth; i++)
            {
                for (int j = 0; j < OutputHeight; j++)
                {
                    int pixelimg1 = 0;
                    int pixelimg2 = 0;
                    if (i < InputImage.Size.Width && j < InputImage.Size.Height)
                        pixelimg1 = toGrayscale(InputImage.GetPixel(i, j));
                    if (i < InputImage2.Size.Width && j < InputImage2.Size.Height)
                        pixelimg2 = toGrayscale(InputImage2.GetPixel(i, j));
                    if (pixelimg1 > 128 || pixelimg2 > 128)
                    {
                        OutputImage[i, j] = 255;
                    }
                    else
                    {
                        OutputImage[i, j] = 0;
                    }
                }
            }
            return OutputImage;
        }

        private int[,] StructureEl(string shape, int size)
        {
            int[,] SE = new int[1 + (size * 2), 1 + (size * 2)];
            if (shape == "+")
            {
                for (int i = 0; i <= (size * 2); i++)
                {
                    SE[size, i] = 1;
                    SE[i, size] = 1;
                }
            }
            else
            {
                for (int x = 0; x < (1 + (size * 2)); x++)
                {
                    for (int y = 0; y < (1 + (size * 2)); y++)
                    {
                        SE[x, y] = 1;
                    }
                }
            }
            SE[size, size] = 2;
            return SE;
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

        private int[,] applyKernel(int[,] img, int[,] kernel, string filter)
        {
            int[,] ImageWithkernel = new int[InputImage.Size.Width, InputImage.Size.Height];
            int size = ((int)Math.Sqrt(kernel.Length) - 1) / 2;
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    if (x + size >= InputImage.Size.Width || x - size < 0 || y + size >= InputImage.Size.Height || y - size < 0)
                        continue;
                    int max = 0;
                    int min = 255;
                    for (int n = -size; n <= size; n++)
                    {
                        for (int m = -size; m <= size; m++)
                        {
                            if (kernel[n + size, m + size] == 0)
                                continue;
                            int value = kernel[n + size, m + size] + img[x + n, y + m];
                            if (value > max)
                                max = value;
                            if (value < min)
                                min = value;
                        }
                    }
                    if (filter == "dilation")
                        ImageWithkernel[x, y] = max;
                    else
                    {
                        ImageWithkernel[x, y] = min;
                    }
                }
            }

            //borderHandling(ImageWithkernel);
            return ImageWithkernel;
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
