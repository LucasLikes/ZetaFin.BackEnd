using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ZetaFin.Application.Interfaces;

namespace ZetaFin.Infrastructure.Services;

/// <summary>
/// Implementação local do serviço de armazenamento.
/// Para produção, use AWS S3, Azure Blob Storage ou Google Cloud Storage.
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly string _baseUrl;

    public LocalFileStorageService(string basePath, string baseUrl)
    {
        _basePath = basePath;
        _baseUrl = baseUrl;

        // Criar diretório se não existir
        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Arquivo inválido");

        // Validar tamanho (10MB)
        if (file.Length > 10_485_760)
            throw new ArgumentException("Arquivo excede o limite de 10MB");

        // Validar tipo de arquivo
        var allowedTypes = new[] { "image/jpeg", "image/png", "application/pdf" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            throw new ArgumentException("Formato de arquivo não suportado");

        // Gerar nome único
        var fileExtension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var folderPath = Path.Combine(_basePath, folder);

        // Criar pasta se não existir
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, uniqueFileName);

        // Salvar arquivo
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Retornar URL pública
        return $"{_baseUrl}/{folder}/{uniqueFileName}";
    }

    public Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            // Extrair caminho do arquivo da URL
            var uri = new Uri(fileUrl);
            var relativePath = uri.AbsolutePath.TrimStart('/');
            var filePath = Path.Combine(_basePath, relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<byte[]> DownloadFileAsync(string fileUrl)
    {
        try
        {
            var uri = new Uri(fileUrl);
            var relativePath = uri.AbsolutePath.TrimStart('/');
            var filePath = Path.Combine(_basePath, relativePath);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Arquivo não encontrado");

            return await File.ReadAllBytesAsync(filePath);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao baixar arquivo: {ex.Message}");
        }
    }
}