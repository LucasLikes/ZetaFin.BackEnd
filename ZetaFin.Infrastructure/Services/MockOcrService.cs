using Microsoft.AspNetCore.Cors.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;

namespace ZetaFin.Infrastructure.Services;

/// <summary>
/// Implementação mock do serviço de OCR para testes.
/// Para produção, integre com Google Cloud Vision, AWS Textract ou Azure Computer Vision.
/// </summary>
public class MockOcrService : IOcrService
{
    public Task<OcrDataDto> ProcessReceiptAsync(string fileUrl)
    {
        // Simular processamento OCR
        // Em produção, aqui você faria a chamada para a API de OCR real

        var random = new Random();
        var ocrData = new OcrDataDto
        {
            ExtractedValue = Math.Round((decimal)(random.NextDouble() * 500 + 50), 2),
            ExtractedDate = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
            MerchantName = GetRandomMerchant(),
            Items = GenerateRandomItems(),
            Confidence = 0.85 + (random.NextDouble() * 0.15) // 0.85 a 1.0
        };

        return Task.FromResult(ocrData);
    }

    private string GetRandomMerchant()
    {
        var merchants = new[]
        {
            "Supermercado Extra",
            "Padaria São José",
            "Restaurante Bom Gosto",
            "Farmácia Saúde",
            "Posto de Gasolina Shell",
            "Loja de Roupas Fashion",
            "Livraria Cultura",
            "Cafeteria Central"
        };

        return merchants[new Random().Next(merchants.Length)];
    }

    private List<OcrItemDto> GenerateRandomItems()
    {
        var items = new List<OcrItemDto>();
        var random = new Random();
        var itemCount = random.Next(2, 6);

        var possibleItems = new[]
        {
            ("Arroz", 5.0m, 15.0m),
            ("Feijão", 4.0m, 12.0m),
            ("Café", 2.0m, 18.0m),
            ("Leite", 3.0m, 5.0m),
            ("Pão", 5.0m, 8.0m),
            ("Carne", 1.0m, 35.0m),
            ("Frango", 2.0m, 22.0m),
            ("Frutas", 3.0m, 10.0m)
        };

        for (int i = 0; i < itemCount; i++)
        {
            var item = possibleItems[random.Next(possibleItems.Length)];
            var quantity = random.Next(1, 4);
            var unitPrice = item.Item3 + (decimal)(random.NextDouble() * 5 - 2.5);
            unitPrice = Math.Max(1, Math.Round(unitPrice, 2));

            items.Add(new OcrItemDto
            {
                Name = item.Item1,
                Quantity = quantity,
                UnitPrice = unitPrice,
                TotalPrice = quantity * unitPrice
            });
        }

        return items;
    }
}

/// <summary>
/// Implementação real usando Google Cloud Vision (exemplo).
/// Descomente e configure quando for usar em produção.
/// </summary>
/*
public class GoogleCloudVisionOcrService : IOcrService
{
    private readonly ImageAnnotatorClient _client;

    public GoogleCloudVisionOcrService(string credentialsPath)
    {
        var credential = GoogleCredential.FromFile(credentialsPath);
        _client = ImageAnnotatorClient.Create();
    }

    public async Task<OcrDataDto> ProcessReceiptAsync(string fileUrl)
    {
        // Baixar imagem
        using var httpClient = new HttpClient();
        var imageBytes = await httpClient.GetByteArrayAsync(fileUrl);
        var image = Image.FromBytes(imageBytes);

        // Processar com Vision API
        var response = await _client.DetectTextAsync(image);
        var text = response.FirstOrDefault()?.Description;

        // Extrair informações do texto
        var ocrData = ParseReceiptText(text);
        
        return ocrData;
    }

    private OcrDataDto ParseReceiptText(string text)
    {
        // Implementar lógica de parsing do texto extraído
        // Pode usar regex, NLP, ou regras específicas
        
        return new OcrDataDto
        {
            // Preencher com dados extraídos
        };
    }
}
*/