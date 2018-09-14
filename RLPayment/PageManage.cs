using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace RLPayment
{
    class PageManage
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int nCurPage = 1;
        /// <summary>
        /// 每页个数
        /// </summary>
        public int nPageCount = 5;
        /// <summary>
        /// 总共页数
        /// </summary>
        public int nTotalPage = 0;

        private ArrayList dataArrayList = new ArrayList();//分页数据

        /// <summary>
        /// 设置分页内容，并返回首页数据
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public ArrayList SetContent(ArrayList _list)
        {
            ArrayList aList = new ArrayList();
            try
            {
                dataArrayList.Clear();
                dataArrayList.AddRange(_list);
                int local1 = dataArrayList.Count;
                if (local1 % nPageCount == 0)
                {
                    nTotalPage = local1 / nPageCount;
                }
                else
                {
                    nTotalPage = local1 / nPageCount + 1;
                }

                aList = GetPageConten(1);
            }
            catch (System.Exception)
            {
                aList = null;
            }

            return aList;
        }


        /// <summary>
        /// 下一页
        /// </summary>
        /// <returns></returns>
        public ArrayList OnNextPage()
        {
            ArrayList aList = new ArrayList();
            try
            {
                if (nCurPage < nTotalPage)
                {
                    nCurPage++;
                    aList = GetPageConten(nCurPage);
                }
                else
                {
                    aList = null;
                }
            }
            catch (System.Exception)
            {
                aList = null;
            }
            return aList;
        }

        /// <summary>
        /// 上一页
        /// </summary>
        /// <returns></returns>
        public ArrayList OnPrePage()
        {
            ArrayList aList = new ArrayList();
            try
            {
                if (nCurPage > 1)
                {
                    nCurPage--;
                    aList = GetPageConten(nCurPage);
                }
                else
                {
                    aList = null;
                }
            }
            catch (System.Exception)
            {
                aList = null;
            }

            return aList;
        }

        /// <summary>
        /// 取某个页的数据
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ArrayList GetPageConten(int nPage)
        {
            ArrayList aList = new ArrayList();

            try
            {
                int count = (nPage - 1) * nPageCount;

                for (int i = count; i < count + nPageCount && i < dataArrayList.Count; i++)
                {
                    aList.Add(dataArrayList[i]);
                }

                if (aList.Count == 0)
                {
                    aList = null;
                }
            }
            catch (System.Exception)
            {
                aList = null;
            }

            return aList;
        }
    }
}
