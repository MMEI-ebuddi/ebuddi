using UnityEngine;
using System.Collections;

public class ClickHazard : MonoBehaviour {

    public AudioClip SoundResource;

    private bool m_bSoundPlayed = false;

    private GameObject m_SoundGO = null;

    public SingleHazardVisible VisibleHazardController;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        if (m_bSoundPlayed)
        {
            if (m_SoundGO==null)
            {
                VisibleHazardController.ShowHazards(gameObject);

                Destroy(gameObject);                
            }
        }

        if (Input.GetMouseButtonDown(0) && !m_bSoundPlayed)
        {            
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (GetComponent<Collider>().Raycast(ray, out hit, 100f))
            {
                m_SoundGO = SimpleSoundClip.PlaySound(MediaNodeManager.LoadAudioClipResource(SoundResource.name));
                m_bSoundPlayed = true;

                VisibleHazardController.HideHazards(gameObject);
            }
        }
    }
}
