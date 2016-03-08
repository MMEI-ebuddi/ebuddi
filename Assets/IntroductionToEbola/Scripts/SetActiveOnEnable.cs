using UnityEngine;
using System.Collections;

public class SetActiveOnEnable : MonoBehaviour {

    public GameObject ObjectToEnable;

	void OnEnable()
    {
        ObjectToEnable.SetActive(true);
    }
}
