using UnityEngine;
using System.Collections;

public class TriggerPlaySound : BaseTrigger {
	public  AudioClip SoundResource;
	
	private GameObject m_SoundGO;
	
	private bool m_bPlayed = false;
	

	void OnEnable()
	{
		m_SoundGO = SimpleSoundClip.PlaySound( MediaNodeManager.LoadAudioClipResource(SoundResource.name) );
		m_bPlayed = true;
	}
	
	public override bool Triggered()
	{
		return (m_bPlayed && m_SoundGO == null);
	}
}
