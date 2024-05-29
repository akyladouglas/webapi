using AtendTeleMedicina.Domain.Entities.Base;
using AtendTeleMedicina.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Interconsulta : EntidadeBase
    {
        #region Propriedades
        public ClassificacaoDeRisco ClassificacaoDeRisco { get; set; }
        public string DiscussaoDoCaso { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAlteracao { get; set; }

        //Adicionando listas de Cid e Ciap
        public List<InterconsultaCid> Cid { get; set; }
        public List<InterconsultaCiap> Ciap { get; set; }
        #endregion

        #region Construtor
        public Interconsulta()
        {

        }
        #endregion
    }
}
