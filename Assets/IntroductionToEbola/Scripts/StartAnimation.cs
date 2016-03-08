using UnityEngine;
using System.Collections;

public class StartAnimation : MonoBehaviour {

    public string AnimationStateName;
    public Animator MyAnimator;

	void OnEnable()
    {
        MyAnimator.CrossFade(AnimationStateName, 0f);
    }
}
