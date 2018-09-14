using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Collections;

namespace Landi.FrameWorks.HardWare
{
    public class Identifier : HardwareBase<Identifier, int>, Landi.FrameWorks.IManagedHardware
    {
        /// <summary>
        /// 打开端口
        /// </summary>
        /// <returns></returns>
        //public static int Open()
        //{
        //    if (!IsUse) return 144;
        //    try
        //    {
        //        int ret = clsICCard.SDT_OpenPort(int.Parse(Port.Substring(3)));
        //        if (ret == 0)
        //        {
        //            AppLog.Write("[Identifier][Open]" + 0.ToString(), AppLog.LogMessageType.Warn);
        //            clsICCard.SDT_ClosePort(int.Parse(Port.Substring(3)));
        //        }
        //        else if (ret != 144)
        //        {
        //            AppLog.Write("[Identifier][Open]" + ret.ToString(), AppLog.LogMessageType.Warn);
        //        }
        //        return ret;
        //    }
        //    catch (System.Exception e)
        //    {
        //        AppLog.Write("[Identifier][InitPort]Error!\n", AppLog.LogMessageType.Error, e);
        //        return -1;
        //    }
        //}

        //public static void Close()
        //{
        //    if (!IsUse) return;
        //    try
        //    {
        //        clsICCard.SDT_ClosePort(int.Parse(Port.Substring(3)));
        //    }
        //    catch (System.Exception ex)
        //    {
        //        AppLog.Write("[Identifier][Close]Error!\n", AppLog.LogMessageType.Error, ex);
        //        return;
        //    }
            
        //}

        /// <summary>
        /// 读身份证
        /// </summary>
        /// <param name="objEDZ"></param>
        /// <param name="existPic"></param>
        /// <param name="picName"></param>
        /// <returns></returns>
        public static bool ReadIdCard(ref clsEDZ objEDZ, ref bool existPic, ref string picName)
        {
            if (!IsUse)
            {
                objEDZ.ADDRESS = "北京";
                objEDZ.BIRTH = Utility.String2Datetime("20000101000000");
                objEDZ.IDC = "350198099889101098";
                objEDZ.Name = "刘备";
                objEDZ.Sex_CName = "男";
                objEDZ.NATION_CName = "中国";
                existPic = false;
                return true;
            }
            try
            {
                clsICCard card = new clsICCard();
                string name = "";
                if (card.ReadICCard(ref name))
                {
                    objEDZ = card.objEDZ;
                    if (File.Exists(name))
                    {
                        existPic = true;
                        picName = name;
                    }
                    else
                    {
                        existPic = false;
                    }
                    return true;
                }
                else
                {
                    AppLog.Write("[Identifier][ReadIdCard]Failed!", AppLog.LogMessageType.Warn);
                    return false;
                }
            }
            catch (System.Exception e)
            {
                AppLog.Write("[Identifier][ReadIdCard]Error!\n", AppLog.LogMessageType.Error, e);
                return false;
            }
        }

        public static int GetStatus()
        {
            if (!IsUse) return 144;
            try
            {
                int iRate = 0;
                int ret = 0;
                ret = clsICCard.SDT_GetCOMBaud(int.Parse(Port.Substring(3)), ref iRate);
                return ret;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[GetStatus] Error!", AppLog.LogMessageType.Error, e);
                return -1;
            }
        }

        #region IManagedHardware
        public object Open()
        {
            if (!IsUse) return 144;
            try
            {
                int ret = clsICCard.SDT_OpenPort(int.Parse(Port.Substring(3)));
                if (ret == 0)
                {
                    AppLog.Write("[Identifier][Open]" + 0.ToString(), AppLog.LogMessageType.Warn);
                    clsICCard.SDT_ClosePort(int.Parse(Port.Substring(3)));
                }
                else if (ret != 144)
                {
                    AppLog.Write("[Identifier][Open]" + ret.ToString(), AppLog.LogMessageType.Warn);
                }
                return ret;
            }
            catch (System.Exception e)
            {
                AppLog.Write("[Identifier][InitPort]Error!\n", AppLog.LogMessageType.Error, e);
                return -1;
            }
        }

        public object Close()
        {
            if (!IsUse) return 0;
            return clsICCard.SDT_ClosePort(int.Parse(Port.Substring(3)));
        }

        public object CheckStatus()
        {
            return GetStatus();
        }

        public bool MeansError(object status)
        {
            switch (AsStatus(status))
            {
                case 144:
                    return false;
                default:
                    return true;
            }
        }
        #endregion
    }


    internal class clsICCard
    {
        //首先，声明通用接口
        [DllImport("sdtapi.dll")]
        public static extern int SDT_OpenPort(int iPortID);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_ClosePort(int iPortID);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_PowerManagerBegin(int iPortID, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_AddSAMUser(int iPortID, string pcUserName, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_SAMLogin(int iPortID, string pcUserName, string pcPasswd, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_SAMLogout(int iPortID, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_UserManagerOK(int iPortID, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_ChangeOwnPwd(int iPortID, string pcOldPasswd, string pcNewPasswd, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_ChangeOtherPwd(int iPortID, string pcUserName, string pcNewPasswd, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_DeleteSAMUser(int iPortID, string pcUserName, int iIfOpen);

        [DllImport("sdtapi.dll")]
        public static extern int SDT_StartFindIDCard(int iPortID, ref int pucIIN, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_SelectIDCard(int iPortID, ref int pucSN, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_ReadBaseMsg(int iPortID, string pucCHMsg, ref int puiCHMsgLen, string pucPHMsg, ref int puiPHMsgLen, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_ReadBaseMsgToFile(int iPortID, string fileName1, ref int puiCHMsgLen, string fileName2, ref int puiPHMsgLen, int iIfOpen);

        [DllImport("sdtapi.dll")]
        public static extern int SDT_WriteAppMsg(int iPortID, ref byte pucSendData, int uiSendLen, ref byte pucRecvData, ref int puiRecvLen, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_WriteAppMsgOK(int iPortID, ref byte pucData, int uiLen, int iIfOpen);

        [DllImport("sdtapi.dll")]
        public static extern int SDT_CancelWriteAppMsg(int iPortID, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_ReadNewAppMsg(int iPortID, ref byte pucAppMsg, ref int puiAppMsgLen, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_ReadAllAppMsg(int iPortID, ref byte pucAppMsg, ref int puiAppMsgLen, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_UsableAppMsg(int iPortID, ref byte ucByte, int iIfOpen);

        [DllImport("sdtapi.dll")]
        public static extern int SDT_GetUnlockMsg(int iPortID, ref byte strMsg, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_GetSAMID(int iPortID, ref byte StrSAMID, int iIfOpen);

        [DllImport("sdtapi.dll")]
        public static extern int SDT_SetMaxRFByte(int iPortID, byte ucByte, int iIfOpen);
        [DllImport("sdtapi.dll")]
        public static extern int SDT_ResetSAM(int iPortID, int iIfOpen);

        [DllImport("WltRS.dll")]
        public static extern int GetBmp(string file_name, int intf);

        [DllImport("sdtapi.dll")]
        public static extern int SDT_GetCOMBaud(int iPortID,ref int iBaudRate);


        public delegate void De_ReadICCardComplete(clsEDZ objEDZ);
        //public event De_ReadICCardComplete ReadICCardComplete;
        public clsEDZ objEDZ = new clsEDZ();
        private int EdziIfOpen = 1;  //自动开关串口
        int EdziPortID;

        public clsICCard()
        {
        }

        public bool ReadICCard(ref string bmpfileName)
        {
            bool bUsbPort = false;
            //int intOpenPortRtn = 0;
            int rtnTemp = 0;
            int pucIIN = 0;
            int pucSN = 0;
            int puiCHMsgLen = 0;
            int puiPHMsgLen = 0;


            objEDZ = new clsEDZ();
            EdziPortID = int.Parse(Identifier.Port.Substring(3));
            ////检测usb口的机具连接，必须先检测usb
            //for (int iPort = 1001; iPort <= 1016; iPort++)
            //{
            //    intOpenPortRtn = SDT_OpenPort(iPort);
            //    if (intOpenPortRtn == 144)
            //    {
            //        EdziPortID = iPort;
            //        bUsbPort = true;
            //        break;
            //    }
            //}
            ////检测串口的机具连接
            //if (!bUsbPort)
            //{
            //    for (int iPort = 1; iPort <= 8; iPort++)
            //    {
            //        intOpenPortRtn = SDT_OpenPort(iPort);
            //        if (intOpenPortRtn == 144)
            //        {
            //            EdziPortID = iPort;
            //            bUsbPort = false;
            //            break;
            //        }
            //    }
            //}
            //if (intOpenPortRtn != 144)
            //{
            //    throw new Exception("端口打开失败，请检测相应的端口或者重新连接读卡器！");
            //    return false;
            //}
            //在这里，如果您想下一次不用再耗费检查端口的检查的过程，您可以把 EdziPortID 保存下来，可以保存在注册表中，也可以保存在配置文件中，我就不多写了，但是，
            //您要考虑机具连接端口被用户改变的情况哦

            //下面找卡
            rtnTemp = SDT_StartFindIDCard(EdziPortID, ref pucIIN, EdziIfOpen);
            if (rtnTemp != 159)
            {
                rtnTemp = SDT_StartFindIDCard(EdziPortID, ref pucIIN, EdziIfOpen);  //再找卡
                if (rtnTemp != 159)
                {
                    //AppLog.Write("未放卡或者卡未放好，请重新放卡!", AppLog.LogMessageType.Error);
                    //rtnTemp = SDT_ClosePort(EdziPortID);
                    //throw new Exception("未放卡或者卡未放好，请重新放卡！");
                    return false;
                }
            }
            //选卡
            rtnTemp = SDT_SelectIDCard(EdziPortID, ref pucSN, EdziIfOpen);
            if (rtnTemp != 144)
            {
                rtnTemp = SDT_SelectIDCard(EdziPortID, ref pucSN, EdziIfOpen);  //再选卡
                if (rtnTemp != 144)
                {
                    AppLog.Write("读卡失败!", AppLog.LogMessageType.Error);
                    //rtnTemp = SDT_ClosePort(EdziPortID);
                    //throw new Exception("读卡失败！");
                    return false;
                }
            }
            //注意，在这里，用户必须有应用程序当前目录的读写权限
            if (!Directory.Exists("IDCardInfo"))
                Directory.CreateDirectory("IDCardInfo");
            string FilePreName = DateTime.Now.ToString("MMddHHmmss");
            string txtName = Path.Combine("IDCardInfo", FilePreName + ".txt");
            string wltName = Path.Combine("IDCardInfo", FilePreName + ".wlt");
            string bmpName = Path.Combine("IDCardInfo", FilePreName + ".bmp");
            bmpfileName = "IDCardInfo/" + FilePreName + ".bmp";
            //FileInfo objFile = new FileInfo("wz.txt");
            //if (objFile.Exists)
            //{
            //    objFile.Attributes = FileAttributes.Normal;
            //    objFile.Delete();
            //}
            //if (objFile.Exists)
            //{
            //    AppLog.Write("Delete \"wz.txt\" Failed!", AppLog.LogMessageType.Warn);
            //    return false;
            //}
            //objFile = new FileInfo("zp.bmp");
            //if (objFile.Exists)
            //{
            //    objFile.Attributes = FileAttributes.Normal;
            //    objFile.Delete();
            //}
            //if (objFile.Exists)
            //{
            //    AppLog.Write("Delete \"zp.bmp\" Failed!", AppLog.LogMessageType.Warn);
            //    return false;
            //}
            //objFile = new FileInfo("zp.wlt");
            //if (objFile.Exists)
            //{
            //    objFile.Attributes = FileAttributes.Normal;
            //    objFile.Delete();
            //}
            //if (objFile.Exists)
            //{
            //    AppLog.Write("Delete \"zp.wlt\" Failed!", AppLog.LogMessageType.Warn);
            //    return false;
            //}
            rtnTemp = SDT_ReadBaseMsgToFile(EdziPortID, txtName, ref puiCHMsgLen, wltName, ref puiPHMsgLen, EdziIfOpen);
            //rtnTemp = SDT_ReadBaseMsgToFile(EdziPortID, "wz.txt", ref puiCHMsgLen, "zp.wlt", ref puiPHMsgLen, EdziIfOpen);
            //rtnTemp = SDT_ReadBaseMsg(EdziPortID, "wz.txt", ref puiCHMsgLen, "zp.wlt", ref puiPHMsgLen, EdziIfOpen);
            if (rtnTemp != 144)
            {
                AppLog.Write("读卡失败!", AppLog.LogMessageType.Error);
                //rtnTemp = SDT_ClosePort(EdziPortID);
                //throw new Exception("读卡失败！");
                return false;
            }
            //下面解析照片，注意，如果在C盘根目录下没有机具厂商的授权文件Termb.Lic，照片解析将会失败
            if (bUsbPort)
                rtnTemp = GetBmp(wltName, 2);
                //rtnTemp = GetBmp("zp.wlt", 2);
            else
                rtnTemp = GetBmp(wltName, 1);
               //rtnTemp = GetBmp("zp.wlt", 1);
            switch (rtnTemp)
            {
                case 0:
                    //MessageBox.Show("调用sdtapi.dll错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppLog.Write("调用sdtapi.dll错误!", AppLog.LogMessageType.Error);
                    return false;
                    //throw new Exception("调用sdtapi.dll错误！");
                    //break;
                case 1:   //正常
                    break;
                case -1:
                    //MessageBox.Show("相片解码错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppLog.Write("相片解码错误!", AppLog.LogMessageType.Error);
                    return false;
                    //throw new Exception("相片解码错误！");
                    //break;
                case -2:
                    //MessageBox.Show("wlt文件后缀错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppLog.Write("wlt文件后缀错误!", AppLog.LogMessageType.Error);
                    return false;
                    //throw new Exception("wlt文件后缀错误！");
                    //break;
                case -3:
                    //MessageBox.Show("wlt文件打开错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppLog.Write("wlt文件打开错误!", AppLog.LogMessageType.Error);
                    return false;
                    //throw new Exception("wlt文件打开错误！");
                    //break;
                case -4:
                    //MessageBox.Show("wlt文件格式错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppLog.Write("wlt文件格式错误!", AppLog.LogMessageType.Error);
                    return false;
                    //throw new Exception("wlt文件格式错误！");
                    //break;
                case -5:
                    //MessageBox.Show("软件未授权！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppLog.Write("软件未授权!", AppLog.LogMessageType.Error);
                    return false;
                    //throw new Exception("软件未授权！");
                    //break;
                case -6:
                    //MessageBox.Show("设备连接错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppLog.Write("设备连接错误!", AppLog.LogMessageType.Error);
                    return false;
                    //throw new Exception("设备连接错误！");
                    //break;
            }
            //rtnTemp = SDT_ClosePort(EdziPortID);
            FileInfo f = new FileInfo(txtName);
            FileStream fs = f.OpenRead();
            byte[] bt = new byte[fs.Length];
            fs.Read(bt, 0, (int)fs.Length);
            fs.Close();

            string str = System.Text.UnicodeEncoding.Unicode.GetString(bt);

            objEDZ.Name = System.Text.UnicodeEncoding.Unicode.GetString(bt, 0, 30).Trim();
            objEDZ.Sex_Code = System.Text.UnicodeEncoding.Unicode.GetString(bt, 30, 2).Trim();
            objEDZ.NATION_Code = System.Text.UnicodeEncoding.Unicode.GetString(bt, 32, 4).Trim();
            string strBird = System.Text.UnicodeEncoding.Unicode.GetString(bt, 36, 16).Trim();
            objEDZ.BIRTH = Convert.ToDateTime(strBird.Substring(0, 4) + "年" + strBird.Substring(4, 2) + "月" + strBird.Substring(6) + "日");
            objEDZ.ADDRESS = System.Text.UnicodeEncoding.Unicode.GetString(bt, 52, 70).Trim();
            objEDZ.IDC = System.Text.UnicodeEncoding.Unicode.GetString(bt, 122, 36).Trim();
            objEDZ.REGORG = System.Text.UnicodeEncoding.Unicode.GetString(bt, 158, 30).Trim();
            string strTem = System.Text.UnicodeEncoding.Unicode.GetString(bt, 188, bt.GetLength(0) - 188).Trim();
            objEDZ.STARTDATE = Convert.ToDateTime(strTem.Substring(0, 4) + "年" + strTem.Substring(4, 2) + "月" + strTem.Substring(6, 2) + "日");
            strTem = strTem.Substring(8);
            if (strTem.Trim() != "长期")
            {
                objEDZ.ENDDATE = Convert.ToDateTime(strTem.Substring(0, 4) + "年" + strTem.Substring(4, 2) + "月" + strTem.Substring(6, 2) + "日");
            }
            else
            {
                objEDZ.ENDDATE = DateTime.MaxValue;
            }
            File.Delete(txtName);
            if (File.Exists(txtName))
            {
                AppLog.Write("删除" + txtName + "文件失败!", AppLog.LogMessageType.Error);
            }

            //FileInfo objFile = new FileInfo(bmpName);
            //if (!objFile.Exists)
            //{
            //    AppLog.Write("照片解析失败，找不到照片文件!", AppLog.LogMessageType.Error);
            //    return false;
            //}
            //if (objFile.Exists)
            //{

            //    FileStream fss = new FileStream("zp.bmp", FileMode.Open);
            //    byte[] imgbyte = new byte[(int)objFile.Length];
            //    fss.Read(imgbyte, 0, (int)objFile.Length);
            //    fss.Close();
            //    MemoryStream ms = new MemoryStream(imgbyte);
            //    Image img = Image.FromStream(ms);
            //    objEDZ.PIC_Image = (Image)img.Clone();
            //    objEDZ.PIC_Byte = imgbyte;
            //    img.Dispose();
            //    ms.Dispose();
            //}

            //ReadICCardComplete(objEDZ);
            return true;
        }
    }


    public class clsEDZ
    {
        private System.Collections.SortedList lstMZ = new SortedList();
        private string _Name;   //姓名
        private string _Sex_Code;   //性别代码
        private string _Sex_CName;   //性别
        private string _IDC;      //身份证号码
        private string _NATION_Code;   //民族代码
        private string _NATION_CName;   //民族
        private DateTime _BIRTH;     //出生日期
        private string _ADDRESS;    //住址
        private string _REGORG;     //签发机关
        private DateTime _STARTDATE;    //身份证有效起始日期
        private DateTime _ENDDATE;    //身份证有效截至日期
        private string _Period_Of_Validity_Code;   //有效期限代码，许多原来系统上面为了一代证考虑，常常存在这样的字段，二代证中已经没有了
        private string _Period_Of_Validity_CName;   //有效期限
        private byte[] _PIC_Byte;    //照片二进制
        private Image _PIC_Image;   //照片

        public clsEDZ()
        {
            lstMZ.Add("01", "汉族");
            lstMZ.Add("02", "蒙古族");
            lstMZ.Add("03", "回族");
            lstMZ.Add("04", "藏族");
            lstMZ.Add("05", "维吾尔族");
            lstMZ.Add("06", "苗族");
            lstMZ.Add("07", "彝族");
            lstMZ.Add("08", "壮族");
            lstMZ.Add("09", "布依族");
            lstMZ.Add("10", "朝鲜族");
            lstMZ.Add("11", "满族");
            lstMZ.Add("12", "侗族");
            lstMZ.Add("13", "瑶族");
            lstMZ.Add("14", "白族");
            lstMZ.Add("15", "土家族");
            lstMZ.Add("16", "哈尼族");
            lstMZ.Add("17", "哈萨克族");
            lstMZ.Add("18", "傣族");
            lstMZ.Add("19", "黎族");
            lstMZ.Add("20", "傈僳族");
            lstMZ.Add("21", "佤族");
            lstMZ.Add("22", "畲族");
            lstMZ.Add("23", "高山族");
            lstMZ.Add("24", "拉祜族");
            lstMZ.Add("25", "水族");
            lstMZ.Add("26", "东乡族");
            lstMZ.Add("27", "纳西族");
            lstMZ.Add("28", "景颇族");
            lstMZ.Add("29", "柯尔克孜族");
            lstMZ.Add("30", "土族");
            lstMZ.Add("31", "达翰尔族");
            lstMZ.Add("32", "仫佬族");
            lstMZ.Add("33", "羌族");
            lstMZ.Add("34", "布朗族");
            lstMZ.Add("35", "撒拉族");
            lstMZ.Add("36", "毛南族");
            lstMZ.Add("37", "仡佬族");
            lstMZ.Add("38", "锡伯族");
            lstMZ.Add("39", "阿昌族");
            lstMZ.Add("40", "普米族");
            lstMZ.Add("41", "塔吉克族");
            lstMZ.Add("42", "怒族");
            lstMZ.Add("43", "乌孜别克族");
            lstMZ.Add("44", "俄罗斯族");
            lstMZ.Add("45", "鄂温克族");
            lstMZ.Add("46", "德昂族");
            lstMZ.Add("47", "保安族");
            lstMZ.Add("48", "裕固族");
            lstMZ.Add("49", "京族");
            lstMZ.Add("50", "塔塔尔族");
            lstMZ.Add("51", "独龙族");
            lstMZ.Add("52", "鄂伦春族");
            lstMZ.Add("53", "赫哲族");
            lstMZ.Add("54", "门巴族");
            lstMZ.Add("55", "珞巴族");
            lstMZ.Add("56", "基诺族");
            lstMZ.Add("57", "其它");
            lstMZ.Add("98", "外国人入籍");
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string Sex_Code
        {
            get { return _Sex_Code; }
            set
            {
                _Sex_Code = value;
                switch (value)
                {
                    case "1":
                        Sex_CName = "男";
                        break;
                    case "2":
                        Sex_CName = "女";
                        break;
                }
            }
        }
        public string Sex_CName
        {
            get { return _Sex_CName; }
            set { _Sex_CName = value; }
        }
        public string IDC
        {
            get { return _IDC; }
            set { _IDC = value; }
        }
        public string NATION_Code
        {
            get { return _NATION_Code; }
            set
            {
                _NATION_Code = value;
                if (lstMZ.Contains(value))
                    NATION_CName = lstMZ[value].ToString();
            }
        }
        public string NATION_CName
        {
            get { return _NATION_CName; }
            set { _NATION_CName = value; }
        }
        public DateTime BIRTH
        {
            get { return _BIRTH; }
            set { _BIRTH = value; }
        }
        public string ADDRESS
        {
            get { return _ADDRESS; }
            set { _ADDRESS = value; }
        }
        public string REGORG
        {
            get { return _REGORG; }
            set { _REGORG = value; }
        }
        public DateTime STARTDATE
        {
            get { return _STARTDATE; }
            set { _STARTDATE = value; }
        }
        public DateTime ENDDATE
        {
            get { return _ENDDATE; }
            set
            {
                _ENDDATE = value;
                if (_ENDDATE == DateTime.MaxValue)
                {
                    _Period_Of_Validity_Code = "3";
                    _Period_Of_Validity_CName = "长期";
                }
                else
                {
                    if (_STARTDATE != DateTime.MinValue)
                    {
                        switch (value.AddDays(1).Year - _STARTDATE.Year)
                        {
                            case 5:
                                _Period_Of_Validity_Code = "4";
                                _Period_Of_Validity_CName = "5年";
                                break;
                            case 10:
                                _Period_Of_Validity_Code = "1";
                                _Period_Of_Validity_CName = "10年";
                                break;
                            case 20:
                                _Period_Of_Validity_Code = "2";
                                _Period_Of_Validity_CName = "20年";
                                break;
                        }
                    }
                }

            }
        }
        public string Period_Of_Validity_Code
        {
            get { return _Period_Of_Validity_Code; }
            set { _Period_Of_Validity_Code = value; }
        }
        public string Period_Of_Validity_CName
        {
            get { return _Period_Of_Validity_CName; }
            set { _Period_Of_Validity_CName = value; }
        }
        public byte[] PIC_Byte
        {
            get { return _PIC_Byte; }
            set { _PIC_Byte = value; }
        }
        public Image PIC_Image
        {
            get { return _PIC_Image; }
            set { _PIC_Image = value; }
        }
    }
}
