using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMClusterManager.Data
{
    public interface IDataProvider
    {
        VMHostGroup GetHostTree(string path);
        void Save(object o);
    }
}
