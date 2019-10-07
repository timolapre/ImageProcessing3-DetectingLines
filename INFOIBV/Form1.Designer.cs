using System.Windows.Forms;

namespace INFOIBV
{
    partial class INFOIBV
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LoadImageButton = new System.Windows.Forms.Button();
            this.openImageDialog = new System.Windows.Forms.OpenFileDialog();
            this.imageFileName = new System.Windows.Forms.TextBox();
            this.openImageDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.saveImageDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveButton = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.Complement = new System.Windows.Forms.CheckBox();
            this.BoundaryTrace = new System.Windows.Forms.CheckBox();
            this.BiggestShape = new System.Windows.Forms.CheckBox();
            this.FullShapes = new System.Windows.Forms.CheckBox();
            this.houghTransformCheckbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // LoadImageButton
            // 
            this.LoadImageButton.Location = new System.Drawing.Point(16, 15);
            this.LoadImageButton.Margin = new System.Windows.Forms.Padding(4);
            this.LoadImageButton.Name = "LoadImageButton";
            this.LoadImageButton.Size = new System.Drawing.Size(131, 28);
            this.LoadImageButton.TabIndex = 0;
            this.LoadImageButton.Text = "Load image...";
            this.LoadImageButton.UseVisualStyleBackColor = true;
            this.LoadImageButton.Click += new System.EventHandler(this.LoadImageButton_Click);
            // 
            // openImageDialog
            // 
            this.openImageDialog.Filter = "Bitmap files (*.bmp;*.gif;*.jpg;*.png;*.tiff;*.jpeg)|*.bmp;*.gif;*.jpg;*.png;*.ti" +
    "ff;*.jpeg";
            this.openImageDialog.InitialDirectory = "..\\..\\images";
            // 
            // imageFileName
            // 
            this.imageFileName.Location = new System.Drawing.Point(155, 17);
            this.imageFileName.Margin = new System.Windows.Forms.Padding(4);
            this.imageFileName.Name = "imageFileName";
            this.imageFileName.ReadOnly = true;
            this.imageFileName.Size = new System.Drawing.Size(420, 22);
            this.imageFileName.TabIndex = 1;
            // 
            // openImageDialog2
            // 
            this.openImageDialog2.Filter = "Bitmap files (*.bmp;*.gif;*.jpg;*.png;*.tiff;*.jpeg)|*.bmp;*.gif;*.jpg;*.png;*.ti" +
    "ff;*.jpeg";
            this.openImageDialog2.InitialDirectory = "..\\..\\images";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(16, 95);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(803, 577);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(637, 15);
            this.applyButton.Margin = new System.Windows.Forms.Padding(4);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(137, 28);
            this.applyButton.TabIndex = 3;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // saveImageDialog
            // 
            this.saveImageDialog.Filter = "Bitmap file (*.bmp)|*.bmp";
            this.saveImageDialog.InitialDirectory = "..\\..\\images";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(1176, 15);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(127, 28);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Save as BMP...";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(827, 93);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(722, 579);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(783, 17);
            this.progressBar.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(368, 25);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 6;
            this.progressBar.Visible = false;
            // 
            // Complement
            // 
            this.Complement.AutoSize = true;
            this.Complement.Location = new System.Drawing.Point(1461, 18);
            this.Complement.Margin = new System.Windows.Forms.Padding(5);
            this.Complement.Name = "Complement";
            this.Complement.Size = new System.Drawing.Size(108, 21);
            this.Complement.TabIndex = 9;
            this.Complement.Text = "Complement";
            this.Complement.UseVisualStyleBackColor = true;
            // 
            // BoundaryTrace
            // 
            this.BoundaryTrace.AutoSize = true;
            this.BoundaryTrace.Location = new System.Drawing.Point(1312, 18);
            this.BoundaryTrace.Margin = new System.Windows.Forms.Padding(5);
            this.BoundaryTrace.Name = "BoundaryTrace";
            this.BoundaryTrace.Size = new System.Drawing.Size(127, 21);
            this.BoundaryTrace.TabIndex = 19;
            this.BoundaryTrace.Text = "Boundary trace";
            this.BoundaryTrace.UseVisualStyleBackColor = true;
            // 
            // BiggestShape
            // 
            this.BiggestShape.AutoSize = true;
            this.BiggestShape.Location = new System.Drawing.Point(1312, 62);
            this.BiggestShape.Margin = new System.Windows.Forms.Padding(5);
            this.BiggestShape.Name = "BiggestShape";
            this.BiggestShape.Size = new System.Drawing.Size(120, 21);
            this.BiggestShape.TabIndex = 20;
            this.BiggestShape.Text = "Biggest shape";
            this.BiggestShape.UseVisualStyleBackColor = true;
            this.BiggestShape.CheckedChanged += new System.EventHandler(this.BiggestShape_CheckedChanged);
            // 
            // FullShapes
            // 
            this.FullShapes.AutoSize = true;
            this.FullShapes.Location = new System.Drawing.Point(1312, 41);
            this.FullShapes.Margin = new System.Windows.Forms.Padding(5);
            this.FullShapes.Name = "FullShapes";
            this.FullShapes.Size = new System.Drawing.Size(102, 21);
            this.FullShapes.TabIndex = 21;
            this.FullShapes.Text = "Full shapes";
            this.FullShapes.UseVisualStyleBackColor = true;
            this.FullShapes.CheckedChanged += new System.EventHandler(this.FullShapes_CheckedChanged);
            // 
            // houghTransformCheckbox
            // 
            this.houghTransformCheckbox.AutoSize = true;
            this.houghTransformCheckbox.Location = new System.Drawing.Point(1461, 41);
            this.houghTransformCheckbox.Margin = new System.Windows.Forms.Padding(5);
            this.houghTransformCheckbox.Name = "houghTransformCheckbox";
            this.houghTransformCheckbox.Size = new System.Drawing.Size(141, 21);
            this.houghTransformCheckbox.TabIndex = 22;
            this.houghTransformCheckbox.Text = "Hough Transform";
            this.houghTransformCheckbox.UseVisualStyleBackColor = true;
            // 
            // INFOIBV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1833, 935);
            this.Controls.Add(this.houghTransformCheckbox);
            this.Controls.Add(this.FullShapes);
            this.Controls.Add(this.BiggestShape);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.BoundaryTrace);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.imageFileName);
            this.Controls.Add(this.LoadImageButton);
            this.Controls.Add(this.Complement);
            this.Location = new System.Drawing.Point(10, 10);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "INFOIBV";
            this.ShowIcon = false;
            this.Text = "INFOIBV";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadImageButton;
        private System.Windows.Forms.OpenFileDialog openImageDialog;
        private System.Windows.Forms.TextBox imageFileName;
        private System.Windows.Forms.OpenFileDialog openImageDialog2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.SaveFileDialog saveImageDialog;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox Complement;
        private CheckBox BoundaryTrace;
        private CheckBox BiggestShape;
        private CheckBox FullShapes;
        private CheckBox houghTransformCheckbox;
    }
}

