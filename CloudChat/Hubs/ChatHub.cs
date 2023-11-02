using CloudChat.Grains;

using Microsoft.AspNetCore.SignalR;

namespace CloudChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IGrainFactory _grains;

        public ChatHub(IGrainFactory grains)
        {
            _grains = grains;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task ChangeRoom(string user, int roomId)
        {
            await _grains.GetGrain<IUserGrain>(user).ChangeRoom(roomId);
        }
    }
}
