using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;

namespace NetCoreSeguridadEmpleados.Filters
{
    public class AuthorizeEmpleadosAttribute : AuthorizeAttribute
        , IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //POR AHORA, NOS DA IGUAL QUIEN SEA EL EMPLEADO
            //SIMPLEMENTE QUE EXISTA
            var user = context.HttpContext.User;

            //necesitamos el controller y el action de donde
            //hemos pulado previamente antes de entrar en filter
            string controller =
                context.RouteData.Values["controller"].ToString();
            string action =
                context.RouteData.Values["action"].ToString();

            //para comprbar si funciona, a consola
            Debug.WriteLine("Controller: " + controller);
            Debug.WriteLine("Action: " + controller);

             ITempDataProvider provider =
                context.HttpContext.RequestServices
                .GetService<ITempDataProvider>();
            //esta clase contiene en su interior el tempdata de nuestra app
            //recuperamos TempData de la app
            var TempData = provider.LoadTempData(context.HttpContext);
            //guardamos la info en tempdata
            TempData["controller"] = controller;
            TempData["action"] = action;

            //volvemos a guardar los cambios de tempdata en la app
            provider.SaveTempData(context.HttpContext, TempData);

            if (user.Identity.IsAuthenticated == false)
            {
                //ENVIAMOS A LA VISTA LOGIN
                context.Result = this.GetRoute("Managed", "Login");
            }
            else
            {
                //hay que comprobar el rol del user
                //para permitir o no el acceso
                if (user.IsInRole("PRESIDENTE") == false
                    && user.IsInRole("ANALISTA") == false
                    && user.IsInRole("DIRECTOR") == false)
                {
                    context.Result = this.GetRoute("Manager", "ErrorAcceso");
                }
            }
        }

        //COMO TENDREMOS MULTIPLES REDIRECCIONES, CREAMOS UN METODO
        //PARA FACILITAR LA REDIRECCION
        private RedirectToRouteResult GetRoute
            (string controller, string action)
        {
            RouteValueDictionary ruta =
                new RouteValueDictionary(
                    new { controller = controller, action = action});
            RedirectToRouteResult result =
                new RedirectToRouteResult(ruta);
            return result;
        }
    }
}
