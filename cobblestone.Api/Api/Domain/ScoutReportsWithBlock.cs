using System;

namespace Api.Domain
{
    // Keyless because it's a view (no primary key). If your view has a stable unique key, you can map it as a key later.
    public class ScoutReportWithBlock
    {
        public int ScoutReportId { get; set; }
        public string ScoutingReportNumber { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string BlockNumber { get; set; }
        public string BlockNumberName { get; set; }
        public string EstPackOut { get; set; }
        public string PeakSize { get; set; }
        public decimal? Brix { get; set; }
        public string SizePick { get; set; }
        public string Acidity { get; set; }
        public decimal? Temperature_F { get; set; }
        public decimal? Humidity { get; set; }
        public string IrrigationStatus { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string GrowerNumber { get; set; }
        public string GrowerNumberName { get; set; }
        public string Observations { get; set; }
        public string PrivateNotes { get; set; }
        public string InsectSigns { get; set; }
        public decimal? EstFancyPct { get; set; }
        public string ColorIndex { get; set; }
        public decimal? EstChoicePct { get; set; }
        public string Defects { get; set; }
        public string SecondPeakSize { get; set; }
        public string HarvestStatus { get; set; }
        // ... include any other columns you need from the view ...

        // Block-side
        public int? BlockId { get; set; }
        public string BlockName { get; set; }
        public string Owner { get; set; }
        public string OwnerName { get; set; }
        public string Ranch { get; set; }
        public string RanchName { get; set; }
        public bool? Locked { get; set; }
        public decimal? Acres { get; set; }
        public string Variety { get; set; }
        public string BlockGrowerNumber { get; set; }
        public string BlockGrowerName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Rootstock { get; set; }
        public int? EstimatedBins { get; set; }
        public int? RemainingBins { get; set; }
        public string BlockCommodity { get; set; }
    }
}
