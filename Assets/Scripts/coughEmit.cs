using UnityEngine;
using System.Collections;

public class coughEmit : MonoBehaviour {

	public ParticleSystem particlesToUse;

	public void coughOn() {
		particlesToUse.Play();
	}

	public void coughOff() {
			particlesToUse.Stop();
	}
}
