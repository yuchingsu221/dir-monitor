﻿using System;
using System.Runtime.Serialization;

namespace WebService.Models.AppResponse
{
    [DataContract]
    public class BaseResponseModel<T>
    {
        [DataMember]
        public string RtnCode { get; set; }
        [DataMember]
        public string RtnMsg { get; set; }
        //RtnData
        [DataMember]
        public T Data { get; set; }

        public BaseResponseModel()
        {
            RtnCode = "0000";
            //RtnMsg = ErrorDefine.GetErrorDefine(ErrorCodeEnum.SUCCESS_CODE).ErrorMsg;
        }
    }
}
