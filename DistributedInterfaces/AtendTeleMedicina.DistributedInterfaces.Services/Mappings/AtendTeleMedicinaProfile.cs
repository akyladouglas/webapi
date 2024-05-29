using System.Collections.Generic;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Integracao;
using AutoMapper;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.Domain.Entities.Dashboard;
using AtendTeleMedicina.Domain.Entities.Integracao;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Mappings
{
    public class AtendTeleMedicinaProfile : Profile
    {
        public AtendTeleMedicinaProfile()
        {
            //Nucleo
            CreateMap<Bairro, BairroModel>().ReverseMap();
            CreateMap<Cidade, CidadeModel>().ReverseMap();
            CreateMap<UnidadeFederativa, UnidadeFederativaModel>().ReverseMap();
            CreateMap<Configuracao, ConfiguracaoModel>().ReverseMap();
            CreateMap<DashboardParams, DashboardParamsModel>().ReverseMap();
            CreateMap<Procedimento, ProcedimentoModel>().ReverseMap();
            CreateMap<Estabelecimento, EstabelecimentoModel>().ReverseMap();
            CreateMap<EstabelecimentoProcedimento, EstabelecimentoProcedimentoModel>().ReverseMap();
            CreateMap<EstabelecimentoProcedimentoHorario, EstabelecimentoProcedimentoHorarioModel>().ReverseMap();
            CreateMap<Individuo, IndividuoModel>().ReverseMap();
            CreateMap<IndividuoProcedimentoAutorizacao, IndividuoProcedimentoAutorizacaoModel>().ReverseMap();
            CreateMap<Notificacao, NotificacaoModel>().ReverseMap();
            CreateMap<Customizacao, CustomizacaoModel>().ReverseMap();
            CreateMap<Acompanhamento, AcompanhamentoModel>().ReverseMap();
            CreateMap<InformacoesUteis, InformacoesUteisModel>().ReverseMap();
            CreateMap<Profissional, ProfissionalModel>().ReverseMap();
            CreateMap<Agendamento, AgendamentoModel>().ReverseMap();
            CreateMap<AgendamentoParticipante, AgendamentoParticipanteModel>().ReverseMap();
            CreateMap<IndividuoGlicemia, IndividuoGlicemiaModel>().ReverseMap();
            CreateMap<Documento, DocumentoModel>().ReverseMap();
            CreateMap<Auditoria, AuditoriaModel>().ReverseMap();
            CreateMap<Exames, ExamesModel>().ReverseMap();
            CreateMap<TipoExame, TipoExameModel>().ReverseMap();
            CreateMap<ExamesF200, ExamesF200Model>().ReverseMap();
            CreateMap<ExamesAfinion2, ExamesAfinion2Model>().ReverseMap();
            CreateMap<IndividuoNotificacao, IndividuoNotificacaoModel>().ReverseMap();
            CreateMap<Pais, PaisModel>().ReverseMap();
            CreateMap<Interconsulta, InterconsultaModel>().ReverseMap();
            CreateMap<InterconsultaCid, InterconsultaCidModel>().ReverseMap();
            CreateMap<InterconsultaCiap, InterconsultaCiapModel>().ReverseMap();
            //Security
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<Profissional, UserModel>().ReverseMap();
            CreateMap<Satisfacao, SatisfacaoModel>().ReverseMap();
            CreateMap<UserClaim, UserClaimModel>().ReverseMap();
            CreateMap<EstabelecimentoProfissional, EstabelecimentoProfissionalModel>().ReverseMap();
            CreateMap<Cid, CidModel>().ReverseMap();
            CreateMap<Ciap, CiapModel>().ReverseMap();
            CreateMap<Atendimento, AtendimentoModel>().ReverseMap();
            CreateMap<IndividuoCiap, IndividuoCiapModel>().ReverseMap();
            CreateMap<IndividuoCid10, IndividuoCid10Model>().ReverseMap();
            CreateMap<IndividuoProcedimento, IndividuoProcedimentoModel>().ReverseMap();
            CreateMap<Lote, LoteModel>().ReverseMap();
            CreateMap<Ocupacao, OcupacaoModel>().ReverseMap();
            CreateMap<Avaliacao, AvaliacaoModel>().ReverseMap();

        }
    }
}
