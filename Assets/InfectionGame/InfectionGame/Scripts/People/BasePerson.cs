using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(NavMeshAgent))]

/// <summary>
/// Base representation of a person.
/// This is a super-early, prototyping class. Requirements aren't set in stone yet, so this will be refactored when final.
/// </summary>
/// 

public class BasePerson : MonoBehaviour {
 	public Sprite [] Sprites; 	
 	
 	public bool StartInfected = false;
    public float WaypointRadius = 0.2f;

    public float IdleMin = 1f;
    public float IdleMax = 5f;

	public float DistancedRadius = 1f;
	
	public enum PERSON_TYPE
	{
		PATIENT,
		MEDIC
	};
	
	public PERSON_TYPE PersonType;
	
	[Flags]
	enum STATUS_FLAGS
	{
		NORMAL = 0,
		INFECTED = 1,
		INFECTION_VISIBLE = 2,
		PPE_EQUIPPED = 4,
	};

    public BaseRoom.ROOM_TYPE[] RoamingRoomTypes;
    public int [] RoamingRoomChances;
    
	private STATUS_FLAGS m_StatusFlags;
	
	private SpriteRenderer m_SpriteRenderer;

    protected NavMeshAgent m_NavAgent;

    private bool m_bIdling = false;

    private PeopleTracker m_PeopleTracker;

    private BaseRoom m_CurrentRoom;
       
	// Use this for initialization
	protected virtual void Start () 
    {
        // Add a reference to ourself in the PeopleTracker.
        m_PeopleTracker = PeopleTracker.Instance;
        m_PeopleTracker.AddPerson(this);
                            
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_NavAgent = GetComponent<NavMeshAgent>();

		ClearStatusFlags();
        IdleWait();
        
        if (StartInfected)
        	SetInfected();

        if (RoundManager.Instance.IsKeepDistanceAllowed())
        {
            m_NavAgent.radius = DistancedRadius;
        }        
	}
	
    // TODO: Part of Room refactor, coming up soon
    protected virtual void SetRoamingRooms()
    {

    }

	protected virtual void Update()
	{
        if (!m_bIdling && m_NavAgent.remainingDistance <= WaypointRadius)
        {           
            IdleWait();
        }
        
        if (RoomManager.Instance.GetRoom(BaseRoom.ROOM_TYPE.ISOLATION).ContainsPerson(this))
        {
            Destroy(gameObject);
        }

        UpdateSprite();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.localEulerAngles = new Vector3(90f, 0f, 0f);
	}

    // TODO: After prototype, make this only do it when status changes, not every frame!
    protected virtual void UpdateSprite()
    {
        if (IsMedic() && IsWearingPPE())
            m_SpriteRenderer.sprite = Sprites[2];
        else
            m_SpriteRenderer.sprite = Sprites[IsInfected() ? 1 : 0];
    }

    private void IdleWait()
    {
        m_bIdling = true;

        Invoke("GetNewTargetPosition", UnityEngine.Random.Range(IdleMin, IdleMax));
    }

    protected void CancelIdle()
    {
        m_bIdling = false;

        CancelInvoke("GetNewTargetPosition");
    }

    // Get a random room based on our chance rolls and return a random position within it
    private void GetNewTargetPosition()
    {
        if (gameObject == null)
            return;

        CancelIdle();

        BaseRoom room = GetRandomIdleRoom();

        if (room == null)
        {
            Debug.Log("ERROR! GetRandomIdleRoom() null room ref");
            return;
        }
            
        Vector3 randomPosition = room.GetRandomPoint();
    
        // Now ensure we get a valid navmesh position nearest to this point
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPosition, out hit, 20f, 1);
        Vector3 finalPosition = hit.position;

        m_NavAgent.SetDestination(finalPosition);       
    }
    
    // Based on our weighted chances, get a random room to idle-roam in next
    private BaseRoom GetRandomIdleRoom()
    {
        int iRandom = UnityEngine.Random.Range(0, 100);
        int iAccum = 0;

        for (int i = 0; i < RoamingRoomTypes.Length; i++)
        {
            iAccum += RoamingRoomChances[i];

            if (iRandom <= iAccum)
            {
                return RoomManager.Instance.GetRoom(RoamingRoomTypes[i]);
            }
        }

        return null;
    }

    // Todo: Move flags to their own StatusFlags class or similar and tidy up
    private void ClearStatusFlags()
    {
    	m_StatusFlags = STATUS_FLAGS.NORMAL;
    }
    
    public void SetInfected()
    {
    	m_StatusFlags |= STATUS_FLAGS.INFECTED;
        RoundManager.Instance.IncreaseInfectedCount();
    }

    public void SetWearingPPE()
    {
        m_StatusFlags |= STATUS_FLAGS.PPE_EQUIPPED;
    }

    public bool IsWearingPPE()
    {
        return ((m_StatusFlags & STATUS_FLAGS.PPE_EQUIPPED) != 0);
    }

	public bool IsInfected()
	{
		return ((m_StatusFlags & STATUS_FLAGS.INFECTED)!=0);
	}

    public bool IsMedic()
    {
        return (PersonType == PERSON_TYPE.MEDIC);
    }

    public bool IsPatient()
    {
        return (PersonType == PERSON_TYPE.PATIENT);
    }

    // Check any cases that might prevent this person from becoming infected
    public bool IsVulnerableToInfection()
    {
        return !IsWearingPPE();
    }
    
	public void SetCurrentRoom(BaseRoom room)
    {
        m_CurrentRoom = room;
    }

    public BaseRoom GetCurrentRoom()
    {
        return m_CurrentRoom;
    }

    /// <summary>
    /// Trigged when the user clicks on this person
    /// </summary>
	public virtual void Clicked()
	{
        if (IsInfected() && RoundManager.Instance.IsIsolationAllowed())
        {
            BaseRoom currentRoom = GetCurrentRoom();

            if (currentRoom != null && currentRoom.RoomType == BaseRoom.ROOM_TYPE.TRIAGE)
            {
                CancelIdle();
                transform.position = RoomManager.Instance.GetRoom(BaseRoom.ROOM_TYPE.ISOLATION).GetRoomCenter();
                //m_NavAgent.SetDestination(RoomManager.Instance.GetRoom(BaseRoom.ROOM_TYPE.ISOLATION).GetRoomCenter());
            }
        }        
	}

    void OnDestroy()
    {
        if (m_PeopleTracker != null)
            m_PeopleTracker.RemovePesron(this);

        CancelInvoke("GetNewTargetPosition");
    }
}
