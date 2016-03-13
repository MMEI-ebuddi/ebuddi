using System.Collections;
using UnityEngine;
using TMSupport;

public class Hazard_BucketUpturned : Hazard
{
	public Avatar		m_Avatar;
	public MeshRenderer bucket;

	float				m_RotateBucketTimer;
	Vector3				m_Rotation;

	override protected void Init()
	{
		base.Init();
		m_HazardType = eType.BucketUpturned;
		m_EventTime = 5.0f;
		m_TimeBeforeClickIsGood = 2.6f;
		m_AnimPlayedOnAvatar = true;
		m_RotateBucketTimer = -1;
		m_Rotation = transform.rotation.eulerAngles;
		HazardManager.Instance.Register(this);
		bucket.enabled = false;
		enabled = false;
//		gameObject.SetActive(false);
	}

	public override void Activate()
	{
		base.Activate();
		m_Avatar.SetAnimationTrigger("PickUpBucket");
//		gameObject.SetActive(true);
		bucket.enabled = true;
		m_RotateBucketTimer = -1;
		m_Rotation = transform.rotation.eulerAngles;
		enabled = true;
	}

	internal override void HandleMessage(string Message)
	{
		m_RotateBucketTimer = 0;
	}


	void Update()
	{
		m_Timer += Time.deltaTime;
		if (m_Timer > m_EventTime)
		{
			HazardManager.Instance.HazardMissed(this);
			bucket.enabled = false;
			enabled = false;
		}
		if(m_RotateBucketTimer >= 0)
		{
			float		Angle = Mathf.Lerp(79, 0, m_RotateBucketTimer);

			if (m_RotateBucketTimer >= 1)
				Angle = 0;

			m_RotateBucketTimer += Time.deltaTime;
			m_Rotation.z = Angle;
			transform.rotation = Quaternion.Euler(m_Rotation);
		}
	}
}
