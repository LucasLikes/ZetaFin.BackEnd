# 🎯 Plano Estratégico de Modernização - ZetaFin Auth & Security

## 📊 Visão Geral

Este documento detalha a estratégia de modernização completa da autenticação, autorização e segurança do ZetaFin para ambiente produtivo, preparando para:
- ✅ Web (atual)
- 📱 Mobile (React Native)
- 🔗 Login Social (Google, Apple, Microsoft)
- 👨‍👩‍👧‍👦 Plano Família
- 🏦 Open Finance (Pluggy)
- ⚖️ Conformidade LGPD

---

## 🏗️ Arquitetura Atual vs. Proposta

### ❌ Problemas Identificados

```
┌─────────────────────────────────────┐
│   AUTENTICAÇÃO ATUAL (PROBLEMA)     │
├─────────────────────────────────────┤
│ ❌ Sem Refresh Token                │
│ ❌ Sem Controle de Sessões          │
│ ❌ Sem Auditoria                    │
│ ❌ Sem Account Lockout              │
│ ❌ Sem Proteção Rate Limiting       │
│ ❌ Senha válida por 24h (Alexa!)    │
│ ❌ Sem Email Confirmation           │
│ ❌ Sem Recuperação de Senha         │
│ ❌ Sem Support para Mobile          │
│ ❌ Sem RBAC Granular                │
│ ❌ Sem LGPD                         │
└─────────────────────────────────────┘
```

### ✅ Arquitetura Proposta

```
┌──────────────────────────────────────────┐
│  AUTENTICAÇÃO MODERNA (PROPOSTO)         │
├──────────────────────────────────────────┤
│ ✅ Refresh Token (30 dias)              │
│ ✅ Access Token (15 min)                │
│ ✅ Sessão por Dispositivo               │
│ ✅ Auditoria Completa                   │
│ ✅ Account Lockout (5 tentativas)       │
│ ✅ Rate Limiting                        │
│ ✅ Email Confirmation                   │
│ ✅ Password Recovery                    │
│ ✅ Mobile First                         │
│ ✅ RBAC Expandido                       │
│ ✅ LGPD Ready                           │
│ ✅ OAuth 2.0                            │
└──────────────────────────────────────────┘
```

---

## 📋 Escopo Detalhado por Fase

### ✅ FASE 1: FUNDAÇÃO SEGURA (IMPLEMENTADA)

**Duração**: 2 semanas | **Status**: CONCLUÍDO

#### 1.1 Domain Model
```
User (Expandido)
├── Id (Guid)
├── Name (string)
├── Email (string)
├── PasswordHash (string, BCrypt)
├── Role (string)
├── IsEmailConfirmed (bool)
├── IsActive (bool)
├── CreatedAt (DateTime)
├── LastLoginAt (DateTime?)
├── FailedLoginAttempts (int)
├── LockedUntil (DateTime?)
└── Relations
    ├── RefreshTokens (1:N)
    ├── Sessions (1:N)
    └── AuditLogs (0:N)

RefreshToken (Nova)
├── Id (Guid)
├── UserId (Guid)
├── Token (string)
├── DeviceName (string)
├── DeviceType (string) // Web, Mobile, Desktop
├── IpAddress (string)
├── CreatedAt (DateTime)
├── ExpiresAt (DateTime)
├── RevokedAt (DateTime?)
├── RevokeReason (string?)
└── IsActive (computed)

UserSession (Nova)
├── Id (Guid)
├── UserId (Guid)
├── DeviceName (string)
├── DeviceType (string)
├── IpAddress (string)
├── UserAgent (string)
├── CreatedAt (DateTime)
├── LastAccessAt (DateTime)
├── TerminatedAt (DateTime?)
├── IsActive (bool)
└── RefreshTokenId (Guid?)

AuditLog (Nova)
├── Id (Guid)
├── UserId (Guid?)
├── Action (string)
├── Resource (string)
├── IpAddress (string)
├── UserAgent (string)
├── Status (string) // Success, Failure
├── Details (string, JSON)
└── CreatedAt (DateTime)
```

#### 1.2 Services Implementados
- ✅ `IJwtTokenService` - Geração de tokens
- ✅ `IPasswordService` - Hash e validação
- ✅ `IAuthenticationService` - Orquestração
- ✅ `IAuditLogService` - Registro de eventos

#### 1.3 Endpoints
```
POST   /api/authentication/register
POST   /api/authentication/login
POST   /api/authentication/refresh
POST   /api/authentication/logout
POST   /api/authentication/logout-all
POST   /api/authentication/change-password
POST   /api/authentication/forgot-password
POST   /api/authentication/reset-password
POST   /api/authentication/confirm-email

GET    /api/sessions/active
DELETE /api/sessions/{sessionId}
DELETE /api/sessions/all
```

#### 1.4 Migrations
```bash
# Criar migration
dotnet ef migrations add AddSecurityEntities -p ZetaFin.Persistence

# Aplicar
dotnet ef database update -p ZetaFin.Persistence
```

**Arquivos Criados**:
- ✅ `RefreshToken.cs` (Entity)
- ✅ `UserSession.cs` (Entity)
- ✅ `AuditLog.cs` (Entity)
- ✅ `UserRole.cs` (Enum)
- ✅ `IRefreshTokenRepository.cs`
- ✅ `IUserSessionRepository.cs`
- ✅ `IAuditLogRepository.cs`
- ✅ `RefreshTokenRepository.cs`
- ✅ `UserSessionRepository.cs`
- ✅ `AuditLogRepository.cs`
- ✅ `IJwtTokenService.cs`
- ✅ `IPasswordService.cs`
- ✅ `IAuthenticationService.cs`
- ✅ `IAuditLogService.cs`
- ✅ `JwtTokenService.cs`
- ✅ `PasswordService.cs`
- ✅ `AuthenticationService.cs`
- ✅ `AuditLogService.cs`
- ✅ `AuthenticationController.cs`
- ✅ `SessionsController.cs`
- ✅ `DTOs` (AuthResponseDto, RegisterDto, etc)

---

### ⏳ FASE 2: AUTENTICAÇÃO TRADICIONAL COMPLETA

**Duração**: 2 semanas | **Status**: PLANEJADO

#### 2.1 Email Confirmation
```csharp
// Novo Endpoint
POST /api/authentication/send-email-confirmation
- Enviar token por email
- Rate limit: 1 por hora

// Novo Serviço
IEmailService
├── SendEmailConfirmation()
├── SendPasswordReset()
└── SendSecurityAlert()
```

**Implementação**:
1. Integrar SendGrid ou mailgun
2. Criar templates HTML
3. Implementar token expiration (1 hora)
4. Webhook para tracking de emails

#### 2.2 Password Recovery Completo
```csharp
Flow:
1. User solicita reset (forgot-password)
2. Sistema gera token + envia email (5 min expiry)
3. User clica link
4. Preenche nova senha
5. Validar força
6. Revoga todos os tokens
7. Força novo login
```

#### 2.3 Two-Factor Authentication (2FA)
```csharp
// Novo Entity
TwoFactorToken
├── UserId
├── Code (6 dígitos)
├── Method (Email, SMS, Authenticator)
├── ExpiresAt
└── IsUsed

// Endpoints
POST /api/authentication/enable-2fa
POST /api/authentication/verify-2fa
POST /api/authentication/backup-codes
```

**Métodos Suportados**:
- Email (simples)
- SMS (Twilio)
- Authenticator (Google Authenticator, Authy)
- Backup codes

---

### ⏳ FASE 3: RBAC EXPANDIDO E AUTORIZAÇÃO

**Duração**: 1 semana | **Status**: PLANEJADO

#### 3.1 Roles Expandidas
```csharp
public enum UserRole
{
    User = 1,           // Usuário comum
    Admin = 2,          // Administrador
    Support = 3,        // Suporte
    FamilyOwner = 4,    // Dono da família
    FamilyAdult = 5,    // Adulto da família
    FamilyDependent = 6 // Dependente
}

// Permissions
public enum Permission
{
    // User
    ManageOwnData,
    ManageAccounts,
    ManageGoals,
    ManageInvestments,

    // Admin
    ManageUsers,
    ManageRoles,
    ViewAuditLogs,
    ConfigureSystem,

    // Support
    ViewUserData,
    ResetPassword,

    // Family
    ManageFamilyMembers,
    ManageFamilyBudget,
    ManageFamilyGoals,
}
```

#### 3.2 Policy-Based Authorization
```csharp
// Startup
services.AddAuthorizationBuilder()
    .AddPolicy("IsAdmin", p => p.RequireRole("Admin"))
    .AddPolicy("IsFamily", p => p.RequireRole("FamilyOwner", "FamilyAdult"))
    .AddPolicy("CanManageUsers", p => 
        p.Requirements.Add(new PermissionRequirement("ManageUsers")))
    .AddPolicy("CanViewUserData", p =>
        p.Requirements.Add(new UserIdMatchRequirement()));

// Usage
[Authorize(Policy = "IsAdmin")]
public async Task<IActionResult> GetAllUsers() { ... }
```

#### 3.3 Claims Estruturados
```json
{
  "sub": "550e8400-e29b-41d4-a716-446655440000",
  "email": "joao@example.com",
  "name": "João Silva",
  "role": "User",
  "permissions": ["ManageOwnData", "ManageAccounts"],
  "familyId": "550e8400-e29b-41d4-a716-446655440001",
  "familyRole": "Owner",
  "openFinanceConsent": true,
  "type": "access_token"
}
```

---

### ⏳ FASE 4: PREPARAÇÃO MOBILE & OAUTH

**Duração**: 2 semanas | **Status**: PLANEJADO

#### 4.1 OAuth 2.0 - Google Login
```csharp
// Novo Endpoint
POST /api/authentication/google-login
{
  "idToken": "google_id_token"
}

Response:
{
  "accessToken": "...",
  "refreshToken": "...",
  "userId": "...",
  "isNewUser": true
}

// Novo Entity
UserSocialLogin
├── Id
├── UserId
├── Provider (Google, Apple, Microsoft)
├── ProviderId
├── Email
├── Name
├── Picture
├── CreatedAt
└── LastLoginAt

// Flow
1. User clica "Login com Google"
2. Frontend obtém Google ID Token
3. Envia para backend
4. Backend valida token com Google
5. Procura ou cria user
6. Retorna ZetaFin tokens
7. Frontend armazena em Secure Storage
```

**Implementação**:
1. Nuget: `Google.Apis.Auth`
2. Validar Google ID Token
3. Mapear Google user -> ZetaFin user
4. Gerenciar múltiplas social logins por usuário

#### 4.2 Apple Login
- Similar ao Google
- Usar Apple Sign in SDK

#### 4.3 Mobile Token Management
```typescript
// React Native - Exemplo
import * as SecureStore from 'expo-secure-store';

// Store tokens
await SecureStore.setItemAsync('accessToken', token);
await SecureStore.setItemAsync('refreshToken', refreshToken);

// Retrieve
const token = await SecureStore.getItemAsync('accessToken');

// Interceptor
const api = axios.create();
api.interceptors.request.use(async (config) => {
  const token = await SecureStore.getItemAsync('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Auto-refresh
api.interceptors.response.use(
  response => response,
  async error => {
    if (error.response?.status === 401) {
      const refreshToken = await SecureStore.getItemAsync('refreshToken');
      const newToken = await refreshToken(refreshToken);
      await SecureStore.setItemAsync('accessToken', newToken);
      return api.request(error.config);
    }
    return Promise.reject(error);
  }
);
```

---

### ⏳ FASE 5: PLANO FAMÍLIA

**Duração**: 2 semanas | **Status**: PLANEJADO

#### 5.1 Nova Estrutura
```
Family (Nova Entity)
├── Id
├── Name
├── OwnerId (Guid, FK User)
├── CreatedAt
└── Members (1:N)

FamilyMember (Nova Entity)
├── Id
├── FamilyId
├── UserId
├── Role (Owner, Adult, Dependent)
├── Status (Active, Invited, Declined)
├── InvitationToken (string?)
├── InvitationExpiresAt (DateTime?)
└── JoinedAt (DateTime?)

FamilyBudget (Nova Entity)
├── Id
├── FamilyId
├── Month
├── TotalBudget
├── Allocations[] (por membro)
```

#### 5.2 Endpoints
```
POST   /api/family/create
GET    /api/family/{familyId}
PATCH  /api/family/{familyId}
POST   /api/family/{familyId}/invite-member
POST   /api/family/{familyId}/members/{memberId}/accept
DELETE /api/family/{familyId}/members/{memberId}
GET    /api/family/{familyId}/members
GET    /api/family/{familyId}/budget
PATCH  /api/family/{familyId}/budget
```

#### 5.3 Authorization
```csharp
[Authorize(Policy = "IsFamilyOwner")]
public async Task<IActionResult> ManageFamily(Guid familyId) { ... }

[Authorize(Policy = "IsFamilyMember")]
public async Task<IActionResult> ViewFamilyData(Guid familyId) { ... }
```

---

### ⏳ FASE 6: OPEN FINANCE (Pluggy Integration)

**Duração**: 2 semanas | **Status**: PLANEJADO

#### 6.1 Consentimento de Open Finance
```
OpenFinanceConsent (Nova Entity)
├── Id
├── UserId
├── PluggyClientId
├── PluggyCode (do usuário)
├── GrantedAt
├── ExpiresAt
├── Revoked (bool)
├── ConsentedAccounts[] (array de contas)
└── Scopes[] (dados autorizados)

ConsentedAccount
├── Id
├── ConsentId
├── BankAccountNumber
├── BankCode
├── Type (Checking, Savings, Credit)
└── Status (Active, Revoked)
```

#### 6.2 Endpoints
```
POST   /api/open-finance/consent/generate
GET    /api/open-finance/consent/status
POST   /api/open-finance/consent/revoke
GET    /api/open-finance/accounts
POST   /api/open-finance/sync
```

#### 6.3 Flow
```
1. User clica "Conectar Banco"
2. Gera link de consentimento com Pluggy
3. User autoriza no app do banco
4. Pluggy callback notifica backend
5. Sistema cria ConsentedAccount
6. Sincroniza dados bancários
7. User vê contas + transações
```

---

### ⏳ FASE 7: LGPD - DATA PRIVACY

**Duração**: 1 semana | **Status**: PLANEJADO

#### 7.1 Direitos Implementados
```
1. RIGHT TO ACCESS
   GET /api/user/my-data
   - Retorna todos dados pessoais em JSON

2. RIGHT TO DELETION
   DELETE /api/user/account
   - Soft delete com retenção de 30 dias
   - Logs persistem para auditoria

3. RIGHT TO REVOKE CONSENT
   POST /api/user/revoke-consent
   - Remove OpenFinance consent
   - Remove permissões de dados

4. RIGHT TO AUDIT ACCESS
   GET /api/user/audit-logs
   - Histórico de logins
   - Acessos a dados
```

#### 7.2 Data Categories
```
Personal Data:
- Name, Email, Phone
- Date of Birth
- CPF/CNPJ
- Address

Financial Data:
- Bank accounts (via Open Finance)
- Transactions
- Goals and budgets

Behavioral Data:
- Login history
- IP addresses
- Device information

Third-party Data:
- Google/Apple login info
- Bank connections
```

#### 7.3 Storage & Retention
```
Retention Policy:
- Active data: indefinido
- Audit logs: 3 anos
- Deleted users: 30 dias soft delete
- Failed login attempts: 90 dias
- IP logs: 90 dias
```

---

### ⏳ FASE 8: PROTEÇÕES AVANÇADAS

**Duração**: 1 semana | **Status**: PLANEJADO

#### 8.1 Rate Limiting
```csharp
services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "login", configure: options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromMinutes(15);
    });

    options.AddFixedWindowLimiter(policyName: "api", configure: options =>
    {
        options.PermitLimit = 100;
        options.Window = TimeSpan.FromMinutes(1);
    });
});

// Endpoint
[HttpPost("login")]
[RequireRateLimiting("login")]
public async Task<IActionResult> Login(LoginDto dto) { ... }
```

#### 8.2 CORS Configurado
```csharp
services.AddCors(options =>
{
    options.AddPolicy("MobileApp", policy =>
    {
        policy.WithOrigins(
            "exp://",
            "capacitor://localhost"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });

    options.AddPolicy("WebApp", policy =>
    {
        policy.WithOrigins("https://zetafin.com")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
```

#### 8.3 Security Headers
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers["Strict-Transport-Security"] = 
        "max-age=63072000; includeSubDomains";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Content-Security-Policy"] = 
        "default-src 'self'; script-src 'self' 'unsafe-inline'";

    await next();
});
```

---

## 📊 Timeline Consolidado

```
┌─────────────────────────────────────────────────────┐
│         CRONOGRAMA DE IMPLEMENTAÇÃO                 │
├─────────────────────────────────────────────────────┤
│ FASE 1 ✅ Semana 1-2   | Fundação Segura           │
│ FASE 2 ⏳ Semana 3-4   | Autenticação Tradicional   │
│ FASE 3 ⏳ Semana 5     | RBAC Expandido             │
│ FASE 4 ⏳ Semana 6-7   | Mobile + OAuth             │
│ FASE 5 ⏳ Semana 8-9   | Plano Família              │
│ FASE 6 ⏳ Semana 10-11 | Open Finance               │
│ FASE 7 ⏳ Semana 12    | LGPD                       │
│ FASE 8 ⏳ Semana 13    | Proteções Avançadas        │
│                                                     │
│ Total: ~13 semanas (3 meses)                        │
└─────────────────────────────────────────────────────┘
```

---

## 🔐 Matriz de Recursos por Plataforma

```
┌──────────────────┬──────┬────────┬────────┬─────────┐
│ Funcionalidade   │ Web  │ Mobile │ OAuth  │ Família │
├──────────────────┼──────┼────────┼────────┼─────────┤
│ Login/Register   │ ✅   │ ✅     │ ✅     │ ✅      │
│ 2FA              │ ✅   │ ✅     │ -      │ ✅      │
│ Biometria        │ -    │ ✅     │ -      │ ✅      │
│ Refresh Token    │ ✅   │ ✅     │ ✅     │ ✅      │
│ Session Control  │ ✅   │ ✅     │ ✅     │ ✅      │
│ Family Features  │ ✅   │ ✅     │ -      │ ✅      │
│ Open Finance     │ ✅   │ ✅     │ -      │ ✅      │
│ Audit Logs       │ ✅   │ ✅     │ ✅     │ ✅      │
└──────────────────┴──────┴────────┴────────┴─────────┘
```

---

## 📈 Critérios de Sucesso

### Segurança
- ✅ Nenhum token hardcoded
- ✅ Todas as senhas com BCrypt
- ✅ Rate limiting implementado
- ✅ Auditoria de 100% dos logins

### Performance
- ✅ Access token gerado em <50ms
- ✅ Refresh token gerado em <100ms
- ✅ Login concluído em <500ms

### Usabilidade
- ✅ Registro em <2 minutos
- ✅ Login em <1 segundo
- ✅ Recovery de senha em <5 minutos

### Conformidade
- ✅ 100% GDPR/LGPD
- ✅ Documentação completa
- ✅ Testes de segurança

---

## 🚀 Como Começar

### Passo 1: Criar Migration
```bash
cd ZetaFin.Persistence
dotnet ef migrations add AddSecurityEntities
dotnet ef database update
```

### Passo 2: Testar Endpoints
```bash
# Register
curl -X POST http://localhost:8080/api/authentication/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João",
    "email": "joao@example.com",
    "password": "SecurePass123!",
    "passwordConfirmation": "SecurePass123!"
  }'

# Login
curl -X POST http://localhost:8080/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@example.com",
    "password": "SecurePass123!"
  }'
```

### Passo 3: Explorar Swagger
- Abrir: http://localhost:8080/swagger
- Testar todos endpoints
- Documentar respostas

### Passo 4: Próxima Fase
- Começar implementação de email confirmation (Fase 2)
- Integrar SendGrid
- Criar templates de email

---

## 📚 Referências

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [JWT.io - Tokens](https://jwt.io/)
- [BCrypt Documentation](https://www.npmjs.com/package/bcrypt)
- [GDPR](https://gdpr-info.eu/)
- [LGPD Brasil](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)
- [OAuth 2.0](https://oauth.net/2/)
- [Microsoft Identity](https://learn.microsoft.com/en-us/entra/identity/)

---

**Próxima Reunião**: Sexta-feira 14:00
**Responsável**: Tech Lead
**Última Atualização**: 2024-01-15

