namespace ii.FirstPeace.Model;

public class CharacterCanEnter
{
    public int Id { get; set; }
    public int CanEnterId { get; set; }
    public int ReturnValue { get; set; }
    public int ReturnToId { get; set; }
    public int Parent { get; set; }
    public int ProductionValue { get; set; }
}