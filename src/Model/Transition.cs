namespace ii.FirstPeace.Model;

public class Transition
{
    public int Id { get; set; }
    public int TileId { get; set; }
    public int Parent { get; set; }
    public int TransitionId { get; set; }
}