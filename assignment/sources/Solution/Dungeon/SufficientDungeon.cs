using GXPEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
}
