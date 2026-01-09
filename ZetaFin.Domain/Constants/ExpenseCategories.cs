namespace ZetaFin.Domain.Constants;

/// <summary>
/// Categorias padrão de despesas do sistema
/// </summary>
public static class ExpenseCategories
{
    // Lista de todas as categorias disponíveis
    public static readonly List<string> All = new()
    {
        "Alimentação",
        "Transporte",
        "Moradia",
        "Saúde",
        "Educação",
        "Lazer",
        "Compras",
        "Contas Fixas",
        "Outros"
    };

    /// <summary>
    /// Retorna um dicionário com todas as categorias zeradas
    /// </summary>
    public static Dictionary<string, decimal> GetDefaultSummary()
    {
        return All.ToDictionary(category => category, _ => 0m);
    }

    /// <summary>
    /// Valida se uma categoria existe
    /// </summary>
    public static bool IsValid(string category)
    {
        return All.Contains(category);
    }
}

// Alternativa com ENUM (caso prefira)
public enum ExpenseCategory
{
    Alimentação,
    Transporte,
    Moradia,
    Saúde,
    Educação,
    Lazer,
    Compras,
    ContasFixas,
    Outros
}