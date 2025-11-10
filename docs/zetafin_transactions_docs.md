# 📘 Documentação - Sistema de Transações e OCR - ZetaFin

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Modelos de Dados](#modelos-de-dados)
3. [Endpoints - Transações](#endpoints---transações)
4. [Endpoints - OCR](#endpoints---ocr)
5. [Implementação Backend](#implementação-backend)
6. [Banco de Dados](#banco-de-dados)
7. [Integração Frontend](#integração-frontend)
8. [Validações e Regras](#validações-e-regras)

---

## 🎯 Visão Geral

Esta funcionalidade permite aos usuários do ZetaFin gerenciarem suas **transações financeiras** (receitas e despesas) de forma integrada, incluindo:

- ✅ Registro manual de receitas e despesas
- ✅ Categorização automática
- ✅ Upload e processamento de recibos via OCR
- ✅ Criação automática de transações a partir de recibos
- ✅ Relatórios e resumos financeiros

**Base URL**: `https://api.zetafin.com/v1` (ou `http://localhost:5291/api` em desenvolvimento)

---

## 📊 Modelos de Dados

### Transaction (Transação)

```csharp
public class Transaction : BaseEntity
{
    public Guid UserId { get; set; }
    public TransactionType Type { get; set; } // Income | Expense
    public decimal Value { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public ExpenseType? ExpenseType { get; set; } // Fixas | Variaveis | Desnecessarios
    public DateTime Date { get; set; }
    public bool HasReceipt { get; set; }
    public string? ReceiptUrl { get; set; }
    public ReceiptOcrData? OcrData { get; set; }
    
    // Navigation
    public User User { get; set; }
    public Receipt? Receipt { get; set; }
}

public enum TransactionType
{
    Income,
    Expense
}

public enum ExpenseType
{
    Fixas,         // Contas fixas (aluguel, internet, assinaturas)
    Variaveis,     // Contas variáveis (supermercado, transporte)
    Desnecessarios // Gastos desnecessários (compras impulsivas)
}
```

### Receipt (Recibo)

```csharp
public class Receipt : BaseEntity
{
    public Guid TransactionId { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public long FileSize { get; set; }
    public string MimeType { get; set; }
    public bool OcrProcessed { get; set; }
    public ReceiptOcrData? OcrData { get; set; }
    
    // Navigation
    public Transaction Transaction { get; set; }
}

public class ReceiptOcrData
{
    public decimal? ExtractedValue { get; set; }
    public DateTime? ExtractedDate { get; set; }
    public string? MerchantName { get; set; }
    public List<ReceiptItem>? Items { get; set; }
    public double Confidence { get; set; }
}

public class ReceiptItem
{
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
```

### Categorias Pré-definidas

#### Receitas (Income)
- Salário
- Freelance
- Investimentos
- Aluguel Recebido
- Bônus
- Outros

#### Despesas (Expense)
- Alimentação
- Transporte
- Moradia
- Saúde
- Educação
- Lazer
- Compras
- Contas Fixas
- Outros

---

## 🔌 Endpoints - Transações

### 1. Criar Transação

**POST** `/api/Transactions`

**Autorização**: Bearer Token

#### Body (Receita):
```json
{
  "type": "Income",
  "value": 5000.00,
  "description": "Salário de Outubro",
  "category": "Salário",
  "date": "2025-10-25"
}
```

#### Body (Despesa):
```json
{
  "type": "Expense",
  "value": 125.50,
  "description": "Supermercado Extra",
  "category": "Alimentação",
  "expenseType": "Variaveis",
  "date": "2025-11-04",
  "hasReceipt": false
}
```

#### Resposta (201 Created):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "type": "Expense",
  "value": 125.50,
  "description": "Supermercado Extra",
  "category": "Alimentação",
  "expenseType": "Variaveis",
  "date": "2025-11-04",
  "hasReceipt": false,
  "receiptUrl": null,
  "createdAt": "2025-11-04T10:30:00Z",
  "updatedAt": "2025-11-04T10:30:00Z"
}
```

---

### 2. Listar Transações

**GET** `/api/Transactions`

#### Query Parameters:
- `type` (opcional): `Income` ou `Expense`
- `startDate` (opcional): Data inicial (ISO 8601)
- `endDate` (opcional): Data final (ISO 8601)
- `category` (opcional): Filtrar por categoria
- `expenseType` (opcional): `Fixas`, `Variaveis`, `Desnecessarios`
- `page` (opcional): Número da página (padrão: 1)
- `limit` (opcional): Itens por página (padrão: 20)

#### Exemplo:
```
GET /api/Transactions?type=Expense&startDate=2025-10-01&endDate=2025-10-31&page=1&limit=20
```

#### Resposta (200 OK):
```json
{
  "transactions": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "type": "Expense",
      "value": 125.50,
      "description": "Supermercado Extra",
      "category": "Alimentação",
      "expenseType": "Variaveis",
      "date": "2025-11-04",
      "hasReceipt": true,
      "receiptUrl": "https://storage.zetafin.com/receipts/550e8400.jpg",
      "createdAt": "2025-11-04T10:30:00Z"
    }
  ],
  "pagination": {
    "currentPage": 1,
    "totalPages": 5,
    "totalItems": 100,
    "itemsPerPage": 20
  },
  "summary": {
    "totalIncome": 8450.00,
    "totalExpense": 4320.00,
    "balance": 4130.00
  }
}
```

---

### 3. Obter Transação por ID

**GET** `/api/Transactions/{id}`

#### Resposta (200 OK):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "type": "Expense",
  "value": 125.50,
  "description": "Supermercado Extra",
  "category": "Alimentação",
  "expenseType": "Variaveis",
  "date": "2025-11-04",
  "hasReceipt": true,
  "receiptUrl": "https://storage.zetafin.com/receipts/550e8400.jpg",
  "ocrData": {
    "extractedValue": 125.50,
    "merchantName": "Supermercado Extra",
    "confidence": 0.95
  },
  "createdAt": "2025-11-04T10:30:00Z",
  "updatedAt": "2025-11-04T10:30:00Z"
}
```

---

### 4. Atualizar Transação

**PUT** `/api/Transactions/{id}`

#### Body:
```json
{
  "value": 150.00,
  "description": "Supermercado Extra - Atualizado",
  "category": "Alimentação",
  "date": "2025-11-05"
}
```

#### Resposta (200 OK):
```json
{
  "message": "Transação atualizada com sucesso",
  "data": { /* transação atualizada */ }
}
```

---

### 5. Deletar Transação

**DELETE** `/api/Transactions/{id}`

#### Resposta (200 OK):
```json
{
  "message": "Transação deletada com sucesso"
}
```

---

### 6. Obter Resumo Financeiro

**GET** `/api/Transactions/summary`

#### Query Parameters:
- `startDate` (opcional): Data inicial
- `endDate` (opcional): Data final
- `month` (opcional): Mês específico (formato: YYYY-MM)

#### Resposta (200 OK):
```json
{
  "period": {
    "startDate": "2025-11-01",
    "endDate": "2025-11-30"
  },
  "income": {
    "total": 8450.00,
    "count": 2,
    "byCategory": {
      "Salário": 8000.00,
      "Freelance": 450.00
    }
  },
  "expense": {
    "total": 4320.00,
    "count": 45,
    "byCategory": {
      "Alimentação": 1200.00,
      "Transporte": 450.00,
      "Lazer": 320.00
    },
    "byType": {
      "Fixas": 2050.00,
      "Variaveis": 1870.00,
      "Desnecessarios": 400.00
    }
  },
  "balance": 4130.00,
  "savingsRate": 0.488
}
```

---

## 📸 Endpoints - OCR

### 1. Upload de Recibo

**POST** `/api/Receipts/upload`

**Content-Type**: `multipart/form-data`

#### Form Data:
- `file`: arquivo (JPG, PNG, PDF)
- `transactionId` (opcional): UUID da transação existente

#### Resposta (201 Created):
```json
{
  "id": "receipt-uuid",
  "transactionId": "transaction-uuid",
  "fileName": "recibo_20251104.jpg",
  "fileUrl": "https://storage.zetafin.com/receipts/receipt-uuid.jpg",
  "fileSize": 2048576,
  "mimeType": "image/jpeg",
  "ocrProcessed": true,
  "ocrData": {
    "extractedValue": 125.50,
    "extractedDate": "2025-11-04",
    "merchantName": "Supermercado Extra",
    "items": [
      {
        "name": "Arroz",
        "quantity": 2,
        "unitPrice": 25.00,
        "totalPrice": 50.00
      },
      {
        "name": "Feijão",
        "quantity": 3,
        "unitPrice": 8.50,
        "totalPrice": 25.50
      }
    ],
    "confidence": 0.95
  },
  "createdAt": "2025-11-04T10:30:00Z"
}
```

---

### 2. Processar OCR Manualmente

**POST** `/api/Receipts/{receiptId}/process-ocr`

Reprocessa o OCR de um recibo específico.

#### Resposta (200 OK):
```json
{
  "message": "OCR processado com sucesso",
  "data": {
    "receiptId": "receipt-uuid",
    "ocrData": { /* dados extraídos */ }
  }
}
```

---

### 3. Criar Transação a partir do OCR

**POST** `/api/Receipts/{receiptId}/create-transaction`

Cria uma transação baseada nos dados extraídos do OCR.

#### Body (opcional - sobrescrever dados do OCR):
```json
{
  "category": "Alimentação",
  "expenseType": "Variaveis",
  "description": "Compras do mês"
}
```

#### Resposta (201 Created):
```json
{
  "message": "Transação criada a partir do recibo",
  "transaction": { /* dados da transação */ },
  "receipt": { /* dados do recibo */ }
}
```

---

## 🧱 Implementação Backend

### DTOs Necessários

Crie os seguintes arquivos em `ZetaFin.Application/DTOs/`:

#### CreateTransactionDto.cs
```csharp
namespace ZetaFin.Application.DTOs;

public class CreateTransactionDto
{
    public TransactionType Type { get; set; }
    public decimal Value { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public ExpenseType? ExpenseType { get; set; }
    public DateTime Date { get; set; }
    public bool HasReceipt { get; set; }
}
```

#### TransactionDto.cs
```csharp
namespace ZetaFin.Application.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Value { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public ExpenseType? ExpenseType { get; set; }
    public DateTime Date { get; set; }
    public bool HasReceipt { get; set; }
    public string? ReceiptUrl { get; set; }
    public ReceiptOcrData? OcrData { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

#### ReceiptDto.cs
```csharp
namespace ZetaFin.Application.DTOs;

public class ReceiptDto
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public bool OcrProcessed { get; set; }
    public ReceiptOcrData? OcrData { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

#### TransactionSummaryDto.cs
```csharp
namespace ZetaFin.Application.DTOs;

public class TransactionSummaryDto
{
    public PeriodDto Period { get; set; } = new();
    public IncomeSummaryDto Income { get; set; } = new();
    public ExpenseSummaryDto Expense { get; set; } = new();
    public decimal Balance { get; set; }
    public double SavingsRate { get; set; }
}

public class PeriodDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class IncomeSummaryDto
{
    public decimal Total { get; set; }
    public int Count { get; set; }
    public Dictionary<string, decimal> ByCategory { get; set; } = new();
}

public class ExpenseSummaryDto
{
    public decimal Total { get; set; }
    public int Count { get; set; }
    public Dictionary<string, decimal> ByCategory { get; set; } = new();
    public Dictionary<string, decimal> ByType { get; set; } = new();
}
```

---

### Services

#### ITransactionService.cs
```csharp
namespace ZetaFin.Application.Interfaces;

public interface ITransactionService
{
    Task<TransactionDto> CreateAsync(CreateTransactionDto dto, Guid userId);
    Task<PagedResult<TransactionDto>> GetAllAsync(TransactionFilterDto filter, Guid userId);
    Task<TransactionDto?> GetByIdAsync(Guid id, Guid userId);
    Task<TransactionDto> UpdateAsync(Guid id, UpdateTransactionDto dto, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<TransactionSummaryDto> GetSummaryAsync(SummaryFilterDto filter, Guid userId);
}
```

#### IReceiptService.cs
```csharp
namespace ZetaFin.Application.Interfaces;

public interface IReceiptService
{
    Task<ReceiptDto> UploadAsync(IFormFile file, Guid userId, Guid? transactionId = null);
    Task<ReceiptDto> ProcessOcrAsync(Guid receiptId, Guid userId);
    Task<TransactionDto> CreateTransactionFromReceiptAsync(Guid receiptId, CreateTransactionFromReceiptDto dto, Guid userId);
}
```

---

### Controllers

#### TransactionsController.cs
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;

namespace ZetaFin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto dto)
    {
        var transaction = await _transactionService.CreateAsync(dto, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TransactionFilterDto filter)
    {
        var result = await _transactionService.GetAllAsync(filter, GetUserId());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var transaction = await _transactionService.GetByIdAsync(id, GetUserId());
        return transaction == null ? NotFound() : Ok(transaction);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTransactionDto dto)
    {
        var transaction = await _transactionService.UpdateAsync(id, dto, GetUserId());
        return Ok(transaction);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _transactionService.DeleteAsync(id, GetUserId());
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] SummaryFilterDto filter)
    {
        var summary = await _transactionService.GetSummaryAsync(filter, GetUserId());
        return Ok(summary);
    }
}
```

#### ReceiptsController.cs
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;

namespace ZetaFin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptService _receiptService;

    public ReceiptsController(IReceiptService receiptService)
    {
        _receiptService = receiptService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] Guid? transactionId = null)
    {
        var receipt = await _receiptService.UploadAsync(file, GetUserId(), transactionId);
        return CreatedAtAction(nameof(Upload), new { id = receipt.Id }, receipt);
    }

    [HttpPost("{receiptId}/process-ocr")]
    public async Task<IActionResult> ProcessOcr(Guid receiptId)
    {
        var receipt = await _receiptService.ProcessOcrAsync(receiptId, GetUserId());
        return Ok(new { message = "OCR processado com sucesso", data = receipt });
    }

    [HttpPost("{receiptId}/create-transaction")]
    public async Task<IActionResult> CreateTransaction(Guid receiptId, [FromBody] CreateTransactionFromReceiptDto dto)
    {
        var transaction = await _receiptService.CreateTransactionFromReceiptAsync(receiptId, dto, GetUserId());
        return CreatedAtAction(nameof(CreateTransaction), new { id = transaction.Id }, transaction);
    }
}
```

---

## 🗄️ Banco de Dados

### Migrations

Adicione a migration para as novas tabelas:

```bash
cd ZetaFin.Persistence
dotnet ef migrations add AddTransactionsAndReceipts --startup-project ../ZetaFin/ZetaFin.API.csproj
dotnet ef database update --startup-project ../ZetaFin/ZetaFin.API.csproj
```

### Configuração do DbContext

Atualize `ApplicationDbContext.cs`:

```csharp
public DbSet<Transaction> Transactions { get; set; }
public DbSet<Receipt> Receipts { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Configuração de Transaction
    modelBuilder.Entity<Transaction>()
        .Property(t => t.Value)
        .HasColumnType("decimal(18,2)");

    modelBuilder.Entity<Transaction>()
        .HasOne(t => t.User)
        .WithMany()
        .HasForeignKey(t => t.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Transaction>()
        .HasOne(t => t.Receipt)
        .WithOne(r => r.Transaction)
        .HasForeignKey<Receipt>(r => r.TransactionId);

    // Configuração de Receipt
    modelBuilder.Entity<Receipt>()
        .Property(r => r.OcrData)
        .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            v => JsonSerializer.Deserialize<ReceiptOcrData>(v, (JsonSerializerOptions)null));
}
```

---

## 🎨 Integração Frontend

### Exemplos de Requisições (TypeScript/JavaScript)

#### 1. Criar Despesa
```typescript
const createExpense = async (data: CreateTransactionDto) => {
  const response = await fetch('http://localhost:5291/api/Transactions', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(data)
  });
  return response.json();
};

// Uso
await createExpense({
  type: 'Expense',
  value: 125.50,
  description: 'Supermercado Extra',
  category: 'Alimentação',
  expenseType: 'Variaveis',
  date: '2025-11-04',
  hasReceipt: false
});
```

#### 2. Upload de Recibo com OCR
```typescript
const uploadReceipt = async (file: File, transactionId?: string) => {
  const formData = new FormData();
  formData.append('file', file);
  if (transactionId) {
    formData.append('transactionId', transactionId);
  }

  const response = await fetch('http://localhost:5291/api/Receipts/upload', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`
    },
    body: formData
  });
  return response.json();
};
```

#### 3. Obter Resumo Mensal
```typescript
const getSummary = async (month: string) => {
  const response = await fetch(
    `http://localhost:5291/api/Transactions/summary?month=${month}`,
    {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    }
  );
  return response.json();
};

// Uso
const summary = await getSummary('2025-11');
console.log(`Saldo: R$ ${summary.balance}`);
console.log(`Taxa de poupança: ${(summary.savingsRate * 100).toFixed(2)}%`);
```

---

## ✅ Validações e Regras

### Regras de Negócio

1. **Valores**
   - Devem ser sempre > 0
   - Decimais com 2 casas (formato monetário)

2. **Datas**
   - Não podem ser futuras para despesas
   - Receitas podem ter data futura (salários agendados)

3. **Categorias**
   - Devem estar na lista pré-definida
   - Caso não encontre, usar "Outros"

4. **Tipo de Despesa**
   - `ExpenseType` obrigatório apenas para `Type = Expense`
   - `null` para receitas

5. **Recibos**
   - Formatos aceitos: JPG, PNG, PDF
   - Tamanho máximo: 10MB
   - OCR automático após upload

6. **Segurança**
   - Usuário só pode acessar suas próprias transações
   - Validação de JWT em todos os endpoints

---

## 🚀 Próximos Passos

1. **Implementar os serviços** (`TransactionService`, `ReceiptService`)
2. **Configurar OCR** (Google Cloud Vision ou Tesseract)
3. **Implementar upload de arquivos** (Azure Blob, AWS S3 ou local)
4. **Criar testes unitários** para as novas funcionalidades
5. **Documentar no Swagger** (adicionar anotações XML)
6. **Integrar com o frontend** (Flutter ou React)

---

## 📞 Suporte

Para dúvidas ou sugestões sobre esta funcionalidade:

📧 **Email**: lucas_likes@hotmail.com  
💼 **LinkedIn**: [Lucas Gabriel Likes](https://www.linkedin.com/in/lucas-gabriel-likes-06a2b9182/)

---

**Versão**: 1.0.0  
**Última Atualização**: Novembro 2025
