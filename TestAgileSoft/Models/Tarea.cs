using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestAgileSoft.Models
{
    public class Tarea
    {
        public long Id { get; set; }
        public bool Estado { get; set; }
        [Required]
        public string Descripcion { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
