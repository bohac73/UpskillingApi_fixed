📘 Upskilling / Reskilling API – README
🚀 Visão Geral

A Upskilling / Reskilling API é uma solução desenvolvida em .NET 10 integrada a um banco de dados Oracle, com foco no gerenciamento de cadastros e progresso de usuários em trilhas de carreira.
A API incorpora boas práticas como JWT, HATEOAS, API Versioning, Caching, Health Checks, Logging, Tracing, e EF Core com Oracle.

🧰 Tecnologias Utilizadas
Backend

.NET 10 Web API

Entity Framework Core 10

Oracle EF Core Provider (Oracle.EntityFrameworkCore)

API Versioning (Microsoft.AspNetCore.Mvc.Versioning)

JWT Authentication

Memory Cache

Swagger / OpenAPI (com versionamento por documento)

Health Checks

Custom Middleware de Logging e Tracing

HATEOAS

🗂️ Estrutura da Solução
UpskillingApi_fixed/
│
├── Configurations/
│     └── ConfigureSwaggerOptions.cs
│
├── Controllers/
│     ├── CadastrosController.cs
│     ├── CadastrosControllerV2.cs
│     └── MeuProgressoController.cs
│
├── Data/
│     └── AppDbContext.cs
│
├── Middleware/
│     └── RequestLoggingMiddleware.cs
│
├── Models/
│     ├── Cadastro.cs
│     ├── MeuProgresso.cs
│     └── HateoasLinks.cs
│
├── Services/
│     ├── DataCacheService.cs
│     └── Interfaces...
│
└── Program.cs

⚙️ Funcionalidades da API
✔️ CRUD completo

Consultar, adicionar, editar e excluir usuários identificados por CPF.

✔️ Gestão de Progresso (MeuProgresso)

Listagem e inserção de progresso em trilhas.

✔️ API Versioning estruturado

/api/v1/...

/api/v2/...

✔️ Paginação nativa

Parâmetros:
pageNumber, pageSize

✔️ HATEOAS

Para facilitar navegação entre recursos.

✔️ JWT Bearer Authentication

Login → token → rotas protegidas.

✔️ Memory Cache

Melhora desempenho em consultas repetidas.

✔️ Health Checks

Valida processamento e conexão ao banco Oracle.

✔️ Logging & Tracing

Middleware personalizado registrando:

Timestamp

Endpoint acessado

Status

IP

Tempo de execução

🧩 Versionamento da API (v1 e v2)

A solução utiliza versionamento por segmento de URL, totalmente compatível com Swagger:

Exemplos
Versão	URL	Controller
v1	/api/v1/cadastros	CadastrosController
v2	/api/v2/cadastros	CadastrosControllerV2

No Swagger, é possível selecionar entre os documentos:

Upskilling API v1

Upskilling API v2

Configuração adicionada por:

AddApiVersioning

AddVersionedApiExplorer

ConfigureSwaggerOptions

🛠️ Instalação e Configuração
1️⃣ Requisitos

.NET 10 SDK

Oracle Database

Visual Studio 2022 atualizado

2️⃣ Configurar Connection String

No arquivo:

appsettings.json


Defina:

"ConnectionStrings": {
  "conn": "User Id=RMxxxxxx;Password=xxxxxx;Data Source=//oracle.fiap.com.br:1521/ORCL;"
}

3️⃣ Restaurar dependências

Menu → Build → Restore NuGet Packages

4️⃣ Executar

Pressione F5 ou execute:

dotnet run


O Swagger abre automaticamente:

https://localhost:7164/swagger/index.html

🔐 Autenticação JWT
1 — Fazer Login
POST /api/v1/auth/login


Body:

{
  "cpf": "12345678900",
  "senha": "Senha123"
}


Retorno:

{
  "token": "eyJhbGciOi..."
}

2 — Usar no Swagger

Clique em Authorize →
Digite:

Bearer SEU_TOKEN_AQUI

🔍 Endpoints Principais
Cadastros
GET    /api/v1/cadastros
GET    /api/v1/cadastros/{cpf}
POST   /api/v1/cadastros
PUT    /api/v1/cadastros/{cpf}
DELETE /api/v1/cadastros/{cpf}

Meu Progresso
GET  /api/v1/meuprogresso
POST /api/v1/meuprogresso

Versão 2
GET /api/v2/cadastros