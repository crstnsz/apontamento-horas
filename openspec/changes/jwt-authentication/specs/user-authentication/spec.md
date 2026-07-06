## ADDED Requirements

### Requirement: Autenticação de Usuário (Login)
O sistema backend SHALL fornecer um endpoint de login `/api/auth/login` que valide as credenciais do usuário (usuário e senha) e retorne um token JWT válido. O sistema frontend SHALL exibir uma tela de login quando o usuário não estiver autenticado e permitir a inserção de credenciais.

#### Scenario: Login com sucesso
- **WHEN** o usuário insere credenciais válidas na tela de login e clica em entrar
- **THEN** o backend retorna o token JWT e o frontend armazena o token e redireciona o usuário para a página inicial

#### Scenario: Login com falha
- **WHEN** o usuário insere credenciais inválidas na tela de login e clica em entrar
- **THEN** o backend retorna erro de não autorizado e o frontend exibe uma mensagem de erro apropriada na tela de login

### Requirement: Proteção de Rotas no Frontend
O sistema frontend SHALL proteger todas as rotas privadas (como `/`, `/projetos`, `/apontamentos` e `/consultas`). Se o usuário tentar acessar essas rotas sem um token JWT válido armazenado, o sistema SHALL redirecioná-lo para a tela de login `/login`.

#### Scenario: Acesso não autenticado a rotas protegidas
- **WHEN** um usuário não autenticado tenta acessar a URL `/projetos`
- **THEN** o frontend intercepta o acesso e redireciona o usuário para `/login`

#### Scenario: Acesso autenticado a rotas protegidas
- **WHEN** um usuário autenticado acessa a URL `/projetos`
- **THEN** o frontend permite o acesso e exibe a tela correspondente

### Requirement: Proteção de Endpoints no Backend
O sistema backend SHALL proteger todos os endpoints da API (sob `/api/projetos`, `/api/apontamentos` e `/api/consultas`), exigindo um token JWT válido no cabeçalho `Authorization` como `Bearer <token>`. O endpoint de login `/api/auth/login` SHALL ser público.

#### Scenario: Chamada de API sem Token
- **WHEN** uma requisição é enviada para `/api/projetos` sem o cabeçalho `Authorization`
- **THEN** o backend retorna status HTTP 401 Unauthorized

#### Scenario: Chamada de API com Token Válido
- **WHEN** uma requisição é enviada para `/api/projetos` com um cabeçalho `Authorization` contendo um token JWT válido
- **THEN** o backend valida o token e processa a requisição retornando status HTTP 200 OK ou outro adequado

### Requirement: Logout de Usuário
O sistema frontend SHALL permitir que o usuário efetue logout através de um botão ou link de saída.

#### Scenario: Logout realizado com sucesso
- **WHEN** o usuário clica no botão de logout
- **THEN** o frontend remove o token JWT armazenado e redireciona o usuário para `/login`
