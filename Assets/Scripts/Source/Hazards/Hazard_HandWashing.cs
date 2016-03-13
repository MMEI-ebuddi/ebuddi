using System.Collections;
using UnityEngine;
using TMSupport;

public class Hazard_HandWashing : Hazard
{
	public Avatar		m_Avatar;

	override protected void Init()
	{
		base.Init();
		m_HazardType = eType.HandWashing;
		m_EventTime = 2.5f;
		m_TimeBeforeClickIsGood = 1.0f;
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
			m_Avatar.SetAnimationTrigger("Exit Time");
			HazardManager.Instance.HazardMissed(this);
			enabled = false;
		}
	}
}
