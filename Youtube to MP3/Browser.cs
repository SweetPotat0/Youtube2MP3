using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Microsoft.WindowsAPICodePack.Shell;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Xml;
using System.Web.Script.Serialization;



namespace Youtube_to_MP3
{
    using Microsoft.WindowsAPICodePack.Shell;
    public partial class Browser : Form
    {

        public static Boolean ForCD = false;
        Downloads_List dl;
        byte closeIT = 0;
        byte isClosed = 0;
        public Browser()
        {
            InitializeComponent();
            Timer tm = new Timer();
            tm.Interval = 100;
            tm.Tick += tm_Tick;
            tm.Start();
            webBrowser1.DocumentTitleChanged += webBrowser1_DocumentTitleChanged;
            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
            this.SizeChanged += Browser_SizeChanged;
        }

        void tm_Tick(object sender, EventArgs e)
        {
            if (webBrowser1.IsBusy)
                pictureBox5.Visible = true;
            else
                pictureBox5.Visible = false;
        }

        void Browser_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Maximized;
        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                if (webBrowser1.Document.Domain.Equals("www.youtube.com"))
                    linkLabel1.Visible = true;
            }
            catch (Exception)
            {

            }
        }

        void webBrowser1_DocumentTitleChanged(object sender, EventArgs e)
        {
            textBox1.Text = webBrowser1.Document.Url.ToString();
            try
            {
                if (webBrowser1.Document.Domain.Equals("www.youtube.com"))
                {
                    pictureBox4.Enabled = true;
                }
                else
                {
                    pictureBox4.Enabled = false;
                    linkLabel1.Visible = false;
                }
            }
            catch (Exception)
            {
                pictureBox4.Enabled = false;
                linkLabel1.Visible = false;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    webBrowser1.Url = new System.Uri(textBox1.Text);
                }
                catch (Exception)
                {
                    try
                    {
                        webBrowser1.Url = new System.Uri("http://" + textBox1.Text);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Something is wrong with your url");
                    }
                }
            }
        }
        private void loadLocalFiles() {
            /** To do:
             * Go on loop over all of the existing files.
             * Check for each if they are .mp3 files and then read them.
             * Check if the limitation has not been exceeded.
             * Add to the progress bar the time.
             */
            string[] files = Directory.GetFiles(Downloads_List.path);
            double sum = 0.0;
            for (int i = 0; i < files.Length; i++) {
                if (files[i].Substring(files[i].Length - 3).Equals("mp3")) {
                   sum += (getSizeOfLocal(files[i]) / 60);
                }
            }
            sum = ((double)((int)(sum * 1000)) / 1000);
            Downloads_List.Minutes += sum;
            if (Downloads_List.Minutes > 80)
            {
                MessageBox.Show("The existing songs exceed the CD Limitation(80 min).", "Exceed CD Limitation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ForCD = false;
                Downloads_List.hideCDProgress();
            }
            else
                Downloads_List.UpdateProgressBar();
        }
        public static double getSizeOfLocal(String path) {
            ShellFile so = ShellFile.FromFilePath(path);
            double nanoseconds;
            try
            {
                double.TryParse(so.Properties.System.Media.Duration.Value.ToString(),
                out nanoseconds);
                return (int)(nanoseconds * 0.0000001);
            }
            catch (Exception) {
                return 0.0;
            }
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            try
            {
                webBrowser1.Url = new System.Uri(textBox1.Text);
            }
            catch (Exception)
            {
                try
                {
                    webBrowser1.Url = new System.Uri("http://" + textBox1.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Something is wrong with your url");
                }
            }
        }
        #region Browser_Back&Forth
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }
        #endregion

        #region styles
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox2.BorderStyle = BorderStyle.Fixed3D;
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox2.BorderStyle = BorderStyle.None;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox1.BorderStyle = BorderStyle.None;
        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox3.BorderStyle = BorderStyle.Fixed3D;
        }

        private void pictureBox3_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox3.BorderStyle = BorderStyle.None;
        }

        private void pictureBox4_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox4.BorderStyle = BorderStyle.Fixed3D;
        }

        private void pictureBox4_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox4.BorderStyle = BorderStyle.None;
        }
        #endregion

        private void Browser_Load(object sender, EventArgs e)
        {
            dl = new Downloads_List();
            dl.Show();
            dl.FormClosing += dl_FormClosing;
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.Description = "Pick A Destination Folder:";
            if (fb.ShowDialog() == DialogResult.OK)
            {
                Downloads_List.path = fb.SelectedPath + @"\";
                if (MessageBox.Show("Is this for a CD? (Should I notify when limitation of disc has reached?)", "Just a simple question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                {
                    ForCD = true;
                    Downloads_List.revealCDProgress();
                    if (Directory.GetFiles(Downloads_List.path).Length != 0)
                    {
                        if (MessageBox.Show("There are existing files in the chosen directory. Should I load them?", "Existing files in the chosen directory", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            loadLocalFiles();
                        }
                    }
                }
            }
            else
            {
                closeIT = 1;
                Application.Exit();
            }
        }

        void dl_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (closeIT == 0)
            {
                e.Cancel = true;
                dl.Hide();
                isClosed = 1;
            }
        }

        double tempMinutes = 0.0;
        Song tempSong;
        byte flagForCDLengthCF = 0;
        //When Client Presses Donwload : =>
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (!webBrowser1.Document.Url.Equals("www.youtube.com"))
        {
                if (ForCD)
                {
                    HttpWebRequest http = (HttpWebRequest)WebRequest.Create("http://youtubeinmp3.com/fetch/?format=JSON&video=" + webBrowser1.Url);
                    http.Method = "GET";
                    http.ReadWriteTimeout=5000;
                    http.Timeout = 5000;
                    try
                    {
                        WebResponse wr = http.GetResponse();
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Song));
                        tempSong = (Song)serializer.ReadObject(wr.GetResponseStream());
                        wr.Close();
                        tempMinutes = (Double)tempSong.length / 60;
                        tempMinutes = ((double)((int)(tempMinutes * 1000)) / 1000);
                        if (80 - Downloads_List.Minutes >= tempMinutes)
                        {
                            try
                            {
                                PickName.name = webBrowser1.Document.Title.Substring(0, webBrowser1.Document.Title.Length - 9);
                            }
                            catch (Exception)
                            {
                                PickName.name = webBrowser1.Document.Title;
                            }
                            if (new PickName().ShowDialog() == DialogResult.OK)
                            {
                                //Waiting for user to pick a name
                                //Download the File
                                Downloads_List.Minutes += tempMinutes;
                                Downloads_List.tempLength = tempMinutes;
                                Downloads_List.UpdateProgressBar();
                                Downloads_List.DownloadFromUrl(webBrowser1.Document.Url.ToString(), PickName.name);
                            }
                        }
                        else if (MessageBox.Show("This song exceeds the CD limitation. Should I continue?", "Reached CD Limitation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                        {

                            try
                            {
                                PickName.name = webBrowser1.Document.Title.Substring(0, webBrowser1.Document.Title.Length - 9);
                            }
                            catch (Exception)
                            {
                                PickName.name = webBrowser1.Document.Title;
                            }
                            if (new PickName().ShowDialog() == DialogResult.OK)
                            {
                                //Waiting for user to pick a name
                                //Download the File
                                ForCD = false;
                                Downloads_List.hideCDProgress();
                                Downloads_List.DownloadFromUrl(webBrowser1.Document.Url.ToString(), PickName.name);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //cant find the length of the video through URL
                        try
                        {
                            PickName.name = webBrowser1.Document.Title.Substring(0, webBrowser1.Document.Title.Length - 9);
                        }
                        catch (Exception)
                        {
                            PickName.name = webBrowser1.Document.Title;
                        }
                        if (new PickName().ShowDialog() == DialogResult.OK)
                        {
                            //Waiting for user to pick a name
                            //Download the File
                            Downloads_List.DownloadFromUrl(webBrowser1.Document.Url.ToString(), PickName.name, true);
                        }
                    }
                }
                else
                {
                    try
                    {
                        PickName.name = webBrowser1.Document.Title.Substring(0, webBrowser1.Document.Title.Length - 9);
                    }
                    catch (Exception)
                    {
                        PickName.name = webBrowser1.Document.Title;
                    }
                    if (new PickName().ShowDialog() == DialogResult.OK)
                    {
                        //Waiting for user to pick a name
                        //Download the File
                        Downloads_List.DownloadFromUrl(webBrowser1.Document.Url.ToString(), PickName.name);
                    }
                }
            }
            
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (isClosed == 1)
            {
                dl.Show();
                isClosed = 0;
            }
            else if (dl.WindowState == FormWindowState.Minimized)
            {
                dl.WindowState = FormWindowState.Normal;
            }
            else
            {
                dl.BringToFront();
            }
        }
    }
    public class Song
    {
        public String title { get; set; }
        public int length { get; set; }
        public string link { get; set; }
    }
}
