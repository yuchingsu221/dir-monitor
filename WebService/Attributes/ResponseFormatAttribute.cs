using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebService.Models.AppResponse;

namespace WebService.Attributes
{
    public class ResponseFormatAttribute : ActionFilterAttribute
    {
        //在controller action執行前後設定其他動作
        public override void OnActionExecuting(ActionExecutingContext context)
        {

        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult result)
            {
                //加上這段的話就可以讓response吃到基本欄位的設定
                result.Value = new ApiResponse(result.Value);
                //可以讓每次打api都寫log

            }
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            OnActionExecuting(context);
            var resultContext = await next();
            OnActionExecuted(resultContext);
        }
    }
}
