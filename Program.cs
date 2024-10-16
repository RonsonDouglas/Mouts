using SalesApi.Data.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

// Configura o Serilog como provedor de logs
builder.Host.UseSerilog();

// Adiciona os serviços de controladores da Web API
builder.Services.AddControllers();
builder.Services.AddSingleton<IVendaRepository, VendaRepository>();

// Configura o Swagger para geração da documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração de Middleware para desenvolvimento
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
