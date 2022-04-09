using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BlogApi.Infrastructure
{
    public class GroupByApiRootConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var controllerNamespace = controller.Attributes.OfType<RouteAttribute>().FirstOrDefault();
            var apiVersion = controllerNamespace?.Template?.Split('/')[1].ToLowerInvariant() ?? "default";
            controller.ApiExplorer.GroupName = apiVersion;
        }
    }
}
