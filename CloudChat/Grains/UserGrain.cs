using Orleans.Runtime;

namespace CloudChat.Grains
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task ChangeRoomAsync(int roomId);
        Task<int?> GetRoomIdAsync();
    }

    public class UserGrain : Grain, IUserGrain
    {
        private readonly UrlDetails _state;
        private readonly IGrainFactory _grains;

        public UserGrain(IGrainFactory grains)
        {
            _state = new UrlDetails
            {
                Username = this.GetPrimaryKeyString(),
                CurrentRoomId = null,
                RoomLoginsStates = new Dictionary<int, int>()
            };

            _grains = grains;
        }

        public Task ChangeRoomAsync(int roomId)
        {
            if (_state.RoomLoginsStates.ContainsKey(roomId))
            {
                _state.RoomLoginsStates[roomId]++;
            }
            else
            {
                _state.RoomLoginsStates[roomId] = 1;
            }

            _state.CurrentRoomId = roomId;

            return Task.CompletedTask;
        }

        public Task<int?> GetRoomIdAsync()
        {
            return Task.FromResult(_state.CurrentRoomId);
        }
    }

    public class UrlDetails
    {
        public string Username { get; set; }

        public int? CurrentRoomId { get; set; }

        public Dictionary<int, int> RoomLoginsStates { get; set; }
    }
}
