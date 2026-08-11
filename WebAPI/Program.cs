var builder = WebApplication.CreateBuilder(args);

// Define un nombre para la política de CORS
var misOrigenes = "misOrigenes";

// Agrega el servicio de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: misOrigenes,
        policy =>
        {
            policy.WithOrigins(
                "https://p2wallet-webapp-e9h3c6c8gtdxaeg3.canadacentral-01.azurewebsites.net",
                "https://localhost:7059",
                "https://p2wallet-api-eyefddgeeda9c2fk.eastus-01.azurewebsites.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

// Configurar acceso a datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    DataAccess.DAO.DataAccess.DAO.SqlDAO.Configure(connectionString);
}

// Add MemoryCache and EmailService
builder.Services.AddMemoryCache();
builder.Services.AddScoped<DataAccess.ServicesAccess.EmailService>();
builder.Services.AddScoped<DataAccess.ServicesAccess.SmsService>();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors(misOrigenes);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
