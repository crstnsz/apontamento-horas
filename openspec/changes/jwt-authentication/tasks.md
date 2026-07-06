## 1. Implementar Autenticação JWT no Backend (.NET)

- [ ] 1.1 Adicionar pacote `Microsoft.AspNetCore.Authentication.JwtBearer` ao projeto `Apontamento.Api`
- [ ] 1.2 Configurar parâmetros do JWT (Secret Key, Issuer, Audience) no `appsettings.json`
- [ ] 1.3 Registrar e configurar os serviços de Autenticação e Autorização em `Program.cs`
- [ ] 1.4 Adicionar o middleware de Autenticação (`app.UseAuthentication()`) e Autorização (`app.UseAuthorization()`) em `Program.cs`
- [ ] 1.5 Criar o endpoint público `/api/auth/login` em `Program.cs` para validar credenciais e retornar o JWT
- [ ] 1.6 Aplicar `.RequireAuthorization()` a todos os endpoints da API (projetos, apontamentos, consultas) para protegê-los

## 2. Implementar Tela de Login e Gerenciamento de Sessão no Frontend (React)

- [ ] 2.1 Criar a página de login em `frontend/src/src/app/login/page.tsx` com formulário de usuário e senha
- [ ] 2.2 Implementar a função de login que chama o endpoint `/api/auth/login`, grava o token no cookie e redireciona para a home
- [ ] 2.3 Criar botão de Logout no layout que remove o cookie do token e redireciona para `/login`

## 3. Implementar Proteção de Rotas e Requisições no Frontend

- [ ] 3.1 Criar o arquivo `middleware.ts` para interceptar e redirecionar acessos sem cookie para `/login`
- [ ] 3.2 Implementar interceptação de requisições para incluir o header `Authorization: Bearer <token>` em todas as chamadas de API

## 4. Testes e Validação

- [ ] 4.1 Testar chamadas diretas à API sem token e validar o retorno 401 Unauthorized
- [ ] 4.2 Testar tela de login com credenciais válidas e inválidas
- [ ] 4.3 Testar o redirecionamento automático ao tentar acessar páginas protegidas deslogado
- [ ] 4.4 Testar o logout e verificar se a sessão é devidamente encerrada
