using UnityEngine;
using System.Collections;

public class RoomManager : MonoBehaviour {

    public static RoomManager Instance { get; private set; }

    public BaseRoom[] Rooms;
    public BaseRoom.ROOM_TYPE[] RoomTypes;

    void Awake()
    {
        Instance = this;
    }
    
    public BaseRoom GetRoom(BaseRoom.ROOM_TYPE roomType)
    {        
        return Rooms[(int)roomType];
    }
}
