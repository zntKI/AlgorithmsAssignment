using GXPEngine;
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
        SetListCapacity(pMinimumRoomSize);

        GenerateRooms(pMinimumRoomSize);
        ModifyRooms(pMinimumRoomSize / 2);
        GenerateDoors();
    }

    void ModifyRooms(int pMinimumRoomSize)
    {
        foreach (var room in rooms)
        {
            //Randomly pick which way to try to reduce first - horizontally or vertically
            bool shouldReduceVertically = Utils.Random(0, 2) == 1;

            Reduce(room, shouldReduceVertically, pMinimumRoomSize);

            //Try to reduce using the given orientation
            //bool isFirstOrientationPossible = TryReduce(room, shouldReduceVertically, pMinimumRoomSize);
            //if (!isFirstOrientationPossible) //Try reducing by the other orientation if the reduction by the first was unsuccessful
            //{
            //    TryReduce(room, !shouldReduceVertically, pMinimumRoomSize);
            //}
        }
    }

    //bool TryReduce(Room room, bool shouldReduceVertically, int pMinimumRoomSize)
    //{
    //    //Checks if it is possible to reduce in the given orientation
    //    bool isReductionImpossible =
    //        (shouldReduceVertically && room.area.Height == pMinimumRoomSize + 1) // '+ 1' For the overlaping of rooms, otherwise '+ 2'
    //        || (!shouldReduceVertically && room.area.Width == pMinimumRoomSize + 1);

    //    if (isReductionImpossible)
    //        return false;

    //    Reduce(room, shouldReduceVertically, pMinimumRoomSize);
    //    return true;
    //}

    void Reduce(Room room, bool shouldReduceVertically, int pMinimumRoomSize)
    {
        bool shouldOffset = Utils.Random(0, 2) == 1;

        if (shouldReduceVertically)
        {
            int reduceRange = room.area.Height - (pMinimumRoomSize + 1);
            int reduceAmount = Utils.Random(1, reduceRange + 1);
            room.area.Height -= reduceAmount;

            if (shouldOffset)
            {
                int offsetAmount = Utils.Random(1, reduceAmount + 1);
                room.area.Y += offsetAmount;
            }
        }
        else
        {
            int reduceRange = room.area.Width - (pMinimumRoomSize + 1);
            int reduceAmount = Utils.Random(1, reduceRange + 1);
            room.area.Width -= reduceAmount;

            if (shouldOffset)
            {
                int offsetAmount = Utils.Random(1, reduceAmount + 1);
                room.area.X += offsetAmount;
            }
        }
    }

    protected override void GenerateDoors()
    {
        //foreach (var room in rooms)
        //{
        //    foreach (var otherRoom in rooms)
        //    {
        //        if (otherRoom == room)
        //            continue;

        //        //Skip room if not neighbouring
        //        (bool isNeighbouringX, bool isNeighbouringY) = CheckForNeighbouring(room, otherRoom);
        //        if (!isNeighbouringX && !isNeighbouringY)
        //            continue;

        //        //Checks if there is already a door between the given rooms
        //        if (CheckForDoorExists(room, otherRoom))
        //            continue;


        //        List<Door> generatedDoors = GenerateDoors(room, otherRoom, isNeighbouringX);

        //        doors.AddRange(generatedDoors);
        //    }
        //}

        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = i + 1; j < rooms.Count; j++)
            {
                if (CheckForRoomInTheMiddle(rooms[i], rooms[j]))
                    continue;

                //Skip room if not neighbouring
                (bool isNeighbouringX, bool isNeighbouringY) = CheckForNeighbouring(rooms[i], rooms[j]);
                if (!isNeighbouringX && !isNeighbouringY)
                    continue;

                //Checks if there is already a door between the given rooms
                //if (CheckForDoorExists(rooms[i], rooms[j])) // NO need for that check anymore since the potential rooms have been skipped due to 'j = i + 1'
                //    continue;


                List<Door> generatedDoors = GenerateDoors(rooms[i], rooms[j], isNeighbouringX);

                doors.AddRange(generatedDoors);
            }
        }
    }

    bool CheckForRoomInTheMiddle(Room room, Room otherRoom)
    {
        if (rooms.Any(r => ((r.area.X > room.area.X && r.area.X < otherRoom.area.X) || (r.area.X > otherRoom.area.X && r.area.X < room.area.X))
                            || ((r.area.Y > room.area.Y && r.area.Y < otherRoom.area.Y) || (r.area.Y > otherRoom.area.Y && r.area.Y < room.area.Y))))
        {
            return true;
        }
        return false;
    }

    protected override (bool, bool) CheckForNeighbouring(Room room, Room otherRoom)
    {
        float difX = Mathf.Abs((room.area.X + room.area.Width / 2f) - (otherRoom.area.X + otherRoom.area.Width / 2f));//Floats because of the possible odd sizes of the rooms
        float difY = Mathf.Abs((room.area.Y + room.area.Height / 2f) - (otherRoom.area.Y + otherRoom.area.Height / 2f));
        bool isNeighbouringX =
            difX <= room.area.Width / 2f + otherRoom.area.Width / 2f + room.area.Width - 2 - 1 //Due to room overlapping
            && otherRoom.area.Y < room.area.Y + room.area.Height - 2 && room.area.Y < otherRoom.area.Y + otherRoom.area.Height - 2; // '- 2' -> In order to prevent doors spawning on corners or borders
        bool isNeighbouringY =
            difY <= room.area.Height / 2f + otherRoom.area.Height / 2f + room.area.Height - 2 - 1 //Due to room overlapping
            && otherRoom.area.X < room.area.X + room.area.Width - 2 && room.area.X < otherRoom.area.X + otherRoom.area.Width - 2; // '- 2' -> In order to prevent doors spawning on corners or borders

        return (isNeighbouringX, isNeighbouringY);
    }

    protected List<Door> GenerateDoors(Room room, Room otherRoom, bool isNeighbouringX)
    {
        int overlapStart = 0;
        int overlapEnd = 0;
        int doorX = 0;
        int doorY = 0;

        List<Door> doors = new List<Door>();
        if (isNeighbouringX)
        {
            overlapStart = Math.Max(room.area.Y, otherRoom.area.Y) + 1;
            overlapEnd = Math.Min(room.area.Y + room.area.Height, otherRoom.area.Y + otherRoom.area.Height) - 2; // '- 2' -> In order to prevent doors spawning on corners or borders
            doorY = Utils.Random(overlapStart, overlapEnd + 1);

            bool isOtherRoomFromTheRight = otherRoom.area.X > room.area.X ? true : false;

            int numOfDoors = isOtherRoomFromTheRight ? otherRoom.area.X - (room.area.X + room.area.Width - 2) : room.area.X - (otherRoom.area.X + otherRoom.area.Width - 2);

            int startCoorX = isOtherRoomFromTheRight ? room.area.X + room.area.Width - 1 : otherRoom.area.X + otherRoom.area.Width - 1;
            for (int i = 0; i < numOfDoors; i++)
            {
                doorX = startCoorX + i;

                Door door = new Door(new Point(doorX, doorY));
                door.roomA = room;
                door.roomB = otherRoom;

                doors.Add(door);
            }
        }
        else
        {
            overlapStart = Math.Max(room.area.X, otherRoom.area.X) + 1;
            overlapEnd = Math.Min(room.area.X + room.area.Width, otherRoom.area.X + otherRoom.area.Width) - 2; // '- 2' -> In order to prevent doors spawning on corners or borders
            doorX = Utils.Random(overlapStart, overlapEnd + 1);

            bool isOtherRoomUnder = otherRoom.area.Y > room.area.Y ? true : false;

            int numOfDoors = isOtherRoomUnder ? otherRoom.area.Y - (room.area.Y + room.area.Height - 2) : room.area.Y - (otherRoom.area.Y + otherRoom.area.Height - 2);

            int startCoorY = isOtherRoomUnder ? room.area.Y + room.area.Height - 1 : otherRoom.area.Y + otherRoom.area.Height - 1;
            for (int i = 0; i < numOfDoors; i++)
            {
                doorY = startCoorY + i;

                Door door = new Door(new Point(doorX, doorY));
                door.roomA = room;
                door.roomB = otherRoom;

                doors.Add(door);
            }
        }

        //Door door = new Door(new Point(doorX, doorY));
        //door.roomA = room;
        //door.roomB = otherRoom;

        return doors;
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
