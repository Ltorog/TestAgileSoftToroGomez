using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiCertiCorp.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TestAgileSoft.Models;

namespace TestAgileSoft.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TareasController : ControllerBase
    {
        private readonly DBContext context;

        public TareasController(DBContext context, IOptions<AppSettings> appSettings)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok("Bienvenido al panel de tareas");
        }

        [HttpGet("ObtenerListado")]
        public async Task<ActionResult> ObtenerListado()
        {
            List<Tarea> lstTareas = new List<Tarea>();
            try
            {
                Usuario datosUsuario = await ObtenerDatosUsuario();

                if (datosUsuario == null)
                {
                    return StatusCode(500, "Error al obtener datos de usuario");
                }

                lstTareas = await context.TSTAG_Tareas.Where(x => x.Usuario.Id == datosUsuario.Id).ToListAsync();
            }
            catch
            {
                return BadRequest(lstTareas);
            }
            return Ok(lstTareas);
        }

        [HttpPost("CrearTarea")]
        public async Task<ActionResult> CrearTarea(Tarea tarea)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(422, ModelState.Where(ms => ms.Value.Errors.Any()).ToList());
                }

                Usuario datosUsuario = await ObtenerDatosUsuario();

                if (datosUsuario == null)
                {
                    return StatusCode(500, "Error al obtener datos de usuario");
                }

                Tarea newTarea = new Tarea()
                {
                    Id = 0,
                    Descripcion = tarea.Descripcion,
                    Estado = false,
                    Usuario = datosUsuario
                };

                await context.TSTAG_Tareas.AddAsync(newTarea);

                await context.SaveChangesAsync();


                return Ok("Tarea creada con exito");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear tarea: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> MarcarTarea(long id)
        {
            try
            {
                Usuario datosUsuario = await ObtenerDatosUsuario();

                if (datosUsuario == null)
                {
                    return StatusCode(500, "Error al obtener datos de usuario");
                }

                Tarea tarea = await context.TSTAG_Tareas.Where(x => x.Id == id && x.Usuario.Id == datosUsuario.Id).FirstOrDefaultAsync();

                if (tarea == null)
                {
                    return Ok("Tarea no encontrada o no asociada por usuario");
                }

                if (tarea.Estado == true)
                {
                    return Ok("Tarea ya se encuentra realizada");
                }

                tarea.Estado = true;

                await context.SaveChangesAsync();

                return Ok("Tarea actualizada con exito");
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo actualizar la tarea: " + ex.Message);
            }
        }


        /// <summary>
        /// Metodo que obtiene los datos del usuario con token
        /// </summary>
        private async Task<Usuario> ObtenerDatosUsuario()
        {
            Usuario usuario = new Usuario();
            try
            {
                // Cast to ClaimsIdentity.
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                // Gets list of claims.
                IEnumerable<Claim> claim = identity.Claims;

                // Gets name from claims. Generally it's an email address.
                var usernameClaim = claim
                    .Where(x => x.Type == ClaimTypes.Name)
                    .FirstOrDefault();

                usuario = await context.TSTAS_Usuarios.Where(x => x.Id.ToString() == usernameClaim.Value).FirstOrDefaultAsync();
            }
            catch
            {

            }

            return usuario;
        }
    }
}
