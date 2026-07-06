using Apontamento.Domain;
using Apontamento.Domain.Repositories;
using Apontamento.Infrastructure.MongoDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();

//builder.Services.AddOpenApi();
builder.Services.AddMongoRepositories(builder.Configuration);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured.");
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}

app.UseCors("AllowAll");
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/login", (LoginRequest request, IConfiguration configuration) =>
{
    if (request.Usuario == configuration.GetValue<string>("Admin:Username")
     && request.Senha == configuration.GetValue<string>("Admin:Password"))
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured.");
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.Usuario),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Results.Ok(new LoginResponse(tokenString));
    }

    return Results.Unauthorized();
}).AllowAnonymous();

var api = app.MapGroup("").RequireAuthorization();

api.MapGet("/api/projetos", async (IProjetoRepository projetoRepository, CancellationToken cancellationToken) =>
{
    try
    {
        var projetos = await projetoRepository.ListarAsync(cancellationToken);
        return Results.Ok(projetos.Select(p => p.ToDto()));
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

api.MapGet("/api/projetos/{id:guid}", async (Guid id, IProjetoRepository projetoRepository, CancellationToken cancellationToken) =>
{
    var projeto = await projetoRepository.ObterPorIdAsync(id, cancellationToken);
    return projeto is null ? Results.NotFound() : Results.Ok(projeto.ToDto());
});

api.MapPost("/api/projetos", async (ProjetoCreateDto input, IProjetoRepository projetoRepository, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(input.Nome))
    {
        return Results.BadRequest("Nome do projeto é obrigatório.");
    }

    if (input.ValoresHora is null || input.ValoresHora.Count == 0)
    {
        return Results.BadRequest("Informe pelo menos um valor hora.");
    }

    try
    {
        var projeto = new Projeto(input.Nome, input.ValoresHora[0].ValorHora);
        for (var i = 1; i < input.ValoresHora.Count; i++)
        {
            var valor = input.ValoresHora[i];
            projeto.DefinirValorHora(valor.Inicio, valor.Fim, valor.ValorHora);
        }

        var store = new ProjetoRegistro(Guid.NewGuid(), projeto);
        await projetoRepository.AdicionarAsync(store, cancellationToken);

        return Results.Created($"/api/projetos/{store.Id}", store.ToDto());
    }
    catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
    {
        return Results.BadRequest(ex.Message);
    }
});

api.MapPut("/api/projetos/{id:guid}", async (Guid id, ProjetoCreateDto input, IProjetoRepository projetoRepository, CancellationToken cancellationToken) =>
{
    var atual = await projetoRepository.ObterPorIdAsync(id, cancellationToken);
    if (atual is null)
    {
        return Results.NotFound();
    }

    if (string.IsNullOrWhiteSpace(input.Nome))
    {
        return Results.BadRequest("Nome do projeto é obrigatório.");
    }

    if (input.ValoresHora is null || input.ValoresHora.Count == 0)
    {
        return Results.BadRequest("Informe pelo menos um valor hora.");
    }

    try
    {
        var projeto = new Projeto(input.Nome, input.ValoresHora[0].ValorHora);
        for (var i = 1; i < input.ValoresHora.Count; i++)
        {
            var valor = input.ValoresHora[i];
            projeto.DefinirValorHora(valor.Inicio, valor.Fim, valor.ValorHora);
        }

        await projetoRepository.AtualizarAsync(new ProjetoRegistro(id, projeto), cancellationToken);
        return Results.NoContent();
    }
    catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
    {
        return Results.BadRequest(ex.Message);
    }
});

api.MapDelete("/api/projetos/{id:guid}", async (Guid id, IProjetoRepository projetoRepository, CancellationToken cancellationToken) =>
{
    var atual = await projetoRepository.ObterPorIdAsync(id, cancellationToken);
    if (atual is null)
    {
        return Results.NotFound();
    }

    await projetoRepository.RemoverAsync(id, cancellationToken);
    return Results.NoContent();
});

api.MapGet("/api/apontamentos", async (IApontamentoRepository apontamentoRepository, CancellationToken cancellationToken) =>
{
    var apontamentos = await apontamentoRepository.ListarAsync(cancellationToken);
    return Results.Ok(apontamentos.Select(a => a.ToDto()));
});

api.MapPost("/api/apontamentos", async (ApontamentoCreateDto input, IProjetoRepository projetoRepository, IApontamentoRepository apontamentoRepository, CancellationToken cancellationToken) =>
{
    var projeto = await projetoRepository.ObterPorIdAsync(input.ProjetoId, cancellationToken);
    if (projeto is null)
    {
        return Results.BadRequest("Projeto não encontrado.");
    }

    try
    {
        var dia = new DiaTrabalhado(input.Data);
        foreach (var periodo in input.Periodos)
        {
            dia.AdicionarPeriodo(periodo.Descricao, projeto.Projeto, periodo.Inicio, periodo.Fim);
        }

        var store = new ApontamentoRegistro(Guid.NewGuid(), dia, projeto.Id);
        await apontamentoRepository.AdicionarAsync(store, cancellationToken);

        return Results.Created($"/api/apontamentos/{store.Id}", store.ToDto());
    }
    catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
    {
        return Results.BadRequest(ex.Message);
    }
});

api.MapPut("/api/apontamentos/{id:guid}", async (Guid id, ApontamentoCreateDto input, IProjetoRepository projetoRepository, IApontamentoRepository apontamentoRepository, CancellationToken cancellationToken) =>
{
    var atual = await apontamentoRepository.ObterPorIdAsync(id, cancellationToken);
    if (atual is null)
    {
        return Results.NotFound();
    }

    var projeto = await projetoRepository.ObterPorIdAsync(input.ProjetoId, cancellationToken);
    if (projeto is null)
    {
        return Results.BadRequest("Projeto não encontrado.");
    }

    try
    {
        var dia = new DiaTrabalhado(input.Data);
        foreach (var periodo in input.Periodos)
        {
            dia.AdicionarPeriodo(periodo.Descricao, projeto.Projeto, periodo.Inicio, periodo.Fim);
        }

        await apontamentoRepository.AtualizarAsync(new ApontamentoRegistro(id, dia, projeto.Id), cancellationToken);
        return Results.NoContent();
    }
    catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
    {
        return Results.BadRequest(ex.Message);
    }
});

api.MapDelete("/api/apontamentos/{id:guid}", async (Guid id, IApontamentoRepository apontamentoRepository, CancellationToken cancellationToken) =>
{
    var atual = await apontamentoRepository.ObterPorIdAsync(id, cancellationToken);
    if (atual is null)
    {
        return Results.NotFound();
    }

    await apontamentoRepository.RemoverAsync(id, cancellationToken);
    return Results.NoContent();
});

api.MapGet("/api/consultas", async (DateOnly inicio, DateOnly fim, IApontamentoRepository apontamentoRepository, CancellationToken cancellationToken) =>
{
    if (fim < inicio)
    {
        return Results.BadRequest("Data fim deve ser maior ou igual à data início.");
    }

    var apontamentos = await apontamentoRepository.ListarPorPeriodoAsync(inicio, fim, cancellationToken);

    var dias = (fim.ToDateTime(TimeOnly.MinValue) - inicio.ToDateTime(TimeOnly.MinValue)).Days + 1;
    var horasPrevistas = dias * 8.5m;

    var horasRealizadas = apontamentos
        .Sum(a => (decimal)a.Dia.TotalHoras.TotalHours);

    return Results.Ok(new ConsultaDto(horasPrevistas, horasRealizadas, horasPrevistas - horasRealizadas));
});

api.MapGet("/api/consultas/export", async (DateOnly inicio, DateOnly fim, IApontamentoRepository apontamentoRepository, CancellationToken cancellationToken) =>
{
    if (fim < inicio)
    {
        return Results.BadRequest("Data fim deve ser maior ou igual à data início.");
    }

    var apontamentos = await apontamentoRepository.ListarPorPeriodoAsync(inicio, fim, cancellationToken);

    var dias = (fim.ToDateTime(TimeOnly.MinValue) - inicio.ToDateTime(TimeOnly.MinValue)).Days + 1;
    var horasPrevistas = dias * 8.5m;
    var horasRealizadas = apontamentos
        .Sum(a => (decimal)a.Dia.TotalHoras.TotalHours);
    var diferenca = horasPrevistas - horasRealizadas;

    var csv = "Inicio,Fim,HorasPrevistas,HorasRealizadas,Diferenca\n" +
              $"{inicio},{fim},{horasPrevistas},{horasRealizadas},{diferenca}\n";

    return Results.File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "consulta.csv");
});

app.Run();

record ProjetoCreateDto(string Nome, List<ValorHoraDto> ValoresHora);
record ValorHoraDto(DateTime Inicio, DateTime? Fim, decimal ValorHora);
record ProjetoDto(Guid Id, string Nome, List<ValorHoraDto> ValoresHora);

record PeriodoDto(TimeOnly Inicio, TimeOnly Fim, string Descricao);
record ApontamentoCreateDto(DateOnly Data, Guid ProjetoId, List<PeriodoDto> Periodos);
record ApontamentoDto(Guid Id, DateOnly Data, Guid ProjetoId, decimal TotalHoras, decimal ValorTotal, List<PeriodoDto> Periodos);

record ConsultaDto(decimal HorasPrevistas, decimal HorasRealizadas, decimal Diferenca);

public record LoginRequest(string Usuario, string Senha);
public record LoginResponse(string Token);

sealed record ProjetoStore(Guid Id, Projeto Projeto)
{
    public ProjetoDto ToDto()
        => new(Id, Projeto.Nome,
            Projeto.ValoresHora
                .Select(v => new ValorHoraDto(v.Inicio, v.Fim, v.ValorHora))
                .ToList());
}

sealed record ApontamentoStore(Guid Id, DiaTrabalhado Dia, Guid ProjetoId)
{
    public ApontamentoDto ToDto()
        => new(Id, Dia.Data, ProjetoId,
            (decimal)Dia.TotalHoras.TotalHours,
            Dia.ValorTotal,
            Dia.Periodos.Select(p => new PeriodoDto(p.Inicio, p.Fim, p.DescricaoServico)).ToList());
}

static class DtoMappings
{
    public static ProjetoDto ToDto(this ProjetoRegistro registro)
        => new ProjetoStore(registro.Id, registro.Projeto).ToDto();

    public static ApontamentoDto ToDto(this ApontamentoRegistro registro)
        => new ApontamentoStore(registro.Id, registro.Dia, registro.ProjetoId).ToDto();
}

public partial class Program;
