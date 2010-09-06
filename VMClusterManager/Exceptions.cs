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

    public class VMOperationException : InvalidOperationException
    {
        private UInt32 errorCode;

        public UInt32 ErrorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }

        public VMOperationException (string message, UInt32 ErrorCode):base(message)
        {
            this.ErrorCode = ErrorCode;
        }
    }

    public class VMStateChangeException : VMOperationException
    {
        

        public VMStateChangeException(UInt32 errorCode)
            : base(GetErrorMessage(errorCode), errorCode)
        {
            this.ErrorCode = errorCode;
            
        }

        private static string GetErrorMessage(UInt32 ErrorCode)
        {
            string message = String.Empty;
            switch (ErrorCode)
            {
                case ReturnCode.AccessDenied: message = "Access denied"; break;
                case ReturnCode.Failed: message = "Failed"; break;
                case ReturnCode.IncorrectDataType: message = "Incorrect data type"; break;
                case ReturnCode.InvalidParameter: message = "Invalid parameter"; break;
                case ReturnCode.InvalidState: message = "Operation is not valid due to current state of the object"; break;
                case ReturnCode.NotSupported: message = "Not supported"; break;
                case ReturnCode.OutofMemory: message = "Out of memory"; break;
                case ReturnCode.SystemInUse: message = "System is in use"; break;
                case ReturnCode.SystemNotAvailable: message = "System not available"; break;
                case ReturnCode.Timeout: message = "Timeout"; break;
                default: message = "Unknown Error"; break;
            }
            return message + ". Error code: " + ErrorCode.ToString();
        }
    }

    public class VMResourcesUpdateException : VMOperationException
    {
        public VMResourcesUpdateException(UInt32 ErrorCode)
            : base("An error occured while updating resources. Error code: " + ErrorCode.ToString(), ErrorCode)
        {
        }
    }

}
