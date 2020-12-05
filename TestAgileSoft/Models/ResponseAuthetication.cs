using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAgileSoft.Models
{
    public class ResponseAuthetication
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Token { get; set; }
        public string NombreUsuario { get; set; }
        public string Code { get; set; }
        public string Mensaje { get; set; }
    }
}
