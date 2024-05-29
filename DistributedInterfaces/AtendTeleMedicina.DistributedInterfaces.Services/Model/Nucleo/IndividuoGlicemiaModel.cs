using System;
using System.Collections;
using System.Collections.Generic;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.Domain.Enums;
using AtendTeleMedicina.Domain.Helpers;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class IndividuoGlicemiaModel : BaseModel
    {
        public string Id { get; set; }
        public string IndividuoId { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public bool? RespondeuCafe { get; set; }
        public int? CafeAntes { get; set; }
        public int? CafeDepois { get; set; }
        public bool? RespondeuAlmoco { get; set; }
        public int? AlmocoAntes { get; set; }
        public int? AlmocoDepois { get; set; }
        public bool? RespondeuJanta { get; set; }
        public int? JantaAntes { get; set; }
        public int? JantaDepois { get; set; }
        public bool? RespondeuDormirMadrugada { get; set; }
        public int? AntesDormirMadrugada { get; set; }
        public string Observacoes { get; set; }
        public virtual Individuo Individuo { get; set; }

        #region Construtores
        public IndividuoGlicemiaModel()
        {
        }
        #endregion

        public void ValidarAdicionar()
        {
            
        }

        public void ValidarEditar()
        {
            
        }

        public void ValidarString()
        {
            
        }

        public void ValidarGet()
        {
            ValidarString();
        }

    }
}
