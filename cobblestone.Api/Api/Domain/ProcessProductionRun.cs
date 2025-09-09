namespace Api.Domain
{
    public class ProcessProductionRun
    {
        public Guid id { get; set; }  // UNIQUEIDENTIFIER, default NEWID() in DB

        // Composite FK to Block
        public string source_database { get; set; } = null!;
        public int GABLOCKIDX { get; set; }

        public int bins { get; set; } = 0;

        public DateTime? run_date { get; set; }      // maps to DATE
        public DateTime? pick_date { get; set; }     // maps to DATE

        public string? location { get; set; }
        public string? pool { get; set; }
        public string? notes { get; set; }

        public int? row_order { get; set; }
        public string? run_status { get; set; }
        public string? batch_id { get; set; }

        public DateTime? time_started { get; set; }      // DATETIME2
        public DateTime? time_completed { get; set; }    // DATETIME2

        // Navigation property
        public Block? Block { get; set; }
    }
}
