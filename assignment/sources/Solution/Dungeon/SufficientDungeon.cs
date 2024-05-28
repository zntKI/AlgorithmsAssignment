using GXPEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class SufficientDungeon : Dungeon
{
    List<Room> dividedRooms;
    List<Room> currentRoomsToDivide;
    List<Room> newRoomsToDivide;

    public SufficientDungeon(Size pSize) : base(pSize)
    {
        dividedRooms = new List<Room>();
        currentRoomsToDivide = new List<Room>() { new Room(new Rectangle(0, 0, size.Width, size.Height)) };
        newRoomsToDivide = new List<Room>();
    }

    protected override void generate(int pMinimumRoomSize)
    {
        GenerateRooms(pMinimumRoomSize);
        GenerateDoors();
    }

    void GenerateRooms(int pMinimumRoomSize)
    {
        //loop until there are no rooms that can be furher divided
        while (true)
        {
            for (int i = 0; i < currentRoomsToDivide.Count; ++i)
            {
                //Randomly pick which way to try to divide first - horizontally or vertically
                bool shouldDivideVertically = Utils.Random(0, 2) == 1;

                //Try to divide using the given orientation
                bool isFirstOrientationPossible = TryDivide(currentRoomsToDivide[i], shouldDivideVertically, pMinimumRoomSize);
                if (!isFirstOrientationPossible) //Try dividing by the other orientation if the division by the first was unsuccessful
                {
                    TryDivide(currentRoomsToDivide[i], !shouldDivideVertically, pMinimumRoomSize);
                }
            }

            //Switch room list with the newly formed rooms
            currentRoomsToDivide.Clear();
            currentRoomsToDivide.AddRange(newRoomsToDivide);
            newRoomsToDivide.Clear();

            if (currentRoomsToDivide.Count == 0)
                break;
        }

        rooms.AddRange(dividedRooms);
    }

    /// <summary>
    /// Checks if division is possible
    /// </summary>
    /// <param name="room">The room to be divided in two</param>
    /// <returns>If the division was possible</returns>
    bool TryDivide(Room room, bool shouldDivideVertically, int pMinimumRoomSize)
    {
        //Checks if it is possible to divide in the given orientation
        bool isDividingImpossible =
            (shouldDivideVertically && room.area.Width < pMinimumRoomSize * 2 + 1) // '+ 1' For the overlaping of rooms, otherwise '+ 2'
            || (!shouldDivideVertically && room.area.Height < pMinimumRoomSize * 2 + 1);

        if (isDividingImpossible)
            return false;

        List<Room> newRooms = Divide(room, shouldDivideVertically, pMinimumRoomSize);

        //Check if any rooms are further dividable
        for (int i = 0; i < 2; ++i)
        {
            Room roomToCheck = newRooms[i];

            bool isFurtherDividingImpossible =
                roomToCheck.area.Width < pMinimumRoomSize * 2 + 1 // '+ 1' For the overlaping of rooms, otherwise '+ 2'
                && roomToCheck.area.Height < pMinimumRoomSize * 2 + 1;
            if (isFurtherDividingImpossible)
            {
                dividedRooms.Add(roomToCheck);
            }
            else
            {
                newRoomsToDivide.Add(roomToCheck);
            }
        }

        return true;
    }

    /// <summary>
    /// Divides the room into two different rooms
    /// </summary>
    /// <param name="room">The room to be divided in two</param>
    /// <returns>The newly formed rooms</returns>
    List<Room> Divide(Room room, bool shouldDivideVertically, int pMinimumRoomSize)
    {
        int roomSize = shouldDivideVertically ? room.area.Width : room.area.Height;

        int room1Size = 0;
        int room2Size = 0;

        //Divide directly in half, without wasting computational power for generating random size
        if (roomSize == pMinimumRoomSize * 2 + 1) // '+ 1' For the overlaping of rooms, otherwise '+ 2'
        {
            room1Size = pMinimumRoomSize + 1;
            room2Size = pMinimumRoomSize + 1;
        }
        else
        {
            room1Size = Utils.Random(0, roomSize + 1); // '+ 1' For the overlaping of rooms, otherwise '+ 0'
            room2Size = roomSize + 1 - room1Size;
            //Generate random size until it meets the requirements
            while (true)
            {
                if ((room1Size <= pMinimumRoomSize || room2Size <= pMinimumRoomSize) || (room1Size + room2Size - 1 != roomSize)) // For the overlaping of rooms, otherwise 'room1Size <= pMinimumRoomSize || room2Size <= pMinimumRoomSize'
                {
                    room1Size = Utils.Random(0, roomSize + 1); // '+ 1' For the overlaping of rooms, otherwise '+ 0'
                    room2Size = roomSize + 1 - room1Size;
                }
                else
                    break;
            }
        }

        return new List<Room>()
        {
            shouldDivideVertically
                ? new Room(new Rectangle(room.area.X, room.area.Y, room1Size, room.area.Height))
                : new Room(new Rectangle(room.area.X, room.area.Y, room.area.Width, room1Size)),
            shouldDivideVertically
                ? new Room(new Rectangle(room.area.X + room1Size - 1, room.area.Y, room2Size, room.area.Height))
                : new Room(new Rectangle(room.area.X, room.area.Y + room1Size - 1, room.area.Width, room2Size))
        };
    }

    void GenerateDoors()
    {
        foreach (var room in rooms)
        {
            foreach (var otherRoom in rooms)
            {
                if (otherRoom == room)
                    continue;

                //Check if the otherRoom is neighbouring to room
                float difX = Mathf.Abs((room.area.X + room.area.Width / 2f) - (otherRoom.area.X + otherRoom.area.Width / 2f));//Floats because of the possible odd sizes of the rooms
                float difY = Mathf.Abs((room.area.Y + room.area.Height / 2f) - (otherRoom.area.Y + otherRoom.area.Height / 2f));
                bool isNeighbouringX =
                    difX == room.area.Width / 2f + otherRoom.area.Width / 2f - 1 //Due to room overlapping
                    && otherRoom.area.Y < room.area.Y + room.area.Height - 2 && room.area.Y < otherRoom.area.Y + otherRoom.area.Height - 2; // '- 2' -> In order to prevent doors spawning on corners or borders
                bool isNeighbouringY =
                    difY == room.area.Height / 2f + otherRoom.area.Height / 2f - 1 //Due to room overlapping
                    && otherRoom.area.X < room.area.X + room.area.Width - 2 && room.area.X < otherRoom.area.X + otherRoom.area.Width - 2; // '- 2' -> In order to prevent doors spawning on corners or borders

                //Skip room if not neighbouring
                if (!isNeighbouringX && !isNeighbouringY)
                    continue;

                //Checks if there is already a door between the given rooms
                bool doorExists = false;
                foreach (var doorCheck in doors)
                {
                    if ((doorCheck.roomA == room && doorCheck.roomB == otherRoom)
                        || (doorCheck.roomA == otherRoom && doorCheck.roomB == room))
                    {
                        doorExists = true;
                        break;
                    }
                }
                if (doorExists)
                    continue;

                int overlapStart = 0;
                int overlapEnd = 0;
                int doorX = 0;
                int doorY = 0;

                //otherRoom is to the right of room
                if (room.area.X + room.area.Width - 1 == otherRoom.area.X)
                {
                    overlapStart = Math.Max(room.area.Y, otherRoom.area.Y) + 1;
                    overlapEnd = Math.Min(room.area.Y + room.area.Height, otherRoom.area.Y + otherRoom.area.Height) - 2; // '- 2' -> In order to prevent doors spawning on corners or borders
                    doorX = room.area.X + room.area.Width - 1;
                    doorY = Utils.Random(overlapStart, overlapEnd + 1);
                }
                //otherRoom is to the left of room
                else if (otherRoom.area.X + otherRoom.area.Width - 1 == room.area.X)
                {
                    overlapStart = Math.Max(room.area.Y, otherRoom.area.Y) + 1;
                    overlapEnd = Math.Min(room.area.Y + room.area.Height, otherRoom.area.Y + otherRoom.area.Height) - 2; // '- 2' -> In order to prevent doors spawning on corners or borders
                    doorX = otherRoom.area.X + otherRoom.area.Width - 1;
                    doorY = Utils.Random(overlapStart, overlapEnd + 1);
                }
                //otherRoom is under room
                else if (room.area.Y + room.area.Height - 1 == otherRoom.area.Y)
                {
                    overlapStart = Math.Max(room.area.X, otherRoom.area.X) + 1;
                    overlapEnd = Math.Min(room.area.X + room.area.Width, otherRoom.area.X + otherRoom.area.Width) - 2; // '- 2' -> In order to prevent doors spawning on corners or borders
                    doorX = Utils.Random(overlapStart, overlapEnd + 1);
                    doorY = room.area.Y + room.area.Height - 1;
                }
                //otherRoom is above room
                else if (otherRoom.area.Y + otherRoom.area.Height - 1 == room.area.Y)
                {
                    overlapStart = Math.Max(room.area.X, otherRoom.area.X) + 1; // '+ 1' because of 0-indexing
                    overlapEnd = Math.Min(room.area.X + room.area.Width, otherRoom.area.X + otherRoom.area.Width) - 2; // '- 2' -> In order to prevent doors spawning on corners or borders
                    doorX = Utils.Random(overlapStart, overlapEnd + 1);
                    doorY = otherRoom.area.Y + otherRoom.area.Height - 1;
                }
                else
                {
                    throw new InvalidOperationException("Something went wrong in door generation!!!");
                }

                Door door = new Door(new Point(doorX, doorY));
                door.roomA = room;
                door.roomB = otherRoom;
                doors.Add(door);
            }
        }
    }
}
