using System.Collections.Generic;

public class LayerGraph
{
    public List<List<RoomNode>> rooms = new();
    public int LayerCount => rooms.Count;
}