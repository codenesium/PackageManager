using Codenesium.PackageManagementLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Codenesium.PackageManagementTester
{
    public partial class FormTester : Form
    {
        public FormTester()
        {
            InitializeComponent();
        }

        private void buttonSelectDirectory_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialogSelect.ShowDialog() == DialogResult.OK)
            {
                textBoxInputDirectory.Text = folderBrowserDialogSelect.SelectedPath;
            }
        }

        private async void buttonCreatePackage_Click(object sender, EventArgs e)
        {
            Packager packageManager = new Packager();
            await packageManager.ZipDirectory(textBoxInputDirectory.Text, textBoxOutputDirectory.Text, Packager.PackageFilenameWithExtension(textBoxPrefix.Text, textBoxMajorVersion.Text, textBoxMinorVersion.Text))
                .ContinueWith(x => MessageBox.Show("Complete!"));
        }

        private void buttonSelectOutputDirectory_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialogSelect.ShowDialog() == DialogResult.OK)
            {
                textBoxOutputDirectory.Text = folderBrowserDialogSelect.SelectedPath;
            }
        }

        private async void buttonExtractPackage_Click(object sender, EventArgs e)
        {
            Packager packageManager = new Packager();
            await packageManager.UnZipDirectory(textBoxPackageFile.Text, textBoxExtractOutputDirectory.Text)
                .ContinueWith(x => MessageBox.Show("Complete!"));
        }

        private async void buttonExtractPackageWithManifest_Click(object sender, EventArgs e)
        {
            ManifestPackager packageManager = new ManifestPackager();
            await packageManager.ExtractPackage(textBoxPackageFile.Text, textBoxExtractOutputDirectory.Text, textBoxExtractTempDirectory.Text)
                   .ContinueWith(x => MessageBox.Show("Complete!"));
        }

        private async void buttonCreatePackageWithManifest_Click(object sender, EventArgs e)
        {
            ManifestPackager packager = new ManifestPackager();
            await packager.CreatePackage(textBoxInputDirectory.Text, textBoxOutputDirectory.Text, textBoxtTempDirectory.Text, Packager.PackageFilenameWithExtension(textBoxPrefix.Text, textBoxMajorVersion.Text, textBoxMinorVersion.Text))
                   .ContinueWith(x => MessageBox.Show("Complete!"));
        }

        private async void buttonExtractLegacyEncryptedPackage_Click(object sender, EventArgs e)
        {
            //Packager packageManager = new Packager();
            //await packageManager.UnZipDirectoryLegacyEncrypted(textBoxPackageFile.Text, textBoxExtractOutputDirectory.Text, textBoxExtractPassword.Text)
            //    .ContinueWith(x => MessageBox.Show("Complete!"));
        }

        private async void buttonCreateLegacyPasswordWithPassword_Click(object sender, EventArgs e)
        {
            //Packager packageManager = new Packager();
            //await packageManager.ZipDirectoryLegacyEncryptedPackage(textBoxInputDirectory.Text, textBoxOutputDirectory.Text, Packager.PackageFilenameWithExtension(textBoxPrefix.Text, textBoxMajorVersion.Text, textBoxMinorVersion.Text), textBoxCreatePackagePassword.Text)
            //    .ContinueWith(x => MessageBox.Show("Complete!"));
        }
    }
}