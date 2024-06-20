namespace Papeleria_MVC.Middlewares
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        public AuthenticationMiddleware(RequestDelegate next)
        {
               _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;

            var excludePaths = new List<string> { "/Login/Login", "/Login/Logout" };

            var token = context.Session.GetString("Token");
            if (token == null && !excludePaths.Contains(path))
            {
                context.Response.Redirect("/Login/Login");
                return;
            }
            context.Request.Headers.Add("Authorization", "Bearer " + token);
            await _next(context);
        }
        
    }
}
