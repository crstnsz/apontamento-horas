"use client";

import { useEffect, useState } from "react";
import {
  createApontamento,
  deleteApontamento,
  getApontamentos,
  getProjetos,
  updateApontamento,
  type ApontamentoCreateInput,
  type ApontamentoDto,
  type ProjetoDto,
} from "@/lib/api";

type PeriodoForm = {
  tempId: string;
  inicio: string;
  fim: string;
  descricao: string;
};

const emptyPeriodo = (): PeriodoForm => ({
  tempId: crypto.randomUUID(),
  inicio: "",
  fim: "",
  descricao: "",
});

const normalizeTime = (value: string) => (value.length === 5 ? `${value}:00` : value);
const toInputTime = (value: string) => value.slice(0, 5);

export default function ApontamentosPage() {
  const [apontamentos, setApontamentos] = useState<ApontamentoDto[]>([]);
  const [projetos, setProjetos] = useState<ProjetoDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [data, setData] = useState("");
  const [projetoId, setProjetoId] = useState("");
  const [periodos, setPeriodos] = useState<PeriodoForm[]>([emptyPeriodo()]);
  const [erro, setErro] = useState<string | null>(null);

  const carregarDados = async () => {
    setLoading(true);
    const [listaApontamentos, listaProjetos] = await Promise.all([
      getApontamentos(),
      getProjetos(),
    ]);
    setApontamentos(listaApontamentos);
    setProjetos(listaProjetos);
    setLoading(false);
  };

  useEffect(() => {
    carregarDados();
  }, []);

  const resetarFormulario = () => {
    setEditingId(null);
    setData("");
    setProjetoId("");
    setPeriodos([emptyPeriodo()]);
    setErro(null);
  };

  const onSalvar = async () => {
    setErro(null);
    if (!data) {
      setErro("Informe a data do apontamento.");
      return;
    }
    if (!projetoId) {
      setErro("Selecione um projeto.");
      return;
    }

    const periodosFiltrados = periodos.filter(
      (periodo) => periodo.inicio && periodo.fim && periodo.descricao.trim()
    );

    if (periodosFiltrados.length === 0) {
      setErro("Informe ao menos um período válido.");
      return;
    }

    const payload: ApontamentoCreateInput = {
      data,
      projetoId,
      periodos: periodosFiltrados.map((periodo) => ({
        inicio: normalizeTime(periodo.inicio),
        fim: normalizeTime(periodo.fim),
        descricao: periodo.descricao,
      })),
    };

    setSaving(true);
    try {
      if (editingId) {
        await updateApontamento(editingId, payload);
      } else {
        await createApontamento(payload);
      }
      await carregarDados();
      resetarFormulario();
    } catch (error) {
      setErro(error instanceof Error ? error.message : "Erro ao salvar.");
    } finally {
      setSaving(false);
    }
  };

  const onEditar = (apontamento: ApontamentoDto) => {
    setEditingId(apontamento.id);
    setData(apontamento.data);
    setProjetoId(apontamento.projetoId);
    setPeriodos(
      apontamento.periodos.map((periodo) => ({
        tempId: crypto.randomUUID(),
        inicio: toInputTime(periodo.inicio),
        fim: toInputTime(periodo.fim),
        descricao: periodo.descricao,
      }))
    );
  };

  const onExcluir = async (id: string) => {
    setSaving(true);
    try {
      await deleteApontamento(id);
      await carregarDados();
      if (editingId === id) {
        resetarFormulario();
      }
    } finally {
      setSaving(false);
    }
  };

  const atualizarPeriodo = (tempId: string, campo: keyof PeriodoForm, valor: string) => {
    setPeriodos((prev) =>
      prev.map((periodo) =>
        periodo.tempId === tempId
          ? {
              ...periodo,
              [campo]: valor,
            }
          : periodo
      )
    );
  };

  const adicionarPeriodo = () => {
    setPeriodos((prev) => [...prev, emptyPeriodo()]);
  };

  const removerPeriodo = (tempId: string) => {
    setPeriodos((prev) => prev.filter((periodo) => periodo.tempId !== tempId));
  };

  return (
    <section className="mx-auto flex w-full max-w-5xl flex-col gap-6">
      <header className="flex flex-col gap-2">
        <h2 className="text-2xl font-semibold">Apontamentos</h2>
        <p className="text-sm text-muted-foreground">
          Cadastre os períodos de trabalho por dia.
        </p>
      </header>
      <div className="rounded-2xl border bg-white p-6 shadow-sm">
        <h3 className="text-lg font-semibold">Novo apontamento</h3>
        <div className="mt-4 grid gap-4 md:grid-cols-3">
          <label className="flex flex-col gap-2 text-sm">
            Data
            <input
              type="date"
              className="h-10 rounded-md border px-3"
              value={data}
              onChange={(event) => setData(event.target.value)}
            />
          </label>
          <label className="flex flex-col gap-2 text-sm">
            Projeto
            <select
              className="h-10 rounded-md border px-3"
              value={projetoId}
              onChange={(event) => setProjetoId(event.target.value)}
            >
              <option value="">Selecione</option>
              {projetos.map((projeto) => (
                <option key={projeto.id} value={projeto.id}>
                  {projeto.nome}
                </option>
              ))}
            </select>
          </label>
        </div>

        <div className="mt-6">
          <h4 className="text-sm font-semibold">Períodos</h4>
          <div className="mt-3 flex flex-col gap-3">
            {periodos.map((periodo) => (
              <div key={periodo.tempId} className="grid gap-3 md:grid-cols-5">
                <label className="flex flex-col gap-2 text-sm">
                  Início
                  <input
                    type="time"
                    className="h-10 rounded-md border px-3"
                    value={periodo.inicio}
                    onChange={(event) =>
                      atualizarPeriodo(periodo.tempId, "inicio", event.target.value)
                    }
                  />
                </label>
                <label className="flex flex-col gap-2 text-sm">
                  Fim
                  <input
                    type="time"
                    className="h-10 rounded-md border px-3"
                    value={periodo.fim}
                    onChange={(event) =>
                      atualizarPeriodo(periodo.tempId, "fim", event.target.value)
                    }
                  />
                </label>
                <label className="md:col-span-2 flex flex-col gap-2 text-sm">
                  Descrição
                  <input
                    className="h-10 rounded-md border px-3"
                    placeholder="Ex.: Desenvolvimento"
                    value={periodo.descricao}
                    onChange={(event) =>
                      atualizarPeriodo(periodo.tempId, "descricao", event.target.value)
                    }
                  />
                </label>
                <div className="flex items-end">
                  <button
                    className="h-10 rounded-md border px-4 text-sm font-medium"
                    onClick={() => removerPeriodo(periodo.tempId)}
                    type="button"
                  >
                    Remover
                  </button>
                </div>
              </div>
            ))}
          </div>
          <button
            className="mt-4 rounded-md border px-4 py-2 text-sm font-medium"
            onClick={adicionarPeriodo}
            type="button"
          >
            Adicionar período
          </button>
        </div>

        {erro ? <p className="mt-4 text-sm text-red-600">{erro}</p> : null}

        <div className="mt-6 flex gap-2">
          <button
            className="rounded-md bg-black px-4 py-2 text-sm font-medium text-white"
            onClick={onSalvar}
            disabled={saving}
          >
            {saving ? "Salvando..." : "Salvar apontamento"}
          </button>
          <button
            className="rounded-md border px-4 py-2 text-sm font-medium"
            onClick={resetarFormulario}
            type="button"
          >
            Limpar
          </button>
        </div>
      </div>

      <div className="rounded-2xl border bg-white p-6 shadow-sm">
        <div className="flex items-center justify-between">
          <h3 className="text-lg font-semibold">Apontamentos cadastrados</h3>
          <input
            className="h-9 rounded-md border px-3 text-sm"
            placeholder="Buscar por data"
          />
        </div>
        <div className="mt-4 rounded-lg border">
          <div className="grid grid-cols-4 border-b bg-muted/40 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-muted-foreground">
            <span>Data</span>
            <span>Horas</span>
            <span>Valor</span>
            <span className="text-right">Ações</span>
          </div>
          {loading ? (
            <div className="px-4 py-3 text-sm text-muted-foreground">
              Carregando...
            </div>
          ) : apontamentos.length === 0 ? (
            <div className="px-4 py-3 text-sm text-muted-foreground">
              Nenhum apontamento cadastrado.
            </div>
          ) : (
            <div className="divide-y">
              {apontamentos.map((apontamento) => (
                <div
                  key={apontamento.id}
                  className="grid grid-cols-4 px-4 py-3 text-sm"
                >
                  <span className="font-medium text-foreground">
                    {apontamento.data}
                  </span>
                  <span className="text-muted-foreground">
                    {apontamento.totalHoras.toFixed(2)}h
                  </span>
                  <span className="text-muted-foreground">
                    R$ {apontamento.valorTotal.toFixed(2)}
                  </span>
                  <div className="flex justify-end gap-2 text-xs">
                    <button
                      className="rounded-md border px-2 py-1"
                      onClick={() => onEditar(apontamento)}
                    >
                      Editar
                    </button>
                    <button
                      className="rounded-md border px-2 py-1"
                      onClick={() => onExcluir(apontamento.id)}
                    >
                      Excluir
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </section>
  );
}
