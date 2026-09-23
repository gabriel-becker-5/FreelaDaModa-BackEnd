using _04_Domain.Entities;

namespace _04_Domain.Entities
{
    public class Notificacao : BaseEntity
    {
        public int UsuarioId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public bool Lida { get; set; } = false;
    }
}