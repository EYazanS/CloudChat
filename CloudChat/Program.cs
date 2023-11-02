using CloudChat.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddRazorPages();

builder.Services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapHub<ChatHub>("/chatHub");

app.MapControllers();

app.Run();