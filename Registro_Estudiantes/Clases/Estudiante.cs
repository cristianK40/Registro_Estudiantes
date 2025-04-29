using System.ComponentModel.DataAnnotations;

namespace Registro_Estudiantes.Clases
{
    public class Estudiante
    {
        [Key]
        public int? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public int TotalCreditos { get; set; }

        public ICollection<EstudianteMateria> EstudiantesMaterias { get; set; }
    }
}
