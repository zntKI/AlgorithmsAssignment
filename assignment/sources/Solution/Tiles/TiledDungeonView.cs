using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class TiledDungeonView : SampleTiledView
{
    Dungeon dungeon;

    public TiledDungeonView(Dungeon pDungeon, TileType pDefaultTileType) : base(pDungeon, pDefaultTileType)
    {
        dungeon = pDungeon;
    }

    protected override void generate()
    {
        //SetTileType(i % columns, i / columns, Utils.Random(0, 2) == 1 ? TileType.GROUND : TileType.WALL);

        //Loop through each room
        foreach (var room in dungeon.rooms)
        {
            //Loop through each tile
            for (int i = 0; i < room.area.Width * room.area.Height; ++i)
            {
                //Relative to the room
                int roomCol = i % room.area.Width;
                int roomRow = i / room.area.Width;

                //Check if border
                bool tileTypeIsWall = roomRow == 0 || roomRow == room.area.Height - 1
                    || roomCol == 0 || roomCol == room.area.Width - 1;

                //Relative to the game
                int col = room.area.Left + roomCol;
                int row = room.area.Top + roomRow;

                int tileIndex = row * columns + col;
                SetTileType(tileIndex % columns, tileIndex / columns, tileTypeIsWall ? TileType.WALL : TileType.GROUND);
            }
        }

        //Loop through each door
        foreach (var door in dungeon.doors)
        {
            int tileIndex = door.location.Y * columns + door.location.X;
            SetTileType(tileIndex % columns, tileIndex / columns, TileType.GROUND);
        }
    }
}
