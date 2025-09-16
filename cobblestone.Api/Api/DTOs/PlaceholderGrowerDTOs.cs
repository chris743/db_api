namespace Api.DTOs;

public record PlaceholderGrowerDto(
    Guid id,
    string grower_name,
    string commodity_name,
    DateTime created_at,
    DateTime? updated_at,
    bool is_active,
    string? notes
);

public record PlaceholderGrowerCreateDto(
    string grower_name,
    string commodity_name,
    bool is_active = true,
    string? notes = null
);

public record PlaceholderGrowerUpdateDto(
    string? grower_name,
    string? commodity_name,
    bool? is_active,
    string? notes
);
