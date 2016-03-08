using UnityEngine;
using System.Collections;

public class AnimEventSwapApron : MonoBehaviour
{
	public void Triggered (AnimationEvent animationEvent)
    {
        transform.FindChild("AnimatedApron").gameObject.SetActive(true);
        transform.FindChild("Apron").gameObject.SetActive(false);
    }
}
