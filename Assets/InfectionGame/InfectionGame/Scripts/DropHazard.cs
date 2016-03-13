using UnityEngine;
using System.Collections;

public class DropHazard : MonoBehaviour {
    public bool Enabled = true;
    public Transform PrefabHazard;

    public float TimerMin, TimerMax;
    public float DropChancePercent = 10f;

    private float m_fDropTimer = 0f;

    private BasePerson m_BasePerson;

	// Use this for initialization
	void Start () {
        m_BasePerson = GetComponent<BasePerson>();

        ResetTimer();
	}
	
	// Update is called once per frame
	void Update () {
        if (!Enabled)
            return;

        if (!m_BasePerson.IsInfected())
            return;

        if (!RoundManager.Instance.IsRoundInProgress())
            return;

        m_fDropTimer -= Time.deltaTime;

        if (m_fDropTimer <= 0.0f)
        {
            RollSpawnChance();
        }
	}

    private void ResetTimer()
    {
        m_fDropTimer = Random.Range(TimerMin, TimerMax);
    }

    private void RollSpawnChance()
    {
        if (Random.Range(0,100) <= DropChancePercent)
        {
            SpawnHazard();
        }

        ResetTimer();
    }

    private void SpawnHazard()
    {
        Transform hazard = Instantiate(PrefabHazard);
        hazard.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        hazard.eulerAngles = new Vector3(hazard.eulerAngles.x, Random.Range(0, 359), hazard.eulerAngles.z);
    }
}
