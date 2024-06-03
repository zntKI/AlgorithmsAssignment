using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ExcellentDungeon : SufficientDungeon
{
    public ExcellentDungeon(Size pSize) : base(pSize)
    {
    }

    protected override void generate(int pMinimumRoomSize)
    {
        base.generate(pMinimumRoomSize);

        OffsetRooms();
    }

    void OffsetRooms()
    {
    }

    protected override void drawRooms(IEnumerable<Room> pRooms, Pen pWallColor, Brush pFillColor = null)
    {
        Brush fill = new SolidBrush(Color.White);
        foreach (Room room in pRooms)
        {
            drawRoom(room, pWallColor, fill);
        }
    }
}
