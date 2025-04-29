using System.Text.Json.Serialization;

namespace Registro_Estudiantes.Clases
{
    public class EstudianteMateria
    {
        public int EstudianteId { get; set; }
        [JsonIgnore]
        public Estudiante? Estudiante { get; set; }
        public int MateriaId { get; set; }
        [JsonIgnore]
        public Materia? Materia { get; set; }
    }
}
