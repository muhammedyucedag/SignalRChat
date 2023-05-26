using Microsoft.AspNetCore.SignalR;
using SignalRChatServerExample.Data;
using SignalRChatServerExample.Models;

namespace SignalRChatServerExample.Hubs
{
    public class ChatHub : Hub
    {
        public async Task GetNickName(string nickName)
        {
            // Verilen nickname ile connectionId birleştirmek için gerekli server kodları yazıldı.
            Client clients = new Client
            {
                ConnectionId = Context.ConnectionId,
                NickName = nickName
            };

            ClientSoruce.Clients.Add(clients);

            //Giriş yapan arkadaşın dışındaki tüm clientlara mesaj göndereceğiz (others)
            await Clients.Others.SendAsync("clientJoined", nickName);

            //Kendisi de dahil herkese mevcut clientlere bilgi göndereceğiz
            await Clients.All.SendAsync("clients", ClientSoruce.Clients);
        }

        //Mesaj gönderme operasyonu
        public async Task SendMessageAsync(string message, string clientName)
        {
            clientName = clientName.Trim();
            Client? senderClient = ClientSoruce.Clients.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            if (clientName == "Tümü")
                await Clients.Others.SendAsync("receiveMessage", message, senderClient.NickName);
            else
            {

                Client? client = ClientSoruce.Clients.FirstOrDefault(c => c.NickName == clientName);
                await Clients.Client(client.ConnectionId).SendAsync("receiveMessage", message, senderClient.NickName);

            }
        }

        //Oda-Grup oluşturma
        //Herhangi bir grup altındaki clientlere erişmek
        public async Task AddGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Group group = new Group { GroupName = groupName };
            group.Clients.Add(ClientSoruce.Clients.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId));
            GroupSource.Groups.Add(group);

            await Clients.All.SendAsync("Groups", GroupSource.Groups);

        }

        //Client'ların odalara/gruplara girmesi
        public async Task AddClientToGroup(IEnumerable<string> groupNames)
        {
            Client? client = ClientSoruce.Clients.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            foreach (var groupName in groupNames)
            {
                Group? _group = GroupSource.Groups.FirstOrDefault(x => x.GroupName == groupName);

                //Bir Clientin bir gruba birden fazla giriş yapmasını engelleme
                var result = _group.Clients.Any(c => c.ConnectionId == Context.ConnectionId);

                if (!result)
                {
                    _group.Clients.Add(client);
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                }
            }
        }

        //Gruptaki tüm clientlere erişme için
        public async Task GetClientToGroup(string groupName)
        {
            Group? group = GroupSource.Groups.FirstOrDefault(x => x.GroupName == groupName);

            //var olan odalara ait cilentlerin listelemesi için 
            await Clients.Caller.SendAsync("Clients", groupName != "-1" ? group.Clients : ClientSoruce.Clients);
        }

        //Seçili grup altındaki tüm client'lara mesja gönderme
        public async Task SendMessageToGroupAsync(string groupName, string message)
        {
            await Clients.OthersInGroup(groupName).SendAsync("receiveMessage", message, ClientSoruce.Clients.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId).NickName);
        }
    }
}
