# WorkSafe

Monitor de Bem-Estar e Saúde no Trabalho Híbrido ou Remoto.

## Visão Geral

WorkSafe permite monitorar sua saúde através de verificações periódicas. O sistema rastreia estresse, sono, satisfação profissional, saúde mental e física. Quando detecta problemas, gera alertas automáticos e oferece dicas personalizadas.

## Integrantes

João dos Santos Cardoso de Jesus - RM560400
Kauê Vinicius Samartino da Silva - RM559317
Davi Praxedes Santos Silva - RM560719

## Arquitetura

- **Entities** - Modelos de dados (User, HealthCheck, WellnessAlert, etc)
- **Services** - Lógica de negócio
- **Repositories** - Acesso a dados
- **Pages** - Interface Razor Pages
- **DTOs** - Transfer objects para comunicação

## Como Rodar

### 1. Restaurar dependências
```bash
dotnet restore
```

### 2. Criar banco de dados
```bash
dotnet ef database update
```

### 3. Executar
```bash
dotnet run
```

Acesse: **https://localhost:7001**

## Configuração

### Banco de Dados

Editar `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WorkSafeDb_Dev;Trusted_Connection=true;"
  }
}
```

## Navegação

| Página | Rota | Função |
|--------|------|--------|
| Dashboard | `/` | Resumo de bem-estar |
| Verificações | `/HealthChecks` | Histórico |
| Nova Verificação | `/HealthChecks/Create` | Criar registro |
| Alertas | `/Alerts` | Ver alertas |
| Dicas | `/Tips` | Dicas personalizadas |

## Como Usar

### 1. Criar Verificação

- Clique em **"Nova Verificação de Saúde"**
- Preencha os valores (1-10):
  - Estresse
  - Sono
  - Satisfação Profissional
  - Saúde Mental
  - Saúde Física
- Clique **"Salvar"**

### 2. Ver Alertas

Os alertas são criados automaticamente quando:
- Estresse ≥ 8
- Sono ≤ 3
- Satisfação Profissional ≤ 3
- Saúde Mental ≤ 3
- Saúde Física ≤ 3

### 3. Resolver Alertas

Vá em `/Alerts` e clique **"Resolver"** para marcar como resolvido.

## Dados de Demonstração

Ao iniciar, o sistema carrega automaticamente:
- 2 usuários de demonstração
- 4 verificações históricas
- 8 dicas de bem-estar
- Resumos de bem-estar

## Desenvolvedor

### Resetar Banco

```bash
dotnet ef database drop
dotnet ef database update
```

### Seeds

Modificar dados em: `Infrastructure/Data/DataSeeder.cs`

## Tecnologias

- .NET 10
- Razor Pages
- Entity Framework Core
- SQL Server
- Bootstrap 5

---

**Versão:** 1.0 | **Ano:** 2025
