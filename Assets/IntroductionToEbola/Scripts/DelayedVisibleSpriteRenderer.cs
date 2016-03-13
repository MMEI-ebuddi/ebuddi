using UnityEngine;
using System.Collections;

public class DelayedVisibleSpriteRenderer : MonoBehaviour {

    public float Delay;

	void OnEnable()
    {
        Invoke("SetVisible", Delay);
    }

    void SetVisible()
    {        
        GetComponent<SpriteRenderer>().enabled = true;
    }
}
