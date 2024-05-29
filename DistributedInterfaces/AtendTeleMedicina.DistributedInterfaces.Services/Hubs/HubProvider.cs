using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Hubs
{
    public class HubProvider : Hub<IHubProvider>
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.ReceivedMessage(message);
        }
        public async Task SendOpenRoom(string roomId)
        {
            await Clients.All.ReceivedOpenRoom(roomId);
        }
        public async Task SendWaitingForDoctor(string agendamentoId)
        {
            await Clients.All.ReceivedWaitingForDoctor(agendamentoId);
        }
        public async Task SendRefreshTable()
        {
            await Clients.All.ReceivedRefreshTable();
        }
    }
}
