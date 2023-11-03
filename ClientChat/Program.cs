using Microsoft.AspNetCore.SignalR.Client;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("SignalR Chat Client");

        Console.Write("Enter the SignalR service address (e.g., 'https://your-signalr-service-url/'): ");

        var serviceUrl = Console.ReadLine();

        Console.Write("Enter your username: ");

        var username = Console.ReadLine();

        var connection = new HubConnectionBuilder()
            .WithUrl($"{serviceUrl}chatHub")
            .Build();

        connection.On<string>("ReceiveMessage", (message) =>
        {
            Console.WriteLine($"{message}");
        });

        connection.On<List<string>>("BulkReceiveMessages", (messages) =>
        {
            foreach (var message in messages)
            {
                Console.WriteLine($"{message}");
            }
        });

        connection.On<string>("ReceiveError", (message) =>
        {
            Console.WriteLine($"{message}");
        });

        try
        {
            await connection.StartAsync();

            Console.WriteLine("Connected to the SignalR service.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return;
        }

        await connection.InvokeAsync("ActivateUser", username);

        while (true)
        {
            Console.Write("Enter a command (e.g., 'Room [room number]' or 'Send [message]'): ");

            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid command. Please try again.");
                continue;
            }

            string[] parts = input.Split(' ');
            string command = parts[0].Trim();
            string argument = input.Substring(command.Length).Trim();

            if (command.Equals("Room", StringComparison.OrdinalIgnoreCase))
            {
                // Implement room joining logic
                Console.WriteLine($"Joining room {argument}");

                if (int.TryParse(argument, out int roomId))
                {
                    await connection.InvokeAsync("ChangeRoom", username, roomId);
                }
                else
                {
                    Console.WriteLine("Please enter a valid room number");
                }
            }
            else if (command.Equals("Send", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(argument))
                {
                    // Send the message to the chat room
                    await connection.InvokeAsync("SendMessage", username, argument);
                }
            }
            else
            {
                Console.WriteLine("Invalid command. Please try again.");
            }
        }
    }
}