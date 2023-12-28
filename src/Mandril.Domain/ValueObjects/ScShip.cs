namespace Mandril.Domain.ValueObjects
{
    public class ScShip
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required float Price { get; set; }
        public required bool IsConcept { get; set; }
        public List<Ccu> CcuList { get; set; } = [];
    }
}
