using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMSupport;

public class RiskManager : MonoBehaviour
{
	static RiskManager				sInstance;
	public static RiskManager		Instance { get { return sInstance; } }

	public int						m_RisksToPresent;

	public List<Risk>[]				m_ListOfRisksInScene;
	public GameObject               m_AllRisksFoundCanvas;
	public GameObject				m_Arrow;

	public UIRisks					m_RisksUI;

	List<Risk>						m_ActiveRisks;
	List<Risk>						m_FoundRisks;
	int								m_RisksLeftToUse;

	int								m_SelectableLayer;
	int								m_LayerMask;

	public bool						Active { get { return m_Active; } }
	bool							m_Active;
	Risk							m_RiskToHighlight;
	int								m_MissedRiskIndex;

	void Start()
	{
		sInstance = this;

		m_Active = false;
		m_AllRisksFoundCanvas.SetActive(false);
//		m_RisksUI.gameObject.SetActive(false);
		ViewHazards.instance.HideRisksUI();
		m_Arrow.SetActive(false);
		m_MissedRiskIndex = -1;

		m_SelectableLayer = LayerMask.NameToLayer("Risk");
		m_LayerMask = (1 << m_SelectableLayer);

		m_ListOfRisksInScene = new List<Risk>[(int)Risk.eType.MAX];
		for(int loop = 0; loop < (int)Risk.eType.MAX; loop++)
		{
			m_ListOfRisksInScene[loop] = new List<Risk>();
		}

		m_ActiveRisks = new List<Risk>();
		m_FoundRisks = new List<Risk>();

		Risk[]		AllRisksInScene = GameObject.FindObjectsOfType<Risk>();

		// Let's 
		for(int loop = 0; loop < AllRisksInScene.Length; loop++)
		{
			m_ListOfRisksInScene[(int)AllRisksInScene[loop].RiskType].Add(AllRisksInScene[loop]);
			AllRisksInScene[loop].gameObject.SetActive(false);
		}

		m_RisksLeftToUse = AllRisksInScene.Length;
		if(m_RisksToPresent > m_RisksLeftToUse)
			m_RisksToPresent = m_RisksLeftToUse;

		// Let's just choose one from each risk type
		for (int loop = 0; loop < m_ListOfRisksInScene.Length; loop++)
		{
			int		RiskToUse = Random.Range(0, m_ListOfRisksInScene[loop].Count);

			m_ActiveRisks.Add(m_ListOfRisksInScene[loop][RiskToUse]);
			m_ListOfRisksInScene[loop][RiskToUse].gameObject.SetActive(true);
		}

// 		for (int loop = 0; loop < m_RisksToPresent; loop++)
// 		{
// 			int		RiskToUse = Random.Range(0, m_RisksLeftToUse);
// 
// 			// Add the random risk
// 			m_ActiveRisks.Add(AllRisksInScene[RiskIndexToUse]);
// 			// Remove the risk from the possibles so we can't choose it again
// 			AllRisksInScene[RiskIndexToUse] = AllRisksInScene[m_RisksLeftToUse - 1];
// 			m_RisksLeftToUse--;
// 		}
	}

	public void Activate(bool Activate = true)
	{
		if(Activate)
		{
			m_Active = true;
			m_AllRisksFoundCanvas.SetActive(true);
			m_RisksUI.Activate(m_ActiveRisks.Count);
			ViewHazards.instance.ShowRisksUI();
		}
		else
		{
			m_Active = false;
			gameObject.SetActive(false);
			ViewHazards.instance.HideRisksUI();

			// Let's switch off the risks manually (as could have some outside of the RiskManager GameObject)
			foreach(List<Risk> RiskList in m_ListOfRisksInScene)
			{
				foreach(Risk Risk in RiskList)
				{
					Risk.gameObject.SetActive(false);
				}
			}
		}
	}

	void Update()
	{
		if(m_RiskToHighlight != null)
		{
			Vector3		Position = m_RiskToHighlight.transform.position;

//			Position.y += 0.2f;
			Position.y += (Mathf.Sin(Time.time * 3) * 0.15f) + 0.20f;
			m_Arrow.transform.position = Position;
			m_Arrow.SetActive(true);

		}

		if(m_Active == false || !m_AllRisksFoundCanvas.activeInHierarchy)
			return;

		Ray				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit		hit;

		// Casts the ray and get the first game object hit
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_LayerMask))
		{			
			if (Input.GetMouseButtonDown(0))
			{
				for (int loop = 0; loop < m_ActiveRisks.Count; loop++)
				{
					if (m_ActiveRisks[loop].gameObject == hit.collider.gameObject)
					{
						// Found the risk
						PointOutRisk(m_ActiveRisks[loop]);
						FoundRisk(loop);
						m_RisksUI.AnotherRiskFound();
						return;
					}
				}
			}
		}
	}

	void FoundRisk(int Index)
	{
		m_ActiveRisks[Index].Activated();
		TMSupport.TMMessenger.Send("FoundRisk".GetHashCode(), (int)m_ActiveRisks[Index].RiskType);
		m_FoundRisks.Add(m_ActiveRisks[Index]);
		m_ActiveRisks.RemoveAt(Index);
	}

	internal bool WereAllRisksFound()
	{
		return(m_ActiveRisks.Count == 0);
	}

	public void OnFinishedClicked()
	{
		TMSupport.TMMessenger.Send("FinishedRisks".GetHashCode());
		m_AllRisksFoundCanvas.SetActive(false);
	}

	public void PointOutRisk(Risk RiskToHighlight)
	{
		m_RiskToHighlight = RiskToHighlight;
	}

	internal Risk.eType HighlightNextMissedRisk()
	{
		m_MissedRiskIndex++;
		if(m_MissedRiskIndex < m_ActiveRisks.Count)
		{
			PointOutRisk(m_ActiveRisks[m_MissedRiskIndex]);
			return (m_ActiveRisks[m_MissedRiskIndex].RiskType);
		}
		else
			return (Risk.eType.MAX);
	}
}
