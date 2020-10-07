using System;

namespace HardwareKeyCreationTool
{
    public class HardwareConfiguratorException : Exception
    {
        public int returnCode { get; set; }
        public HardwareConfiguratorException(int ReturnCode, string message) : base(message)
        {
            returnCode = ReturnCode;
        }
    }
    public enum NonHaspExceptions
    {
        GENERAL_FAIL = -1,
        KEY_VALUE_SIZE_ZERO_FAIL = -2,
        HIDDEN_MEMORY_XML_FAIL = -3,
        KEY_FINGERPRINT_EMPTY = -4,
        KEY_FINGERPRINT_XML_FAIL = -5,
    }
}
