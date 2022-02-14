using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Hubs
{
    public class QrCode : Hub
    {
        public async Task CheckQr(string Token)
        {
            await Clients.All.SendAsync("newMessage", "anonymous", Token);
        }

        
    }
}
