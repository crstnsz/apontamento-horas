#Diretivas de negócio

1. O projeto é um sistema de apontamento de horas. 
2. Eu Informo o dia trabalhado e o projeto, um dia pode ter vários periodos de apontamento com um hora inicio e um hora fim.
3. O Total de hora do período é a hora fim meno a hora inicio.
4. O total de horas de um  dia é a soma das horas do período
5. O Valor Total do período é calculado pegando o Valor hora do período do projeto e multiplicando pela hora do período
6. O Valor Total do dia é a soma do valor total dos períodos
7. Cada projeto pode ter mais de um valor hora. Para achar o valor hora correto a data digitada deve estar no intervalo valido do valor hora do projeto. Se a data fim do valor hora não estiver preenchido ele deve ser a data "9999-21-31 23:59:59" 

#Diretivas Técnicas

1. Uso domínios ricos
2. Mantenha um estado do objeto sempre válido impedindo alterar valor sem consistir.
3. Use Clean Code
4. Use DDD

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

#Status atual (10/02/2026)

- API criada em backend/src/Apontamento.Api com endpoints de Projetos, Apontamentos e Consultas.
- CRUD completo no front para Projetos e Apontamentos (listar, criar, editar, excluir).
- Consultas ainda sem integração com API no front.
- API em memória (sem banco).

#Pendências para continuar amanhã

1. Rodar o front-end (npm run dev em frontend/src).
2. Ajustar consulta no front consumindo /api/consultas e exportação (/api/consultas/export).
3. Revisar erros do dotnet watch (usar dotnet run ou corrigir task).
4. Limpar pastas vazias criadas dentro de frontend/src/backend (se ainda existirem).
5. Adicionar persistência (banco) se necessário.
6. Melhorar UX (mensagens de sucesso/erro, validações de formulário, máscara de moeda).

