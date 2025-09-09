using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain
{
    // This is a view; it has no primary key.
    [Keyless]
    [Table("VW_ScoutReportWithBlock", Schema = "dbo")]
    public class ScoutReportWithBlock
    {
        [Column("ScoutReportId")]
        public long? ScoutReportId { get; set; }

        [Column("Scouting Report Number")]
        public string? ScoutingReportNumber { get; set; }

        [Column("Created Time")]
        public DateTime? CreatedTime { get; set; }

        [Column("Block Number")]
        public long? BlockNumber { get; set; }

        [Column("Block Number Name")]
        public string? BlockNumberName { get; set; }

        [Column("Est Pack Out")]
        [Precision(5, 2)]
        public decimal? EstPackOut { get; set; }

        [Column("Peak Size")]
        public int? PeakSize { get; set; }

        [Column("Brix")]
        [Precision(16, 2)]
        public decimal? Brix { get; set; }

        [Column("Size Pick")]
        public bool? SizePick { get; set; }

        [Column("Acidity")]
        [Precision(16, 9)]
        public decimal? Acidity { get; set; }

        [Column("Temperature (F)")]
        [Precision(16, 9)]
        public decimal? Temperature_F { get; set; }

        [Column("Humidity")]
        [Precision(16, 9)]
        public decimal? Humidity { get; set; }

        [Column("Irrigation Status")]
        public string? IrrigationStatus { get; set; }

        [Column("Follow Up Date")]
        public DateTime? FollowUpDate { get; set; }

        [Column("Grower Number")]
        public long? GrowerNumber { get; set; }

        [Column("Observations")]
        public string? Observations { get; set; }

        [Column("Private Notes")]
        public string? PrivateNotes { get; set; }

        [Column("Insect Signs")]
        public string? InsectSigns { get; set; }

        [Column("Est Fancy %")]
        [Precision(5, 2)]
        public decimal? EstFancyPct { get; set; }

        [Column("Color Index")]
        public int? ColorIndex { get; set; }

        [Column("Est Choice %")]
        [Precision(5, 2)]
        public decimal? EstChoicePct { get; set; }

        [Column("Defects")]
        public string? Defects { get; set; }

        [Column("Second Peak Size")]
        public int? SecondPeakSize { get; set; }

        [Column("Mandarin 44")]
        public int? Mandarin44 { get; set; }

        [Column("Scouting Bin Change")]
        public string? ScoutingBinChange { get; set; }

        [Column("Firmness")]
        public string? Firmness { get; set; }

        [Column("Harvest Status")]
        public string? HarvestStatus { get; set; }

        [Column("Mandarin 24")]
        public int? Mandarin24 { get; set; }

        [Column("Mandarin 28")]
        public int? Mandarin28 { get; set; }

        [Column("Mandarin 18")]
        public int? Mandarin18 { get; set; }

        [Column("Mandarin 21")]
        public int? Mandarin21 { get; set; }

        [Column("Mandarin 15")]
        public int? Mandarin15 { get; set; }

        [Column("Mandarin 36")]
        public int? Mandarin36 { get; set; }

        [Column("Mandarin 40")]
        public int? Mandarin40 { get; set; }

        [Column("Mandarin 32")]
        public int? Mandarin32 { get; set; }

        [Column("Fruit Sample Size")]
        public string? FruitSampleSize { get; set; }

        [Column("CA STD")]
        public string? CaStd { get; set; }

        [Column("ScoutCommodity")]
        public string? ScoutCommodity { get; set; }

        [Column("Lemon 75")]
        public int? Lemon75 { get; set; }

        [Column("Lemon 63")]
        public int? Lemon63 { get; set; }

        [Column("Raspberry Orange 113")]
        public int? RaspberryOrange113 { get; set; }

        [Column("Pummelo 8")]
        public string? Pummelo8 { get; set; }

        [Column("Raspberry Orange 88")]
        public int? RaspberryOrange88 { get; set; }

        [Column("Pummelo 6")]
        public string? Pummelo6 { get; set; }

        [Column("Raspberry Orange 163")]
        public int? RaspberryOrange163 { get; set; }

        [Column("Pummelo 12")]
        public string? Pummelo12 { get; set; }

        [Column("Raspberry Orange 138")]
        public int? RaspberryOrange138 { get; set; }

        [Column("Pummelo 10")]
        public string? Pummelo10 { get; set; }

        [Column("Navel 40")]
        public string? Navel40 { get; set; }

        [Column("Pummelo 18")]
        public string? Pummelo18 { get; set; }

        [Column("Navel 36")]
        public string? Navel36 { get; set; }

        [Column("Pummelo 14")]
        public string? Pummelo14 { get; set; }

        [Column("Lemon 115")]
        public int? Lemon115 { get; set; }

        [Column("Navel 72")]
        public string? Navel72 { get; set; }

        [Column("Pummelo 27")]
        public string? Pummelo27 { get; set; }

        [Column("Lemon 95")]
        public int? Lemon95 { get; set; }

        [Column("Navel 48")]
        public string? Navel48 { get; set; }

        [Column("Pummelo 23")]
        public string? Pummelo23 { get; set; }

        [Column("Lemon 165")]
        public int? Lemon165 { get; set; }

        [Column("Navel 113")]
        public string? Navel113 { get; set; }

        [Column("Lemon 140")]
        public int? Lemon140 { get; set; }

        [Column("Navel 88")]
        public string? Navel88 { get; set; }

        [Column("Lemon 235")]
        public int? Lemon235 { get; set; }

        [Column("Lemon 200")]
        public int? Lemon200 { get; set; }

        [Column("Navel 138")]
        public string? Navel138 { get; set; }

        [Column("Minneola 48")]
        public int? Minneola48 { get; set; }

        [Column("Grapefruit 18")]
        public int? Grapefruit18 { get; set; }

        [Column("Navel 163")]
        public string? Navel163 { get; set; }

        [Column("Minneola 64")]
        public int? Minneola64 { get; set; }

        [Column("Grapefruit 27")]
        public string? Grapefruit27 { get; set; }

        [Column("Minneola 56")]
        public int? Minneola56 { get; set; }

        [Column("Grapefruit 23")]
        public string? Grapefruit23 { get; set; }

        [Column("Minneola 100")]
        public int? Minneola100 { get; set; }

        [Column("Grapefruit 36")]
        public string? Grapefruit36 { get; set; }

        [Column("Minneola 80")]
        public int? Minneola80 { get; set; }

        [Column("Grapefruit 32")]
        public string? Grapefruit32 { get; set; }

        [Column("Minneola 150")]
        public int? Minneola150 { get; set; }

        [Column("Grapefruit 48")]
        public string? Grapefruit48 { get; set; }

        [Column("Minneola 125")]
        public int? Minneola125 { get; set; }

        [Column("Grapefruit 40")]
        public string? Grapefruit40 { get; set; }

        [Column("Raspberry Orange 56")]
        public int? RaspberryOrange56 { get; set; }

        [Column("Raspberry Orange 48")]
        public int? RaspberryOrange48 { get; set; }

        [Column("Grapefruit 56")]
        public int? Grapefruit56 { get; set; }

        [Column("Raspberry Orange 72")]
        public int? RaspberryOrange72 { get; set; }

        [Column("Pummelo 4")]
        public string? Pummelo4 { get; set; }

        [Column("Grapefruit 64")]
        public string? Grapefruit64 { get; set; }

        [Column("Navel 56")]
        public string? Navel56 { get; set; }

        [Column("Mandarin 44 %")]
        [Precision(5, 2)]
        public decimal? Mandarin44Pct { get; set; }

        [Column("Size 1")]
        public string? Size1 { get; set; }

        [Column("Size 2")]
        public string? Size2 { get; set; }

        [Column("Size 4")]
        public string? Size4 { get; set; }

        [Column("Size 3")]
        public string? Size3 { get; set; }

        [Column("Size 6")]
        public string? Size6 { get; set; }

        [Column("Size 5")]
        public string? Size5 { get; set; }

        [Column("Size 7")]
        public string? Size7 { get; set; }

        [Column("Size 8")]
        public string? Size8 { get; set; }

        [Column("Size 9")]
        public string? Size9 { get; set; }

        [Column("Total Sample Size")]
        public string? TotalSampleSize { get; set; }

        [Column("Location")]
        public string? Location { get; set; }

        [Column("Debug Mode")]
        public bool? DebugMode { get; set; }

        [Column("ImageHTML")]
        public string? ImageHtml { get; set; }

        [Column("Size and Count 8")]
        public string? SizeAndCount8 { get; set; }

        [Column("Size and Count 7")]
        public string? SizeAndCount7 { get; set; }

        [Column("Size and Count 10")]
        public string? SizeAndCount10 { get; set; }

        [Column("Size and Count 9")]
        public string? SizeAndCount9 { get; set; }

        [Column("Size and Count 1")]
        public string? SizeAndCount1 { get; set; }

        [Column("Size and Count 11")]
        public string? SizeAndCount11 { get; set; }

        [Column("Size and Count 3")]
        public string? SizeAndCount3 { get; set; }

        [Column("Size and Count 2")]
        public string? SizeAndCount2 { get; set; }

        [Column("Size and Count 5")]
        public string? SizeAndCount5 { get; set; }

        [Column("Size and Count 4")]
        public string? SizeAndCount4 { get; set; }

        [Column("Size and Count 6")]
        public string? SizeAndCount6 { get; set; }

        [Column("Email_Status")]
        public string? EmailStatus { get; set; }

        [Column("Sync_Status")]
        public string? SyncStatus { get; set; }

        [Column("Photo URL 1")]
        public string? PhotoUrl1 { get; set; }

        [Column("Photo URL 2")]
        public string? PhotoUrl2 { get; set; }

        [Column("Photo URL 3")]
        public string? PhotoUrl3 { get; set; }

        [Column("Photo URL 4")]
        public string? PhotoUrl4 { get; set; }

        [Column("Photo URL 5")]
        public string? PhotoUrl5 { get; set; }

        [Column("BlockId")]
        public long? BlockId { get; set; }

        [Column("Block Name")]
        public string? BlockName { get; set; }

        [Column("Owner")]
        public long? Owner { get; set; }

        [Column("Owner Name")]
        public string? OwnerName { get; set; }

        [Column("Modified By")]
        public long? ModifiedBy { get; set; }

        [Column("Modified By Name")]
        public string? ModifiedByName { get; set; }

        [Column("Modified Time")]
        public DateTime? ModifiedTime { get; set; }

        [Column("Last Activity Time")]
        public DateTime? LastActivityTime { get; set; }

        [Column("Tag")]
        public string? Tag { get; set; }

        [Column("Unsubscribed Mode")]
        public string? UnsubscribedMode { get; set; }

        [Column("Unsubscribed Time")]
        public DateTime? UnsubscribedTime { get; set; }

        [Column("Ranch")]
        public long? Ranch { get; set; }

        [Column("Ranch Name")]
        public string? RanchName { get; set; }

        [Column("BlockNumber_Ref")]
        public int? BlockNumber_Ref { get; set; }

        [Column("Locked")]
        public bool? Locked { get; set; }

        [Column("Acres")]
        [Precision(16, 9)]
        public decimal? Acres { get; set; }

        [Column("Row Spacing")]
        public int? RowSpacing { get; set; }

        [Column("Tree Spacing")]
        public int? TreeSpacing { get; set; }

        [Column("Variety")]
        public string? Variety { get; set; }

        [Column("BlockGrowerNumber")]
        public long? BlockGrowerNumber { get; set; }

        [Column("BlockGrowerName")]
        public string? BlockGrowerName { get; set; }

        [Column("Latitude")]
        [Precision(16, 2)]
        public decimal? Latitude { get; set; }

        [Column("Longitude")]
        [Precision(16, 2)]
        public decimal? Longitude { get; set; }

        [Column("Rootstock")]
        public string? Rootstock { get; set; }

        [Column("Estimated Bins")]
        public int? EstimatedBins { get; set; }

        [Column("Past Holding Conditions")]
        public string? PastHoldingConditions { get; set; }

        [Column("Remaining Bins")]
        public int? RemainingBins { get; set; }

        [Column("Grower Number Link")]
        public int? GrowerNumberLink { get; set; }

        [Column("Famous Block ID")]
        public int? FamousBlockId { get; set; }

        [Column("Block Inactive")]
        public bool? BlockInactive { get; set; }

        [Column("BlockCommodity")]
        public string? BlockCommodity { get; set; }
    }
}
