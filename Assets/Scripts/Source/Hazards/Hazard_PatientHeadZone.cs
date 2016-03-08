using System.Collections;
using UnityEngine;
using TMSupport;

public class Hazard_PatientHeadZone : Hazard
{
	public Avatar		m_Avatar;

	override protected void Init()
	{
		base.Init();
		m_HazardType = eType.PatientHeadZone;
		m_EventTime = 4.8f;
		m_TimeBeforeClickIsGood = 0.5f;
		m_AnimPlayedOnAvatar = true;
		HazardManager.Instance.Register(this);
		enabled = false;
	}

	public override void Activate()
	{
		base.Activate();
		m_Avatar.SetAnimationTrigger("EnterHeadZone");
		enabled = true;
	}

	void Update()
	{
		m_Timer += Time.deltaTime;
		if (m_Timer > m_EventTime)
		{
//			if (!m_Found) {
				m_Avatar.SetAnimationTrigger("Exit Time");
				HazardManager.Instance.HazardMissed(this);
				enabled = false;
//			}
		}
	}
}
