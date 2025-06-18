namespace IntelliBlog_backend.Domain.Interfaces;

public interface ICookieService
{
    void SetCookie(string key, string value, int? expireDays = null);
    string? GetCookie(string key);
    void DeleteCookie(string key);
}