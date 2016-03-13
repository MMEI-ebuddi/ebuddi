using System.Collections;
using UnityEngine;
using TMSupport;

public class Hazard_UncoveringWrists : Hazard
{
	public Avatar		m_Avatar;

	override protected void Init()
	{
		base.Init();
		m_HazardType = eType.UncoveringWrists;
		m_EventTime = 6.0f;
		m_TimeBeforeClickIsGood = 2.0f;
		m_AnimPlayedOnAvatar = true;
		HazardManager.Instance.Register(this);
		enabled = false;
	}

	public override void Activate()
	{
		base.Activate();
		m_Avatar.SetAnimationTrigger("RevealWrist");
		enabled = true;
	}

	void Update()
	{
		m_Timer += Time.deltaTime;
		if (m_Timer > m_EventTime)
		{
			m_Avatar.SetAnimationTrigger("Exit Time");
			HazardManager.Instance.HazardMissed(this);
			enabled = false;
		}
	}
}
