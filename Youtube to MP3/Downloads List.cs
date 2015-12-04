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
using System.IO;

namespace Youtube_to_MP3
{
    public partial class Downloads_List : Form
    {

        #region Vars
        static WebClient_Identified[] wc;
        static ProgressBar[] pb;
        static Label[] labelsPer;
        static Label[] labelsNames;
        static int padding = 10;
        static Form mainForm;
        public static String path = @"C:\Users\Guy\Desktop\";
        public static Double Minutes=0;
        static ProgressBar MinutesPB;
        static Label labelForCD;
        static Label labelPForCD;
        static ProgressBar progressForCD;
        public static double tempLength;
        #endregion

        public Downloads_List()
        {
            InitializeComponent();
            mainForm = this;
            MinutesPB = this.progressBar1;
            wc = new WebClient_Identified[0];
            pb = new ProgressBar[0];
            labelsPer = new Label[0];
            labelsNames = new Label[0];
            labelForCD = label1;
            labelPForCD = label2;
            progressForCD = progressBar1;
        }

        #region CD Progress Bar Status Methods(reveal,hide,update)
        public static void revealCDProgress() {
            labelForCD.Visible = true;
            labelPForCD.Visible = true;
            progressForCD.Visible = true;
        }
        public static void hideCDProgress() {
            labelForCD.Visible = false;
            labelPForCD.Visible = false;
            progressForCD.Visible = false;
        }
        public static void UpdateProgressBar() {
            MinutesPB.Value = (int)((Double)((Double)(Minutes / 80)) * 100);
            labelPForCD.Text = MinutesPB.Value + "%(" + Minutes + "/80min)";
        }
        #endregion

        #region Song Donload Handlers Methods(Donwload,AddProgress)
        /// <summary>
        /// Adds a progress bar and labels of the song.
        /// </summary>
        /// <param name="name"></param>
        public static void AddProgress(String name) {
            //Adds another place in the arrays
            Array.Resize<ProgressBar>(ref pb, pb.Length + 1);
            Array.Resize<Label>(ref labelsPer, labelsPer.Length + 1);
            Array.Resize<Label>(ref labelsNames, labelsNames.Length + 1);
            //Initializes the items
            pb[pb.Length - 1] = new ProgressBar();
            labelsPer[labelsPer.Length - 1] = new Label();
            labelsNames[labelsNames.Length - 1] = new Label();
            //If its the first Progress item (The position is different)
            if (pb.Length == 1)
            {
                pb[pb.Length - 1].Location = new System.Drawing.Point(12, 50);
                labelsPer[labelsPer.Length - 1].Location = new System.Drawing.Point(20, 80);
                labelsNames[labelsNames.Length - 1].Location = new System.Drawing.Point(220+padding, 80);
            }
            else {
                pb[pb.Length - 1].Location = new System.Drawing.Point(12, pb[pb.Length-2].Location.Y+padding+50);
                labelsPer[labelsPer.Length - 1].Location = new System.Drawing.Point(20, pb[pb.Length - 2].Location.Y + padding + 80);
                labelsNames[labelsNames.Length - 1].Location = new System.Drawing.Point(220 + padding, pb[pb.Length - 2].Location.Y + padding + 80);
            }
            //Set the rest of the information and adds them to the form
            pb[pb.Length-1].Name = "pb"+pb.Length;
            labelsPer[labelsPer.Length - 1].Name = "Lab" + labelsPer.Length;
            labelsNames[labelsNames.Length - 1].Name = "Lab" + labelsPer.Length;
            labelsPer[labelsPer.Length - 1].Text = "0%";
            labelsNames[labelsNames.Length - 1].Text = name;
            labelsNames[labelsNames.Length - 1].AutoSize = true;
            labelsPer[labelsPer.Length - 1].AutoSize = true;
            labelsPer[labelsPer.Length - 1].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            labelsNames[labelsNames.Length - 1].BackColor = System.Drawing.Color.Transparent;
            pb[pb.Length-1].Size = new System.Drawing.Size(200, 50);
            pb[pb.Length-1].TabIndex = 0;
            mainForm.Controls.Add(labelsPer[labelsPer.Length - 1]);
            mainForm.Controls.Add(labelsNames[labelsNames.Length - 1]);
            mainForm.Controls.Add(pb[pb.Length - 1]);
        }

        /// <summary>
        /// A method to download a new Song.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        public static void DownloadFromUrl(string url, string name)
        {
            //Adding another WebClient
            Array.Resize<WebClient_Identified>(ref wc, wc.Length + 1);
            wc[wc.Length - 1] = new WebClient_Identified(wc.Length - 1, "http://youtubeinmp3.com/fetch/?video=" + url, name);
            AddProgress(name);
            wc[wc.Length - 1].length = tempLength;
            wc[wc.Length - 1].DownloadFileCompleted += wc_DownloadFileCompleted;
            wc[wc.Length - 1].DownloadProgressChanged += wc_DownloadProgressChanged;
            wc[wc.Length - 1].DownloadFileAsync(new Uri("http://youtubeinmp3.com/fetch/?video=" + url), path + name + ".mp3");
        }
        public static void DownloadFromUrl(string url, string name,bool isErr)
        {
            //Adding another WebClient
            Array.Resize<WebClient_Identified>(ref wc, wc.Length + 1);
            wc[wc.Length - 1] = new WebClient_Identified(wc.Length - 1, "http://youtubeinmp3.com/fetch/?video=" + url, name);
            AddProgress(name);
            wc[wc.Length - 1].length = tempLength;
            wc[wc.Length - 1].DownloadFileCompleted += wc_DownloadFileCompleted;
            wc[wc.Length - 1].DownloadProgressChanged += wc_DownloadProgressChanged;
            wc[wc.Length - 1].lengthError = 1;
            wc[wc.Length - 1].DownloadFileAsync(new Uri("http://youtubeinmp3.com/fetch/?video=" + url), path + name + ".mp3");
        }
        #endregion

        #region WebClient Events(ProgressChanged,DownloadComplete)
        static void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (wc[((WebClient_Identified)sender).id].bytesToRe == -1) {
                wc[((WebClient_Identified)sender).id].setBytesToRecive(e.TotalBytesToReceive);
            }
            pb[((WebClient_Identified)sender).id].Value = e.ProgressPercentage;
            labelsPer[((WebClient_Identified)sender).id].Text = e.ProgressPercentage.ToString()+"% ("+e.BytesReceived/1000000+"MB/"+e.TotalBytesToReceive/1000000+"MB)";
        }
        static WebClient_Identified temp_FD;
        static void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            temp_FD = wc[((WebClient_Identified)sender).id];
            //Checks For Error On Download
            if (temp_FD.bytesToRe / 1000000 == 0)
            {
                if (!temp_FD.isBurned)
                {
                    temp_FD.setBytesToRecive(-1);
                    temp_FD.tries++;
                    temp_FD.DownloadFileAsync(new Uri(temp_FD.url), path + temp_FD.name + ".mp3");
                }
                else
                {
                    try
                    {
                        File.Delete(path + temp_FD.name + ".mp3");
                    }
                    catch (Exception)
                    {

                    }
                    Downloads_List.Minutes -= temp_FD.length;
                    UpdateProgressBar();
                    labelsPer[((WebClient_Identified)sender).id].Text = "Error.";
                    MessageBox.Show("Song " + temp_FD.name + " failed to dowload. (Are you sure its a song?)");
                    temp_FD.Dispose();
                }
            }
            else {
                labelsPer[((WebClient_Identified)sender).id].Text = "Completed! (" + temp_FD.bytesToRe / 1000000 + "MB/" + temp_FD.bytesToRe / 1000000+"MB)";
                if (temp_FD.lengthError == 1) {
                    double tempSize = Browser.getSizeOfLocal(path + temp_FD.name +".mp3")/60;
                    tempSize = ((double)((int)(tempSize * 1000)) / 1000);
                    Downloads_List.Minutes += tempSize;
                    if (Minutes > 80 && Browser.ForCD)
                    {
                        MessageBox.Show("The song " + temp_FD.name+" exceeds CD Limitation.","Exceed cd limitation",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        Browser.ForCD = false;
                        hideCDProgress();
                    }
                    else if(Browser.ForCD)
                        UpdateProgressBar();
                }
            }
        }
        #endregion
    }
}
