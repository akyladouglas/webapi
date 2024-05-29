namespace AtendTeleMedicina.Domain.Entities.Security
{
  public class UserClaim
  {
        public string Id { get; set; }
        public string UsuarioId { get; set; }
        public string IndividuoId { get; set; }
        public string ProfissionalId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}