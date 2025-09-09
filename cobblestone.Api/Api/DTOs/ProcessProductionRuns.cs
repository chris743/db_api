namespace Api.Contracts;

public record ProductionRunDto(
    Guid id,
    string source_database,
    int gablockidx,
    int bins,
    DateTime? run_date,
    DateTime? pick_date,
    string? location,
    string? pool,
    string? notes,
    int? row_order,
    string? run_status,
    string? batch_id,
    DateTime? time_started,
    DateTime? time_completed,
    BlockInfoDto? block,
    CommodityInfoDto? commodity
);


public record ProductionRunCreateDto
{
    public string source_database { get; init; } = default!;
    public int GABLOCKIDX { get; init; }
    public int? bins { get; init; }
    public DateTime? run_date { get; init; }
    public DateTime? pick_date { get; init; }
    public string? location { get; init; }
    public string? pool { get; init; }
    public string? notes { get; init; }
    public int? row_order { get; init; }
    public string? run_status { get; init; }
    public string? batch_id { get; init; }
    public DateTime? time_started { get; init; }
    public DateTime? time_completed { get; init; }
}

public record ProductionRunUpdateDto
{
    public string? source_database { get; init; }
    public int? GABLOCKIDX { get; init; }
    public int? bins { get; init; }
    public DateTime? run_date { get; init; }
    public DateTime? pick_date { get; init; }
    public string? location { get; init; }
    public string? pool { get; init; }
    public string? notes { get; init; }
    public int? row_order { get; init; }
    public string? run_status { get; init; }
    public string? batch_id { get; init; }
    public DateTime? time_started { get; init; }
    public DateTime? time_completed { get; init; }
}
