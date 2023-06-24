using GestionDeGastos.Data;
using GestionDeGastos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;


namespace GestionDeGastos.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly GestionDeGastosContext _Context;

        private static Usuario usuarioGlobal;
        private static Etiqueta etiquetaGlobal;
        private static Consumo consumoGlobal;

        public UsuarioController(GestionDeGastosContext context)
        {
            _Context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            return View();
        }
        public IActionResult NuevaEtiqueta()
        {
            return View();
        }
       
       
        public async Task<IActionResult> NuevoConsumo()
        {
            if (usuarioGlobal != null)
            {
                var etiquetas = await _Context.Etiqueta.ToListAsync();
                ViewBag.Etiquetas = etiquetas;

                return View();
            }

            return View();
        }
        

        // Perfil/CrearPerfil
        [HttpPost]
        public async Task<IActionResult> CrearPerfil(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (await _Context.Usuario.AnyAsync(u => u.Correo == usuario.Correo))
                {
                    ModelState.AddModelError(string.Empty, "El correo ya está registrado.");
                    return View(usuario);
                }

                usuario.Fondo = 0;
                _Context.Add(usuario);
                await _Context.SaveChangesAsync();
                usuarioGlobal = usuario;
                return RedirectToAction("Perfil");
            }
            return View(usuario);

        }
        [HttpPost]
        public async Task<IActionResult> Login(string correo, string password)
        {
            if (string.IsNullOrEmpty(correo))
            {
                ModelState.AddModelError("Correo", "El correo es obligatorio");
            }

            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("Password", "La contraseña es obligatoria");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var usuario = await _Context.Usuario.FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario != null && CryptoHelper.Decrypt(usuario.Password) == password)
            {
                HttpContext.Response.Cookies.Append("UserId", usuario.Id.ToString());

                usuarioGlobal = usuario;
                ViewBag.Bienvenida = "Bienvenido: " + usuarioGlobal.Nombre; 
                
                return RedirectToAction("Perfil");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos");
                return View();
            }
        }

     

        [HttpPost]
        public async Task<IActionResult> ModificarFondo(decimal nuevoFondo)
        {

            if (HttpContext.Request.Cookies.TryGetValue("UserId", out string userIdString) && int.TryParse(userIdString, out int userId))
            {
                var usuario = await _Context.Usuario.FindAsync(userId);
                if (usuario != null)
                {
                    usuario.Fondo = nuevoFondo;
                    _Context.Update(usuario);
                    await _Context.SaveChangesAsync();
                    ViewBag.Fondo = nuevoFondo;
                    return RedirectToAction("Perfil");
                }
            }

            return RedirectToAction("Perfil");
        }

       
        [HttpPost]
        public async Task<IActionResult> CrearEtiqueta(Etiqueta etiqueta)
        {
            if (usuarioGlobal != null)
            {
                etiquetaGlobal = etiqueta;
                etiqueta.IdUsuario = usuarioGlobal.Id;

                _Context.Add(etiqueta);
                await _Context.SaveChangesAsync();

                return RedirectToAction("NuevaEtiqueta");
            }

            return RedirectToAction("NuevaEtiqueta");
        }

        [HttpPost]
        public async Task<IActionResult> NuevoConsumo(Consumo consumo)
        {
            if (usuarioGlobal != null)
            {

                consumoGlobal = consumo;
                consumo.IdUsuario = usuarioGlobal.Id;
                consumo.IdEtiqueta = etiquetaGlobal.Id;
                consumo.Fecha = consumo.Fecha;
                _Context.Add(consumo);
                await _Context.SaveChangesAsync();
                
                var etiquetas = await _Context.Etiqueta.ToListAsync();
                ViewBag.Etiquetas = etiquetas;

                var usuario = await _Context.Usuario.FindAsync(usuarioGlobal.Id);
                usuario.Fondo -= consumo.Monto;
                _Context.Update(usuario);
                await _Context.SaveChangesAsync();

                return RedirectToAction("NuevoConsumo");
            }

            return RedirectToAction("NuevoConsumo");
        }





        //No guarda descripcion ni monto, la desc lo puedo eliminar, el monto NO 
        //desde el perfil no entra a nuevo consumo con login
        //Falta:
        // Graficos de resumen, en perfil o aparte?
        //Barra superior, modificar. Agregar boton log out
        //Agregar a la formula de nuevo consumo, la resta del dinero del fondo



    }
}
