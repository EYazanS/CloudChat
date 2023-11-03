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
        private ConnectionMultiplexer _redis;

        public RoomGrain(IConfiguration configuration)
        {
            string connectionStirng = configuration["Redis"];

            if (string.IsNullOrEmpty(connectionStirng))
            {
                throw new NotImplementedException("Please enter a valid redis connection string");
            }

            _redis = ConnectionMultiplexer.Connect(connectionStirng);
        }

        public async Task<string[]> GetLatestMessagesAsync()
        {
            IDatabase db = _redis.GetDatabase();

            int start = -5;      // Start index (negative index indicates counting from the end)

            int stop = -1;       // Stop index (inclusive)

            // Retrieve the latest 5 items from the end of the list
            RedisValue[] latestItems = await db.ListRangeAsync($"rooms:{this.GetPrimaryKey()}:messages", start, stop);

            return latestItems.Select(value => value.ToString()).ToArray();
        }

        public async Task SaveMessageAsync(string message)
        {
            IDatabase db = _redis.GetDatabase();

            // Push the message to the right end of the list
            await db.ListRightPushAsync($"rooms:{this.GetPrimaryKey()}:messages", message);
        }
    }
}
