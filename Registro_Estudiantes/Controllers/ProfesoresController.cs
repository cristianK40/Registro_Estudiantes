using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Registro_Estudiantes.Clases;
using Registro_Estudiantes.Service;

namespace Registro_Estudiantes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfesoresController : ControllerBase
    {
        [HttpPut]
        public async Task<List<Profesor>> CrearProfesoresPrueba(AppDbContext _context)
        {
            try
            {
                List<Profesor> profesores = new();
                List<string> nombres = new List<string> { "Cristian", "Carlos", "Antonio", "Messi", "Cristiano" };
                for (int i = 0; i < 5; i++)
                {
                    profesores.Add(new Profesor { Nombre = nombres[i] });
                }
                await _context.Profesores.AddRangeAsync(profesores);
                await _context.SaveChangesAsync();
                return profesores;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
