using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks.Iso8583
{
    /// <summary>
    /// 表示 ISO 8583 包的字段填充数据
    /// </summary>
    public enum Iso8583PadType
    {
        /// <summary>
        /// 填充空格
        /// </summary>
        Space,
        /// <summary>
        /// 填充'/0'
        /// </summary>
        Null,
        /// <summary>
        /// 填充'0'
        /// </summary>
        Zero,
    }
}
