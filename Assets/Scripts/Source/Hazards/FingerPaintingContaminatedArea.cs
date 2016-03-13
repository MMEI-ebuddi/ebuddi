using UnityEngine;
using System.Collections;

public class FingerPaintingContaminatedArea : MonoBehaviour {

	public ParticleSystem ebolaParticles;
	public GameObject selectedSphere;
	public GameObject missedSphere;

	private bool isSelected = false;
	public bool IsSelected {
		get {
			return isSelected;
		}
		set {
			isSelected = value;
			selectedSphere.SetActive(isSelected);
		}
	}

	private bool isContaminated = false;
	public bool IsContaminated {
		get {
			return isContaminated;
		}
		set {
			isContaminated = value;
			if (isContaminated) {
				ebolaParticles.gameObject.SetActive(true);
				ebolaParticles.Play();
				missedSphere.SetActive(!IsSelected);
			}
			else {
				missedSphere.SetActive(false);
			}
		}
	}
	

	public void Reset() {
		IsContaminated = false;
		IsSelected = false;
		ebolaParticles.gameObject.SetActive(false);
		ebolaParticles.Stop();
	}



}
