using Seekatar.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.InsertSharedDevSettings();

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

app.MapControllers();

app.Run();
