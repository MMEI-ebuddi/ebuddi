using System.Collections;
using UnityEngine;
using TMSupport;

public class Risk : MonoBehaviour
{
	public enum eType
	{
		PaperTowel = 0,
		PatientHead,
		BucketOfVomit,
		ContaminatedSheets,
		VomitOnFloor,
		BucketObstacle,
		OverflowingSharps,
		MAX
	}
	[SerializeField]
	eType			m_RiskType;
	public eType	RiskType { get { return m_RiskType; } }

	public virtual void Activated()
	{
	}
}
