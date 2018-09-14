using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Landi.FrameWorks
{
    public class FrameActivity : Activity
    {
        private static string returnName;
        private static string comName;

        protected override void OnEnter()
        {
            try
            {
                if (GetElementById(returnName) != null)
                {
                    GetElementById(returnName).Click += new HtmlElementEventHandler(ReturnClick);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ReturnClick(object sender, HtmlElementEventArgs e)
        {
            FrameReturnClick();
        }

        protected virtual void FrameReturnClick()
        {
        }

        protected void setRetName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            returnName = name;
        }

        protected void IsConDisplay(bool isDisplay)
        {
            GetElementById(comName).Style = isDisplay ? "visibility: visible;" : "visibility: hidden;";
        }

        protected void setComName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            comName = name;
        }
    }
}
