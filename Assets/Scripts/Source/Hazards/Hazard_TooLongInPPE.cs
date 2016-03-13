using System.Collections;
using UnityEngine;
using TMSupport;

public class Hazard_TooLongInPPE : Hazard
{
	public Transform		m_HourHand;
	public Transform		m_MinuteHand;

	override protected void Init()
	{
		base.Init();
		m_HazardType = eType.TooLongInPPE;
		m_EventTime = 5.0f;
		m_TimeBeforeClickIsGood = 0.0f;
		m_AnimPlayedOnAvatar = true;
		HazardManager.Instance.Register(this);
		enabled = false;
	}

	public override void Activate()
	{
		base.Activate();
		enabled = true;
	}

	void Update()
	{
		Vector3		RotationEular = new Vector3(90, 90, 0);

		m_Timer += Time.deltaTime;
		if (m_Timer > m_EventTime)
		{
			HazardManager.Instance.HazardMissed(this);
			enabled = false;
		}

		RotationEular.z = (m_Timer * 360) % 360;
		m_MinuteHand.localRotation = Quaternion.Euler(RotationEular);
		RotationEular.z = (m_Timer * 6) % 360;
		m_HourHand.localRotation = Quaternion.Euler(RotationEular);
	}
}
