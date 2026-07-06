## Context

O sistema é composto por uma API backend em .NET C# e um frontend em Next.js (App Router). Atualmente, não há autenticação, e todos os endpoints e páginas estão abertos publicamente. O objetivo é proteger todas as telas e endpoints utilizando autenticação JWT.

## Goals / Non-Goals

**Goals:**
- Proteger todos os endpoints da API (exceto o endpoint de login).
- Proteger todas as rotas do frontend Next.js, redirecionando usuários não autenticados para `/login`.
- Fornecer um mecanismo simples de login e logout no frontend.
- Emitir e validar tokens JWT contendo claims básicas do usuário.

**Non-Goals:**
- Implementar um fluxo complexo de registro de usuário ou recuperação de senha (será utilizada uma conta padrão/administrador configurada ou cadastrada inicialmente).
- Integração com provedores de identidade externos (OAuth2/OIDC).
- Implementação de Refresh Tokens (foco apenas no Access Token JWT).

## Decisions

### 1. Autenticação JWT no Backend (.NET)
- **Escolha**: Usar o pacote oficial `Microsoft.AspNetCore.Authentication.JwtBearer`.
- **Funcionamento**: 
  - Registrar serviços de autenticação e autorização em `Program.cs`.
  - Configurar validação do token JWT com chave de segurança, issuer e audience armazenados no `appsettings.json`.
  - Proteger os endpoints existentes exigindo autorização (`RequireAuthorization()`).
  - Adicionar um endpoint `/api/auth/login` que valide as credenciais recebidas e gere o token JWT.
- **Alternativa Considerada**: Utilizar cookies de sessão clássicos. *Rejeitado* porque o requisito explícito do usuário é autenticação baseada em token JWT.

### 2. Controle de Sessão e Rotas no Frontend (Next.js)
- **Escolha**: Armazenar o token JWT em um Cookie (por exemplo, `auth-token`) e utilizar o Middleware do Next.js para controle de acesso.
- **Razão**: Usar cookies permite que o Middleware do Next.js intercepte as requisições no servidor antes de renderizar a página, evitando o "flash" de conteúdo não autenticado que ocorre quando o controle é feito apenas no lado do cliente (via `localStorage`).
- **Funcionamento**:
  - Tela de login: Envia requisição para `/api/auth/login`, recebe o token, grava no cookie e redireciona para `/`.
  - Middleware do Next.js: Intercepta rotas protegidas e verifica a presença do cookie `auth-token`. Se ausente, redireciona para `/login`.
  - Cliente HTTP (fetch wrapper): Lê o token do cookie (ou passa via contexto) e injeta no cabeçalho `Authorization: Bearer <token>`.
- **Alternativa Considerada**: Armazenar apenas em `localStorage` e validar no cliente (`useEffect`). *Rejeitada* devido à incapacidade do servidor de ler o `localStorage`, prejudicando o SSR (Server-Side Rendering) e a segurança no carregamento inicial da página.

## Risks / Trade-offs

- **Exposição de Chaves de Assinatura JWT**: Chaves fracas ou expostas no código fonte.
  - *Mitigação*: Utilizar chaves fortes e carregá-las via variáveis de ambiente ou segredos de configuração no ambiente de produção.
- **Cookie XSS / CSRF**: O token JWT armazenado em cookie pode ficar vulnerável se não for bem gerenciado.
  - *Mitigação*: O cookie do token será configurado com `SameSite=Strict` e `Secure` (em produção) para reduzir riscos.
