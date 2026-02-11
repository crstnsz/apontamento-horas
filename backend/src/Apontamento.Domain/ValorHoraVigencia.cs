using System;

namespace Apontamento.Domain;

public sealed class ValorHoraVigencia
{
        private static readonly DateTime DataFimAberta = new(9999, 12, 31, 23, 59, 59);

        private ValorHoraVigencia(decimal valorHora, DateTime inicio, DateTime fim)
        {
                ValorHora = valorHora;
                Inicio = inicio;
                Fim = fim;
        }

        public decimal ValorHora { get; }
        public DateTime Inicio { get; }
        public DateTime Fim { get; }

        public static ValorHoraVigencia Criar(decimal valorHora, DateTime inicio, DateTime? fim)
        {
                if (!CustoHoraValido(valorHora))
                {
                        throw new ArgumentException("Custo por hora deve ser positivo e menor que 10000", nameof(valorHora));
                }

                var fimResolvido = fim ?? DataFimAberta;

                if (inicio > fimResolvido)
                {
                        throw new ArgumentException("Data de início deve ser menor ou igual à data fim.", nameof(inicio));
                }

                return new ValorHoraVigencia(valorHora, inicio, fimResolvido);
        }

        public bool HoraFimAberta() => Fim == DataFimAberta;

        public bool Contem(DateTime dataHora)
                => dataHora >= Inicio && dataHora <= Fim;

        private static bool CustoHoraValido(decimal custoHora)
                => custoHora >= 0.01m && custoHora <= 10000m;
}
