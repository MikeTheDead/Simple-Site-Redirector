using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace altsite.Attributes;

public class LocalAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var remoteIp = context.HttpContext.Connection.RemoteIpAddress;

        // Check if the IP is not null and is a loopback address
        if (remoteIp == null || !IPAddress.IsLoopback(remoteIp))
        {
            context.Result = new StatusCodeResult(403);  // Forbidden
        }

        base.OnActionExecuting(context);
    }
}