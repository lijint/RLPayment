using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Landi.FrameWorks
{
    internal partial class AdFormBase : Form
    {
        protected AdFormBase()
        {
            InitializeComponent();
        }

        protected virtual bool OnInitialize(string dir) { return false; }
        protected virtual void OnShowAd() { }
        protected virtual void OnHideAd() { }

        internal bool Initialize(string dir, int left, int top, int width, int height, bool topMost)
        {
            Location = new Point(left, top);
            Size = new Size(width, height);
            TopMost = topMost;
            return OnInitialize(dir);
        }

        internal void ShowAd()
        {
            OnShowAd();
            Visible = true;
            AdManager.NotifyEnterAd();
        }

        internal void HideAd()
        {
            OnHideAd();
            Visible = false;
            AdManager.NotifyLeaveAd();
        }
    }
}