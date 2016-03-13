using System.Collections;
using UnityEngine;
using TMSupport;

public class Risk_BucketObstacle : Risk
{
	public Transform		m_OtherPosition;

	public override void Activated()
	{
		transform.position = m_OtherPosition.position;
	}
}
