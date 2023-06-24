using System.ComponentModel.DataAnnotations;

namespace GestionDeGastos.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? Password { get; set; }
        public decimal Fondo { get; set; }
    }
}
