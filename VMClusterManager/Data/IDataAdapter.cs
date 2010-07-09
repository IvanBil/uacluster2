using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMClusterManager.Data
{
    public interface IDataAdapter
    {
        public VMHostGroup GetHostTree(string path);
    }
}
