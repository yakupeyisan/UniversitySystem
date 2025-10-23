namespace Core.Domain.Specifications;

/// <summary>
/// ISoftDelete interface - Soft delete pattern'ı standardize ediyor
/// 
/// Repository'ler ve specifications'lar bu interface'i kullanmalı
/// </summary>
public interface ISoftDelete
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get;}
    Guid DeletedBy { get; }

    void Delete(Guid deletedBy);
    void Restore();
}