using System;
using System.Collections.Generic;
using System.Linq;

namespace Apontamento.Domain;

public class DiaTrabalhado
{
        private readonly List<Periodo> _periodos = new();

        public DiaTrabalhado(DateOnly data)
        {
                if (data == default)
                {
                        throw new ArgumentException("Data do dia trabalhado é obrigatória.", nameof(data));
                }

                Data = data;
        }

        public DateOnly Data {get; private set;}

        public IReadOnlyCollection<Periodo> Periodos => _periodos.AsReadOnly();

        public void AdicionarPeriodo(Periodo periodo)
        {
                ArgumentNullException.ThrowIfNull(periodo, nameof(periodo));
                _periodos.Add(periodo);
        }

        public void AdicionarPeriodo(string descricaoServico, Projeto projeto, TimeOnly inicio, TimeOnly fim)
                => _periodos.Add(new Periodo(descricaoServico, projeto, inicio, fim));

        public TimeSpan TotalHoras => TimeSpan.FromHours(_periodos.Sum(p => p.TotalHoras.TotalHours));

        public decimal ValorTotal => _periodos.Sum(p => p.CalcularValorTotal(Data));
}