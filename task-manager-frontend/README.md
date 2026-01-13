# 🎨 Task Manager - Frontend

Interface web moderna para gerenciar tarefas, consumindo a API Task Manager.

## 🚀 Tecnologias

- **JavaScript Vanilla** (ES6+)
- **Vite** (Build tool e dev server)
- **CSS3** (Design moderno e responsivo)
- **Fetch API** (Comunicação com backend)

## ✨ Funcionalidades

- ✅ **Listar todas as tarefas** em cards visuais
- ➕ **Criar nova tarefa** via modal
- ✏️ **Editar tarefa existente**
- 🗑️ **Deletar tarefa** com confirmação
- 🔍 **Filtrar por status**: Todas / Pendentes / Em Progresso / Concluídas
- 💫 **Feedback visual**: Loading, mensagens de erro
- 📱 **Design responsivo**: Funciona em desktop e mobile

## 📁 Estrutura do Projeto

```
task-manager-frontend/
├── src/
│   ├── components/
│   │   ├── TaskItem.js      # Componente de card de tarefa
│   │   ├── TaskForm.js      # Modal de formulário
│   │   └── TaskFilter.js    # Filtro por status
│   ├── services/
│   │   └── taskService.js   # Serviço de API
│   ├── main.js              # Arquivo principal
│   └── style.css            # Estilos globais
├── index.html               # HTML principal
├── package.json
└── vite.config.js
```

## ⚙️ Configuração e Execução

### Pré-requisitos

- Node.js (versão 16+)
- npm ou yarn
- API rodando em `http://localhost:5194`

### Instalação

```bash
# Navegar para o diretório
cd task-manager-frontend

# Instalar dependências
npm install

# Executar em modo de desenvolvimento
npm run dev
```

A aplicação estará disponível em: **http://localhost:5173**

### Build para Produção

```bash
# Gerar build otimizado
npm run build

# Preview do build
npm run preview
```

## 🎯 Como Usar

### 1. Listar Tarefas
- Ao abrir a aplicação, todas as tarefas são carregadas automaticamente
- Cada tarefa é exibida em um card com:
  - Título
  - Descrição (se houver)
  - Status (Pendente/Em Progresso/Concluída)
  - Data de criação
  - Data de conclusão (se houver)
  - Botões de editar e deletar

### 2. Criar Nova Tarefa
1. Clique no botão **➕ Nova Tarefa**
2. Preencha o formulário:
   - **Título*** (obrigatório, máx 100 caracteres)
   - **Descrição** (opcional)
   - **Status*** (selecione: Pendente, Em Progresso ou Concluída)
   - **Data de Conclusão** (opcional)
3. Clique em **Criar**

### 3. Editar Tarefa
1. Clique no botão **✏️ Editar** no card da tarefa
2. Modifique os campos desejados
3. Clique em **Atualizar**

### 4. Deletar Tarefa
1. Clique no botão **🗑️ Deletar**
2. Confirme a exclusão no popup

### 5. Filtrar Tarefas
- Clique nos botões de filtro no topo:
  - **Todas**: Mostra todas as tarefas
  - **Pendentes**: Apenas tarefas pendentes
  - **Em Progresso**: Apenas tarefas em andamento
  - **Concluídas**: Apenas tarefas finalizadas

## 🎨 Design

### Cores por Status

- **Pendente**: Cinza (`#e2e8f0`)
- **Em Progresso**: Amarelo (`#fef3c7`)
- **Concluída**: Verde (`#d1fae5`)

### Responsividade

- **Desktop** (>768px): Grid de 3 colunas
- **Tablet** (768px): Grid de 2 colunas
- **Mobile** (<768px): 1 coluna

## 🔌 Integração com API

### Configuração

A URL da API está configurada em `src/services/taskService.js`:

```javascript
const API_URL = 'http://localhost:5194/api/tasks';
```

### Endpoints Consumidos

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/tasks` | Buscar todas as tarefas |
| GET | `/api/tasks/{id}` | Buscar tarefa por ID |
| POST | `/api/tasks` | Criar nova tarefa |
| PUT | `/api/tasks/{id}` | Atualizar tarefa |
| DELETE | `/api/tasks/{id}` | Deletar tarefa |

### Formato de Dados

**Request (Criar/Atualizar):**
```json
{
  "titulo": "Implementar frontend",
  "descricao": "Criar interface com JavaScript",
  "status": 1,
  "dataConclusao": null
}
```

**Response (Tarefa):**
```json
{
  "id": 1,
  "titulo": "Implementar frontend",
  "descricao": "Criar interface com JavaScript",
  "dataCriacao": "2026-01-13T10:00:00",
  "dataConclusao": null,
  "status": 1
}
```

## 🐛 Troubleshooting

### Erro: "Erro ao carregar tarefas"

**Causa**: API não está rodando ou CORS não configurado

**Solução**:
1. Verifique se a API está rodando em `http://localhost:5194`
2. Verifique se CORS está habilitado no `Program.cs` da API:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### Erro: "Failed to fetch"

**Causa**: URL da API incorreta

**Solução**: Verifique a URL em `src/services/taskService.js`

### Página em branco

**Causa**: Erro de JavaScript no console

**Solução**: 
1. Abra o DevTools (F12)
2. Verifique erros no console
3. Verifique se todos os arquivos foram criados corretamente

## 📦 Scripts Disponíveis

```json
{
  "dev": "vite",           // Modo desenvolvimento
  "build": "vite build",   // Build produção
  "preview": "vite preview" // Preview do build
}
```

## 📝 Licença

Este projeto faz parte do desafio Task Manager.

---

**Desenvolvido com 💙 usando JavaScript Vanilla**
