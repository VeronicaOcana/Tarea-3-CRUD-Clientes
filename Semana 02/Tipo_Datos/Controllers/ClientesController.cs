using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tipo_Datos.Data;
using Tipo_Datos.Models.Entidades;

namespace Tipo_Datos.Controllers
{
    public class ClientesController : Controller
    {
        private readonly DatosDbContext _dbContext;
        public ClientesController(DatosDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        /* public async Task<IActionResult> Index()
         {
             return View(await _dbContext.Clientes.ToListAsync());
         }*/

        public async Task<IActionResult> Index(string cedulaRUC)
        {
            IQueryable<ClientesModel> consulta = _dbContext.Clientes;

            if (!string.IsNullOrWhiteSpace(cedulaRUC))
            {
                consulta = consulta.Where(c => c.Cedula_RUC == cedulaRUC);
                ViewBag.Mensaje = await consulta.AnyAsync()
                    ? null
                    : "No se encontró ningún cliente con ese RUC.";
                ViewBag.RucBuscado = cedulaRUC;
            }

            var clientes = await consulta.ToListAsync();
            return View(clientes);
        }

        public IActionResult Nuevo() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Nuevo([Bind("Nombres, Email, Telefono, Direccion, Cedula_RUC, Create_At, Update_At, isDelete")] ClientesModel cliente)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Add(cliente);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return  View(cliente);
        }
        

        // Aqui empieza la tarea

        //Para Editar Ciente
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _dbContext.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("Id, Nombres, Email, Telefono, Direccion, Cedula_RUC, Create_At, Update_At, isDelete")] ClientesModel cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(cliente);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_dbContext.Clientes.Any(e => e.Id == cliente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        //Para eliminar Cliente

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _dbContext.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Eliminar
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var cliente = await _dbContext.Clientes.FindAsync(id);
            _dbContext.Clientes.Remove(cliente);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // buscar por RUC-Cedula

        [HttpGet]
        public async Task<IActionResult> BuscarPorRUC(string cedulaRUC)
        {
            if (string.IsNullOrWhiteSpace(cedulaRUC))
            {
                ModelState.AddModelError("", "Debe ingresar un número de cédula o RUC.");
                return View("Buscar");
            }

            var cliente = await _dbContext.Clientes
                .FirstOrDefaultAsync(c => c.Cedula_RUC == cedulaRUC);

            if (cliente == null)
            {
                ViewBag.Mensaje = "No se encontró ningún cliente con ese RUC.";
                return View("Buscar");
            }

            return View("ResultadoBusqueda", cliente);
        }



    }
}


