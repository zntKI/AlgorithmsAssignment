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
                SetTileType(room.area.Left + roomCol, room.area.Top + roomRow, tileTypeIsWall ? TileType.WALL : TileType.GROUND);
            }
        }

        //Loop through each door
        foreach (var door in dungeon.doors)
        {
            SetTileType(door.location.X, door.location.Y, TileType.GROUND);
        }
    }
}
