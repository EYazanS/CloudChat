using CloudChat.Hubs;

using Orleans.Runtime;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddRazorPages();

builder.Services.AddSignalR();

builder.Host.UseOrleans(siloBuilder => siloBuilder.UseLocalhostClustering());

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapHub<ChatHub>("/chatHub");

app.MapControllers();

app.Run();