using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatHouse.Models
{
    public class Plan
    {
        [Key]
        public int Codigo { get; set; }

        public string Nombre { get; set; }

        public double Precio { get; set; }

        public int CantidadUsuarios { get; set; }

        public string Descripcion { get; set; }
    }
}
