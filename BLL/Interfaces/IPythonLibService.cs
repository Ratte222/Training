using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces
{
    public interface IPythonLibService
    {
        int CalcArraySum(int[] arr, bool useIronPython = true);
    }
}
