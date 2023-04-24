using LockboxControl.Core.Models;
using LockBoxControl.Api.Extensions;
using LockboxControl.Storage.Extensions;
using LockboxControl.Storage.Models.Contexts;
using LockboxControl.Storage.Models;
using LockboxControl.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//configure ports:
builder.Services.Configure<PortConfiguration>(builder.Configuration.GetSection(nameof(PortConfiguration)));

builder.Services.AddScoped<RequestManager>();
builder.Services.AddScoped<PortManager>();

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// run migrations
app.MigrateDatabase<DataContext>();

app.Run();
