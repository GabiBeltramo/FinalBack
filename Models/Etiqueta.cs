using System.ComponentModel.DataAnnotations;

namespace GestionDeGastos.Models
{
    public class Etiqueta
    {
        [Key]
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int IdUsuario { get; set; }

    }
}
