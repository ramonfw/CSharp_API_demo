using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using myapi_cs.Models;


namespace myapi_cs.Middlewares
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string vPath = httpContext.Request.Path.ToString();
            string vMethod = httpContext.Request.Method.ToString();

            if (vPath == "/api/Employee" && vMethod == "GET")
            {
                await _next(httpContext);
            }
/*
            else if (vPath.IndexOf("api/Employee/list/") >0 )
            {
                await _next(httpContext);
            }
*/
            else
            {
                string authHeader = httpContext.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.IndexOf("Basic ") >= 0)
                {
                    string auth = authHeader.Split(new char[] { ' ' })[1];
                    Encoding encoding = Encoding.GetEncoding("UTF-8");
                    var usernameAndPassword = encoding.GetString(Convert.FromBase64String(auth));
                    string username = usernameAndPassword.Split(new char[] { ':' })[0];
                    string password = usernameAndPassword.Split(new char[] { ':' })[1];
                    if (username == "abc" && password == "123")
                    {
                        await _next(httpContext);
                    }
                    else
                    {
                        httpContext.Response.StatusCode = 401;
                        httpContext.Response.Headers.Add("X-Unauthorized-string", "Nombre de usuario o clave incorrectos");
                        return;
                    }
                }
                else if (authHeader != null && authHeader.IndexOf("Bearer ") >= 0)
                {
                    //--
                    string auth = authHeader.Split(new char[] { ' ' })[1];
                    string vToken = auth;

                    Int64 vEmployeeId = 0;
                    using (var context = new chinookContext())
                    {
                        foreach (UserAccess usuarios in context.UserAccesses.Where(usuario => usuario.AuthToken == vToken))
                            vEmployeeId = usuarios.EmployeeId;
                    }

                    if (vEmployeeId > 0 || vToken == "1234567890")
                    {
                        httpContext.Response.Headers.Add("X-Authorized-token", vToken);
                        await _next(httpContext);
                    }
                    else
                    {
                        httpContext.Response.StatusCode = 401;
                        httpContext.Response.Headers.Add("X-Unauthorized-string", "Bearer Token incorrecto");
                        return;
                    }
                }
                else
                {
                    httpContext.Response.Headers.Add("X-Unauthorized-string", "Debe enviar token, o usuario y clave en su solicitud.");
                    httpContext.Response.StatusCode = 401;
                    return;
                }
            }

        }
    }

    public static class BasicAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BasicAuthMiddleware>();
        }
    }
}