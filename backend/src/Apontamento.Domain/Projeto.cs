using System;
using System.Collections.Generic;
using System.Linq;

namespace Apontamento.Domain;

public class Projeto
{
        private readonly List<ValorHoraVigencia> _valoresHora = new();

        public Projeto(string nome, decimal custoHora)
        {
                Nome = !NomeValido(nome)
                        ? throw new ArgumentException("Nome do projeto é obrigatório e deve ter até 100 caracteres")
                        : nome;

                DefinirValorHora(DateTime.MinValue, null, custoHora);
        }

        public string Nome { get; private set; }

        public decimal CustoHora => ObterValorHora(DateTime.Now);

        public IReadOnlyCollection<ValorHoraVigencia> ValoresHora => _valoresHora.AsReadOnly();

        public void DefinirValorHora(DateTime inicio, DateTime? fim, decimal valorHora)
        {
                FecharVigenciaAnterior(inicio);
                var vigencia = ValorHoraVigencia.Criar(valorHora, inicio, fim);
                ValidarSobreposicao(vigencia);
                _valoresHora.Add(vigencia);
        }

        public decimal ObterValorHora(DateOnly data)
                => ObterValorHora(data.ToDateTime(TimeOnly.MinValue));

        public decimal ObterValorHora(DateTime dataHora)
        {
                var valor = _valoresHora
                        .OrderByDescending(v => v.Inicio)
                        .FirstOrDefault(v => v.Contem(dataHora));

                if (valor is null)
                {
                        throw new InvalidOperationException("Não existe valor hora válido para a data informada.");
                }

                return valor.ValorHora;
        }

        private void FecharVigenciaAnterior(DateTime novoInicio)
        {
                var vigenciaAnterior = _valoresHora
                        .Where(v => v.Fim > novoInicio
                                && v.HoraFimAberta())
                        .OrderByDescending(v => v.Inicio)
                        .FirstOrDefault();

                if (vigenciaAnterior != null)
                {
                        var fimAjustado = novoInicio.AddSeconds(-1);
                        if (vigenciaAnterior.Inicio >= fimAjustado)
                        {
                                throw new InvalidOperationException("O início da nova vigência é anterior ou igual ao início da vigência atual.");
                        }
                        _valoresHora.Remove(vigenciaAnterior);
                        _valoresHora.Add(ValorHoraVigencia.Criar(vigenciaAnterior.ValorHora, vigenciaAnterior.Inicio, fimAjustado));
                }
        }

        private static bool NomeValido(string nome)
                => !string.IsNullOrWhiteSpace(nome) && nome.Length <= 100;

        private void ValidarSobreposicao(ValorHoraVigencia novo)
        {
                if (_valoresHora.Any(v => IntervalosSobrepostos(v, novo)))
                {
                        throw new InvalidOperationException("Já existe um valor hora cadastrado para o período informado.");
                }
        }

        private static bool IntervalosSobrepostos(ValorHoraVigencia atual, ValorHoraVigencia novo)
                => novo.Inicio <= atual.Fim && novo.Fim >= atual.Inicio;
}