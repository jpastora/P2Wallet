var builder = WebApplication.CreateBuilder(args);

// Define un nombre para la política de CORS
var misOrigenes = "misOrigenes";

// Agrega el servicio de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: misOrigenes,
                      policy =>
                      {
                          // Permite solicitudes desde tu frontend local
                          policy.WithOrigins("https://localhost:7059")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(misOrigenes);

app.UseAuthorization();

app.MapControllers();

app.Run();
