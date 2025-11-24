# WorkSafe

Monitor de Bem-Estar e Saúde no Trabalho Híbrido ou Remoto.

## Visão Geral

WorkSafe é uma aplicação web em .NET 10 que monitora a saúde e bem-estar de profissionais. Permite criar verificações periódicas, receber alertas automáticos e obter dicas personalizadas.

**Funcionalidades:**
- Verificações de saúde (Estresse, Sono, Satisfação, Saúde Mental/Física)
- Alertas automáticos baseados em limiares
- Dicas personalizadas de bem-estar
- Dashboard com métricas
- Histórico com filtros e paginação

## Integrantes

| RM | Nome |
|--------|------|
| RM560400 | João dos Santos Cardoso de Jesus |
| RM559317 | Kauê Vinicius Samartino da Silva |
| RM560719 | Davi Praxedes Santos Silva |

## Arquitetura

```
Domain/                  # Entidades (User, HealthCheck, WellnessAlert, etc)
Application/             # Serviços + DTOs com validações
Infrastructure/          # Repositórios + DbContext + Migrations
Pages/                   # Razor Pages (6 páginas)
```

**Decisões Arquiteturais:**
- Razor Pages (melhor para aplicações page-focused)
- Repository Pattern para abstração de dados
- DTOs para transferência segura entre camadas
- Validação em 3 camadas (DTO → Service → Repository)
- Middleware centralizado para tratamento de erros

## Tecnologias

- .NET 10 | Razor Pages | Entity Framework Core 10 | SQL Server | Bootstrap 5

## Como Rodar

### Pré-requisitos
- .NET 10 SDK
- SQL Server LocalDB

### Passos

```bash
# 1. Clonar repositório
git clone https://github.com/joaoscj/worksafe-global-solution.git
cd WorkSafe/WorkSafe

# 2. Restaurar dependências
dotnet restore

# 3. Criar banco de dados
dotnet ef database update

# 4. Executar
dotnet run

# 5. Acessar
# https://localhost:7001
```

## Configuração

Editar `appsettings.json` se necessário:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WorkSafeDb_Dev;Trusted_Connection=true;Encrypt=false;"
  }
}
```

## Rotas Principais

| Rota | Função |
|------|--------|
| `/` | Dashboard com resumo de bem-estar |
| `/HealthChecks` | Histórico de verificações |
| `/HealthChecks/Create` | Criar verificação |
| `/Alerts` | Ver alertas |
| `/Tips` | Dicas de bem-estar |

## Como Usar

### 1. Criar Verificação
- Clique em **"Nova Verificação de Saúde"**
- Preencha valores 1-10 para: Estresse, Sono, Satisfação, Saúde Mental, Saúde Física
- Clique **"Salvar"**

### 2. Alertas Automáticos
Alertas são criados quando:
- Estresse ≥ 8 → Alerta de Estresse Elevado
- Sono ≤ 3 → Alerta de Sono Ruim
- Satisfação ≤ 3 → Alerta de Satisfação Baixa
- Saúde Mental ≤ 3 → Alerta de Saúde Mental
- Saúde Física ≤ 3 → Alerta de Saúde Física

### 3. Resolver Alertas
- Vá para **"Alertas"**
- Revise as recomendações
- Clique **"Resolver"**

## Dados de Demonstração

Ao iniciar, o sistema carrega automaticamente:
- 2 usuários de teste
- 4 verificações de exemplo
- 8 dicas de bem-estar
- Resumos iniciais

## Serviços (Application Layer)

### HealthCheckService
```csharp
GetHealthCheckByIdAsync(int id)
GetHealthChecksByUserIdAsync(int userId)
GetLatestHealthCheckAsync(int userId)
CreateHealthCheckAsync(CreateHealthCheckDto dto)
UpdateHealthCheckAsync(int id, UpdateHealthCheckDto dto)
DeleteHealthCheckAsync(int id)
```

### WellnessAlertService
```csharp
GetAlertByIdAsync(int id)
GetAlertsByUserIdAsync(int userId)
GetUnresolvedAlertsByUserIdAsync(int userId)
CreateAlertFromHealthCheckAsync(HealthCheck healthCheck)
ResolveAlertAsync(int alertId)
```

### TipsService
```csharp
GetTipsForWellnessScoreAsync(int wellnessScore)
GetTipsByCategoryAsync(string category)
```

## Validação de Dados

Implementada em 3 camadas:

**1. DTOs com Data Annotations**
```csharp
[Required]
[Range(1, 10, ErrorMessage = "Deve estar entre 1 e 10")]
public int StressLevel { get; set; }
```

**2. Métodos de Negócio**
```csharp
public bool IsValid() => StressLevel >= 1 && StressLevel <= 10;
```

**3. Repositórios**
```csharp
if (!healthCheck.IsValid())
    throw new InvalidOperationException("Invalid values");
```

## Tratamento de Erros

Middleware centralizado retorna ProblemDetails:

```json
{
  "status": 400,
  "title": "Operação Inválida",
  "detail": "Health check has invalid values.",
  "type": "https://httpstatuses.com/400"
}
```

## Operações de Banco (Desenvolvedor)

```bash
# Resetar banco completamente
dotnet ef database drop --force
dotnet ef database update

# Criar migration
dotnet ef migrations add NomeMigration
dotnet ef database update

# Ver migrations
dotnet ef migrations list
```

Modificar dados de demonstração em: `Infrastructure/Data/DataSeeder.cs`

## Exemplos

### Criar Verificação
```
GET https://localhost:7001/HealthChecks/Create?UserId=1
POST com dados preenchidos
```

### Buscar com Filtros
```
https://localhost:7001/HealthChecks?UserId=1&PageNumber=1&PageSize=10
```

### Ver Alertas
```
https://localhost:7001/Alerts?UserId=1