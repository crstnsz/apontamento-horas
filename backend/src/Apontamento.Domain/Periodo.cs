using System;

namespace Apontamento.Domain;

public class Periodo
{
        public Periodo(string descricaoServico, Projeto projeto, TimeOnly inicio, TimeOnly fim)
        {
                if (!DescricaoServicoValida(descricaoServico))
                {
                        throw new ArgumentException("Descrição do serviço é obrigatória e deve ter até 100 caracteres", nameof(descricaoServico));
                }

                ArgumentNullException.ThrowIfNull(projeto, nameof(projeto));

                DescricaoServico = descricaoServico;
                Projeto = projeto;
                Inicio = new ArredondadorPeriodo(inicio).PrefencialmentePraBaixo();
                Fim = new ArredondadorPeriodo(fim).PrefencialmentePraBaixo();

                if (Fim <= Inicio)
                {
                        throw new ArgumentException("Hora fim deve ser maior que hora início.", nameof(fim));
                }
        }
        
        private static bool DescricaoServicoValida(string descricaoServico)
                => !string.IsNullOrWhiteSpace(descricaoServico) && descricaoServico.Length <= 100;

        public Projeto Projeto { get; private set; }

        public TimeOnly Inicio {get; private set;}

        public TimeOnly Fim  {get; private set;}

        public TimeSpan TotalHoras => Fim - Inicio;

        public decimal CalcularValorTotal(DateOnly data)
                => Convert.ToDecimal(TotalHoras.TotalHours) * Projeto.ObterValorHora(data);

        public decimal CalcularValorTotal(DateTime dataHora)
                => Convert.ToDecimal(TotalHoras.TotalHours) * Projeto.ObterValorHora(dataHora);

        public string DescricaoServico {get; private set;}


}