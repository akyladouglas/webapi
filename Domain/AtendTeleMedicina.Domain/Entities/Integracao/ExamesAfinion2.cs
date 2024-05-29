using AtendTeleMedicina.Domain.Entities.Base;
using System;
using AtendTeleMedicina.Domain.Entities.Nucleo;

namespace AtendTeleMedicina.Domain.Entities.Integracao
{
    public class ExamesAfinion2 : EntidadeBase
    {
        #region Propriedades
        
        public string ControlId { get; set; }
        public string VersionId { get; set; }
        public DateTime CreationDatetime { get; set; }
        public string RoleCode { get; set; }
        public DateTime ObservationDate { get; set; }
        public string StatusCode { get; set; }
        public string ReasonCode { get; set; }
        public int SequenceNumber { get; set; }
        public string PatientId { get; set; }
        public string PatientId2 { get; set; }
        public string PatientId3 { get; set; }
        public string PatientId4 { get; set; }
        public string ObservationId { get; set; }
        public string ExamValue { get; set; }
        public string ExamUnit { get; set; }
        public string ExamMethodCode { get; set; }
        public string ExamStatusCode { get; set; }
        public string OperatorId { get; set; }
        public string ExamName { get; set; }
        public string LotNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Url { get; set; }
        public string Formato { get; set; }

        public virtual Individuo Individuo { get; set; }
        //public virtual TipoExame TipoExame { get; set; }
        // public virtual Profissional Profissional { get; set; }

        #endregion

        #region Construtores
        public ExamesAfinion2()
        {
            //
        }
        #endregion
    }
}