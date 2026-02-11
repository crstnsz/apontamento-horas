export default function ConsultasPage() {
  return (
    <section className="mx-auto flex w-full max-w-5xl flex-col gap-6">
      <header className="flex flex-col gap-2">
        <h2 className="text-2xl font-semibold">Consultas</h2>
        <p className="text-sm text-muted-foreground">
          Consulte horas previstas e realizadas por período.
        </p>
      </header>
      <div className="rounded-2xl border bg-white p-6 shadow-sm">
        <h3 className="text-lg font-semibold">Consulta por período</h3>
        <div className="mt-4 grid gap-4 md:grid-cols-3">
          <label className="flex flex-col gap-2 text-sm">
            Data início
            <input type="date" className="h-10 rounded-md border px-3" />
          </label>
          <label className="flex flex-col gap-2 text-sm">
            Data fim
            <input type="date" className="h-10 rounded-md border px-3" />
          </label>
          <div className="flex items-end gap-2">
            <button className="h-10 rounded-md bg-black px-4 text-sm font-medium text-white">
              Consultar
            </button>
            <button className="h-10 rounded-md border px-4 text-sm font-medium">
              Exportar Excel
            </button>
          </div>
        </div>
      </div>

      <div className="grid gap-4 md:grid-cols-3">
        <div className="rounded-2xl border bg-white p-6 shadow-sm">
          <p className="text-sm text-muted-foreground">Horas previstas</p>
          <p className="mt-4 text-2xl font-semibold">0h</p>
          <p className="mt-2 text-xs text-muted-foreground">8.5h/dia</p>
        </div>
        <div className="rounded-2xl border bg-white p-6 shadow-sm">
          <p className="text-sm text-muted-foreground">Horas realizadas</p>
          <p className="mt-4 text-2xl font-semibold">0h</p>
          <p className="mt-2 text-xs text-muted-foreground">Período selecionado</p>
        </div>
        <div className="rounded-2xl border bg-white p-6 shadow-sm">
          <p className="text-sm text-muted-foreground">Diferença</p>
          <p className="mt-4 text-2xl font-semibold">0h</p>
          <p className="mt-2 text-xs text-muted-foreground">Previsto x realizado</p>
        </div>
      </div>
    </section>
  );
}
