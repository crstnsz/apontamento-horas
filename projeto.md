#Diretivas de negócio

1. O projeto é um sistema de apontamento de horas. 
2. Eu Informo o dia trabalhado e o projeto, um dia pode ter vários periodos de apontamento com um hora inicio e um hora fim.
3. O Total de hora do período é a hora fim meno a hora inicio.
4. O total de horas de um  dia é a soma das horas do período
5. O Valor Total do período é calculado pegando o Valor hora do período do projeto e multiplicando pela hora do período
6. O Valor Total do dia é a soma do valor total dos períodos
7. Cada projeto pode ter mais de um valor hora. Para achar o valor hora correto a data digitada deve estar no intervalo valido do valor hora do projeto. Se a data fim do valor hora não estiver preenchido ele deve ser a data "9999-12-31 23:59:59" 

#Diretivas Técnicas

1. Uso domínios ricos
2. Mantenha um estado do objeto sempre válido impedindo alterar valor sem consistir.
3. Use Clean Code
4. Use DDD

#Arquitetura de repositórios (atualizado em 24/02/2026)

##Decisão

- Aplicar padrão Repository com interfaces no Core (`Apontamento.Domain`) e implementação concreta em projeto separado de infraestrutura (`Apontamento.Infrastructure.MongoDb`).
- A API (`Apontamento.Api`) não acessa MongoDB diretamente; consome apenas contratos do domínio via injeção de dependência.

##Estrutura criada

- `backend/src/Apontamento.Domain/Repositories`
    - `IProjetoRepository`
    - `IApontamentoRepository`
    - `ProjetoRegistro` e `ApontamentoRegistro` (registros de persistência com Id técnico)
- `backend/src/Apontamento.Infrastructure.MongoDb`
    - `Configuration/MongoDbSettings`
    - `Documents/*` (documentos Mongo)
    - `Mappers/*` (conversão Documento <-> Domínio)
    - `Repositories/*` (implementações Mongo dos contratos)
    - `DependencyInjection.cs` (`AddMongoRepositories`)
- `backend/src/Apontamento.Api`
    - `Program.cs` refatorado para usar repositórios via DI
    - `appsettings.json` com seção `MongoDb`

##Configuração

- Seção adicionada em `appsettings.json`:

```json
"MongoDb": {
    "ConnectionString": "mongodb://192.168.0.67:27017",
    "DatabaseName": "apontamento_horas",
    "ProjetosCollectionName": "projetos",
    "ApontamentosCollectionName": "apontamentos"
}
```

##Resultado técnico

- API deixou de usar listas em memória para Projetos e Apontamentos.
- Endpoints `/api/projetos`, `/api/apontamentos` e `/api/consultas` agora consultam repositórios.
- Persistência Mongo implementada mantendo regras de negócio no domínio rico.
- Testes de domínio executados com sucesso após a refatoração.

##Atualizações realizadas em 24/02/2026

- Connection string da API atualizada para Mongo remoto (`192.168.0.67:27017`).
- Conectividade validada com sucesso via endpoints da API (`/api/projetos` e `/api/apontamentos`).
- Testes de API criados com `WebApplicationFactory` e dublês de repositório em memória.
- Suíte de testes backend executada com sucesso: 58 testes passando.
- UX de datas no front melhorada com date picker de mercado (`react-datepicker` + `date-fns`).
- Campos de data atualizados nas páginas `Projetos`, `Apontamentos` e `Consultas`.

#Plano de implementação das páginas (Front-end)

1. Levantar requisitos das telas (listas, cadastro e apontamento de horas)
    1. A tela  de Projetos é um cadastro simples. Onde eu crio edito e exclui projetos e valores hora
    2. A tela de Apontamento de Horas é o cadastro principal, também um CRUD padrão
    3. Deve permitir exportar para Excel
    4. Deve ter uma consulta por período que calcule horas previstas (8.5h/dia) e realizadas
2. Criar wireframes/protótipos rápidos das telas
    1. Projetos (CRUD)
        - Lista com busca, botão Novo, ações Editar/Excluir
        - Formulário: Nome do projeto
        - Subformulário: Valores hora (Início, Fim opcional, Valor)
    2. Apontamento de Horas (CRUD principal)
        - Lista por dia com total de horas e valor do dia
        - Formulário: Data, Projetos, Períodos (Início/Fim/Descrição)
    3. Consulta por Período
        - Filtros: Data início, Data fim
        - Resultado: Horas previstas (dias * 8.5h), Horas realizadas, Diferença
        - Ação: Exportar para Excel
3. Inicializar front-end em Next.js + TypeScript (Concluído em frontend/src)
4. Configurar Tailwind CSS + shadcn/ui (Concluído)
5. Implementar layout base, navegação e tema (Concluído com menu lateral)
6. Construir páginas principais e formulários (Estrutura base concluída)
7. Integrar com a API/serviços do domínio (API inicial criada)
8. Validar regras e criar testes de UI
9. Publicar (deploy) e ajustes finais

#Status atual (24/02/2026)

- API criada em backend/src/Apontamento.Api com endpoints de Projetos, Apontamentos e Consultas.
- CRUD completo no front para Projetos e Apontamentos (listar, criar, editar, excluir).
- Consultas ainda sem integração com API no front.
- API com repositórios e persistência MongoDB em projeto separado de infraestrutura.
- Solução `.sln` atualizada com o projeto `Apontamento.Infrastructure.MongoDb`.
- Testes de API adicionados com `WebApplicationFactory` e repositórios fake in-memory (sem dependência de Mongo para teste).
- API configurada para Mongo remoto e conexão validada em execução.
- Front-end com date picker mais ergonômico nos principais campos de data.

#Pendências para continuar amanhã

1. Ajustar consulta no front consumindo `/api/consultas` e exportação (`/api/consultas/export`).
2. Revisar erros do `dotnet watch` (usar `dotnet run` ou corrigir task).
3. Limpar pastas vazias criadas dentro de `frontend/src/backend` (se ainda existirem).
4. Configurar `appsettings.Development.json` e variáveis de ambiente para connection string Mongo por ambiente.
5. Melhorar UX restante (mensagens de sucesso/erro, validações de formulário, máscara de moeda e ergonomia dos campos de hora).
6. Adicionar camada Application (casos de uso) para desacoplar regras de orquestração dos endpoints mínimos.
7. Avaliar migração para banco relacional ou versionamento de documentos se houver novas regras de consulta.

#Infraestrutura e DevOps (atualizado em 15/04/2026)

##Decisão

- Adotar Kubernetes para orquestração de containers, permitindo deploy escalável e acessível em intranet.
- Criar Dockerfiles para backend (.NET) e frontend (Next.js) para containerização.
- Configurar manifests YAML para MongoDB (StatefulSet com persistência), backend e frontend, com Services LoadBalancer para exposição externa.

##Estrutura criada

- `devops/`
    - `mongo.yaml`: StatefulSet, PVC (10Gi), ConfigMap e Service para MongoDB.
    - `backend.yaml`: Deployment, ConfigMap e Service LoadBalancer para API .NET.
    - `frontend.yaml`: Deployment e Service LoadBalancer para Next.js.
    - `README.md`: Instruções para deploy e acesso.
- `backend/src/Apontamento.Api/`
    - `Dockerfile`: Multi-stage build para .NET 8.
    - `appsettings.json`: Atualizado com connection string para K8s e Kestrel HTTP na porta 8080.
- `frontend/src/`
    - `Dockerfile`: Multi-stage build para Next.js standalone.
    - `next.config.ts`: Adicionado `output: 'standalone'` para otimização.

##Configuração

- Imagens Docker: `apontamento-api:latest` e `apontamento-frontend:latest` (devem ser built e pushed para registry).
- Acesso: Services LoadBalancer expõem backend (porta 80) e frontend (porta 80) para intranet.
- Persistência: MongoDB com PVC para dados duráveis.

##Resultado técnico

- Aplicação containerizada e orquestrada, pronta para deploy em cluster K8s.
- Configurações ajustadas para variáveis de ambiente e HTTP interno (sem HTTPS forçado).
- Exposição externa via LoadBalancer, acessível de outras máquinas na intranet.

##Atualizações realizadas em 15/04/2026

- Dockerfiles criados para backend e frontend.
- Manifests K8s configurados com ConfigMaps e persistência.
- `appsettings.json` e `next.config.ts` ajustados para K8s.
- Pasta `devops/` adicionada com documentação.

## Build e Deployment das Imagens Docker

### Processo de Build das Imagens

As imagens Docker são construídas localmente para backend e frontend.

- **Backend**: Construído a partir do diretório `backend/`, usando o Dockerfile em `src/Apontamento.Api/`.
- **Frontend**: Construído a partir do diretório `frontend/src/`, usando o Dockerfile na raiz.

Comandos de build:
```bash
# Backend
cd backend
docker build -f src/Apontamento.Api/Dockerfile -t apontamento-api:latest .

# Frontend
cd frontend/src
docker build -t apontamento-frontend:latest .
```

### Processo de Movimentação Local das Imagens

Para implantar em um cluster Kubernetes local (como kind), as imagens construídas localmente precisam ser carregadas no cluster, pois o Kubernetes não acessa diretamente o Docker local.

#### Passos para Movimentar as Imagens Localmente:

1. **Carregar as imagens no cluster kind**:
   ```bash
   kind load docker-image apontamento-api:latest
   kind load docker-image apontamento-frontend:latest
   ```

2. **Configurar os manifests K8s** (`devops/backend.yaml` e `devops/frontend.yaml`):
   - Adicionar `imagePullPolicy: Never` nas especificações dos containers para usar as imagens locais sem tentar puxar de um registry.

3. **Aplicar os manifests**:
   ```bash
   kubectl apply -f devops/backend.yaml -f devops/frontend.yaml
   ```

#### Exemplo Completo:

```bash
# 1. Carregar imagens no kind
kind load docker-image apontamento-api:latest
kind load docker-image apontamento-frontend:latest

# 2. Aplicar manifests (com imagePullPolicy: Never já configurado)
kubectl apply -f devops/backend.yaml -f devops/frontend.yaml
```

Isso resolve o `ImagePullBackOff` em clusters locais como kind, permitindo que os pods usem as imagens carregadas diretamente nos nodes do cluster.

##Últimos procedimentos realizados em 15/04/2026

- Identificamos que o serviço frontend não era exposto corretamente em kind usando `LoadBalancer`.
- Atualizamos `devops/frontend.yaml` para `type: NodePort`, expondo o frontend na porta `31854` no nó.
- Verificamos o serviço com `kubectl get services frontend-service` e confirmamos `80:31854/TCP`.
- Testamos conectividade de rede com `ping 192.168.0.67` e funcionou corretamente.
- Ajustamos o firewall UFW do Linux Mint para liberar a porta `31854`: `sudo ufw allow 31854`.
- Resultado: o frontend deve ficar acessível externamente em `http://192.168.0.67:31854/`.

##Últimos procedimentos realizados em 15/04/2026 (Docker Compose)

- Optamos por uma solução mais simples com Docker Compose puro, evitando complexidades de Kubernetes local.
- Criamos `devops/docker-compose.yml` com três serviços: `mongo`, `backend` e `frontend`.
- Configuramos rede Docker (`apontamento-net`) e volume persistente para MongoDB (`mongo-data`).
- Subimos a aplicação com `docker-compose up -d` no diretório `devops/`.
- Verificamos status com `docker-compose ps`: todos os containers estão `Up`.
- Acesso: Frontend em `http://<ip-da-máquina>:3000`, Backend em `http://<ip-da-máquina>:8080`.
- Atualizamos `devops/README.md` com instruções para ambas as opções (Docker Compose e Kubernetes).

##Próximo passo

- Testar a aplicação completa via navegador em outra máquina da rede.
- Validar CRUD de projetos e apontamentos, consultas e exportação.
- Se necessário, ajustar configurações de rede ou firewall adicional.

##Resumo para o próximo dia

- A aplicação está rodando via Docker Compose e acessível na intranet.
- Próximos testes: funcionalidade completa, performance e possíveis ajustes de UX ou backend.

##Atualizações realizadas em 15/04/2026 (Correção de UUID e Compatibilidade)

###Problema identificado

- Erro "Uncaught TypeError: crypto.randomUUID is not a function" ao acessar páginas do frontend (ex.: /projetos, /apontamentos) de outra máquina na intranet.
- O método `crypto.randomUUID()` não era suportado em todos os contextos ou navegadores, apesar do Firefox 147.0.4 (64 bits) suportar nativamente.

###Solução implementada

- Criada função `generateUUID()` em `frontend/src/src/lib/utils.ts` como alternativa compatível, usando `Math.random()` para gerar UUID v4.
- Substituído todas as ocorrências de `crypto.randomUUID()` por `generateUUID()` nos arquivos:
  - `frontend/src/src/app/apontamentos/page.tsx` (2 usos).
  - `frontend/src/src/app/projetos/page.tsx` (2 usos).
- Adicionados imports de `generateUUID` nos respectivos arquivos.

###Processo de deploy

- Reconstruída imagem Docker do frontend: `docker-compose build frontend`.
- Reiniciados containers: `docker-compose up -d`.
- Frontend atualizado com correção aplicada.

###Resultado técnico

- Erro de UUID resolvido; aplicação agora funciona corretamente em navegadores diversos.
- Função `generateUUID()` garante compatibilidade universal, independente de suporte nativo a `crypto.randomUUID()`.
- Testes de acesso externo confirmados: frontend acessível em `http://192.168.0.67:3000`, backend em `http://192.168.0.67:8080`.

###Correção adicional: URL da API em containers

- Problema identificado: Frontend em container usava URL padrão `http://localhost:5130` em vez da configurada `http://192.168.0.67:8080`, causando erro CORS.
- Causa: Variáveis `NEXT_PUBLIC_*` são definidas em build-time no Next.js; docker-compose define em runtime, mas build não acessa.
- Solução: Alterado fallback em `api.ts` para `http://192.168.0.67:8080`.
- Processo: Rebuild da imagem frontend e restart dos containers.
- Resultado: Frontend agora acessa API corretamente via containers, sem erro CORS.

###Correção de CORS e tratamento de erros

- Problema: Mesmo com CORS configurado, requisições falhavam com 500 e erro "falta cabeçalho 'Access-Control-Allow-Origin'".
- Causa: Ordem dos middlewares incorreta (UseHttpsRedirection antes de UseCors); UseHttpsRedirection redirecionando HTTP para HTTPS não configurado, causando 500; exceções impediam adição de headers CORS.
- Solução: Reordenado middlewares (UseCors antes de UseHttpsRedirection); removido UseHttpsRedirection para ambiente HTTP-only; adicionado try-catch em endpoints para retornar detalhes de erro.
- Processo: Rebuild da imagem backend e restart.
- Resultado: Headers CORS adicionados; redirecionamentos HTTPS removidos; detalhes de falha expostos para debug.

###Implementação de Proxy API no Frontend

- Problema: CORS bloqueando requisições mesmo com configuração correta na API, devido a problemas de ordem de middlewares ou headers não adicionados em respostas 500.
- Solução: Configurar Next.js para proxy de requisições `/api/*` para o backend (`http://backend:8080/api/*`), tornando as chamadas same-origin.
- Alterações: 
  - `api.ts`: Alterado fallback da URL da API para `""` (relativo).
  - `next.config.ts`: Adicionado `rewrites` para proxy de `/api/*` para `http://backend:8080/api/*`.
- Processo: Rebuild da imagem frontend e restart dos containers.
- Resultado: Requisições da API passam pelo frontend, evitando CORS; aplicação acessível sem erros de cross-origin.

###Próximos passos

- Testar aplicação completa de outra máquina; verificar se CRUD e consultas funcionam.
- Monitorar performance e ajustar se necessário.

