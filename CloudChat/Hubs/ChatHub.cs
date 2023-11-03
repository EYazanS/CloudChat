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
                var builtMessage = await user.BuildMessageAsync(message);

                await Clients.Group(currentRoom.ToString()).SendAsync("ReceiveMessage", builtMessage);

                var room = _grains.GetGrain<IRoomGrain>(currentRoom.Value);

                await room.SaveMessageAsync(builtMessage);
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveError", "Please join a room by typing ROOM and then room number");
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

            var room = _grains.GetGrain<IRoomGrain>(roomId);

            var messages = await room.GetLatestMessagesAsync();

            await Clients.Client(Context.ConnectionId).SendAsync("BulkReceiveMessages", messages);
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
