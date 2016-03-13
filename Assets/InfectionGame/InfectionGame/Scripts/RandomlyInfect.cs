using UnityEngine;
using System.Collections;

/// <summary>
/// This component is attached to a person who needs to randomly become infected. A random time range can be speciefied and a % chance for the infection check to pass and become infected
/// </summary>
public class RandomlyInfect : MonoBehaviour 
{
    public float InfectTimerMin = 5f;       // Random time range values for performing infection roll
    public float InfectTimerMax = 30f;

    public float[] InfectionPercentages;

    private float m_fInfectionChance = 0f;

    private BasePerson m_Person;

	// Use this for initialization
	void Start () 
    {
        m_Person = GetComponent<BasePerson>();

        InvokeNextRoll();

        m_fInfectionChance = InfectionPercentages[RoundManager.Instance.GetCurrentRound()];
	}
	
    // Roll the dice based on our chance of randomly becoming infected
    private void PerformInfectionRoll()
    {
        if (m_Person == null)
            return;

        if (m_Person.IsInfected() || !m_Person.IsVulnerableToInfection())
            return;
        
        // Perform the random chance check to see if this person should become infected
        if (Random.Range(0, 100) <= m_fInfectionChance)
        {
            m_Person.SetInfected();
        }

        // We should always invoke the next check incase something happens that makes us lose our infection or PPE gets removed, etc
        InvokeNextRoll();
    }

    private void InvokeNextRoll()
    {
        Invoke("PerformInfectionRoll", Random.Range(InfectTimerMin, InfectTimerMax));
    }

}
