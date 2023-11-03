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

        public async Task ActivateUser(string username)
        {
            var user = _grains.GetGrain<IUserGrain>(username);

            int? currentRoom = await user.GetRoomIdAsync();

            if (currentRoom != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, currentRoom.ToString());

                IRoomGrain room = _grains.GetGrain<IRoomGrain>(currentRoom.Value);

                string[] messages = await room.GetLatestMessagesAsync();

                await Clients.Client(Context.ConnectionId).SendAsync("BulkReceiveMessages", messages);
            }
        }

        public async Task SendMessage(string username, string message)
        {
            IUserGrain user = _grains.GetGrain<IUserGrain>(username);

            int? currentRoom = await user.GetRoomIdAsync();

            if (currentRoom != null)
            {
                string builtMessage = await user.BuildMessageAsync(message);

                await Clients.Group(currentRoom.ToString()).SendAsync("ReceiveMessage", builtMessage);

                IRoomGrain room = _grains.GetGrain<IRoomGrain>(currentRoom.Value);

                await room.SaveMessageAsync(builtMessage);
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveError", "Please join a room by typing ROOM and then room number");
            }
        }

        public async Task ChangeRoom(string username, int roomId)
        {
            IUserGrain user = _grains.GetGrain<IUserGrain>(username);

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

            IRoomGrain room = _grains.GetGrain<IRoomGrain>(roomId);

            string[] messages = await room.GetLatestMessagesAsync();

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
