namespace Registro_Estudiantes.Clases
{
    public class EstudianteDto
    {
        public int? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public int TotalCreditos { get; set; }
        public List<int>? IdMaterias { get; set; }
        public List<MateriasAux>? Materias { get; set; } = new();
    }
    public class MateriasAux 
    {
        public string? NombreMateria { get; set; }
        public string? Profesor { get; set; }
        public List<string>? Integrantes { get; set; }
    }
}
