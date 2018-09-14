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
    internal partial class MediaAdForm : AdFormBase
    {
        public MediaAdForm()
        {
            InitializeComponent();
        }

        protected override void OnShowAd()
        {
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        protected override void OnHideAd()
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        protected override bool OnInitialize(string dir)
        {
            getMediaFile(dir);
            if (mFilePath == null)
                return false;

            axWindowsMediaPlayer1.Dock = DockStyle.Fill;
            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.URL = mFilePath;
            axWindowsMediaPlayer1.settings.setMode("loop", true); //设置重复播放
            return true;
        }

        private string mFilePath;

        /// <summary>
        /// 搜索媒体文件
        /// </summary>
        /// <param name="dir">搜索目录</param>
        private void getMediaFile(string dir)
        {
            if (Directory.Exists(dir))
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        string ext = Path.GetExtension(d).ToLower();
                        if (ext == ".avi" || ext == ".swf" || ext == ".wmv" || ext == ".asf" || ext == ".wma" || ext == ".mpeg")
                        {
                            mFilePath = d;
                            break;
                        }
                    }
                    else if (mFilePath == null)
                        getMediaFile(d);
                    else
                        break;
                }
            }
        }

        private void axWindowsMediaPlayer1_ClickEvent(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        {
            HideAd();
        }
    }
}