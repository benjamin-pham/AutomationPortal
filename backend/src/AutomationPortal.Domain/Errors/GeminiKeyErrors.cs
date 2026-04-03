using AutomationPortal.Domain.Abstractions;

namespace AutomationPortal.Domain.Errors;

public static class GeminiKeyErrors
{
    public static readonly Error NotFound =
        new("GeminiKey.NotFound", "Gemini Key không tồn tại.");

    public static readonly Error NameAlreadyExists =
        new("GeminiKey.NameAlreadyExists", "Tên Gemini Key đã được sử dụng.");

    public static readonly Error UserAlreadyHasKey =
        new("GeminiKey.UserAlreadyHasKey", "Người dùng này đã có Gemini Key.");
}
