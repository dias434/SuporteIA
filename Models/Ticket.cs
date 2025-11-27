using System;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;

namespace SuporteIA.Models
{
    public class Ticket
    {
        public required string Id { get; set; }
        public required string Numero { get; set; }
        public required string Assunto { get; set; }
        public required string Descricao { get; set; }
        public required string Categoria { get; set; }
        public required string Status { get; set; }
        public required Color StatusColor { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
        public List<Mensagem> Mensagens { get; set; } = new List<Mensagem>();
        public string Prioridade { get; set; } = "Normal";
    }

    public class Mensagem
    {
        public required string Id { get; set; }
        public required string Remetente { get; set; }
        public required string Texto { get; set; }
        public DateTime DataEnvio { get; set; }
        public bool EhUsuario { get; set; }
        public bool EhSuporte => !EhUsuario;
        public LayoutOptions Alinhamento => EhUsuario ? LayoutOptions.End : LayoutOptions.Start;
    }

    // REMOVA ESTA CLASSE USUARIO DAQUI - ela já está no arquivo Usuario.cs separado
}