using System;
using System.Runtime.InteropServices;

namespace VMClusterManager
{
    public class RPCCallException : COMException
    {
        public RPCCallException(VMHost host, COMException ex):base(ex.Message,ex.ErrorCode)
        {
            host.Status = VMHostStatus.ERROR;
            host.StatusString = ex.Message;
            if ((uint)ex.ErrorCode == 0x80070721)
            {
                throw new UnauthorizedAccessException(ex.Message, ex);
            }
        }
    }
}
