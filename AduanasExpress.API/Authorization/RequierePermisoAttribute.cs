using Microsoft.AspNetCore.Mvc;
using AduanasExpress.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

public class RequierePermisoAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _modulo;
    private readonly string _accion;

    public RequierePermisoAttribute(string modulo, string accion)
    {
        _modulo = modulo;
        _accion = accion;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var rolIdClaim = context.HttpContext.User.FindFirst("rolId")?.Value;
        if (rolIdClaim == null || !int.TryParse(rolIdClaim, out var rolId))
        {
            context.Result = new ForbidResult();
            return;
        }

        var permisoService = context.HttpContext.RequestServices.GetRequiredService<IPermisoService>();
        var permitido = await permisoService.TienePermiso(rolId, _modulo, _accion);

        if (!permitido)
        {
            context.Result = new ObjectResult(new { mensaje = "No tienes permiso para realizar esta acción." }) { StatusCode = 403 };
            return;
        }

        await next();
    }
}