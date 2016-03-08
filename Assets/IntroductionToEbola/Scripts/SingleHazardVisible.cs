using UnityEngine;
using System.Collections;

public class SingleHazardVisible : MonoBehaviour {

    public GameObject[] Hazards;

	public void ShowHazards(GameObject objectToIgnore)
    {
        foreach (GameObject go in Hazards)
        {
            if (go != null && go != objectToIgnore)
            {
                go.SetActive(true);
            }
        }
    }

    public void HideHazards(GameObject objectToIgnore)
    {
        foreach (GameObject go in Hazards)
        {
            if (go!=null && go != objectToIgnore)
            {
                go.SetActive(false);
            }
        }
    }
}
