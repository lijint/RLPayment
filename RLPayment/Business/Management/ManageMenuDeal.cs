using System.Reflection;
using System.Windows.Forms;
using EudemonLink;
using Landi.FrameWorks;

namespace YAPayment.Business.Management
{
    class ManageMenuDeal : Activity
    {
        protected override void OnEnter()
        {
            string ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            GetElementById("CloseSys").Click += CloseSys_Click;
            GetElementById("RestartSys").Click += RestartSys_Click;
            GetElementById("CloseProg").Click += CloseProg_Click;
            GetElementById("ModifyPass").Click += ModifyPass_Click;
            GetElementById("ParamConfig").Click += ParamConfig_Click;
            GetElementById("QuitManage").Click += QuitManage_Click;
            GetElementById("QryTraceNo").Click += QryTraceNo_Click;
        }

        private void CloseSys_Click(object sender, HtmlElementEventArgs e)
        {
            if (MessageBox.Show("确定要关闭机器？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                WindowsController.ExitWindows(RestartOptions.PowerOff, true);
                Sleep(5000);
            }
        }

        private void RestartSys_Click(object sender, HtmlElementEventArgs e)
        {
            if (MessageBox.Show("确定要重启机器？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                WindowsController.ExitWindows(RestartOptions.Reboot, true);
                Sleep(5000);
            }
        }

        private void CloseProg_Click(object sender, HtmlElementEventArgs e)
        {
            if (MessageBox.Show("确定要关闭程序？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (GlobalAppData.GetInstance().EudemonSwitch)
                {
                    EudemonHandler.Instance.CloseEudemon("landi123");
                }
#if !DEBUG
                SetAutoRunCtrlRegInfo(false);
#endif
                Quit();
            }
        }

        private void ModifyPass_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("管理修改密码");
        }

        private void ParamConfig_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主画面显示更改");
        }

        private void QuitManage_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("初始化");
        }

        private void QryTraceNo_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("管理查看流水文件");
        }

        public override bool CanQuit()
        {
            return true;
        }
    }
}
