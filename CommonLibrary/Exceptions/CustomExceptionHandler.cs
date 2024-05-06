using System;
using System.Runtime.Serialization;

namespace CommonLibrary
{
    [Serializable]
    public class CustomExceptionHandler : Exception, ISerializable
    {
        public CustomExceptionHandler(ErrorCodeEnum errorCode, string customErrorMsg) : base(((int)errorCode).ToString() + "||" + customErrorMsg) { }
    }
}
