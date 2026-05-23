# 🔐 Guia de Segurança e Autenticação - ZetaFin

## Arquitetura de Segurança Implementada

### 1. **Autenticação**

#### JWT (JSON Web Tokens)
- **Access Token**: Válido por 15 minutos (configurável em `Jwt:AccessTokenExpirationMinutes`)
- **Claims Inclusos**:
  - `sub`: User ID
  - `email`: Email do usuário
  - `name`: Nome do usuário
  - `role`: Papel do usuário
  - `type`: "access_token"

#### Refresh Token
- **Duração**: 30 dias (configurável em `Jwt:RefreshTokenExpirationDays`)
- **Armazenamento**: Banco de dados (tabela `RefreshTokens`)
- **Revogação**: Automática ao fazer logout ou trocar senha
- **Device Tracking**: Cada token rastreia dispositivo, IP e tipo de dispositivo

#### Password Hashing
- **Algoritmo**: BCrypt com factor de 12
- **Requisitos**:
  - Mínimo 8 caracteres
  - Pelo menos 1 maiúscula
  - Pelo menos 1 minúscula
  - Pelo menos 1 número
  - Pelo menos 1 caractere especial

### 2. **Proteções de Segurança**

#### Account Lockout
- **Tentativas Falhas**: Máximo 5 tentativas
- **Duração do Lockout**: 30 minutos
- **Reset**: Automático após sucesso ou após expiração

#### Session Management
- **Rastreamento de Sessões**: Cada login cria nova sessão
- **Informações Capturadas**:
  - Nome do dispositivo
  - Tipo de dispositivo (Web, Mobile, Desktop)
  - Endereço IP
  - User Agent
  - Último acesso
- **Encerramento Remoto**: Usuário pode encerrar qualquer sessão remotamente

#### Audit Logging
- **Eventos Registrados**:
  - Login (sucesso/falha)
  - Logout
  - Mudança de senha
  - Reset de senha
  - Confirmação de email
  - Tentativas de acesso não autorizado
- **Retenção**: Mínimo 7 dias (configurável)

### 3. **Endpoints de Autenticação**

#### Registro
```
POST /api/authentication/register
Content-Type: application/json

{
  "name": "João Silva",
  "email": "joao@example.com",
  "password": "SecurePass123!",
  "passwordConfirmation": "SecurePass123!"
}

Response:
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "base64_string",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "name": "João Silva",
  "email": "joao@example.com",
  "role": "User",
  "accessTokenExpiresIn": 900,
  "tokenType": "Bearer"
}
```

#### Login
```
POST /api/authentication/login
Content-Type: application/json
X-Device-Name: iPhone 12
X-Device-Type: Mobile

{
  "email": "joao@example.com",
  "password": "SecurePass123!"
}

Response: [igual ao registro]
```

#### Refresh Token
```
POST /api/authentication/refresh
Content-Type: application/json

{
  "refreshToken": "base64_string",
  "deviceName": "iPhone 12"
}

Response: [novo accessToken e refreshToken]
```

#### Logout
```
POST /api/authentication/logout
Authorization: Bearer eyJhbGc...

Response:
{
  "message": "Logged out successfully"
}
```

#### Logout de Todas as Sessões
```
POST /api/authentication/logout-all
Authorization: Bearer eyJhbGc...

Response:
{
  "message": "Logged out from all sessions successfully"
}
```

#### Mudar Senha
```
POST /api/authentication/change-password
Authorization: Bearer eyJhbGc...
Content-Type: application/json

{
  "currentPassword": "OldPass123!",
  "newPassword": "NewPass456!",
  "newPasswordConfirmation": "NewPass456!"
}

Response:
{
  "message": "Password changed successfully"
}
```

#### Solicitar Reset de Senha
```
POST /api/authentication/forgot-password
Content-Type: application/json

{
  "email": "joao@example.com"
}

Response:
{
  "message": "If the email exists, you will receive a password reset link"
}
```

#### Reset de Senha
```
POST /api/authentication/reset-password
Content-Type: application/json

{
  "token": "reset_token_from_email",
  "email": "joao@example.com",
  "newPassword": "NewPass456!",
  "newPasswordConfirmation": "NewPass456!"
}

Response:
{
  "message": "Password reset successfully"
}
```

#### Confirmar Email
```
POST /api/authentication/confirm-email?email=joao@example.com&token=email_confirmation_token

Response:
{
  "message": "Email confirmed successfully"
}
```

### 4. **Endpoints de Sessão**

#### Listar Sessões Ativas
```
GET /api/sessions/active
Authorization: Bearer eyJhbGc...

Response:
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "deviceName": "iPhone 12",
    "deviceType": "Mobile",
    "ipAddress": "192.168.1.1",
    "createdAt": "2024-01-15T10:30:00Z",
    "lastAccessAt": "2024-01-15T12:30:00Z",
    "isActive": true,
    "isCurrentSession": true
  }
]
```

#### Encerrar Sessão Específica
```
DELETE /api/sessions/{sessionId}
Authorization: Bearer eyJhbGc...

Response:
{
  "message": "Session terminated successfully"
}
```

#### Encerrar Todas as Sessões
```
DELETE /api/sessions/all
Authorization: Bearer eyJhbGc...

Response:
{
  "message": "All sessions terminated successfully"
}
```

## 🔐 Configuração de Variáveis de Ambiente

### Desenvolvimento (`appsettings.Development.json`)
```json
{
  "Jwt": {
    "Secret": "your-super-secret-key-min-32-characters-very-secure",
    "Issuer": "https://localhost:5001",
    "Audience": "https://localhost:5001",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 30
  },
  "Security": {
    "MaxFailedLoginAttempts": 5,
    "LockoutDurationMinutes": 30,
    "RequireEmailConfirmation": false,
    "PasswordPolicy": {
      "MinLength": 8,
      "RequireUppercase": true,
      "RequireLowercase": true,
      "RequireDigit": true,
      "RequireSpecialChar": true
    }
  }
}
```

### Produção (`appsettings.Production.json`)
```json
{
  "Jwt": {
    "Secret": "${JWT_SECRET}", // Use variável de ambiente
    "Issuer": "https://zetafin.com",
    "Audience": "https://zetafin.com",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 30
  },
  "Security": {
    "MaxFailedLoginAttempts": 5,
    "LockoutDurationMinutes": 30,
    "RequireEmailConfirmation": true,
    "PasswordPolicy": {
      "MinLength": 12,
      "RequireUppercase": true,
      "RequireLowercase": true,
      "RequireDigit": true,
      "RequireSpecialChar": true
    }
  }
}
```

## ⚠️ Boas Práticas de Segurança

### Para Desenvolvedores

1. **Nunca commit de secrets**
   - Use `appsettings.{Environment}.json` local (não commitado)
   - Use variáveis de ambiente ou Azure Key Vault em produção

2. **Validação de Entrada**
   - Sempre validar email, senha e dados de entrada
   - Usar data annotations e FluentValidation

3. **HTTPS Obrigatório**
   - Nunca enviar tokens por HTTP
   - Em produção, forçar HTTPS com HSTS

4. **Renovação de Tokens**
   - Sempre usar refresh tokens para obter novos access tokens
   - Nunca renovar access tokens expirados sem refresh token

### Para Usuários Mobile

1. **Armazenamento Seguro**
   - **NUNCA** armazenar tokens em SharedPreferences/localStorage
   - Usar Keychain (iOS) ou Keystore (Android)
   - Usar SecureStorage em React Native

2. **Fluxo de Autenticação**
   ```
   1. Usuário faz login
   2. Recebe accessToken (15 min) + refreshToken (30 dias)
   3. Armazena ambos em storage seguro
   4. Usa accessToken para requisições API
   5. Se accessToken expirar, usa refreshToken para renovar
   6. Se refreshToken expirar, força novo login
   ```

3. **Biometria**
   - Validar biometria antes de renovar sessão
   - Usar biometria como camada adicional de segurança

## 🛡️ Conformidade LGPD

### Dados Capturados
- **Necessários**: Email, Senha, Nome
- **Rastreamento**: IP, User-Agent, Dispositivo, Horários
- **Auditoria**: Logs de ações de autenticação

### Direitos do Usuário

1. **Exportação de Dados** (implementar)
   - Endpoint: `GET /api/user/export`
   - Retorna: Todos os dados pessoais em JSON

2. **Exclusão de Conta** (implementar)
   - Endpoint: `DELETE /api/user/account`
   - Ação: Remove usuário e dados após 30 dias (soft delete)

3. **Revogação de Consentimento** (implementar)
   - Endpoint: `POST /api/user/revoke-consent`
   - Ação: Remove permissões de dados

4. **Auditoria de Acessos** (implementado)
   - Endpoint: `GET /api/user/audit-logs`
   - Retorna: Histórico de logins e acessos

## 📋 Checklist de Implementação

### Fase 1 - Fundamentação ✅
- [x] JWT com access + refresh token
- [x] RefreshToken entity e repositório
- [x] UserSession entity e repositório
- [x] AuditLog entity e repositório
- [x] PasswordService com BCrypt
- [x] Account lockout
- [x] AuthenticationService centralizado

### Fase 2 - Funcionalidades
- [ ] Email confirmation flow
- [ ] Password reset com token
- [ ] Email notifications (SendGrid/mailgun)
- [ ] Rate limiting (middleware)
- [ ] CORS configurado por domínio

### Fase 3 - OAuth
- [ ] Google OAuth
- [ ] Microsoft OAuth
- [ ] Apple OAuth

### Fase 4 - Mobile
- [ ] Secure token storage docs
- [ ] Biometria support
- [ ] Push notifications

### Fase 5 - LGPD
- [ ] Data export endpoint
- [ ] Account deletion
- [ ] Consent management
- [ ] Privacy policy

## 📚 Próximos Passos

1. **Migração do Banco de Dados**
   ```bash
   dotnet ef migrations add AddSecurityEntities
   dotnet ef database update
   ```

2. **Testes de Segurança**
   - Testes unitários para autenticação
   - Testes de integração com banco de dados
   - Testes de carga (rate limiting)

3. **Documentação Swagger**
   - Adicionar exemplos de requisição/resposta
   - Documentar status codes
   - Adicionar exemplos de erro

4. **Monitoramento**
   - Alertas de múltiplas tentativas de login falhadas
   - Alertas de IP suspeito
   - Dashboards de auditoria
