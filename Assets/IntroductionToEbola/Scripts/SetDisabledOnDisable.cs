using UnityEngine;
using System.Collections;

public class SetDisabledOnDisable : MonoBehaviour {

    public GameObject ObjectToDisable;

    void OnDisable()
    {
        if (ObjectToDisable!=null)
            ObjectToDisable.SetActive(false);
    }
}
