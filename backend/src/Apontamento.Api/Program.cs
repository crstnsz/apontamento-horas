using Apontamento.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

var projetos = new List<ProjetoStore>();
var apontamentos = new List<ApontamentoStore>();

app.MapGet("/api/projetos", () =>
    Results.Ok(projetos.Select(p => p.ToDto())));

app.MapGet("/api/projetos/{id:guid}", (Guid id) =>
{
    var projeto = projetos.FirstOrDefault(p => p.Id == id);
    return projeto is null ? Results.NotFound() : Results.Ok(projeto.ToDto());
});

app.MapPost("/api/projetos", (ProjetoCreateDto input) =>
{
    if (string.IsNullOrWhiteSpace(input.Nome))
    {
        return Results.BadRequest("Nome do projeto é obrigatório.");
    }

    if (input.ValoresHora is null || input.ValoresHora.Count == 0)
    {
        return Results.BadRequest("Informe pelo menos um valor hora.");
    }

    var projeto = new Projeto(input.Nome, input.ValoresHora[0].ValorHora);
    for (var i = 1; i < input.ValoresHora.Count; i++)
    {
        var valor = input.ValoresHora[i];
        projeto.DefinirValorHora(valor.Inicio, valor.Fim, valor.ValorHora);
    }

    var store = new ProjetoStore(Guid.NewGuid(), projeto);
    projetos.Add(store);

    return Results.Created($"/api/projetos/{store.Id}", store.ToDto());
});

app.MapPut("/api/projetos/{id:guid}", (Guid id, ProjetoCreateDto input) =>
{
    var index = projetos.FindIndex(p => p.Id == id);
    if (index < 0)
    {
        return Results.NotFound();
    }

    var projeto = new Projeto(input.Nome, input.ValoresHora[0].ValorHora);
    for (var i = 1; i < input.ValoresHora.Count; i++)
    {
        var valor = input.ValoresHora[i];
        projeto.DefinirValorHora(valor.Inicio, valor.Fim, valor.ValorHora);
    }

    projetos[index] = new ProjetoStore(id, projeto);
    return Results.NoContent();
});

app.MapDelete("/api/projetos/{id:guid}", (Guid id) =>
{
    var removido = projetos.RemoveAll(p => p.Id == id);
    return removido == 0 ? Results.NotFound() : Results.NoContent();
});

app.MapGet("/api/apontamentos", () =>
    Results.Ok(apontamentos.Select(a => a.ToDto())));

app.MapPost("/api/apontamentos", (ApontamentoCreateDto input) =>
{
    var projeto = projetos.FirstOrDefault(p => p.Id == input.ProjetoId);
    if (projeto is null)
    {
        return Results.BadRequest("Projeto não encontrado.");
    }

    var dia = new DiaTrabalhado(input.Data);
    foreach (var periodo in input.Periodos)
    {
        dia.AdicionarPeriodo(periodo.Descricao, projeto.Projeto, periodo.Inicio, periodo.Fim);
    }

    var store = new ApontamentoStore(Guid.NewGuid(), dia, projeto.Id);
    apontamentos.Add(store);

    return Results.Created($"/api/apontamentos/{store.Id}", store.ToDto());
});

app.MapPut("/api/apontamentos/{id:guid}", (Guid id, ApontamentoCreateDto input) =>
{
    var index = apontamentos.FindIndex(a => a.Id == id);
    if (index < 0)
    {
        return Results.NotFound();
    }

    var projeto = projetos.FirstOrDefault(p => p.Id == input.ProjetoId);
    if (projeto is null)
    {
        return Results.BadRequest("Projeto não encontrado.");
    }

    var dia = new DiaTrabalhado(input.Data);
    foreach (var periodo in input.Periodos)
    {
        dia.AdicionarPeriodo(periodo.Descricao, projeto.Projeto, periodo.Inicio, periodo.Fim);
    }

    apontamentos[index] = new ApontamentoStore(id, dia, projeto.Id);
    return Results.NoContent();
});

app.MapDelete("/api/apontamentos/{id:guid}", (Guid id) =>
{
    var removido = apontamentos.RemoveAll(a => a.Id == id);
    return removido == 0 ? Results.NotFound() : Results.NoContent();
});

app.MapGet("/api/consultas", (DateOnly inicio, DateOnly fim) =>
{
    if (fim < inicio)
    {
        return Results.BadRequest("Data fim deve ser maior ou igual à data início.");
    }

    var dias = (fim.ToDateTime(TimeOnly.MinValue) - inicio.ToDateTime(TimeOnly.MinValue)).Days + 1;
    var horasPrevistas = dias * 8.5m;

    var horasRealizadas = apontamentos
        .Where(a => a.Dia.Data >= inicio && a.Dia.Data <= fim)
        .Sum(a => (decimal)a.Dia.TotalHoras.TotalHours);

    return Results.Ok(new ConsultaDto(horasPrevistas, horasRealizadas, horasPrevistas - horasRealizadas));
});

app.MapGet("/api/consultas/export", (DateOnly inicio, DateOnly fim) =>
{
    var dias = (fim.ToDateTime(TimeOnly.MinValue) - inicio.ToDateTime(TimeOnly.MinValue)).Days + 1;
    var horasPrevistas = dias * 8.5m;
    var horasRealizadas = apontamentos
        .Where(a => a.Dia.Data >= inicio && a.Dia.Data <= fim)
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
