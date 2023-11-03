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

        public async Task SendMessage(string username, string message)
        {
            var user = _grains.GetGrain<IUserGrain>(username);

            int? currentRoom = await user.GetRoomIdAsync();

            if (currentRoom != null)
            {
                await Clients.Group(currentRoom.ToString()).SendAsync("ReceiveMessage", user, message);
            }
        }

        public async Task ChangeRoom(string username, int roomId)
        {
            var user = _grains.GetGrain<IUserGrain>(username);

            int? currentRoom = await user.GetRoomIdAsync();

            if (currentRoom != null && currentRoom != roomId)
            {
                string oldRoomId = currentRoom.ToString();

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, oldRoomId);

                await Clients.Group(oldRoomId).SendAsync("ReceiveMessage", $"{username} has left the room!");
            }

            string newRoomId = roomId.ToString();

            await Groups.AddToGroupAsync(Context.ConnectionId, newRoomId);

            await Clients.Group(newRoomId).SendAsync("ReceiveMessage", $"{username} has joined the room!");

            await user.ChangeRoomAsync(roomId);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
