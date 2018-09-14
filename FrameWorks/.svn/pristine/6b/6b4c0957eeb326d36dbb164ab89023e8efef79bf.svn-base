using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks.Iso8583
{
    /// <summary>
    /// 表示 ISO 8583 包的数据类型
    /// </summary>
    public enum Iso8583DataType
    {
        // 除了A表示不压缩，B表示压缩外，R表示不压缩，但是数据内容为byte型，其他暂时没用

        /// <summary>
        /// 字母字符，A至Z或a至z，左靠，右部多余部分填空格
        /// </summary>
        A,
        /// <summary>
        /// 二进制位，左靠，右部多余部分填零。
        /// </summary>
        B,
        /// <summary>
        /// 16进制数据，但是不压缩
        /// </summary>
        R,

        /// <summary>
        /// 数字字符，0至9，右靠，左边多余部分填零。若表示金额，无小数点符号，最后两位表示角分
        /// </summary>
        N,
        /// <summary>
        /// 特殊字符
        /// </summary>
        S,
        /// <summary>
        /// 借贷符号，贷记为“C”，借记为“D”，后接一个数字型金额数据元。
        /// </summary>
        X,
        /// <summary>
        /// 由ISO 7811和ISO 7813制定的磁卡第二、三磁道数据
        /// </summary>
        Z,
        /// <summary>
        /// 字母或数字，左靠，右部多余部分填空格
        /// </summary>
        AN,
        /// <summary>
        /// 字母、数字或特殊符号，左靠，右部多余部分填空格
        /// </summary>
        ANS
    }
}
