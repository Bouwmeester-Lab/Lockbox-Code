using LockBoxControl.Api.Extensions;
using LockBoxControl.Storage.Extensions;
using LockBoxControl.Storage.Models.Contexts;
using LockBoxControl.Storage.Models;
using System.Text.Json.Serialization;
using LockBoxControl.Core.Models;
using LockBoxControl.Core.Backend.Services;
using LockBoxControl.Api.Hubs;
//using Microsoft.AspNetCore.Hosting.StaticWebAssets;

var builder = WebApplication.CreateBuilder(args);

//StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:5001/")
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials(); 
    });
});

//configure ports:
builder.Services.Configure<PortConfiguration>(builder.Configuration.GetSection(nameof(PortConfiguration)));

builder.Services.AddScoped<RequestManager>();
builder.Services.AddScoped<PortManager>();
builder.Services.AddScoped<PingManager>();

var opts = builder.Configuration.GetSection(nameof(DatabaseConfigurationOptions)).Get<DatabaseConfigurationOptions>() ?? throw new InvalidOperationException($"Make sure {nameof(DatabaseConfigurationOptions)} is set.");

if (opts.DatabaseType != DatabaseType.SQLite)
{
    // replace the password with the real password stored in secrets
    opts.ConnectionString = opts.ConnectionString.Replace("[DB_PW]", builder.Configuration["DB_PW"]);
}

builder.Services.Configure<DatabaseConfigurationOptions>((options) =>
{
    options.ConnectionString = opts.ConnectionString;
    options.Password = builder.Configuration["DB_PW"] ?? throw new ArgumentNullException("DB_PW");
    options.DatabaseType = opts.DatabaseType;
});



builder.Services.ConfigureStorage(opts);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//builder.Services.AddRazorPages();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<StatusHub>("/Hubs/Status");
app.UseCors();

// run migrations
await app.MigrateDatabaseAsync<DataContext>();

app.Run();
