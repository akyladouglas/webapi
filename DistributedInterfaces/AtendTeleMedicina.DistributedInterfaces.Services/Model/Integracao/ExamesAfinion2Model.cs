using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using System;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Integracao
{
    public class ExamesAfinion2Model : BaseModel
    {
        #region Propriedades

        public string Id { get; set; }
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


        public virtual IndividuoModel Individuo { get; set; }
        //public virtual TipoExameModel TipoExame { get; set; }
        //public virtual ProfissionalModel Profissional { get; set; }

        #endregion
    }
}
