using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PlacementManagementSystem.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizeRoleAttribute : Attribute, IActionFilter
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userId = session.GetInt32(SessionConstants.UserId);
            var userRole = session.GetString(SessionConstants.UserRole);

            if (!userId.HasValue || string.IsNullOrEmpty(userRole))
            {
                var controller = context.Controller as Controller;
                if (controller != null)
                {
                    controller.TempData["Error"] = "Please log in to access this page.";
                }

                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl });
                return;
            }

            if (_roles.Length > 0 && !_roles.Contains(userRole))
            {
                var controller = context.Controller as Controller;
                if (controller != null)
                {
                    controller.TempData["Error"] = "Access Denied: You do not have permission to perform this action.";
                }

                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
