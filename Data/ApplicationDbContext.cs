using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GestionDeGastos.Data
{
    public class GestionDeGastosContext : DbContext
    {
        public GestionDeGastosContext(DbContextOptions<GestionDeGastosContext> options) : base(options)
        { }
        public DbSet<GestionDeGastos.Models.Usuario> Usuario { get; set; } = default!;
        public DbSet<GestionDeGastos.Models.Consumo> Consumo { get; set; } = default!;
        public DbSet<GestionDeGastos.Models.Etiqueta> Etiqueta { get; set; } = default!;
        
            
    }
}

