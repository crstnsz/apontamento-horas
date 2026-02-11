namespace Apontamento.Domain;

public record ArredondadorPeriodo(TimeOnly Valor)
{
        public TimeOnly PrefencialmentePraBaixo()
            => (Valor.Minute % 10) switch
            {
                    1 or 2 => Valor.AddMinutes(-(Valor.Minute % 10)),
                    3 or 4 or 6 or 7 => Valor.AddMinutes(-(Valor.Minute % 10)).AddMinutes(5),
                    8 or 9 => Valor.AddMinutes(-(Valor.Minute % 10)).AddMinutes(10),
                    _ => Valor
            };
}