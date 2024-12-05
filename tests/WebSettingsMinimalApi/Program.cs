using Seekatar.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.InsertSharedDevSettings(reloadOnChange: false, System.Environment.GetEnvironmentVariable("CONFIG_FILE"));

// Add services to the container.

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

app.MapGet("/config/{value}", (string value, IConfiguration config) => {
    return config[value] ?? "";
});

app.MapGet("/config", (IConfiguration configuration) =>
{
    return configuration.AsEnumerable().Where( o => o.Key.StartsWith("In")).OrderBy(o => o.Key).ToDictionary(x => x.Key, x => x.Value);
})
.WithName("GetConfig");

app.MapControllers();

app.Run();

