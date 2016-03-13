using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfectionRoomControls : MonoBehaviour {
    private PeopleTracker m_PeopleTracker;
    	
	public UIActionBar ActionBar;
	
	// Use this for initialization
	void Start () {
        m_PeopleTracker = PeopleTracker.Instance;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (m_PeopleTracker==null)
        {
            Debug.Log("ERROR! InfectionRoomControls - No people tracker found");
            return;
        }
        
		if (Input.GetMouseButtonDown(0))
		{
            List<BasePerson> people = m_PeopleTracker.GetPeople();
			
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			foreach (BasePerson person in people)
			{
                if (person == null)
                    continue;

				if (person.GetComponent<Collider>().Raycast(ray, out hit, 100f))
				{
                    PersonClicked(person);
					return;
				}	
			}
		}
	}

    // This person has just been clicked on, handle the possible actions and interactions here as they're added.
    private void PersonClicked(BasePerson person)
    {
        person.Clicked();        
    }
    
    
}
