using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Tracks the current round and handles changing rounds.
/// </summary>
public class RoundManager : MonoBehaviour 
{
    public GameObject[] RoundPeopleParentNodes;
    public bool [] IsolationAvailable;
    public bool [] PPEAvailable;
    public bool[] KeepDistanceAvailable;
    public UIPanelNewRound PanelNewRound;
    public UIPanelRoundSummary PanelRoundSummary;

    public static RoundManager Instance { get; private set; }

    // TODO: This needs reworking. Lots of bools is bad, but doing it for a quick build fix!
    private bool m_bIsolationAllowed = false;
    private bool m_bPPEAllowed = false;
    private bool m_bKeepDistanceAllowed = false;
    private int m_iCurrentRound = -1;

    private bool m_bRoundInProgress = false;

    public Text TextTimeRemaining;
    public Text TextInfectionCounter;

    public float RoundTime = 30f;
    private float m_fRoundTime;

    private int m_iInfectedCount;

    void Awake()
    {
        Instance = this;
    }	

    void Start()
    {
        NextRound();
    }

    void Update()
    {
        TextTimeRemaining.text = ((int)m_fRoundTime).ToString() + " seconds remaining!";
        TextInfectionCounter.text = "Infection Count: " + RoundManager.Instance.GetInfectedCount().ToString();

        if (m_bRoundInProgress)
        {
            m_fRoundTime -= Time.deltaTime;

            if (m_fRoundTime <= 0.0f)
            {
                StopRound();
                PanelRoundSummary.Show(m_iCurrentRound);
            }
        }
    }
    
    public void SetIsolationAllowed(bool bFlag)
    {
        m_bIsolationAllowed = bFlag;
    }
   
    public bool IsIsolationAllowed()
    {
        return m_bIsolationAllowed;
    }

    public void SetPPEAllowed(bool bFlag)
    {
        m_bPPEAllowed = bFlag;
    }
   
    public bool IsPPEAllowed()
    {
        return m_bPPEAllowed;
    }

    public void SetKeepDistanceAllowed(bool bFlag)
    {
        m_bKeepDistanceAllowed = bFlag;
    }

    public bool IsKeepDistanceAllowed()
    {
        return m_bKeepDistanceAllowed;
    }

    public void NextRound()
    {
        m_iCurrentRound++;

        if (m_iCurrentRound>=RoundPeopleParentNodes.Length || m_iCurrentRound<0)
        {
            Debug.Log("Error! NextRound() - Trying to move to an invalid round");
            return;
        }

        SetPPEAllowed(PPEAvailable[m_iCurrentRound]);
        SetKeepDistanceAllowed(KeepDistanceAvailable[m_iCurrentRound]);
        SetIsolationAllowed(IsolationAvailable[m_iCurrentRound]);

        PanelNewRound.Show(m_iCurrentRound, IsPPEAllowed());        

        ClearInfectedCount();
    }

    public void StartRound()
    {
        m_fRoundTime = RoundTime;
        m_bRoundInProgress = true;

        if (m_iCurrentRound>=RoundPeopleParentNodes.Length || m_iCurrentRound<0)
        {
            Debug.Log("Error! StartRound() - Trying to start an invalid round");
            return;
        }

        PeopleTracker.Instance.ClearList();

        RoundPeopleParentNodes[m_iCurrentRound].SetActive(true);
    }

    public void StopRound()
    {
        m_bRoundInProgress = false;
        RoundPeopleParentNodes[m_iCurrentRound].SetActive(false);
        PeopleTracker.Instance.ClearList();
    }

    public bool IsRoundInProgress()
    {
        return m_bRoundInProgress;
    }

    public int GetInfectedCount()
    {
        return m_iInfectedCount;
    }

    public void IncreaseInfectedCount()
    {
        m_iInfectedCount++;
    }

    private void ClearInfectedCount()
    {
        m_iInfectedCount = 0;
    }

    public int GetCurrentRound()
    {
        return m_iCurrentRound;
    }
}
