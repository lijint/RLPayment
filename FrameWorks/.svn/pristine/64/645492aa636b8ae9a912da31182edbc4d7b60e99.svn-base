using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Landi.FrameWorks
{
    internal partial class PictureAdForm : AdFormBase
    {
        public PictureAdForm()
        {
            InitializeComponent();
        }

        private System.Windows.Forms.Timer mTimer;
        private int currpicture = 0;

        /// <summary>
        /// 广告图片计时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmr_Tick(object sender, EventArgs e)
        {
            currpicture = (currpicture + 1) % mPictures.Count;
            pictureBox1.Load(mPictures[currpicture]);
        }

        private List<string> mPictures = new List<string>();

        /// <summary>
        /// 读取所有图片
        /// </summary>
        /// <param name="dir"></param>
        private void getAllPicture(string dir)
        {
            if (Directory.Exists(dir))
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        string ext = Path.GetExtension(d).ToLower();
                        if (ext == ".jpg" || ext == ".bmp")
                        {
                            mPictures.Add(d);
                        }
                    }
                    else
                        getAllPicture(d);
                }
            }
        }

        protected override bool OnInitialize(string dir)
        {
            mTimer = new Timer();
            int interval = 0;
            if (!int.TryParse(ConfigFile.ReadConfigAndCreate("AdConfig", "SwitchInterval"), out interval))
                interval = 5;
            mTimer.Interval = interval * 1000;
            mTimer.Tick += new EventHandler(tmr_Tick);
            mPictures.Clear();
            getAllPicture(dir);
            if (mPictures.Count == 0)
                return false;
            else
            {
                pictureBox1.Load(mPictures[currpicture]);
                mTimer.Start();
                return true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            HideAd();
        }
    }
}