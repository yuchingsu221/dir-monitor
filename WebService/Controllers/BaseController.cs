using CommonLibrary.Util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebService.Attributes;

namespace WebService.Controllers
{
    [ApiController]
    [ResponseFormat]
    [Produces("application/json", "text/plain")]
    public class BaseController : ControllerBase
    {
        protected string GetClientIpAddress(bool havePort = false)
        {
            var clientAddress = string.Empty;

            try
            {
                var xForwardedFor = Request.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                if (string.IsNullOrEmpty(xForwardedFor))
                {
                    clientAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                }
                else
                {
                    var ips = xForwardedFor.Split(',').Select(ip => ip.Trim()).ToArray();

                    clientAddress = ips[0];
                    int portIndex = clientAddress.IndexOf(":");
                    if (!havePort && portIndex != -1)
                    {
                        // Remove port number
                        clientAddress = clientAddress.Substring(0, portIndex);
                    }
                }
                LogUtility.LogInfo($"x-Forwarded-For: {xForwardedFor}");
                LogUtility.LogInfo($"clientAddress: {clientAddress}");
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("Get Client ip error", ex);
            }

            return clientAddress;
        }

    }
}
