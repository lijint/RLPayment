using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Landi.FrameWorks.HardWare
{
    public class EC_PBOC
    {
        #region ec_pboc.dll

        [DllImport("ec_pboc.dll")]
        protected static extern short EMVL2APPInit(int CardReaderType, long hand);

        [DllImport("ec_pboc.dll")]
        protected static extern short EMVL2StartEmvApp(int aidNo,int cProtocol, int inTransType, byte[] inTrace, byte[] inDay, byte[] inTime, byte[] inAmount, byte[] inOtherAmount);

        [DllImport("ec_pboc.dll")]
        protected static extern short EMVL2TermRiskManageProcessRestrict();

        [DllImport("ec_pboc.dll")]
        protected static extern short EMVL2CardHolderValidate(ref int cTimes);

        [DllImport("ec_pboc.dll")]
        protected static extern short EMVL2ContinueCardHolderValidate(int type, ref int cTimes);

        [DllImport("ec_pboc.dll")]
        protected static extern short EMVL2SelectApp(int cProtocol,byte[] appList, ref int nListNum);

        [DllImport("ec_pboc.dll")]
        protected static extern short EMVActionAnalysis();

        [DllImport("ec_pboc.dll")]
        protected static extern short EMVL2OnlineDataProcess(int retResult, byte[] buf55, int buf55Len, byte[] auth_id, int authLen);


        [DllImport("ec_pboc.dll")]
        protected static extern void EMVL2GetCardNo(byte[] cardNo, ref int lenCard, byte[] cardTrack2, ref int track2Len, byte[] expData, ref int expLen, byte[] cardSeqNum);        

        [DllImport("ec_pboc.dll")]
        protected static extern void EMVSetCardReaderType(int CardReaderType);

        [DllImport("ec_pboc.dll")]
        protected static extern void EMVGetField55(byte[] field55,ref int field55Len);
        

        [DllImport("ec_pboc.dll")]
        protected static extern void EMVL2SetOfflinePinData(byte[] pin,int pinLen);

        [DllImport("ec_pboc.dll")]
        protected static extern void setLoadAmount(byte[] amount);


        [DllImport("ec_pboc.dll")]
        protected static extern void EMVL2GetECashBal(byte[] bal);
        [DllImport("ec_pboc.dll")]
        protected static extern short EMVL2GetECashLog(int aidNo, byte[] line, byte[] row, byte[] log);
        [DllImport("ec_pboc.dll")]
        protected static extern void EMVSetAidAndCAFileName(byte[] fileName, int nameLen);
        #endregion   

        public short App_EMVLInit(int CardReaderType, long hand)
        {
            try
            {
                return EMVL2APPInit(CardReaderType, hand);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void App_EMVSetAidAndCAFileName(byte[] fileName, int nameLen)
        {
            EMVSetAidAndCAFileName(fileName, nameLen);
        }

        public short App_EMVStartEmvApp(int aidNo,int cProtocol, int inTransType, byte[] inTrace, byte[] inDay, byte[] inTime, byte[] inAmount, byte[] inOtherAmount)
        {
            return EMVL2StartEmvApp(aidNo,cProtocol, inTransType, inTrace, inDay, inTime, inAmount, inOtherAmount);
        }

        public short App_EMVL2SelectApp(int cProtocol, byte[] appList, ref int nListNum)
        {
            return EMVL2SelectApp(cProtocol,appList, ref nListNum);
        }

        public short App_EMVOnlineDataProcess(int retResult,byte[] buf55,int buf55Len,byte[] auth_id,int authLen)
        {
            return EMVL2OnlineDataProcess(retResult, buf55, buf55Len, auth_id, authLen);
        }

        public void App_EMVGetECashBal(byte[] bal)
        {
            EMVL2GetECashBal(bal);
        }

        public short App_EMVGetECashLog(int aidNo,byte[] line, byte[] row, byte[] log)
        {
            return EMVL2GetECashLog(aidNo, line, row, log);
        }

        public short App_EMVTermRiskManageProcessRestrict()
        {
            return EMVL2TermRiskManageProcessRestrict();
        }


        public void App_EMVGetCardNo(byte[] cardNo, ref int lenCard, byte[] cardTrack2, ref int track2Len, byte[] expData, ref int expLen, byte[] cardSeqNum)
        {
            EMVL2GetCardNo(cardNo, ref lenCard, cardTrack2, ref track2Len, expData, ref expLen, cardSeqNum);
        }

        public short App_EMVCardHolderValidate(ref int cTimes)
        {
            return EMVL2CardHolderValidate(ref cTimes);
        }

        public short App_EMVContinueCardHolderValidate(int type,ref int cTimes)
        {
            return EMVL2ContinueCardHolderValidate(type, ref cTimes);
        }

        

        public void App_EMVGetField55(byte[] field55, ref int field55Len)
        {
            EMVGetField55(field55, ref field55Len);
        }
        

        public short App_EMVActionAnalysis()
        {
            return EMVActionAnalysis();
        }

        public void App_EMVSetOfflinePinData(byte[] pin,int pinLen)
        {
            EMVL2SetOfflinePinData(pin, pinLen);
        }

        public void setAmount(byte[] amount)
        {
            setLoadAmount(amount);
        }
      
    }
}
