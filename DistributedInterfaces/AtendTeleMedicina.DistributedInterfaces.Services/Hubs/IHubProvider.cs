using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Hubs
{
    public interface IHubProvider
    {
        Task ReceivedMessage(string message);
        Task ReceivedOpenRoom(string roomId);
        Task ReceivedWaitingForDoctor(string agendamentoId);
        Task ReceivedRefreshTable();
    }

}
