using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces
{
    public interface IComputerVision
    {
        string CatVsDogClassifier_(string path);
    }
}
