using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Registro_Estudiantes.Clases;
using Registro_Estudiantes.Service;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Registro_Estudiantes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudianteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EstudianteController(AppDbContext context) 
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            List<Estudiante> estudiantes = await _context.Estudiantes.Include(e=> e.EstudiantesMaterias).ToListAsync();
            return Ok(estudiantes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Estudiante? estudiante = await _context.Estudiantes.Include(e => e.EstudiantesMaterias).FirstOrDefaultAsync(n=> n.Id == id);
            if (estudiante == null) 
            {
                return NotFound("estudiante no encontrado");
            }
            return Ok(estudiante);
        }

        [HttpGet("editar/{id}")]
        public async Task<IActionResult> GetEdit(int id) 
        {
            EstudianteDto? estudiante = await _context.Estudiantes
                .Include(e => e.EstudiantesMaterias)
                .Select(e => new EstudianteDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Correo = e.Correo,
                    TotalCreditos = e.TotalCreditos,
                    IdMaterias = e.EstudiantesMaterias.Select(em => em.MateriaId).ToList()
                }).FirstOrDefaultAsync(e => e.Id == id);

            List<Materia>? materias = _context.Materias.Include(m=> m.Profesor).Where(m => estudiante.IdMaterias.Contains(m.Id)).ToList();
            
            foreach (var materia in materias) 
            {
                List<string?> integrantes = _context.EstudianteMaterias.Include(em => em.Estudiante)
                    .Where(em => em.MateriaId == materia.Id)
                    .Select(e => e.Estudiante.Nombre).ToList();

                estudiante.Materias.Add(new MateriasAux() 
                {
                    NombreMateria = materia.Nombre,
                    Integrantes = integrantes,
                    Profesor = materia.Profesor.Nombre
                });
            }
            return Ok(estudiante.Materias);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Estudiante estudiante)
        {
            try
            {
                IEnumerable<int>? idMaterias = estudiante.EstudiantesMaterias.Select(em => em.MateriaId);
                estudiante.TotalCreditos = _context.Materias.Where(m => idMaterias.Contains(m.Id)).Select(o => o.Creditos).Sum();
                if (estudiante.TotalCreditos == 9)
                {
                    List<Materia> materiasInvalidas = new();
                    Materia auxMateria = new();
                    List<Materia>? materias = _context.Materias.Where(m => idMaterias.Contains(m.Id)).ToList();
                    var materiasGrupo = materias.GroupBy(m => m.ProfesorId).ToList();
                    
                    foreach (var grupo in materiasGrupo) 
                    {
                        if (grupo.Count() > 1) 
                        {
                            foreach (var aux in grupo)
                            {
                                materiasInvalidas.Add(aux);
                            }
                            var materiasInfo = materiasInvalidas.Select(m => $"{m.Nombre}(Profesor Id: {m.ProfesorId}) ").ToList();
                            string mensaje = "El estudiante no puede seleccionar materias con el mismo profesor: " + string.Join(",", materiasInfo);
                            return BadRequest(new { mensaje = mensaje });
                        }
                    }

                    await _context.Estudiantes.AddAsync(estudiante);
                    await _context.SaveChangesAsync();
                    return Ok("Estudiante Creado con Exito.");
                }
                else 
                {
                    return BadRequest(new { mensaje = "Estudiante debe seleccionar 3 Materias." });
                }
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("modificar/{id}")]
        public async Task<IActionResult> Put(int id,[FromBody]Estudiante estudianteActualizado)
        {
            try
            {
                var estudianteExistente = await _context.Estudiantes
                    .Include(e => e.EstudiantesMaterias)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (estudianteExistente == null)
                {
                    return NotFound(new { mensaje = "Estudiante no encontrado" });
                }


                estudianteExistente.Nombre = estudianteActualizado.Nombre;
                estudianteExistente.Correo = estudianteActualizado.Correo;


                _context.EstudianteMaterias.RemoveRange(estudianteExistente.EstudiantesMaterias);

                estudianteExistente.EstudiantesMaterias = estudianteActualizado.EstudiantesMaterias;


                var idMaterias = estudianteActualizado.EstudiantesMaterias.Select(em => em.MateriaId);
                estudianteExistente.TotalCreditos = _context.Materias
                    .Where(m => idMaterias.Contains(m.Id))
                    .Select(m => m.Creditos)
                    .Sum();


                if (estudianteExistente.TotalCreditos != 9)
                {
                    return BadRequest(new { mensaje = "El estudiante debe seleccionar exactamente 3 materias" });
                }

                
                var materias = await _context.Materias
                    .Where(m => idMaterias.Contains(m.Id))
                    .ToListAsync();

                var materiasGrupo = materias.GroupBy(m => m.ProfesorId);

                List<Materia> materiasInvalidas = new();

                foreach (var grupo in materiasGrupo)
                {
                    if (grupo.Count() > 1)
                    {
                        materiasInvalidas.AddRange(grupo);
                    }
                }

                if (materiasInvalidas.Any())
                {
                    var materiasInfo = materiasInvalidas
                        .Select(m => $"{m.Nombre}(Profesor Id: {m.ProfesorId})")
                        .ToList();
                    string mensaje = "No puedes seleccionar materias con el mismo profesor: " + string.Join(", ", materiasInfo);
                    return BadRequest(new { mensaje = mensaje });
                }

                await _context.SaveChangesAsync();
                return Ok(new { mensaje= "Estudiante actualizado con éxito" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = $"Ocurrió un error inesperado : {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try 
            {
                var estudiante = await _context.Estudiantes.Include(e => e.EstudiantesMaterias).FirstOrDefaultAsync(e => e.Id == id);
                if (estudiante == null)
                {
                    return NotFound(new {mensaje = "estudiante no encontrado" });
                }
                _context.EstudianteMaterias.RemoveRange(estudiante.EstudiantesMaterias);
                _context.Estudiantes.Remove(estudiante);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje= "Estudiante Eliminado" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { mensaje = $"Ocurrió un error inesperado : {ex.Message}" });
            }
        }
    }
}
