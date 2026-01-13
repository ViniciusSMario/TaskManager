# Task Manager API

API RESTful para gerenciamento de tarefas desenvolvida em C# com ASP.NET Core, seguindo os princípios de Clean Code e arquitetura em camadas.

## 🚀 Tecnologias Utilizadas

### Backend
- **ASP.NET Core Web API** (.NET 10.0)
- **Entity Framework Core** (Persistência de dados)
- **SQL Server** (Banco de dados)
- **Swagger/OpenAPI** (Documentação da API)
- **xUnit, Moq, FluentAssertions** (Testes automatizados)

### Frontend
- **JavaScript Vanilla** (ES6+)
- **Vite** (Build tool e dev server)
- **CSS3** (Estilização responsiva)

## 📋 Funcionalidades

A API implementa operações CRUD completas para gerenciamento de tarefas:

- ✅ **Criar** uma nova tarefa
- 📖 **Ler** todas as tarefas ou uma tarefa específica pelo Id
- ✏️ **Atualizar** uma tarefa existente
- ❌ **Deletar** uma tarefa

## 📊 Modelo de Dados

Cada tarefa possui os seguintes campos:

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | int | Identificador único (auto incrementado) |
| `Titulo` | string | Título da tarefa (obrigatório, máx. 100 caracteres) |
| `Descricao` | string | Descrição detalhada (opcional) |
| `DataCriacao` | DateTime | Data de criação (gerada automaticamente) |
| `DataConclusao` | DateTime? | Data de conclusão (opcional) |
| `Status` | enum | Status da tarefa: `Pendente`, `EmProgresso`, `Concluida` |

### Validações

- ✔️ O campo `Titulo` é obrigatório e deve ter no máximo 100 caracteres
- ✔️ O campo `Titulo` deve ser único
- ✔️ A `DataConclusao` não pode ser anterior à `DataCriacao`
- ✔️ Retorna status HTTP apropriados (400 para erros de validação, 404 para não encontrado, 500 para erros de servidor)

## 🏗️ Arquitetura

O projeto segue a arquitetura em camadas seguindo os princípios SOLID:

```
TaskManager/
├── TaskManager.Domain/          # Entidades e regras de negócio
│   └── TaskEntity.cs
├── TaskManager.Application/     # Interfaces e contratos
│   ├── ITaskRepository.cs
│   └── ITaskService.cs
├── TaskManager.Infrastructure/  # Implementações e acesso a dados
│   ├── TaskDbContext.cs
│   ├── TaskRepository.cs
│   └── TaskService.cs
└── TaskManager.API/             # Camada de apresentação (Controllers)
    ├── Controllers/
    │   └── TasksController.cs
    └── Program.cs
```

### Princípios Aplicados

- **SRP (Single Responsibility Principle)**: Cada classe tem uma única responsabilidade
- **DIP (Dependency Inversion Principle)**: Dependências por abstrações (interfaces)
- **Clean Code**: Nomes claros, métodos pequenos e focados
- **Separation of Concerns**: Separação clara entre camadas

## ⚙️ Configuração do Ambiente

### Pré-requisitos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB, Express ou instância completa)
- [Node.js](https://nodejs.org/) (v18 ou superior) - para o frontend
- Editor de código (Visual Studio, VS Code)

### Instalação do Backend

1. **Clone o repositório**
```bash
git clone https://github.com/ViniciusSMario/TaskManager.git
cd TaskManager
```

2. **Configure a Connection String**

Edite o arquivo `TaskManager.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TaskManagerDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

> **Nota**: Esta connection string está configurada para SQL Server Express.

3. **Execute as Migrations**

No diretório do projeto, execute:

```bash
cd TaskManager.API
dotnet ef database update --project ..\TaskManager.Infrastructure
```

Isso criará o banco de dados `TaskManagerDb` e a tabela `Tasks`.

4. **Execute a API**

```bash
dotnet run --project TaskManager.API
```

A API estará disponível em:
- HTTP: `http://localhost:5194`
- Swagger: `http://localhost:5194/swagger`

### Instalação do Frontend

1. **Navegue até o diretório do frontend**
```bash
cd task-manager-frontend
```

2. **Instale as dependências**
```bash
npm install
```

3. **Execute o servidor de desenvolvimento**
```bash
npm run dev
```

O frontend estará disponível em:
- `http://localhost:5173` (ou outra porta se 5173 estiver ocupada)

### Executando Backend e Frontend Juntos

Para executar a aplicação completa, você precisa de **dois terminais**:

**Terminal 1 - Backend:**
```bash
cd TaskManager.API
dotnet run
```

**Terminal 2 - Frontend:**
```bash
cd task-manager-frontend
npm run dev
```

Acesse `http://localhost:5173` no navegador para usar a aplicação!

## 📚 Documentação da API

Após iniciar a aplicação, acesse o Swagger UI para testar os endpoints:

```
https://localhost:5194/swagger
```

### Endpoints Disponíveis

#### GET /api/tasks
Retorna todas as tarefas

**Resposta de Sucesso (200 OK):**
```json
[
  {
    "id": 1,
    "titulo": "Implementar API",
    "descricao": "Desenvolver API de gerenciamento de tarefas",
    "dataCriacao": "2026-01-13T10:00:00",
    "dataConclusao": null,
    "status": 1
  }
]
```

#### GET /api/tasks/{id}
Retorna uma tarefa específica pelo Id

**Resposta de Sucesso (200 OK):**
```json
{
  "id": 1,
  "titulo": "Implementar API",
  "descricao": "Desenvolver API de gerenciamento de tarefas",
  "dataCriacao": "2026-01-13T10:00:00",
  "dataConclusao": null,
  "status": 1
}
```

**Erro (404 Not Found):** Tarefa não encontrada

#### POST /api/tasks
Cria uma nova tarefa

**Request Body:**
```json
{
  "titulo": "Nova Tarefa",
  "descricao": "Descrição da tarefa",
  "status": 0,
  "dataConclusao": null
}
```

**Resposta de Sucesso (201 Created):**
```json
{
  "id": 2,
  "titulo": "Nova Tarefa",
  "descricao": "Descrição da tarefa",
  "dataCriacao": "2026-01-13T10:30:00",
  "dataConclusao": null,
  "status": 0
}
```

**Erro (400 Bad Request):** Dados de validação inválidos

#### PUT /api/tasks/{id}
Atualiza uma tarefa existente

**Request Body:**
```json
{
  "titulo": "Tarefa Atualizada",
  "descricao": "Nova descrição",
  "status": 2,
  "dataConclusao": "2026-01-13T15:00:00"
}
```

**Resposta de Sucesso (204 No Content)**

**Erro (404 Not Found):** Tarefa não encontrada
**Erro (400 Bad Request):** Dados de validação inválidos

#### DELETE /api/tasks/{id}
Deleta uma tarefa

**Resposta de Sucesso (204 No Content)**

**Erro (404 Not Found):** Tarefa não encontrada

### Status da Tarefa (Enum)

```
0 = Pendente
1 = EmProgresso
2 = Concluida
```

## 🧪 Testando a API

### Usando cURL

**Criar uma tarefa:**
```bash
curl -X POST "https://localhost:5194/api/tasks" \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "Minha Primeira Tarefa",
    "descricao": "Testar a API",
    "status": 0
  }'
```

**Listar todas as tarefas:**
```bash
curl -X GET "https://localhost:5194/api/tasks"
```

**Obter tarefa por Id:**
```bash
curl -X GET "https://localhost:5194/api/tasks/1"
```

**Atualizar uma tarefa:**
```bash
curl -X PUT "https://localhost:5194/api/tasks/1" \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "Tarefa Atualizada",
    "status": 1
  }'
```

**Deletar uma tarefa:**
```bash
curl -X DELETE "https://localhost:5194/api/tasks/1"
```

## 🧪 Testes Automatizados

O projeto inclui uma suíte completa de testes automatizados cobrindo todas as camadas da aplicação:

### Estrutura de Testes

```
TaskManager.Tests/
├── Domain/                      # Testes de entidade e validações
│   └── TaskEntityTests.cs
├── Application/                 # Testes de serviços e lógica de negócio
│   └── TaskServiceTests.cs
└── API/                        # Testes de controllers e endpoints
    └── TasksControllerTests.cs
```

### Frameworks de Testes

- **xUnit** - Framework de testes
- **Moq** - Biblioteca de mocking
- **FluentAssertions** - Asserções fluentes e legíveis

### Executar os Testes

Para executar todos os testes:

```bash
cd TaskManager.Tests
dotnet test
```

Para executar com relatório detalhado:

```bash
dotnet test --verbosity normal
```

Para executar com cobertura de código:

```bash
dotnet test /p:CollectCoverage=true
```

### Cobertura de Testes

Os testes cobrem:

✅ **Domain Layer (10 testes)**
- Validação de tarefas válidas
- Validação de data e hora de conclusão anterior à criação
- Validação de título vazio
- Validação de título com mais de 100 caracteres
- Validação de data de conclusão nula
- Validação de data de conclusão futura
- Validação de todos os status
- Validação de hora e minutos (11:00 vs 11:01)
- Validação de conclusão antes da criação (10:59 vs 11:00)
- Validação de mesma data e hora

✅ **Application Layer (14 testes)**
- Obter todas as tarefas
- Obter tarefa por ID (válido e inválido)
- Criar tarefa válida (com definição automática de DataCriacao)
- Criar tarefa inválida
- Criar tarefa com data de conclusão inválida
- Criar tarefa com título duplicado (validação de título único)
- Atualizar tarefa para título já existente (validação de título único)
- Atualizar tarefa válida
- Atualizar tarefa inexistente
- Atualizar status para Concluída (definição automática de DataConclusao)
- Deletar tarefa válida
- Deletar tarefa inexistente

✅ **API Layer (13 testes)**
- GET /api/tasks - Retornar todas as tarefas
- GET /api/tasks/{id} - Retornar tarefa por ID
- GET /api/tasks/{id} - Retornar 404 para ID inválido
- POST /api/tasks - Criar tarefa válida
- POST /api/tasks - Retornar 400 para tarefa inválida
- POST /api/tasks - Retornar 400 para título duplicado (validação de título único)
- PUT /api/tasks/{id} - Atualizar tarefa válida
- PUT /api/tasks/{id} - Retornar 404 para tarefa inexistente
- PUT /api/tasks/{id} - Retornar 400 para dados inválidos
- PUT /api/tasks/{id} - Retornar 400 para título duplicado (validação de título único)
- DELETE /api/tasks/{id} - Deletar tarefa
- DELETE /api/tasks/{id} - Retornar 404 para tarefa inexistente

**Total: 37 testes ✅**

### Importante: Validação de Data e Hora

A validação de data de conclusão considera **data E hora** completa (hora, minutos, segundos):

✅ **Válido**: Criar às 11:00:00, concluir às 11:01:00  
❌ **Inválido**: Criar às 11:00:00, concluir às 10:59:00

Isso garante que uma tarefa não pode ser concluída antes de ser criada, mesmo que seja no mesmo dia.

## 🛠️ Comandos Úteis

### Criar nova migration
```bash
dotnet ef migrations add <NomeDaMigration> --project TaskManager.Infrastructure --startup-project TaskManager.API
```

### Aplicar migrations ao banco
```bash
dotnet ef database update --project TaskManager.Infrastructure --startup-project TaskManager.API
```

### Remover última migration
```bash
dotnet ef migrations remove --project TaskManager.Infrastructure --startup-project TaskManager.API
```

### Limpar e reconstruir o projeto
```bash
dotnet clean
dotnet build
```

### Executar a API
```bash
dotnet run --project TaskManager.API
```

### Executar os testes
```bash
dotnet test TaskManager.Tests
```

## 📦 Estrutura de Pacotes NuGet

### API Project
- `Microsoft.EntityFrameworkCore.SqlServer` (10.0.1)
- `Microsoft.EntityFrameworkCore.Design` (10.0.1)
- `Microsoft.EntityFrameworkCore.Tools` (10.0.1)
- `Swashbuckle.AspNetCore` (10.1.0)

### Test Project
- `xUnit` (2.9.2)
- `xUnit.runner.visualstudio` (3.1.4)
- `Moq` (4.20.72)
- `FluentAssertions` (8.8.0)
- `Microsoft.AspNetCore.Mvc.Testing` (10.0.1)

### Frontend Project
- `Vite` (6.4.1)
- JavaScript Vanilla ES6+

## 🎨 Frontend

O projeto inclui uma interface web desenvolvida em JavaScript Vanilla com Vite para gerenciar tarefas através da API.

### Funcionalidades do Frontend

- ✅ Listar todas as tarefas
- ✅ Criar nova tarefa
- ✅ Editar tarefa existente
- ✅ Deletar tarefa
- ✅ Filtrar tarefas por status (Todas, Pendente, Em Progresso, Concluída)
- ✅ Visualização de data e hora de criação e conclusão
- ✅ Interface responsiva e intuitiva
- ✅ Feedback visual com loading e mensagens de erro

### Estrutura do Frontend

```
task-manager-frontend/
├── src/
│   ├── components/
│   │   ├── TaskItem.js       # Componente de item de tarefa
│   │   ├── TaskForm.js       # Formulário de criação/edição
│   │   └── TaskFilter.js     # Filtro por status
│   ├── services/
│   │   └── taskService.js    # Comunicação com a API
│   ├── main.js               # Ponto de entrada da aplicação
│   └── style.css             # Estilos da aplicação
├── index.html
├── package.json
└── vite.config.js
```

### Executar o Frontend

```bash
cd task-manager-frontend
npm install
npm run dev
```

### Build para Produção

```bash
npm run build
```

O build será gerado no diretório `dist/`.

## 🔒 Tratamento de Erros

A API implementa tratamento global de exceções:

- **400 Bad Request**: Erros de validação de dados
- **404 Not Found**: Recurso não encontrado
- **500 Internal Server Error**: Erros internos do servidor

## 📝 Licença

Este projeto foi desenvolvido como parte de um desafio técnico de uma vaga para desenvolvedor para demonstrar boas práticas de desenvolvimento de APIs RESTful em C#.

## 👤 Autor

Desenvolvido seguindo as melhores práticas de Clean Code e SOLID.