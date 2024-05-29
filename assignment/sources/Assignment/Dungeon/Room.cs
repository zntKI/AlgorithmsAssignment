using System.Collections.Generic;
using System.Drawing;

/**
 * This class represents (the data for) a Room, at this moment only a rectangle in the dungeon.
 */
class Room
{
	public Rectangle area;
	public Brush fillColor;

	public Door[] Doors => doors.ToArray();

    private List<Door> doors;

	public Room (Rectangle pArea)
	{
		area = pArea;
        
		doors = new List<Door>();
    }

	public void AddDoor(Door door)
		=> doors.Add(door);

    public override string ToString()
    {
        return $"Room: {area.Location} Area: {area.Width * area.Height}";
    }
}
