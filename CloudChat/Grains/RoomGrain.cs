using StackExchange.Redis;

namespace CloudChat.Grains
{
    public interface IRoomGrain : IGrainWithIntegerKey
    {
        Task<string[]> GetLatestMessagesAsync();
        Task SaveMessageAsync(string message);
    }

    public class RoomGrain : Grain, IRoomGrain
    {
        private string[] _messages;

        private ConnectionMultiplexer _redis;

        public RoomGrain(IConfiguration configuration)
        {
            string connectionStirng = configuration["Redis"];

            if (string.IsNullOrEmpty(connectionStirng))
            {
                throw new NotImplementedException("Please enter a valid redis connection string");
            }

            _redis = ConnectionMultiplexer.Connect(connectionStirng);

            _messages = new string[10];
        }

        public async Task<string[]> GetLatestMessagesAsync()
        {
            IDatabase db = _redis.GetDatabase();

            int start = -10;      // Start index (negative index indicates counting from the end)

            int stop = -1;       // Stop index (inclusive)

            // Retrieve the latest 10 items from the end of the list
            RedisValue[] latestItems = await db.ListRangeAsync(this.GetPrimaryKey().ToString(), start, stop);

            for (int i = 0; i < latestItems.Length; i++)
            {
                _messages[i] = latestItems[i].ToString();
            }

            return _messages;
        }

        public async Task SaveMessageAsync(string message)
        {
            IDatabase db = _redis.GetDatabase();

            // Push the message to the right end of the list
            await db.ListRightPushAsync(this.GetPrimaryKey().ToString(), message);
        }
    }
}
