using UnityEngine;
using System.Collections;

public class AnimEventSwapFaceShield : MonoBehaviour
{
    public Avatar TargetAvatar;

	public void TriggerSwapFaceShield (AnimationEvent animationEvent)
    {
        Debug.Log("triggered!");
        TargetAvatar.SwapFaceShield();    
    }
}
