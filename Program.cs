using Connectify.Db;
using GachiHubBackend.Extensions;
using GachiHubBackend.Hubs;
using GachiHubBackend.Repositories;
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

var db = new DbConnectifyContext(builder.Configuration);
db.Database.EnsureDeleted();
db.Database.EnsureCreated();

builder.Services.AddDbContext<DbConnectifyContext>();
builder.Services.AddRepositories();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RoomRepository>();
builder.Services.AddScoped<MessagesRepository>();

var app = builder.Build();

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
