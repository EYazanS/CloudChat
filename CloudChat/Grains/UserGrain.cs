using System.Text;

using Newtonsoft.Json;

using StackExchange.Redis;

namespace CloudChat.Grains
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task<string> BuildMessageAsync(string message);
        Task ChangeRoomAsync(int roomId);
        Task<int?> GetRoomIdAsync();
    }

    public class UserGrain : Grain, IUserGrain
    {
        private UrlDetails _state;
        private readonly IGrainFactory _grains;
        private ConnectionMultiplexer _redis;

        public UserGrain(
            IGrainFactory grains,
            IConfiguration configuration
        )
        {
            _state = new UrlDetails
            {
                Username = this.GetPrimaryKeyString(),
                CurrentRoomId = null,
                RoomLoginsStates = new Dictionary<int, int>()
            };

            _grains = grains;

            string connectionStirng = configuration["Redis"];

            if (string.IsNullOrEmpty(connectionStirng))
            {
                throw new NotImplementedException("Please enter a valid redis connection string");
            }

            _redis = ConnectionMultiplexer.Connect(connectionStirng);

            var db = _redis.GetDatabase();

            string json = db.StringGet($"users:{_state.Username}:logins");

            if (!string.IsNullOrEmpty(json))
            {
                _state.RoomLoginsStates = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);
            }

            string room = db.StringGet($"users:{_state.Username}:room");

            if (!string.IsNullOrEmpty(room))
            {
                _state.CurrentRoomId = int.Parse(room);
            }
        }

        public Task<string> BuildMessageAsync(string message)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(_state.Username);
            stringBuilder.Append(" - Logins:");
            stringBuilder.Append(_state.RoomLoginsStates[_state.CurrentRoomId.Value]);
            stringBuilder.Append(" - ");
            stringBuilder.Append(message);

            return Task.FromResult(stringBuilder.ToString());
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

            string json = JsonConvert.SerializeObject(_state.RoomLoginsStates);

            IDatabase db = _redis.GetDatabase();

            db.StringSet($"users:{_state.Username}:logins", json);
            db.StringSet($"users:{_state.Username}:room", roomId);

            return Task.CompletedTask;
        }

        public Task<int?> GetRoomIdAsync()
        {
            return Task.FromResult(_state.CurrentRoomId);
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            IDatabase db = _redis.GetDatabase();

            _state = new()
            {
                Username = this.GetPrimaryKeyString(),
                CurrentRoomId = null,
                RoomLoginsStates = new Dictionary<int, int>()
            };

            string json = db.StringGet($"users:{_state.Username}:logins");

            if (!string.IsNullOrEmpty(json))
            {
                _state.RoomLoginsStates = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);
            }

            string room = db.StringGet($"users:{_state.Username}:room");

            if (!string.IsNullOrEmpty(json))
            {
                _state.CurrentRoomId = int.Parse(room);
            }

            return base.OnActivateAsync(cancellationToken);
        }
    }

    public class UrlDetails
    {
        public string Username { get; set; }

        public int? CurrentRoomId { get; set; }

        public Dictionary<int, int> RoomLoginsStates { get; set; }
    }
}
