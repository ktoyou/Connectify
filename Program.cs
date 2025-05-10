using Connectify.Db;
using Connectify.Db.Model;
using FluentValidation;
using GachiHubBackend.Extensions;
using GachiHubBackend.Hubs;
using GachiHubBackend.Hubs.Handlers;
using GachiHubBackend.Hubs.Interfaces;
using GachiHubBackend.Repositories;
using GachiHubBackend.Services;
using GachiHubBackend.Services.Interfaces;
using GachiHubBackend.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .RegisterAuthentication(builder.GetJwtConfiguration());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new() { Title = "Connectify API", Version = "v1" });
    opts.AddSignalRSwaggerGen();
});
builder.Services.AddSignalR();
builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowCredentials();
        policy.SetIsOriginAllowed(_ => true);
    });
});

builder.Services.AddHttpClient<IJanusService, JanusService>();
builder.Services.AddScoped<IValidator<User>, UserValidation>();
builder.Services.AddScoped<IValidator<Room>, RoomValidation>();
builder.Services.AddSingleton<AvatarService>();
builder.Services.AddDbContext<DbConnectifyContext>();

builder.Services.AddRepositories();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RoomRepository>();
builder.Services.AddScoped<MessagesRepository>();

builder.Services.AddScoped<IRoomHubContextService, RoomHubContextService>();
builder.Services.AddScoped<IRoomHubConnectedHandler, UserConnectedHandler>();
builder.Services.AddScoped<IRoomHubDisconnectedHandler, UserDisconnectedHandler>();
builder.Services.AddRoomHubHandlers();

builder.Services.AddScoped<IRoomHubHandlerService, RoomHubHandlerService>();

var app = builder.Build();

app.UseCors();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapHub<RoomHub>("/connectify");

app.Run();
