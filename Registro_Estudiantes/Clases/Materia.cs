using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Registro_Estudiantes.Clases
{
    public class Materia
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Creditos { get; set; } = 3;

        [ForeignKey("Profesor")]
        public int ProfesorId { get; set; }
        public Profesor Profesor { get; set; }

        public ICollection<EstudianteMateria> EstudiantesMaterias { get; set; }
    }
}
