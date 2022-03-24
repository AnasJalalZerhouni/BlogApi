using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace BlogApi.Infrastructure
{
    public class ValidatorActionFilter : IActionFilter
    {

        private readonly ILogger logger;

        public ValidatorActionFilter(ILogger<ValidatorActionFilter> logger)
        {
            this.logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext filtercontext)
        {
            if (!filtercontext.ModelState.IsValid)
            {
                var result = new ContentResult();
                var errors = new Dictionary<string, string[]>();

                foreach (var valuePair in filtercontext.ModelState )
                {
                    errors.Add(valuePair.Key, valuePair.Value.Errors.Select(e=>e.ErrorMessage).ToArray());
                }

                string content = JsonSerializer.Serialize(new {errors});
                result.Content = content;
                result.ContentType = "application/json";

                filtercontext.HttpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                filtercontext.Result = result;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}
