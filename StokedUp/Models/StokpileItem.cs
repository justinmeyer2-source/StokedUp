using SQLite;

namespace StokedUp.Models
{
    [Table("StokpileItems")]
    public class StokpileItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; } = string.Empty;

        [NotNull]
        public double Quantity { get; set; }

        public string Unit { get; set; } = string.Empty;

        [NotNull]
        public string Category {  get; set; } = string.Empty;

        public DateTime ExpirationDate { get; set; }

        public string Notes { get; set; } = string.Empty;
    }
}
