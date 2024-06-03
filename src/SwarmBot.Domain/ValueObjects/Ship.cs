using SwarmBot.Domain.ValueObjects;

namespace SwarmBot.Domain.ValueObjects
{
    public class Ship
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required float Price { get; set; }
        public required string FlyableStatus { get; set; }
        public required ShipImages Images { get; set; }
        public required ShipManufacturer Manufacturer { get; set; }
        public required string Focus { get; set; }
        public required string Type { get; set; }
        public required string Link { get; set; }
        public List<ShipCcu> CcuList { get; set; } = [];

        public List<ShipStandalone> StandaloneList { get; set; } = [];
    }
}
