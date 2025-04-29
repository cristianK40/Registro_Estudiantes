using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Registro_Estudiantes.Clases;
using Registro_Estudiantes.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Registro_Estudiantes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriasController : ControllerBase
    {
        [HttpPut]
        public async Task<List<Materia>> CrearMateriasPrueba(AppDbContext _context)
        {
            try
            {
                List<Materia> materias = new List<Materia>
                {
                    new Materia { Nombre = "Matemáticas", ProfesorId = 3 },
                    new Materia { Nombre = "Física", ProfesorId = 3 },
                
                    new Materia { Nombre = "Química", ProfesorId = 4 },
                    new Materia { Nombre = "Biología", ProfesorId = 4 },
                
                    new Materia { Nombre = "Lengua", ProfesorId = 5 },
                    new Materia { Nombre = "Historia", ProfesorId = 5 },
                
                    new Materia { Nombre = "Geografía", ProfesorId = 6 },
                    new Materia { Nombre = "Educación Física", ProfesorId = 6 },
                
                    new Materia { Nombre = "Arte", ProfesorId = 7 },
                    new Materia { Nombre = "Música", ProfesorId = 7 },
                };
                await _context.AddRangeAsync(materias);
                await _context.SaveChangesAsync();
                return materias;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public async Task<List<Materia>> ConsultarMateria(AppDbContext _context) 
        {
            var test = await _context.Materias.Include(m => m.Profesor).ToListAsync();
            return test;
        }
    }
}
