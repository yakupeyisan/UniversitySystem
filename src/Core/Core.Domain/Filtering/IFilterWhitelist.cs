namespace Core.Domain.Filtering;
public interface IFilterWhitelist
{
    bool IsAllowed(string propertyName);
    IReadOnlySet<string> GetAllowedProperties();
}