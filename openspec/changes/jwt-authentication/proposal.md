## Why

O sistema atualmente não possui controle de acesso, permitindo que qualquer pessoa acesse as telas e endpoints da API. A adição de autenticação JWT garante que apenas usuários autenticados possam visualizar e gerenciar os dados de apontamento de horas.

## What Changes

- **Login Screen**: Criação de uma nova tela de login no frontend React
- **Session Management**: Armazenamento do JWT token no frontend e redirecionamento de usuários não autenticados.
- **API Protection**: Proteção de todos os endpoints da API backend do .NET usando middleware JWT.
- **Login Endpoint**: Novo endpoint `/api/auth/login` para validar credenciais e emitir tokens JWT.
- **HTTP client interceptor**: Atualização das requisições do frontend para incluir o header `Authorization: Bearer <token>`.

## Capabilities

### New Capabilities

- `user-authentication`: Fornece login de usuário, geração de token JWT, validação no backend e proteção de rotas no frontend.

### Modified Capabilities

## Impact

- **Backend**: Inclusão de biblioteca de autenticação JWT no `Apontamento.Api`, novos esquemas de configuração de JWT no `appsettings.json`, e adição de políticas de autorização.
- **Frontend**: Criação da página `/login`, alteração no layout principal ou middleware do React para controle de sessão, e injeção do header de autorização nas chamadas à API.
