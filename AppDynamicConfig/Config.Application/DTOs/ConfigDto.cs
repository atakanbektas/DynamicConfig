namespace Config.Application.DTOs;

public sealed record ConfigDto(
    string ApplicationName,
    string Name,
    string Type,      // "string" | "int" | "double" | "bool"
    string Value,
    bool IsActive);
