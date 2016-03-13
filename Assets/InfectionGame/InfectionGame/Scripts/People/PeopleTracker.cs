using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Used to track references to all the people in the scene, so we don't have to find objects by tag/name/etc
/// </summary>
public class PeopleTracker : MonoBehaviour {

    public static PeopleTracker Instance { get; private set; }

    private List<BasePerson> m_People;

    void Awake()
    {
        Instance = this;
        ClearList();
    }

    public void AddPerson(BasePerson person)
    {
        m_People.Add(person);
    }

    public void RemovePesron(BasePerson person)
    {        
        m_People.Remove(person);
    }

	public List<BasePerson> GetPeople()
    {
        return m_People;
    }

    // Clear our list of people references. We don't need to do anything to tidyup the actual objects here.
    public void ClearList()
    {
        if (m_People!= null && m_People.Count > 0)
        {
            for (int i = m_People.Count - 1; i >= 0; i--)
                Destroy(m_People[i].gameObject);
        }

        m_People = new List<BasePerson>();
    }
}
