export default function Home() {
  return (
    <section className="mx-auto flex w-full max-w-5xl flex-col gap-8">
      <div className="rounded-2xl border bg-white p-8 shadow-sm">
        <p className="text-sm uppercase tracking-wide text-muted-foreground">
          Visão geral
        </p>
        <h2 className="mt-2 text-3xl font-semibold">Bem-vindo ao Apontamento</h2>
        <p className="mt-3 max-w-2xl text-sm text-muted-foreground">
          Registre horas trabalhadas, acompanhe o valor diário e exporte para
          Excel. Use o menu lateral para acessar Projetos, Apontamentos e
          Consultas.
        </p>
      </div>

      <div className="grid gap-4 md:grid-cols-3">
        <div className="rounded-2xl border bg-white p-6 shadow-sm">
          <p className="text-sm text-muted-foreground">Horas previstas</p>
          <p className="mt-4 text-2xl font-semibold">0h</p>
          <p className="mt-2 text-xs text-muted-foreground">
            Baseado em 8.5h/dia no período
          </p>
        </div>
        <div className="rounded-2xl border bg-white p-6 shadow-sm">
          <p className="text-sm text-muted-foreground">Horas realizadas</p>
          <p className="mt-4 text-2xl font-semibold">0h</p>
          <p className="mt-2 text-xs text-muted-foreground">
            Total de apontamentos no período
          </p>
        </div>
        <div className="rounded-2xl border bg-white p-6 shadow-sm">
          <p className="text-sm text-muted-foreground">Diferença</p>
          <p className="mt-4 text-2xl font-semibold">0h</p>
          <p className="mt-2 text-xs text-muted-foreground">
            Previsto vs realizado
          </p>
        </div>
      </div>
    </section>
  );
}
