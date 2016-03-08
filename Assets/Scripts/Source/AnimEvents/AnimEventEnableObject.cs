using UnityEngine;
using System.Collections;

public class AnimEventEnableObject : MonoBehaviour {

	public void Triggered (AnimationEvent animationEvent)
    {
        transform.FindChild(animationEvent.stringParameter).gameObject.SetActive(true);
    }
}
