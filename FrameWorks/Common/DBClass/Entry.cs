using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks
{
    public class Entry:System.MarshalByRefObject
    {
        public DataAccessInterface GetDataAccess()
        {
            DataAccess da = new DataAccess();
            return da;
        }
    }
}
