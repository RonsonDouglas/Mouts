using SalesApi.Data.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

// Configura o Serilog como provedor de logs
builder.Host.UseSerilog();

// Adiciona os servi�os de controladores da Web API
builder.Services.AddControllers();
builder.Services.AddSingleton<IVendaRepository, VendaRepository>();

// Configura o Swagger para gera��o da documenta��o da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura��o de Middleware para desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
