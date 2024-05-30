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
    protected List<Room> dividedRooms;

    protected List<Room> currentRoomsToDivide;
    protected List<Room> newRoomsToDivide;

    public SufficientDungeon(Size pSize) : base(pSize)
    {
        dividedRooms = new List<Room>();
        currentRoomsToDivide = new List<Room>() { new Room(new Rectangle(0, 0, size.Width, size.Height)) };
        newRoomsToDivide = new List<Room>();
    }

    protected override void generate(int pMinimumRoomSize)
    {
        SetListCapacity(pMinimumRoomSize);

        GenerateRooms(pMinimumRoomSize);
        GenerateDoors();
    }

    protected void SetListCapacity(int pMinimumRoomSize)
    {
        int capacity =  CalculateCapacity(pMinimumRoomSize);

        dividedRooms.Capacity = capacity;
        currentRoomsToDivide.Capacity = capacity;
        newRoomsToDivide.Capacity = capacity;
    }

    protected void GenerateRooms(int pMinimumRoomSize)
    {
        //loop until there are no rooms that can be furher divided
        while (true)
        {
            foreach (var currentRoom in currentRoomsToDivide)
            {
                //Randomly pick which way to try to divide first - horizontally or vertically
                bool shouldDivideVertically = Utils.Random(0, 2) == 1;

                //Try to divide using the given orientation
                bool isFirstOrientationPossible = TryDivide(currentRoom, shouldDivideVertically, pMinimumRoomSize);
                if (!isFirstOrientationPossible) //Try dividing by the other orientation if the division by the first was unsuccessful
                {
                    TryDivide(currentRoom, !shouldDivideVertically, pMinimumRoomSize);
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

        Room[] newRooms = Divide(room, shouldDivideVertically, pMinimumRoomSize);

        //Check if any rooms are further dividable
        foreach (var roomToCheck in newRooms)
        {
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
    Room[] Divide(Room room, bool shouldDivideVertically, int pMinimumRoomSize)
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

        return new Room[]
        {
            shouldDivideVertically
                ? new Room(new Rectangle(room.area.X, room.area.Y, room1Size, room.area.Height))
                : new Room(new Rectangle(room.area.X, room.area.Y, room.area.Width, room1Size)),
            shouldDivideVertically
                ? new Room(new Rectangle(room.area.X + room1Size - 1, room.area.Y, room2Size, room.area.Height))
                : new Room(new Rectangle(room.area.X, room.area.Y + room1Size - 1, room.area.Width, room2Size))
        };
    }

    protected void GenerateDoors()
    {
        foreach (var room in rooms)
        {
            foreach (var otherRoom in rooms)
            {
                if (otherRoom == room)
                    continue;

                //Skip room if not neighbouring
                (bool isNeighbouringX, bool isNeighbouringY) = CheckForNeighbouring(room, otherRoom);
                if (!isNeighbouringX && !isNeighbouringY)
                    continue;

                //Checks if there is already a door between the given rooms
                if (CheckForDoorExists(room, otherRoom))
                    continue;


                Door door = GenerateDoor(room, otherRoom, isNeighbouringX);

                room.AddDoor(door);
                otherRoom.AddDoor(door);

                doors.Add(door);
            }
        }
    }

    /// <summary>
    /// Check if the otherRoom is neighbouring to room
    /// </summary>
    /// <returns>Whether the rooms are neighbouring</returns>
    (bool, bool) CheckForNeighbouring(Room room, Room otherRoom)
    {
        float difX = Mathf.Abs((room.area.X + room.area.Width / 2f) - (otherRoom.area.X + otherRoom.area.Width / 2f));//Floats because of the possible odd sizes of the rooms
        float difY = Mathf.Abs((room.area.Y + room.area.Height / 2f) - (otherRoom.area.Y + otherRoom.area.Height / 2f));
        bool isNeighbouringX =
            difX == room.area.Width / 2f + otherRoom.area.Width / 2f - 1 //Due to room overlapping
            && otherRoom.area.Y < room.area.Y + room.area.Height - 2 && room.area.Y < otherRoom.area.Y + otherRoom.area.Height - 2; // '- 2' -> In order to prevent doors spawning on corners or borders
        bool isNeighbouringY =
            difY == room.area.Height / 2f + otherRoom.area.Height / 2f - 1 //Due to room overlapping
            && otherRoom.area.X < room.area.X + room.area.Width - 2 && room.area.X < otherRoom.area.X + otherRoom.area.Width - 2; // '- 2' -> In order to prevent doors spawning on corners or borders

        return (isNeighbouringX, isNeighbouringY);
    }

    /// <summary>
    /// Checks if there is already a door between the given rooms
    /// </summary>
    bool CheckForDoorExists(Room room, Room otherRoom)
    {
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

        return doorExists;
    }

    /// <summary>
    /// Picks a position for the Door and creates it
    /// </summary>
    Door GenerateDoor(Room room, Room otherRoom, bool isNeighbouringX)
    {
        int overlapStart = 0;
        int overlapEnd = 0;
        int doorX = 0;
        int doorY = 0;

        if (isNeighbouringX)
        {
            overlapStart = Math.Max(room.area.Y, otherRoom.area.Y) + 1;
            overlapEnd = Math.Min(room.area.Y + room.area.Height, otherRoom.area.Y + otherRoom.area.Height) - 2; // '- 2' -> In order to prevent doors spawning on corners or borders
            doorY = Utils.Random(overlapStart, overlapEnd + 1);

            bool isOtherRoomFromTheRight = room.area.X + room.area.Width - 1 == otherRoom.area.X ? true : false;
            doorX = isOtherRoomFromTheRight ? room.area.X + room.area.Width - 1 : otherRoom.area.X + otherRoom.area.Width - 1;
        }
        else
        {
            overlapStart = Math.Max(room.area.X, otherRoom.area.X) + 1;
            overlapEnd = Math.Min(room.area.X + room.area.Width, otherRoom.area.X + otherRoom.area.Width) - 2; // '- 2' -> In order to prevent doors spawning on corners or borders
            doorX = Utils.Random(overlapStart, overlapEnd + 1);

            bool isOtherRoomUnder = room.area.Y + room.area.Height - 1 == otherRoom.area.Y ? true : false;
            doorY = isOtherRoomUnder ? room.area.Y + room.area.Height - 1 : otherRoom.area.Y + otherRoom.area.Height - 1;
        }

        Door door = new Door(new Point(doorX, doorY));
        door.roomA = room;
        door.roomB = otherRoom;

        return door;
    }
}
