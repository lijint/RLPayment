using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks
{
    public abstract class SingleTableActivity : MultiTableActivity
    {
        private string mTypeName;

        protected override void OnCreate()
        {
            mTypeName = this.GetType().FullName;
        }

        protected void InitTable(string data, int rowsPerPage, string pageUpButtonName, string pageDownButtonName)
        {
            base.InitTable(mTypeName, data, rowsPerPage, pageUpButtonName, pageDownButtonName, ParseData, ShowData, ClearData, GetData);
        }

        protected abstract List<string[]> ParseData(string data);
        protected abstract void ShowData(List<string[]> datas);
        protected abstract void ClearData();
        protected abstract string GetData();

        protected int CurrentPage
        {
            get { return GetCurrentPage(mTypeName); }
        }

        protected int TotalPage
        {
            get { return GetTotalPage(mTypeName); }
        }

        protected string[] GetContentByRow(int row)
        {
            return base.GetContentByRow(mTypeName, row);
        }
    }
}
