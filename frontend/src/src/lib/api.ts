export type ValorHoraDto = {
  inicio: string;
  fim: string | null;
  valorHora: number;
};

export type ProjetoDto = {
  id: string;
  nome: string;
  valoresHora: ValorHoraDto[];
};

export type ApontamentoDto = {
  id: string;
  data: string;
  projetoId: string;
  totalHoras: number;
  valorTotal: number;
  periodos: {
    inicio: string;
    fim: string;
    descricao: string;
  }[];
};

export type ProjetoCreateInput = {
  nome: string;
  valoresHora: ValorHoraDto[];
};

export type ApontamentoCreateInput = {
  data: string;
  projetoId: string;
  periodos: {
    inicio: string;
    fim: string;
    descricao: string;
  }[];
};

const baseUrl =
  process.env.API_URL || process.env.NEXT_PUBLIC_API_URL || "http://localhost:5130";

async function safeFetch<T>(path: string): Promise<T> {
  const response = await fetch(`${baseUrl}${path}`, { cache: "no-store" });
  if (!response.ok) {
    throw new Error("Erro ao carregar dados da API.");
  }
  return response.json() as Promise<T>;
}

async function sendJson(path: string, method: "POST" | "PUT" | "DELETE", body?: unknown) {
  const response = await fetch(`${baseUrl}${path}`, {
    method,
    headers: {
      "Content-Type": "application/json",
    },
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || "Erro ao salvar dados.");
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

export async function getProjetos(): Promise<ProjetoDto[]> {
  try {
    return await safeFetch<ProjetoDto[]>("/api/projetos");
  } catch {
    return [];
  }
}

export async function createProjeto(input: ProjetoCreateInput) {
  return sendJson("/api/projetos", "POST", input);
}

export async function updateProjeto(id: string, input: ProjetoCreateInput) {
  return sendJson(`/api/projetos/${id}`, "PUT", input);
}

export async function deleteProjeto(id: string) {
  return sendJson(`/api/projetos/${id}`, "DELETE");
}

export async function getApontamentos(): Promise<ApontamentoDto[]> {
  try {
    return await safeFetch<ApontamentoDto[]>("/api/apontamentos");
  } catch {
    return [];
  }
}

export async function createApontamento(input: ApontamentoCreateInput) {
  return sendJson("/api/apontamentos", "POST", input);
}

export async function updateApontamento(id: string, input: ApontamentoCreateInput) {
  return sendJson(`/api/apontamentos/${id}`, "PUT", input);
}

export async function deleteApontamento(id: string) {
  return sendJson(`/api/apontamentos/${id}`, "DELETE");
}
