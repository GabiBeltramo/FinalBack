using System.ComponentModel.DataAnnotations;

namespace GestionDeGastos.Models
{
    public class Consumo
    {
        [Key]
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public int IdEtiqueta { get; set; }
        public DateTime? Fecha { get; set; } = DateTime.Now;
        public int IdUsuario { get; set; }
        public string Descripcion { get; set; }
    }
}
