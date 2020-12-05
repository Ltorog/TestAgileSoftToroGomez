using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiCertiCorp.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TestAgileSoft.Models;

namespace TestAgileSoft.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly DBContext context;
        private readonly AppSettings appSettings;

        public HomeController(DBContext context, IOptions<AppSettings> appSettings)
        {
            this.context = context;
            this.appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Bienvenido!");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(Usuario user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(422, ModelState.Where(ms => ms.Value.Errors.Any()).ToList());
                }

                var userExists = await context.TSTAS_Usuarios.Where(x => x.UserName == user.UserName).FirstOrDefaultAsync();

                if (userExists != null)
                {
                    return Ok("Usuario ya existe");
                }

                await context.TSTAS_Usuarios.AddAsync(user);

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return Created("", "Usuario creado con exito");
        }

        [HttpPost("Authentication")]
        public async Task<ActionResult> Authentication(Login user)
        {
            ResponseAuthetication response = new ResponseAuthetication();
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(422, ModelState.Where(ms => ms.Value.Errors.Any()).ToList());
                }
                response = await context.TSTAS_Usuarios.Where(x => x.UserName == user.UserName && x.Password == user.Password).Select(x => new ResponseAuthetication
                {
                    NombreUsuario = x.UserName,
                    Id = x.Id,
                    Nombre = x.Nombre,
                }).FirstOrDefaultAsync();

                if (response != null)
                {
                    response.Token = GenerarJWT(response);
                    response.Code = "200";
                    response.Mensaje = "Usuario encontrado con exito";
                }
                else
                {
                    response = new ResponseAuthetication() {
                        Id = 0,
                        NombreUsuario = "",
                        Nombre = "",
                        Token = "",
                        Code = "200",
                        Mensaje = "Usuario no encontrado"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new ResponseAuthetication()
                {
                    Id = 0,
                    NombreUsuario = "",
                    Nombre = "",
                    Token = "",
                    Code = "500",
                    Mensaje = "Error al buscar usuario: " + ex.Message
                };

                return BadRequest(response);
            }

            return Ok(response);
        }


        /// <summary>
        /// Crea metodo para escribir token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string GenerarJWT(ResponseAuthetication user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(appSettings.Key);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.NombreUsuario.ToString()),
                        new Claim(ClaimTypes.SerialNumber, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim(ClaimTypes.Version, "V1.0")
                    }),
                    Expires = DateTime.UtcNow.AddDays(2),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
