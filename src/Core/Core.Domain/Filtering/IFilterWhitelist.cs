namespace Core.Domain.Filtering;

/// <summary>
/// Filter yapısında kullanılabilecek property'leri whitelist'ler
/// 
/// Amaç:
/// - Reflection hack'lerini prevent etmek
/// - Unauthorized property access'i block etmek
/// - Soft delete, audit fields'ları filter'dan koru
/// 
/// Örnek:
/// AllowedProperties = { "FirstName", "Email", "CreatedAt", "Address.City" }
/// "DeletedBy|eq|admin" → DENIED (not in whitelist)
/// "FirstName|contains|John" → ALLOWED
/// </summary>
public interface IFilterWhitelist
{
    /// <summary>
    /// Property'nin filter'ında kullanılabilir olup olmadığını kontrol et
    /// </summary>
    bool IsAllowed(string propertyName);

    /// <summary>
    /// Tüm izin verilen properties'i getir
    /// </summary>
    IReadOnlySet<string> GetAllowedProperties();
}