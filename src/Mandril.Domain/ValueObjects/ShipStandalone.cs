namespace Mandril.Domain.ValueObjects
{
    public class ShipStandalone
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Title { get; set; }
        public required string Subtitle { get; set; }
        public required string Url { get; set; }
        public required string Body { get; set; }
        public required string Excerpt { get; set; }
        public required string Type { get; set; }
        public required ShipImages Images { get; set; }
        public required float Price { get; set; }

        public required float PriceWithTax { get; set; }
        public required string TaxDescription { get; set; }

        public required bool IsWarbond { get; set; }
        public required bool IsPackage { get; set; }
    }
}