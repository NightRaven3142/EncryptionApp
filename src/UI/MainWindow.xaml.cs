﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Encryption_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<String> DropDownItems = new List<string> { "Choose Option...", "Encrypt a file", "Encrypt a file for sending to someone" };


        public MainWindow()
        {
            InitializeComponent();
            DropDown.ItemsSource = DropDownItems;
            DropDown.SelectedIndex = 0;

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FilePath_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                DecryptFileLocBox.Text = openFileDialog.FileName;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                FileTxtBox.Text = openFileDialog.FileName;
            }
        }

        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            string pwd = InpTxtBox.Text;
            string ofilePath = FileTxtBox.Text;
            Encryptor encryptor = new Encryptor();
            string filePath = encryptor.SymEncrypt(ofilePath, Encoding.UTF8.GetBytes(pwd));
            byte[] encryptedData;
            using (var br = new BinaryReader(File.OpenRead(filePath)))
            {
                encryptedData = br.ReadBytes((int)new FileInfo(filePath).Length);
            }

            if (encryptedData.Length == 0)
            {
                MessageBox.Show("Encryption Failed");
            }
            else
            {
                using (var bw = new BinaryWriter(File.Create(filePath)))
                {
                    bw.Write(encryptedData);
                }
                MessageBox.Show("Successfully Encrypted");
            }
            File.Copy(@"C:\Users\johnk\source\repos\EncryptionApp\src\Backend\tempoutfile.noedit", ofilePath, true);
        }

        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            string pwd = PwdTxtBox.Text;
            string ofilePath = DecryptFileLocBox.Text;
            byte[] data;

            FileInfo f = new FileInfo(ofilePath);

            Encryptor encryptor = new Encryptor();

            string filePath = encryptor.SymDecrypt(ofilePath, Encoding.UTF8.GetBytes(pwd));

            //if (data.Length == 0)
            //{
            //    MessageBox.Show("Decryption Failed");
            //}
            //else
            //{
            //    MessageBox.Show("Successfully Decrypted");
            //}
            File.Copy(@"C:\Users\johnk\source\repos\EncryptionApp\src\Backend\tempoutfile.noedit", ofilePath, true);
        }
    }
}
