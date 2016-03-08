using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// If person is infected, attempt to spread the infection to anyone in range
/// </summary>
public class SpreadInfection : MonoBehaviour {
	public float InfectionRadius = 2f;
    public bool RequiresPerson = true;

	private BasePerson m_BasePerson;

    private PeopleTracker m_PeopleTracker;

	void Start()
	{
		m_BasePerson = GetComponent<BasePerson>();
		
		if (m_BasePerson==null && RequiresPerson)
		{
			Debug.Log("WARNING! - SpreadInfection::Awake() - no BasePerson component found!");
		}

        m_PeopleTracker = PeopleTracker.Instance;
	}

	void Update()
	{
		if (m_BasePerson==null && RequiresPerson)
			return;
			
		if (RequiresPerson && !m_BasePerson.IsInfected())
			return;

        if (m_PeopleTracker == null)
            return;
		
		float fDistance = 0f;			

        List<BasePerson> people = m_PeopleTracker.GetPeople();

        Vector3 myPos = transform.position;
        myPos.y = 0f;

        Vector3 theirPos;

		// Check the distance between this person and all the others and spread the infection if in-range
        foreach (BasePerson person in people)
		{
            if (person == null)
                continue;

			if (person.gameObject != transform.gameObject)
			{
                if (!person.IsInfected() && person.IsVulnerableToInfection())
				{
                    theirPos = person.transform.position;
                    theirPos.y = 0f;

					fDistance = Vector3.Distance(myPos, theirPos);
					
					if (fDistance <= InfectionRadius)
					{
                        person.SetInfected();
					}
				}
			}
		}
	}	
}
