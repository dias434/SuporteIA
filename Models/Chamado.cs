namespace SuporteIA.Models
{
    public class Chamado
    {
        public int ChamadoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Status { get; set; } = "Aberto";
        public DateTime DataAbertura { get; set; }
        public int UsuarioId { get; set; }
        public string Prioridade { get; set; } = "Média";
    }

    public class Usuario
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}