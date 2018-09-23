using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Landi.Tools
{
    public static class DelegateHelper
    {
        /// <summary>
        /// 设置控件style
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="strId"></param>
        /// <param name="strStyle"></param>
        public delegate void WBSetIDStyleHandler(WebBrowser wb, string strId, string strStyle);
        public static void WBSetIDStyle(WebBrowser wb, string strId, string strStyle)
        {
            try
            {
                if (wb.InvokeRequired)
                {

                    WBSetIDStyleHandler openvideohandler = new WBSetIDStyleHandler(WBSetIDStyle);
                    wb.Invoke(openvideohandler, wb, strId, strStyle);
                }
                else
                {
                    wb.Document.GetElementById(strId).Style = strStyle;
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        /// <summary>
        /// 设置控件内容
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="strId"></param>
        /// <param name="strInnerText"></param>
        public delegate void WBSetInnerTextHandler(WebBrowser wb, string strId, string strInnerText);
        public static void WBSetInnerText(WebBrowser wb, string strId, string strInnerText)
        {
            try
            {
                if (wb.InvokeRequired)
                {

                    WBSetInnerTextHandler openvideohandler = new WBSetInnerTextHandler(WBSetInnerText);
                    wb.Invoke(openvideohandler, wb, strId, strInnerText);
                }
                else
                {
                    wb.Document.GetElementById(strId).InnerText = strInnerText;
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        /// <summary>
        /// 获取控件内容
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="strId"></param>
        /// <param name="strInnerText"></param>
        public delegate string WBGetInterTextHandler(WebBrowser wb, string strId);
        public static string WBGetInterText(WebBrowser wb, string strId)
        {
            string strReturn = "";
            try
            {
                if (wb.InvokeRequired)
                {

                    WBGetInterTextHandler openvideohandler = new WBGetInterTextHandler(WBGetInterText);
                    strReturn = (wb.Invoke(openvideohandler, wb, strId)).ToString();
                }
                else
                {
                    strReturn = wb.Document.GetElementById(strId).InnerText;
                }
            }
            catch (System.Exception ex)
            {

            }
            return strReturn;
        }




        /// <summary>
        /// 设置控件内容
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="strId"></param>
        /// <param name="strInnerText"></param>
        public delegate void WBSetInnerHtmlHandler(WebBrowser wb, string strId, string strInnerText);
        public static void WBSetInnerHtml(WebBrowser wb, string strId, string strInnerText)
        {
            try
            {
                if (wb.InvokeRequired)
                {

                    WBSetInnerHtmlHandler openvideohandler = new WBSetInnerHtmlHandler(WBSetInnerHtml);
                    wb.Invoke(openvideohandler, wb, strId, strInnerText);
                }
                else
                {
                    wb.Document.GetElementById(strId).InnerHtml = strInnerText;
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        /// <summary>
        /// 获取控件内容
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="strId"></param>
        /// <param name="strInnerText"></param>
        public delegate string WBGetInterHtmlHandler(WebBrowser wb, string strId);
        public static string WBGetInterHtml(WebBrowser wb, string strId)
        {
            string strReturn = "";
            try
            {
                if (wb.InvokeRequired)
                {

                    WBGetInterHtmlHandler openvideohandler = new WBGetInterHtmlHandler(WBGetInterHtml);
                    strReturn = wb.Invoke(openvideohandler, wb, strId).ToString();
                }
                else
                {
                    strReturn = wb.Document.GetElementById(strId).InnerHtml;
                }
            }
            catch (System.Exception ex)
            {

            }
            return strReturn;
        }

        /// <summary>
        /// 获取控件内容
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="strId"></param>
        /// <param name="strInnerText"></param>
        public delegate string WBGetValueHandler(WebBrowser wb, string strId);
        public static string WBGetValueText(WebBrowser wb, string strId)
        {
            string strReturn = "";
            try
            {
                if (wb.InvokeRequired)
                {

                    WBGetValueHandler openvideohandler = new WBGetValueHandler(WBGetValueText);
                    strReturn = (wb.Invoke(openvideohandler, wb, strId)).ToString();
                }
                else
                {
                    strReturn = wb.Document.GetElementById(strId).GetAttribute("value");
                }
            }
            catch (System.Exception ex)
            {

            }
            return strReturn;
        }


        /// <summary>
        /// 获取控件内容
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="strId"></param>
        /// <param name="strInnerText"></param>
        public delegate void WBSetValueHandler(WebBrowser wb, string strId, string strText);
        public static void WBSetValueText(WebBrowser wb, string strId, string strText)
        {
            try
            {
                if (wb.InvokeRequired)
                {

                    WBSetValueHandler openvideohandler = new WBSetValueHandler(WBSetValueText);
                    wb.Invoke(openvideohandler, wb, strId, strText);
                }
                else
                {
                    wb.Document.GetElementById(strId).SetAttribute("value", strText);
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        public delegate HtmlElement WBGetElementByIdHandler(WebBrowser wb, string id);
        public static HtmlElement WBGetElementById(WebBrowser wb, string id)
        {
            HtmlElement returnHtmlElement = null;
            try
            {
                if (wb.InvokeRequired)
                {
                    WBGetElementByIdHandler getElementbById = new WBGetElementByIdHandler(WBGetElementById);
                    wb.Invoke(getElementbById, wb, id);
                }
                else
                {
                    returnHtmlElement = wb.Document.GetElementById(id);
                }
            }
            catch (Exception ex)
            {

            }
            return returnHtmlElement;
        }

    }
}
