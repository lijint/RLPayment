using System; 
using System.Collections.Generic; 
using System.Text; 
using System.IO; 

namespace Landi.FrameWorks
{ 
    /// <summary> 
    /// 文件夹复制 
    /// zgke@Sina.com 
    /// </summary> 
    public class CopyDirectory 
    { 

        /* 使用方法 
        private void button1_Click(object sender, EventArgs e) 
        { 
            Zgke.Copy.CopyDirectory _Info = new Zgke.Copy.CopyDirectory(@"F:/项目文件/产品化/联络中心V1.4.0/Source", @"E:/Temp"); 
            _Info.MyCopyRun += new Zgke.Copy.CopyDirectory.CopyRun(_Info_MyCopyRun); 
            _Info.MyCopyEnd += new Zgke.Copy.CopyDirectory.CopyEnd(_Info_MyCopyEnd); 
            _Info.StarCopy(); 
        } 

        void _Info_MyCopyEnd() 
        { 
            MessageBox.Show("复制完成"); 
        } 
        void _Info_MyCopyRun(int FileCount, int CopyCount, long FileSize, long CopySize, string FileName) 
        { 
            this.Invoke((MethodInvoker)delegate { 
                progressBar1.Maximum = FileCount; 
                progressBar1.Value = CopyCount; 
                label1.Text = CopySize.ToString() + "/" + FileSize.ToString(); 
                label2.Text = FileName; 
            });             
        } 
        */ 

        /// <summary> 
        /// 源目录 
        /// </summary> 
        private DirectoryInfo _Source;      
        /// <summary> 
        /// 目标目录 
        /// </summary> 
        private DirectoryInfo _Target; 
    
        /// <summary> 
        /// 文件复制完成 
        /// </summary> 
        /// <param name="FileCount">文件数量合计</param> 
        /// <param name="CopyCount">复制完成的数量</param> 
        /// <param name="FileSize">文件大小合计</param> 
        /// <param name="CopySize">复制完成的大小</param> 
        /// <param name="FileName">复制完成的文件名</param> 
        public delegate void CopyRun(int FileCount,int CopyCount,long FileSize,long CopySize,string FileName); 
        public event CopyRun MyCopyRun; 
        /// <summary> 
        /// 复制完成 
        /// </summary> 
        public delegate void CopyEnd(); 
        public event CopyEnd MyCopyEnd; 

        private int _FileCount = 0; 
        private int _CopyCount = 0; 
        private long _FileSize = 0; 
        private long _CopySize = 0; 
           
        /// <summary> 
        /// 复制目录包含文件 
        /// </summary> 
        /// <param name="p_SourceDirectory">源目录</param> 
        /// <param name="p_TargetDirectory">目标目录</param> 
        public CopyDirectory(string p_SourceDirectory,string p_TargetDirectory) 
        { 
            _Source = new DirectoryInfo(p_SourceDirectory); 
            _Target = new DirectoryInfo(p_TargetDirectory); 
            FileSystemInfo[] Temp = _Source.GetFileSystemInfos();            
        } 
       

        /// <summary> 
        /// 开始复制 
        /// </summary> 
        public void StarCopy() 
        { 
            GetFile(_Source); 
            System.Threading.Thread Th = new System.Threading.Thread(new System.Threading.ThreadStart(Run)); 
            Th.Start(); 
        } 

        private void Run() 
        {            
            Copy(_Source,_Target ); 
            if (MyCopyEnd != null) MyCopyEnd(); 
        } 

        /// <summary> 
        /// 复制目录到指定目录 
        /// </summary> 
        /// <param name="source">源目录</param> 
        /// <param name="target">目标目录</param> 
        private void GetFile(DirectoryInfo MySiurceDirectory) 
        { 
            foreach (FileInfo _File in MySiurceDirectory.GetFiles())    //循环文件    
            {                
                _FileCount++; 
                _FileSize += _File.Length; 
            } 


            foreach (DirectoryInfo _SourceSubDir in MySiurceDirectory.GetDirectories())  //循环子目录 
            { 
                GetFile(_SourceSubDir); 
            } 
        } 
        

        /// <summary> 
        /// 复制目录到指定目录 
        /// </summary> 
        /// <param name="source">源目录</param> 
        /// <param name="target">目标目录</param> 
        private void Copy(DirectoryInfo p_Source, DirectoryInfo p_Target) 
        {           
            if (!Directory.Exists(p_Target.FullName))Directory.CreateDirectory(p_Target.FullName);                  

            foreach (FileInfo _File in p_Source.GetFiles())       //循环文件  
            {                
                _File.CopyTo(Path.Combine(p_Target.ToString(), _File.Name), true);                
                _CopyCount++; 
                _CopySize += _File.Length; 
                if (MyCopyRun != null) MyCopyRun(_FileCount, _CopyCount, _FileSize, _CopySize, _File.Name); 
            } 

            foreach (DirectoryInfo _SourceSubDir in p_Source.GetDirectories())  //循环子目录 
            { 
                DirectoryInfo _NextDir = p_Target.CreateSubdirectory(_SourceSubDir.Name); 
                Copy(_SourceSubDir, _NextDir); 
            } 
        } 
        
    } 
} 
