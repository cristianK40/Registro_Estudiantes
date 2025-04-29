using System.ComponentModel.DataAnnotations;

namespace Registro_Estudiantes.Clases
{
    public class Profesor
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
