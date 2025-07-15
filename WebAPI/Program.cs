var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Define un nombre para la política de CORS
/*var misOrigenes = "misOrigenes";

// Agrega el servicio de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: misOrigenes,
                      policy =>
                      {
                          // Permite solicitudes desde tu frontend local
                          policy.WithOrigins("https://p2wallet-webapp-e9h3c6c8gtdxaeg3.canadacentral-01.azurewebsites.net/*")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});*/

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowAll");
//app.UseCors("AllowFrontendApp");

// Configure the HTTP request pipeline.
app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();

//app.UseCors(misOrigenes);

app.UseAuthorization();

app.MapControllers();

app.Run();
