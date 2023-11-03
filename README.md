# SignalR Chat Project

### Description

This project demonstrates a simple SignalR chat application. It consists of a server and a client. The server is responsible for managing chat rooms and messages, while the client allows users to join rooms and send messages.

### Prerequisites

Before running the project, make sure you have the following prerequisites installed:

- .NET SDK
- Redis

### Getting Started

1. Clone the repository:

``` bash
git clone https://github.com/yourusername/signalr-chat.git
```

2. Navigate to the project directory:

``` bash
cd signalr-chat
```

3. Server Setup:

- Update the Redis connection string in the appsettings.json file located in the Server directory. Replace your-redis-connection-string with your actual Redis connection string.

``` json
"Redis":  "your-redis-connection-string"
```

3. Build and run the server:

``` bash
cd CloudChat
dotnet build
dotnet run
```

The server will start and listen on a specified port (e.g., https://localhost:7178).

4. Client Setup:

- Navigate to the Client directory:

``` bash
cd ../ClientChat
```

- Run the client:

``` bash
dotnet build
dotnet run
```

The client application will start in the console. Follow the instructions provided by the client application to join chat rooms and send messages.

### Usage

- Open the client app in the console or alternatively open your web browser and navigate to http://localhost:7178 to use the client application in the broswer.
- Use the client to connect to the SignalR server, join chat rooms, and send messages.

### Contributing

Contributions are welcome! Feel free to open issues and pull requests.

### License

This project is licensed under the MIT License - see the LICENSE file for details.