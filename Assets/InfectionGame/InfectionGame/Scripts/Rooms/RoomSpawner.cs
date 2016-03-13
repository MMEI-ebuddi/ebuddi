using UnityEngine;
using System.Collections;

/// <summary>
/// When attached to a BaseRoom object, attempts to spawn the specified type of people every X seconds, if the room capacity is below a specified threshold
/// </summary>
public class RoomSpawner : MonoBehaviour {

    public Transform PrefabPersonToSpawn;

    public float MinSpawnTime = 5f;
    public float MaxSpawnTime = 10f;

    public int MaxRoomCapacity = 15;

    private BaseRoom m_BaseRoom;

    private float m_fSpawnTimer = 0f;

    void Start()
    {
        m_BaseRoom = GetComponent<BaseRoom>();

        InvokeNextSpawn();
    }
	
    void Update()
    {
        if (!RoundManager.Instance.IsRoundInProgress())
            return;

        if (m_fSpawnTimer > 0.0f)
        {
            m_fSpawnTimer -= Time.deltaTime;

            if (m_fSpawnTimer <= 0.0f)
            {
                Spawn();
            }
        }
    }

    private void InvokeNextSpawn()
    {
        m_fSpawnTimer = Random.Range(MinSpawnTime, MaxSpawnTime);        
    }

    private void Spawn()
    {
        if (m_BaseRoom == null)
            return;        

        if (m_BaseRoom.GetCurrentCapacity() < MaxRoomCapacity)
        {
            Transform newPerson = Instantiate(PrefabPersonToSpawn);
            newPerson.position = m_BaseRoom.GetRandomPoint();
        }

        InvokeNextSpawn();
    }
}
