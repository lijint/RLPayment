using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Landi.FrameWorks
{
    public abstract class MultiTableActivity : Activity
    {
        private Dictionary<string, TableInfo> mTableInfos;
        private Dictionary<string, TableInfo> mMaps;
        private class TableInfo
        {
            public string mPageUpButton;
            public string mPageDownButton;
            public List<string[]> mAllContent;
            public List<string[]> mCurrentContent;
            public int mCurrentPage;
            public int mTotalPage;
            public int mTotalCount;
            public int mRowsPerPage = 5;
            public bool mPendingShow;

            public ParseDataHandler ParseData;
            public ShowDataHandler ShowData;
            public ClearDataHandler ClearData;
            public GetDataHandler GetData;
        }

        protected int GetCurrentPage(string tableName)
        {
            return getTableInfoByName(tableName).mCurrentPage;
        }

        protected int GetTotalPage(string tableName)
        {
            return getTableInfoByName(tableName).mTotalPage;
        }

        public MultiTableActivity()
        {
            mTableInfos = new Dictionary<string, TableInfo>();
            mMaps = new Dictionary<string, TableInfo>();
        }

        protected delegate List<string[]> ParseDataHandler(string data);
        protected delegate void ShowDataHandler(List<string[]> datas);
        protected delegate void ClearDataHandler();
        protected delegate string GetDataHandler();

        private string defaultGetData()
        {
            return null;
        }

        protected void InitTable(string tableName, string data, int rowsPerPage, string pageUpButtonName,
            string pageDownButtonName, ParseDataHandler parseData, ShowDataHandler showData, ClearDataHandler clearData, GetDataHandler getData)
        {
            lock (this)
            {
                if (!mTableInfos.ContainsKey(tableName))
                {
                    if (parseData == null)
                        throw new Exception("解析函数不能为空");
                    else if (showData == null)
                        throw new Exception("显示函数不能为空");
                    else if (clearData == null)
                        throw new Exception("清除函数不能为空");

                    if (!string.IsNullOrEmpty(data))
                    {
                        TableInfo info = new TableInfo();
                        if (rowsPerPage > 0)
                            info.mRowsPerPage = rowsPerPage;
                        info.mCurrentPage = 1;
                        info.mTotalPage = (info.mTotalCount + info.mRowsPerPage - 1) / info.mRowsPerPage;
                        info.mCurrentContent = new List<string[]>();
                        info.mPageUpButton = pageUpButtonName;
                        info.mPageDownButton = pageDownButtonName;
                        info.ParseData = parseData;
                        info.ShowData = showData;
                        info.ClearData = clearData;
                        info.mAllContent = new List<string[]>();
                        List<string[]> dataList = info.ParseData(data);
                        if (dataList != null)
                        {
                            for (int i = 0; i < dataList.Count; i++)
                                info.mAllContent.Add(dataList[i]);
                        }
                        info.mTotalCount = info.mAllContent.Count;
                        info.GetData = defaultGetData;
                        if (getData != null)
                            info.GetData = getData;
                        mTableInfos.Add(tableName, info);
                        if (!string.IsNullOrEmpty(info.mPageUpButton) && !mMaps.ContainsKey(info.mPageUpButton))
                            mMaps.Add(info.mPageUpButton, info);
                        if (!string.IsNullOrEmpty(info.mPageDownButton) && !mMaps.ContainsKey(info.mPageDownButton))
                            mMaps.Add(info.mPageDownButton, info);
                        updateContent(info);
                    }
                }
            }
        }

        private void show()
        {
            foreach (KeyValuePair<string, TableInfo> info in mTableInfos)
            {
                if (info.Value.mPendingShow && info.Value.mCurrentContent.Count > 0 && info.Value.ShowData != null)
                {
                    info.Value.mPendingShow = false;
                    info.Value.ShowData(info.Value.mCurrentContent);
                }
            }
        }

        private TableInfo getTableInfoByName(string tableName)
        {
            return mTableInfos[tableName];
        }

        private void detachClick()
        {
            foreach (KeyValuePair<string, TableInfo> info in mTableInfos)
            {
                GetElementById(info.Value.mPageUpButton).Click -= new System.Windows.Forms.HtmlElementEventHandler(PageUp_Click);
                GetElementById(info.Value.mPageDownButton).Click -= new System.Windows.Forms.HtmlElementEventHandler(PageDown_Click);
            }
        }

        private void attachClick()
        {
            foreach (KeyValuePair<string, TableInfo> info in mTableInfos)
            {
                GetElementById(info.Value.mPageUpButton).Click += new System.Windows.Forms.HtmlElementEventHandler(PageUp_Click);
                GetElementById(info.Value.mPageDownButton).Click += new System.Windows.Forms.HtmlElementEventHandler(PageDown_Click);
            }
        }

        protected abstract void Prepare();

        protected sealed override void OnEnter()
        {
            Prepare();
            lock (this)
            {
                show();
                attachClick();
            }
        }

        protected string[] GetContentByRow(string tableName, int row)
        {
            lock (this)
            {
                TableInfo info = getTableInfoByName(tableName);
                if (info != null && row < info.mCurrentContent.Count)
                    return info.mCurrentContent[row];
                return null;
            }
        }

        private bool updateContent(TableInfo info)
        {
            if (info.mAllContent == null)
                return false;
            List<string[]> extr = null;
            int start = info.mRowsPerPage * (info.mCurrentPage - 1);
            int count = info.mRowsPerPage;
            bool ret = false;
            if (start + count > info.mAllContent.Count)
            {
                try
                {
                    if (info.GetData != null)
                    {
                        string data = info.GetData();
                        if (!string.IsNullOrEmpty(data))
                            extr = info.ParseData(data);
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error(this, ex);
                }
                if (extr != null && extr.Count > 0)
                {
                    for (int i = 0; i < extr.Count; i++)
                        info.mAllContent.Add(extr[i]);
                    info.mTotalCount = info.mAllContent.Count;
                    info.mTotalPage = (info.mTotalCount + info.mRowsPerPage - 1) / info.mRowsPerPage;
                    ret = true;
                }
            }
            if (start + count > info.mAllContent.Count)
                count = info.mAllContent.Count - start;
            if (count > 0)
            {
                info.mCurrentContent.Clear();
                for (int i = start; i < start + count; i++)
                    info.mCurrentContent.Add(info.mAllContent[i]);
                info.mPendingShow = true;
            }
            else if (info.mCurrentPage > 1)
            {
                info.mCurrentPage--;
            }
            return ret;
        }

        private void PageUp_Click(object sender, HtmlElementEventArgs e)
        {
            lock (this)
            {
                detachClick();
                TableInfo info = getTableInfoById(((HtmlElement)sender).Id);
                if (info.mCurrentPage > 1)
                {
                    info.mCurrentPage--;
                    updateContent(info);
                    if (info.ClearData != null)
                        info.ClearData();
                    show();
                }
                attachClick();
            }
        }

        private TableInfo getTableInfoById(string id)
        {
            return mMaps[id];
        }

        private void PageDown_Click(object sender, HtmlElementEventArgs e)
        {
            lock (this)
            {
                detachClick();
                TableInfo info = getTableInfoById(((HtmlElement)sender).Id);
                bool remainCurrentPage = false;
                if (info.mCurrentPage == info.mTotalPage && info.mCurrentContent.Count < info.mRowsPerPage)
                {
                    remainCurrentPage = true;
                }
                if (!remainCurrentPage)
                {
                    info.mCurrentPage++;
                }
                updateContent(info);
                if (info.ClearData != null)
                    info.ClearData();
                show();
                attachClick();
            }
        }

        protected override void OnLeave()
        {
            foreach (KeyValuePair<string, TableInfo> info in mTableInfos)
            {
                info.Value.mPendingShow = true;
            }
        }
    }
}
