"use client";

import { useEffect, useState } from "react";
import {
  createProjeto,
  deleteProjeto,
  getProjetos,
  updateProjeto,
  type ProjetoCreateInput,
  type ProjetoDto,
  type ValorHoraDto,
} from "@/lib/api";
import { DatePickerInput } from "@/components/date-picker-input";

type ValorHoraForm = ValorHoraDto & { tempId: string };

const emptyValorHora = (): ValorHoraForm => ({
  tempId: crypto.randomUUID(),
  inicio: "",
  fim: null,
  valorHora: 0,
});

export default function ProjetosPage() {
  const [projetos, setProjetos] = useState<ProjetoDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [nome, setNome] = useState("");
  const [valoresHora, setValoresHora] = useState<ValorHoraForm[]>([
    emptyValorHora(),
  ]);
  const [erro, setErro] = useState<string | null>(null);

  const carregarProjetos = async () => {
    setLoading(true);
    const lista = await getProjetos();
    setProjetos(lista);
    setLoading(false);
  };

  useEffect(() => {
    carregarProjetos();
  }, []);

  const resetarFormulario = () => {
    setEditingId(null);
    setNome("");
    setValoresHora([emptyValorHora()]);
    setErro(null);
  };

  const onSalvar = async () => {
    setErro(null);
    if (!nome.trim()) {
      setErro("Informe o nome do projeto.");
      return;
    }

    const valoresFiltrados = valoresHora.filter((valor) => valor.inicio && valor.valorHora > 0);
    if (valoresFiltrados.length === 0) {
      setErro("Informe ao menos um valor hora com data de início.");
      return;
    }

    const payload: ProjetoCreateInput = {
      nome,
      valoresHora: valoresFiltrados.map(({ inicio, fim, valorHora }) => ({
        inicio,
        fim: fim || null,
        valorHora,
      })),
    };

    setSaving(true);
    try {
      if (editingId) {
        await updateProjeto(editingId, payload);
      } else {
        await createProjeto(payload);
      }
      await carregarProjetos();
      resetarFormulario();
    } catch (error) {
      setErro(error instanceof Error ? error.message : "Erro ao salvar.");
    } finally {
      setSaving(false);
    }
  };

  const onEditar = (projeto: ProjetoDto) => {
    setEditingId(projeto.id);
    setNome(projeto.nome);
    setValoresHora(
      projeto.valoresHora.map((valor) => ({
        ...valor,
        tempId: crypto.randomUUID(),
      }))
    );
  };

  const onExcluir = async (id: string) => {
    setSaving(true);
    try {
      await deleteProjeto(id);
      await carregarProjetos();
      if (editingId === id) {
        resetarFormulario();
      }
    } finally {
      setSaving(false);
    }
  };

  const atualizarValor = (tempId: string, campo: keyof ValorHoraDto, valor: string) => {
    setValoresHora((prev) =>
      prev.map((item) =>
        item.tempId === tempId
          ? {
              ...item,
              [campo]: campo === "valorHora" ? Number(valor) : valor,
            }
          : item
      )
    );
  };

  const adicionarValorHora = () => {
    setValoresHora((prev) => [...prev, emptyValorHora()]);
  };

  const removerValorHora = (tempId: string) => {
    setValoresHora((prev) => prev.filter((item) => item.tempId !== tempId));
  };

  return (
    <section className="mx-auto flex w-full max-w-5xl flex-col gap-6">
      <header className="flex flex-col gap-2">
        <h2 className="text-2xl font-semibold">Projetos</h2>
        <p className="text-sm text-muted-foreground">
          Cadastre projetos e valores hora com vigência.
        </p>
      </header>
      <div className="rounded-2xl border bg-white p-6 shadow-sm">
        <h3 className="text-lg font-semibold">Novo projeto</h3>
        <div className="mt-4 grid gap-4 md:grid-cols-3">
          <label className="flex flex-col gap-2 text-sm">
            Nome do projeto
            <input
              className="h-10 rounded-md border px-3"
              placeholder="Ex.: Projeto X"
              value={nome}
              onChange={(event) => setNome(event.target.value)}
            />
          </label>
        </div>

        <div className="mt-6">
          <h4 className="text-sm font-semibold">Valores hora</h4>
          <div className="mt-3 flex flex-col gap-3">
            {valoresHora.map((valor) => (
              <div key={valor.tempId} className="grid gap-3 md:grid-cols-4">
                <label className="flex flex-col gap-2 text-sm">
                  Início
                  <DatePickerInput
                    value={valor.inicio}
                    onChange={(novoValor) =>
                      atualizarValor(valor.tempId, "inicio", novoValor)
                    }
                  />
                </label>
                <label className="flex flex-col gap-2 text-sm">
                  Fim (opcional)
                  <DatePickerInput
                    value={valor.fim ?? ""}
                    onChange={(novoValor) =>
                      atualizarValor(valor.tempId, "fim", novoValor)
                    }
                  />
                </label>
                <label className="flex flex-col gap-2 text-sm">
                  Valor hora
                  <input
                    type="number"
                    className="h-10 rounded-md border px-3"
                    placeholder="0,00"
                    value={valor.valorHora || ""}
                    onChange={(event) =>
                      atualizarValor(valor.tempId, "valorHora", event.target.value)
                    }
                  />
                </label>
                <div className="flex items-end">
                  <button
                    className="h-10 rounded-md border px-4 text-sm font-medium"
                    onClick={() => removerValorHora(valor.tempId)}
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
            onClick={adicionarValorHora}
            type="button"
          >
            Adicionar valor hora
          </button>
        </div>

        {erro ? (
          <p className="mt-4 text-sm text-red-600">{erro}</p>
        ) : null}

        <div className="mt-6 flex gap-2">
          <button
            className="rounded-md bg-black px-4 py-2 text-sm font-medium text-white"
            onClick={onSalvar}
            disabled={saving}
          >
            {saving ? "Salvando..." : "Salvar projeto"}
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
          <h3 className="text-lg font-semibold">Projetos cadastrados</h3>
          <input
            className="h-9 rounded-md border px-3 text-sm"
            placeholder="Buscar projeto"
          />
        </div>
        <div className="mt-4 rounded-lg border">
          <div className="grid grid-cols-3 border-b bg-muted/40 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-muted-foreground">
            <span>Projeto</span>
            <span>Valores hora</span>
            <span className="text-right">Ações</span>
          </div>
          {loading ? (
            <div className="px-4 py-3 text-sm text-muted-foreground">
              Carregando...
            </div>
          ) : projetos.length === 0 ? (
            <div className="px-4 py-3 text-sm text-muted-foreground">
              Nenhum projeto cadastrado.
            </div>
          ) : (
            <div className="divide-y">
              {projetos.map((projeto) => (
                <div key={projeto.id} className="grid grid-cols-3 px-4 py-3 text-sm">
                  <span className="font-medium text-foreground">
                    {projeto.nome}
                  </span>
                  <span className="text-muted-foreground">
                    {projeto.valoresHora.length} vigência(s)
                  </span>
                  <div className="flex justify-end gap-2 text-xs">
                    <button
                      className="rounded-md border px-2 py-1"
                      onClick={() => onEditar(projeto)}
                    >
                      Editar
                    </button>
                    <button
                      className="rounded-md border px-2 py-1"
                      onClick={() => onExcluir(projeto.id)}
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
