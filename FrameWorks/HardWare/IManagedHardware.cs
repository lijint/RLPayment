using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks
{
    public interface IManagedHardware
    {
        object Open();
        object Close();
        object CheckStatus();
        bool MeansError(object status);
    }

    public interface IManageNet
    {
    }
}
