using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GoodDungeon : SufficientDungeon
{
    public GoodDungeon(Size pSize) : base(pSize)
    {
    }

    protected override void generate(int pMinimumRoomSize)
    {
        GenerateRooms(pMinimumRoomSize);
        RemoveBiggestAndSmallestRooms();
        GenerateDoors();
        SetRoomsColor();
    }

    void RemoveBiggestAndSmallestRooms()
    {
        //Figure out the biggest and smallest room sizes
        int maxSize = rooms.Max(r => r.area.Width * r.area.Height);
        int minSize = rooms.Min(r => r.area.Width * r.area.Height);

        //Remove rooms meeting the requirements
        for (int i = rooms.Count - 1; i >= 0; i--)
        {
            int roomArea = rooms[i].area.Width * rooms[i].area.Height;
            if (roomArea == maxSize || roomArea == minSize)
            {
                rooms.RemoveAt(i);
            }
        }
    }

    void SetRoomsColor()
    {
        foreach (var room in rooms)
        {
            int countDoors = doors.Count(d => d.roomA == room || d.roomB == room);
            if (countDoors >= 4)
                room.fillColor = new SolidBrush(Color.Purple);
            else
            {
                switch (countDoors)
                {
                    case 3:
                        room.fillColor = new SolidBrush(Color.Green);
                        break;
                    case 2:
                        room.fillColor = new SolidBrush(Color.Yellow);
                        break;
                    case 1:
                        room.fillColor = new SolidBrush(Color.Orange);
                        break;
                    case 0:
                        room.fillColor = new SolidBrush(Color.Red);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    protected override void drawRooms(IEnumerable<Room> pRooms, Pen pWallColor, Brush pFillColor = null)
    {
        foreach (Room room in pRooms)
        {
            drawRoom(room, pWallColor, room.fillColor);
        }
    }
}
