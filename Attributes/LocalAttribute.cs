using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace altsite.Attributes;

public class LocalAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        var remoteIp = request.HttpContext.Connection.RemoteIpAddress;
        var isLocal = IPAddress.IsLoopback(remoteIp);

        if (!isLocal)
        {
            context.Result = new StatusCodeResult(403);  // Forbidden
        }
        base.OnActionExecuting(context);
    }
}