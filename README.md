# 💰 ZetaFin
### Sistema de Controle de Metas Financeiras Compartilhadas

O **ZetaFin** é uma aplicação backend em **.NET 8 (C#)** que permite que **duas ou mais pessoas planejem e acompanhem juntas seus investimentos** rumo a um objetivo em comum — como comprar um imóvel, viajar ou construir uma reserva financeira.

---

## 🎯 Objetivo do Projeto

Fornecer uma API segura, escalável e colaborativa para o **gerenciamento de metas financeiras compartilhadas**, com integração a pagamentos via **Pix (Mercado Pago)**, controle de aportes e autenticação JWT.

---

## 🧩 Principais Funcionalidades

| Funcionalidade | Status |
|----------------|---------|
| Criar metas financeiras com valor e prazo | ✅ |
| Registrar aportes (depósitos) | ✅ |
| Associar usuários a metas | ✅ |
| Definir metas mensais individuais | ✅ |
| Calcular contribuição mensal ideal | ✅ |
| Visualizar progresso das metas | ✅ |
| Testes automatizados (xUnit) | ✅ |
| Documentação Swagger | ✅ |
| Autenticação JWT (Bearer Token) | ✅ |
| Integração Mercado Pago (Pix - em progresso) | 🔜 |

---

## 🧱 Arquitetura

O projeto segue o padrão **Clean Architecture**, baseado nos princípios **SOLID**, **DRY** e **KISS**.

```
ZetaFin.sln
├── ZetaFin.API           → Controllers, Middlewares e Endpoints (Swagger)
├── ZetaFin.Application   → Services, DTOs, Interfaces e Validadores
├── ZetaFin.Domain        → Entidades e Regras de Domínio
├── ZetaFin.Infrastructure→ Integrações externas (ex: Mercado Pago)
├── ZetaFin.Persistence   → Repositórios com EF Core
├── ZetaFin.Tests         → Testes com xUnit (unitários e integração)
```

---

## 🔐 Autenticação

A API utiliza **JWT (Bearer)**.  
Chave configurada:  
```
zeta-fin-jwt-super-secret-key
```

### Fluxo de Autenticação
1. Criar usuário via `POST /api/Users`
2. Fazer login:
```json
POST /api/Auth/login
{
  "email": "usuario@email.com",
  "password": "senha"
}
```
3. Copiar o token retornado e inserir no botão **Authorize** do Swagger (`Bearer <token>`).

---

## 📘 Endpoints Principais

### 🔑 AuthController
| Método | Rota | Descrição |
|--------|------|------------|
| POST | `/api/Auth/login` | Autentica usuário e retorna JWT |

---

### 👥 UsersController
| Método | Rota | Descrição |
|--------|------|------------|
| POST | `/api/Users` | Cria novo usuário |
| GET | `/api/Users` | Lista todos os usuários |
| GET | `/api/Users/{id}` | Busca usuário por ID |

---

### 🎯 GoalsController
| Método | Rota | Descrição |
|--------|------|------------|
| POST | `/api/Goals` | Cria uma nova meta |
| GET | `/api/Goals` | Lista todas as metas |
| GET | `/api/Goals/{id}` | Retorna meta por ID |

---

### 💵 DepositsController
| Método | Rota | Descrição |
|--------|------|------------|
| POST | `/api/Deposits` | Registra novo depósito |
| GET | `/api/Deposits/goal/{goalId}` | Lista depósitos de uma meta |

---

### 🤝 UserGoalsController
| Método | Rota | Descrição |
|--------|------|------------|
| POST | `/api/UserGoals` | Associa usuário a meta |
| PUT | `/api/UserGoals/{goalId}/{userId}` | Atualiza meta mensal individual |
| GET | `/api/UserGoals/{goalId}` | Lista usuários vinculados a uma meta |

---

### ⚡ WebhookController
| Método | Rota | Descrição |
|--------|------|------------|
| POST | `/api/Webhook/mercadopago` | Recebe notificação Pix e atualiza meta automaticamente |

**Exemplo de payload recebido via Webhook Mercado Pago:**
```json
{
  "goalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "amount": 100.0,
  "pixKey": "email@exemplo.com",
  "transactionId": "pix-1234"
}
```

---

## 🧪 Testes Automatizados

| Teste | Cenário | Esperado |
|-------|----------|----------|
| `HandlePixNotification_ShouldReturnOk_WhenGoalExists` | Quando a meta existe e o Pix é recebido | Atualiza a meta e retorna `200 OK` |
| `HandlePixNotification_ShouldReturnNotFound_WhenGoalDoesNotExist` | Quando o goalId não existe | Retorna `404 Not Found` |
| `GoalDomainTests` | Regras de domínio (AddAmount, Data e Validação) | Todas passam conforme esperado |
| `UserGoalTests` | Associação e cálculo de metas mensais | Regras de negócio testadas com sucesso |

---

## 🐳 Execução com Docker

### 🧰 Build e Run

```bash
# Build da imagem
docker build -t zetafin-api .

# Executar container
docker run -d -p 32769:8080 zetafin-api
```

Depois acesse o Swagger:

👉 [http://localhost:32769/swagger](http://localhost:32769/swagger)

---

## 🧭 Como Contribuir

1. Faça um **fork** do repositório  
2. Clone o projeto localmente  
   ```bash
   git clone https://github.com/<seu-usuario>/ZetaFin.git
   ```
3. Crie uma branch para sua task  
   ```bash
   git checkout -b feature/minha-task
   ```
4. Desenvolva e teste sua feature  
5. Envie um **Pull Request (PR)** —  
   se o código estiver sólido, será adicionado à **main** 🎯  
   Caso contrário, ficará como aprendizado colaborativo.

---

## 📂 Estrutura de Documentação (sugestão de pastas)

```
/docs
 ├── overview.md               → visão geral do projeto
 ├── setup.md                  → como rodar localmente (Docker e manual)
 ├── architecture.md           → explicação das camadas e padrões
 ├── api_reference.md          → resumo dos endpoints (Swagger)
 ├── contributing.md           → guia para novos contribuidores
 └── tasks.md                  → tarefas abertas e ideias de features
```

---

## 🚀 Futuras Implementações

- Integração completa com **Mercado Pago (Pix)** via webhook
- Painel web (frontend em Flutter Web ou React)
- Sistema de ranking entre usuários por contribuição
- Notificações automáticas por e-mail e push
- Deploy automatizado via Azure App Service + CI/CD

---

## 💬 Contato e Comunidade

💻 **Criador:** [Lucas Gabriel Likes](https://www.linkedin.com/in/lucas-gabriel-likes-06a2b9182/)  
📬 **E-mail:** lucas_likes@hotmail.com  
🌍 **Contribua:** issues, pull requests e ideias são bem-vindos!

---

> 🧠 **ZetaFin** é um projeto colaborativo que une aprendizado, boas práticas e propósito.  
> Cada contribuição ajuda a tornar o sistema mais robusto, seguro e inspirador para novos desenvolvedores se desafiarem e terem contato com algo um pouco mais mercado de trabalho.
