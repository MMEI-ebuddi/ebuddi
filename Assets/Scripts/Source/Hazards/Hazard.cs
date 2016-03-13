using System.Collections;
using UnityEngine;
using TMSupport;

public class Hazard : MonoBehaviour
{
	public Collider				m_HitCollider;

	public enum eType
	{
		UncoveringWrists = 0,
		FeelingFeint,
		BucketUpturned,
		PatientHeadZone,
		TooLongInPPE,
		HandWashing,
		Fake_Clipboard,
// 		DirtyLinen,
// 		RaisingArmsAboveName,
// 		TouchingGoggles,
		MAX
	}
	protected eType							m_HazardType;
	public eType							HazardType { get { return m_HazardType; } }
	protected float							m_EventTime;
	protected bool							m_AnimPlayedOnAvatar;
	protected float							m_TimeBeforeClickIsGood;
	protected bool							m_Fake;

	protected float							m_Timer;

	public EquipmentManager.eEquipment		RequiredEquipment { get { return m_RequiredEquipment; } }
	protected EquipmentManager.eEquipment	m_RequiredEquipment = EquipmentManager.eEquipment.MAX;

	public bool									m_Found;

	void Start()
	{
		Init();
	}

	protected virtual void Init()
	{
		m_HazardType = eType.MAX;
		m_EventTime = 0;
		m_TimeBeforeClickIsGood = 0;
		m_Fake = false;
		m_AnimPlayedOnAvatar = false;
		m_Found = false;
	}

	public virtual void Activate()
	{
		m_Found = false;
		m_Timer = 0;
	}

	public virtual bool Clicked()
	{
		return (m_Found == false && m_Timer > m_TimeBeforeClickIsGood);
	}

	internal virtual void HandleMessage(string Message)
	{
	}

	void Update()
	{
	}

	internal void SetFound()
	{
		m_Found = true;
	}
}
