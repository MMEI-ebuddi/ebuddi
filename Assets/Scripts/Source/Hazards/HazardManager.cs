using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMSupport;

public class HazardManager : MonoBehaviour
{
	static HazardManager			sInstance;
	public static HazardManager		Instance { get { return sInstance; } }

	public FingerPainting fingerPainting;
	public Hazard_FeelingFeint fainting;
	public int						m_NumberOfHazardsToPresent;
	public float					m_MinTimeToNextHazard;
	public float					m_MaxTimeToNextHazard;
	public GameObject				m_HazardDisplay;

	public List<Hazard>				m_HazardsWeCanUse;
	List<Hazard>					m_HazardsToPresent;
	bool[]							m_HazardsFound;

	class Tracking 
	{
		Hazard.eType		m_Type;
		float				m_TimeToNotice;
	}	
	
	Hazard							m_ActiveHazard;
	float							m_TimeToNextHazard;

	int								m_SelectableLayer;
	int								m_LayerMask;

	bool							m_Active;
	
	float							m_HazardDisplayTimer;
	int								m_HazardIndex;
	bool							m_WaitingForBuddy;

	Material						m_HazardDisplayMaterial;

	public Hazard debugHazard;
	public bool skipRandomHazards;
	public bool debugTimeGaps;
	public bool skipFainting = false;

	void OnDestroy()
	{
		TMMessenger.Instance.RemoveFromMessageQueue(this);
	}

	void Awake()
	{
		sInstance = this;
		TMMessenger.Instance.AddToMessageQueue(new MessageCallback(this.MessageCallback));

		m_Active = false;
		m_SelectableLayer = LayerMask.NameToLayer("Hazard");
		m_LayerMask = (1 << m_SelectableLayer);
		
		m_ActiveHazard = null;
		m_HazardDisplayTimer = -1;
		m_HazardIndex = 0;
		m_WaitingForBuddy = false;

		m_HazardDisplay.SetActive(false);
		m_HazardDisplayMaterial = m_HazardDisplay.GetComponent<Renderer>().material;


	}

	protected void MessageCallback(TMMessageNode Message)
	{
		if(!m_Active)
			return;

		if (m_WaitingForBuddy && Message.Message == "BuddyDialogClosed".GetHashCode())
		{
			m_WaitingForBuddy = false;

		}
	}

	public void MoveToNextHazard()
	{
//		Debug.Log("Move to next hazard");
		m_HazardIndex++;

		if (m_HazardIndex >= m_HazardsToPresent.Count)
		{
			m_TimeToNextHazard = 0;
		}
		else {
			m_TimeToNextHazard = Random.Range(m_MinTimeToNextHazard, m_MaxTimeToNextHazard);
			if (debugHazard != null || debugTimeGaps) m_TimeToNextHazard = 1f;
		}
	}



	public void Activate()
	{
		m_Active = true;
		m_TimeToNextHazard = Random.Range(m_MinTimeToNextHazard, m_MaxTimeToNextHazard);
		if (debugHazard != null || debugTimeGaps) m_TimeToNextHazard = 1f;

		List<Hazard> randomHazards = new List<Hazard>(FindObjectsOfType<Hazard>());
		randomHazards.Remove(fainting);
		randomHazards.Remove(fingerPainting);
	


		//shuffle
		randomHazards.Shuffle();
		m_HazardsToPresent = new List<Hazard>();
		if (!skipRandomHazards) m_HazardsToPresent.AddRange(randomHazards);
		if (!skipFainting) m_HazardsToPresent.Add(fainting);
		m_HazardsToPresent.Add(fingerPainting);


		foreach (Hazard hazard in m_HazardsToPresent) Debug.Log(hazard.gameObject.name);

	}

	void Update()
	{
		if (m_Active == false)
			return;

		if(m_ActiveHazard == null && !m_WaitingForBuddy)
		{
			m_TimeToNextHazard -= Time.deltaTime;
			if (m_TimeToNextHazard <= 0)
			{
				if (m_HazardIndex >= m_HazardsToPresent.Count)
				{
					TMSupport.TMMessenger.Send("FinishedHazards".GetHashCode());
					m_Active = false;
					return;
				}
				else
					TriggerNextHazzard();
			}
		}

		DoRaycastChecks();

		if(m_HazardDisplayTimer >= 0)
		{
			Color		HazardColour = Color.red;

			if (m_HazardDisplayTimer >= 1)
				HazardColour.a = 0.5f;
			if(m_HazardDisplayTimer < 1)
				HazardColour.a = m_HazardDisplayTimer * 0.5f;

			m_HazardDisplayMaterial.color = HazardColour;

			m_HazardDisplayTimer -= Time.deltaTime;
			if(m_HazardDisplayTimer < 0)
				m_HazardDisplay.SetActive(false);
		}
	}

	 


	IEnumerator FinishedRandomHazards() {

		//next is finger painting camera pan back to buddy to swap avatars
//		OurCamera.Instance.MoveTo(OurCamera.eCameraFocus.Buddy, 1f);
		yield return new WaitForSeconds(2f);
		if (debugHazard != null) m_ActiveHazard = debugHazard;
		m_ActiveHazard.Activate();
		m_TimeToNextHazard = float.MaxValue;
	}




	public void TriggerNextHazzard()
	{
		Debug.Log("Trigger next hazard " + m_HazardsToPresent[m_HazardIndex]);
		m_ActiveHazard = m_HazardsToPresent[m_HazardIndex];

		if (m_HazardsToPresent[m_HazardIndex] != fainting) {

			if (debugHazard != null) m_ActiveHazard = debugHazard;
			m_ActiveHazard.Activate();
			m_TimeToNextHazard = float.MaxValue;
		}
		else {
			StartCoroutine(FinishedRandomHazards());
		}
	}







	private void DoRaycastChecks()
	{
		Ray				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit		hit;		

		// Casts the ray and get the first game object hit
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_LayerMask))
		{			
			if (Input.GetMouseButtonDown(0))
			{
				if (m_ActiveHazard != null && m_ActiveHazard.m_HitCollider == hit.collider && m_ActiveHazard.Clicked())
				{
					m_ActiveHazard.SetFound();
					FoundHazard(m_ActiveHazard);
				}
			}
		}
	}

	void CreateHazard()
	{
		TMSupport.TMMessenger.Send("FinishedHazards".GetHashCode());
	}

	void FoundHazard(Hazard FoundHazard)
	{
		Debug.Log("Hazard found " + FoundHazard.name);

		SphereCollider		Collider = m_ActiveHazard.m_HitCollider as SphereCollider;

		m_HazardDisplay.SetActive(true);
		m_HazardDisplayTimer = 2.0f;
		m_HazardDisplay.transform.position = m_ActiveHazard.m_HitCollider.transform.position + Collider.center;
		if (Collider != null)
			m_HazardDisplay.transform.localScale = Vector3.one * Collider.radius * 2;
		else
			m_HazardDisplay.transform.localScale = Vector3.one * 2;

		FoundHazard.m_Found = true;
//		m_HazardsFound[m_HazardIndex] = true;

		TMSupport.TMMessenger.Send("FoundHazard".GetHashCode(), (int)FoundHazard.HazardType);
		m_ActiveHazard = null;
		m_WaitingForBuddy = true;
	}


	internal void HazardMissed(Hazard MissedHazard)
	{
		Debug.Log("Hazard missed " + MissedHazard.name);

		if (!MissedHazard.m_Found)
		{
			MissedHazard.m_Found = false;
			TMSupport.TMMessenger.Send("MissedHazard".GetHashCode(), (int)MissedHazard.HazardType);
			m_WaitingForBuddy = true;
		}

		m_ActiveHazard = null;
		MoveToNextHazard();
	}


	internal bool WereAllHazardsFound()
	{
		foreach (Hazard hazard in m_HazardsToPresent) {
			if (!hazard.m_Found) {
				//ignore fainting and finger painting
				if (hazard != fingerPainting && hazard != fainting) {
					return false;
				}
			}
		}
		return (true);
	}

	internal Hazard GetHazard(int Index)
	{
		if(Index >= 0 && Index < m_HazardsToPresent.Count)
			return(m_HazardsToPresent[Index]);
		else
			return(null);
	}
	internal bool WasHazardFound(int Index)
	{
		//ovveriding last hazard - fainting
		if (Index == 4) return true;
		else if (Index >= 0 && Index < m_HazardsFound.Length)
			return (m_HazardsFound[Index]);
		else
			return(false);
	}

	internal void HandleMessage(string Message)
	{
		if (m_ActiveHazard != null)
			m_ActiveHazard.HandleMessage(Message);
	}

	internal void Register(Hazard NewHazard)
	{
		m_HazardsWeCanUse.Add(NewHazard);
	}
}
