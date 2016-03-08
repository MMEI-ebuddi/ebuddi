using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseRoom : MonoBehaviour {

	public Collider ColliderBounds;

    private List<BasePerson> m_PeopleInRoom;

    private PeopleTracker m_PeopleTracker;
    
    public enum ROOM_TYPE
    {
        OUTSIDE,
        TRIAGE,
        ISOLATION
    };

    public ROOM_TYPE RoomType;

	// Use this for initialization
	void Start () {
        m_PeopleTracker = PeopleTracker.Instance;
        m_PeopleInRoom = new List<BasePerson>();
	}
	
	// Update is called once per frame
	void Update () {
        FindPeopleInRoom();
	}
	
	/// <summary>
	/// Return the center position of this room's bounding box
	/// </summary>
	/// <returns>The room center.</returns>
	public Vector3 GetRoomCenter()
	{
		return ColliderBounds.bounds.center;
	}
	
	
	/// <summary>
	/// Return a random position within the bounds of this room
	/// </summary>
	/// <returns>The random point.</returns>
	public Vector3 GetRandomPoint()
	{
		Bounds roomBounds = ColliderBounds.bounds;
		
		return new Vector3(Random.Range(roomBounds.min.x, roomBounds.max.x),
		                   Random.Range(roomBounds.min.y, roomBounds.max.y),
		                   Random.Range(roomBounds.min.z, roomBounds.max.z));
	}

    /// <summary>
    /// Is the specific person within the bounds of this room?
    /// </summary>
    /// <param name="?"></param>
    /// <returns></returns>
    public bool ContainsPerson(BasePerson person)
    {
        return ColliderBounds.bounds.Contains(person.transform.position);
    }

    private void FindPeopleInRoom()
    {
        if (m_PeopleTracker == null)
            return;

        m_PeopleInRoom.Clear();

        List<BasePerson> people = m_PeopleTracker.GetPeople();

        foreach (BasePerson person in people)
        {
            if (ColliderBounds.bounds.Contains(person.transform.position))
            {
                m_PeopleInRoom.Add(person);
                person.SetCurrentRoom(this);
            }
        }
    }    

    public int GetCurrentCapacity()
    {
        return m_PeopleInRoom.Count;
    }
}
