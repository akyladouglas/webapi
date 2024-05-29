using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class PaisModel : BaseModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        #region Construtores
        public PaisModel()
        {

        }
        #endregion

    }
}
