namespace Codenesium.PackageManagementTester
{
    partial class FormTester
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
            this.folderBrowserDialogSelect = new System.Windows.Forms.FolderBrowserDialog();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBoxExtractTempDirectory = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonExtractPackageWithManifest = new System.Windows.Forms.Button();
            this.buttonExtractPackage = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxExtractOutputDirectory = new System.Windows.Forms.TextBox();
            this.textBoxPackageFile = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxPrefix = new System.Windows.Forms.TextBox();
            this.textBoxtTempDirectory = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonCreatePackageWithManifest = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxInputDirectory = new System.Windows.Forms.TextBox();
            this.textBoxMinorVersion = new System.Windows.Forms.TextBox();
            this.textBoxMajorVersion = new System.Windows.Forms.TextBox();
            this.textBoxOutputDirectory = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCreatePackage = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBoxExtractTempDirectory);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.buttonExtractPackageWithManifest);
            this.tabPage2.Controls.Add(this.buttonExtractPackage);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.textBoxExtractOutputDirectory);
            this.tabPage2.Controls.Add(this.textBoxPackageFile);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(618, 329);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Extract Package";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBoxExtractTempDirectory
            // 
            this.textBoxExtractTempDirectory.Location = new System.Drawing.Point(21, 91);
            this.textBoxExtractTempDirectory.Name = "textBoxExtractTempDirectory";
            this.textBoxExtractTempDirectory.Size = new System.Drawing.Size(445, 20);
            this.textBoxExtractTempDirectory.TabIndex = 14;
            this.textBoxExtractTempDirectory.Text = "c:\\tmp\\PackageTemp";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 72);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Temp Directory";
            // 
            // buttonExtractPackageWithManifest
            // 
            this.buttonExtractPackageWithManifest.Location = new System.Drawing.Point(162, 243);
            this.buttonExtractPackageWithManifest.Name = "buttonExtractPackageWithManifest";
            this.buttonExtractPackageWithManifest.Size = new System.Drawing.Size(108, 51);
            this.buttonExtractPackageWithManifest.TabIndex = 10;
            this.buttonExtractPackageWithManifest.Text = "Extract Package with Manifest";
            this.buttonExtractPackageWithManifest.UseVisualStyleBackColor = true;
            this.buttonExtractPackageWithManifest.Click += new System.EventHandler(this.buttonExtractPackageWithManifest_Click);
            // 
            // buttonExtractPackage
            // 
            this.buttonExtractPackage.Location = new System.Drawing.Point(21, 243);
            this.buttonExtractPackage.Name = "buttonExtractPackage";
            this.buttonExtractPackage.Size = new System.Drawing.Size(108, 51);
            this.buttonExtractPackage.TabIndex = 9;
            this.buttonExtractPackage.Text = "Extract Package";
            this.buttonExtractPackage.UseVisualStyleBackColor = true;
            this.buttonExtractPackage.Click += new System.EventHandler(this.buttonExtractPackage_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Output Directory";
            // 
            // textBoxExtractOutputDirectory
            // 
            this.textBoxExtractOutputDirectory.Location = new System.Drawing.Point(21, 142);
            this.textBoxExtractOutputDirectory.Name = "textBoxExtractOutputDirectory";
            this.textBoxExtractOutputDirectory.Size = new System.Drawing.Size(445, 20);
            this.textBoxExtractOutputDirectory.TabIndex = 7;
            this.textBoxExtractOutputDirectory.Text = "c:\\tmp\\PackageExtract";
            // 
            // textBoxPackageFile
            // 
            this.textBoxPackageFile.Location = new System.Drawing.Point(21, 41);
            this.textBoxPackageFile.Name = "textBoxPackageFile";
            this.textBoxPackageFile.Size = new System.Drawing.Size(445, 20);
            this.textBoxPackageFile.TabIndex = 3;
            this.textBoxPackageFile.Text = "C:\\tmp\\PackageOutput\\Package_2016.3.6.929.1515.zip";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Input File";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.textBoxPrefix);
            this.tabPage1.Controls.Add(this.textBoxtTempDirectory);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.buttonCreatePackageWithManifest);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.textBoxInputDirectory);
            this.tabPage1.Controls.Add(this.textBoxMinorVersion);
            this.tabPage1.Controls.Add(this.textBoxMajorVersion);
            this.tabPage1.Controls.Add(this.textBoxOutputDirectory);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.buttonCreatePackage);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(618, 329);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Create Package";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(322, 161);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Prefix";
            // 
            // textBoxPrefix
            // 
            this.textBoxPrefix.Location = new System.Drawing.Point(325, 177);
            this.textBoxPrefix.Name = "textBoxPrefix";
            this.textBoxPrefix.Size = new System.Drawing.Size(138, 20);
            this.textBoxPrefix.TabIndex = 14;
            this.textBoxPrefix.Text = "Package_";
            // 
            // textBoxtTempDirectory
            // 
            this.textBoxtTempDirectory.Location = new System.Drawing.Point(18, 81);
            this.textBoxtTempDirectory.Name = "textBoxtTempDirectory";
            this.textBoxtTempDirectory.Size = new System.Drawing.Size(445, 20);
            this.textBoxtTempDirectory.TabIndex = 12;
            this.textBoxtTempDirectory.Text = "c:\\tmp\\PackageTemp";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 62);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Temp Directory";
            // 
            // buttonCreatePackageWithManifest
            // 
            this.buttonCreatePackageWithManifest.Location = new System.Drawing.Point(161, 265);
            this.buttonCreatePackageWithManifest.Name = "buttonCreatePackageWithManifest";
            this.buttonCreatePackageWithManifest.Size = new System.Drawing.Size(108, 51);
            this.buttonCreatePackageWithManifest.TabIndex = 11;
            this.buttonCreatePackageWithManifest.Text = "Create Package with Manifest";
            this.buttonCreatePackageWithManifest.UseVisualStyleBackColor = true;
            this.buttonCreatePackageWithManifest.Click += new System.EventHandler(this.buttonCreatePackageWithManifest_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(169, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Minor Version";
            // 
            // textBoxInputDirectory
            // 
            this.textBoxInputDirectory.Location = new System.Drawing.Point(18, 30);
            this.textBoxInputDirectory.Name = "textBoxInputDirectory";
            this.textBoxInputDirectory.Size = new System.Drawing.Size(445, 20);
            this.textBoxInputDirectory.TabIndex = 1;
            this.textBoxInputDirectory.Text = "C:\\tmp\\build";
            // 
            // textBoxMinorVersion
            // 
            this.textBoxMinorVersion.Location = new System.Drawing.Point(172, 177);
            this.textBoxMinorVersion.Name = "textBoxMinorVersion";
            this.textBoxMinorVersion.Size = new System.Drawing.Size(138, 20);
            this.textBoxMinorVersion.TabIndex = 9;
            this.textBoxMinorVersion.Text = "6";
            // 
            // textBoxMajorVersion
            // 
            this.textBoxMajorVersion.Location = new System.Drawing.Point(18, 177);
            this.textBoxMajorVersion.Name = "textBoxMajorVersion";
            this.textBoxMajorVersion.Size = new System.Drawing.Size(138, 20);
            this.textBoxMajorVersion.TabIndex = 7;
            this.textBoxMajorVersion.Text = "2016.3";
            // 
            // textBoxOutputDirectory
            // 
            this.textBoxOutputDirectory.Location = new System.Drawing.Point(18, 129);
            this.textBoxOutputDirectory.Name = "textBoxOutputDirectory";
            this.textBoxOutputDirectory.Size = new System.Drawing.Size(445, 20);
            this.textBoxOutputDirectory.TabIndex = 5;
            this.textBoxOutputDirectory.Text = "c:\\tmp\\PackageOutput";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Input Directory";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 161);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Major Version";
            // 
            // buttonCreatePackage
            // 
            this.buttonCreatePackage.Location = new System.Drawing.Point(21, 265);
            this.buttonCreatePackage.Name = "buttonCreatePackage";
            this.buttonCreatePackage.Size = new System.Drawing.Size(108, 51);
            this.buttonCreatePackage.TabIndex = 3;
            this.buttonCreatePackage.Text = "Create Package";
            this.buttonCreatePackage.UseVisualStyleBackColor = true;
            this.buttonCreatePackage.Click += new System.EventHandler(this.buttonCreatePackage_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Output Directory";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(626, 355);
            this.tabControl1.TabIndex = 11;
            // 
            // FormTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 380);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormTester";
            this.Text = "Form1";
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogSelect;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button buttonExtractPackage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxExtractOutputDirectory;
        private System.Windows.Forms.TextBox textBoxPackageFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button buttonCreatePackageWithManifest;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxInputDirectory;
        private System.Windows.Forms.TextBox textBoxMinorVersion;
        private System.Windows.Forms.TextBox textBoxMajorVersion;
        private System.Windows.Forms.TextBox textBoxOutputDirectory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonCreatePackage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TextBox textBoxtTempDirectory;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonExtractPackageWithManifest;
        private System.Windows.Forms.TextBox textBoxExtractTempDirectory;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxPrefix;
    }
}

