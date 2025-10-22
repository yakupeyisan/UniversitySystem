namespace Core.Application.Abstractions;

/// <summary>
/// IApplicationService - Application Service base interface
/// 
/// Sorumluluğu:
/// - Reusable application services için base interface sağlamak
/// - Use case implementation'lar bu interface'i implement etmeli
/// 
/// Kullanım:
/// - Custom application services (cross-cutting) için
/// - Command/Query handlers'dan farklı
/// 
/// Örnekler:
/// - IEmailService (Email gönderme)
/// - INotificationService (Notification gönderme)
/// - IReportService (Report oluşturma)
/// 
/// Not:
/// - Sadece interface'dir, marker role'ü var
/// - Concrete implementations'lar specific interfaces implement et
/// </summary>
public interface IApplicationService
{
    // Marker interface - herhangi bir method'u yoktur
    // Tag olarak kullanılır DI container'da
}