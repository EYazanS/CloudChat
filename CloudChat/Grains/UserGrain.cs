using Orleans.Runtime;

namespace CloudChat.Grains
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task ChangeRoom(int roomId);
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
                CurrentRoom = null
            };

            _grains = grains;
        }

        public Task ChangeRoom(int roomId)
        {
            _state.CurrentRoom = roomId;
            return Task.CompletedTask;
        }
    }

    public class UrlDetails
    {
        public string Username { get; set; }

        public int? CurrentRoom { get; set; }
    }
}
