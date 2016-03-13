using UnityEngine;
using System.Collections;
using System;

public class  PersonCleaner : BasePerson {

    public float CleaningRange = 0.5f;

    public override void Clicked()
    {
        base.Clicked();

        if (!IsInfected() && RoundManager.Instance.IsPPEAllowed())
            SetWearingPPE();
    }

    protected override void Update()
    {
        base.Update();

        // If there's any infection hazards, find the nearest one and head towards it. 
        GameObject[] infectionHazards = GameObject.FindGameObjectsWithTag("InfectionHazard");

        float fNearest = float.MaxValue;
        float fDist = 0f;

        GameObject nearestHazard = null;

        Vector3 myPos = transform.position;
        myPos.y = 0f;

        Vector3 hazardPos;

        for (int i=infectionHazards.Length-1; i>=0;i--)
        {
            GameObject hazard = infectionHazards[i];

            hazardPos = hazard.transform.position;
            hazardPos.y = 0f;

            fDist = Vector3.Distance(myPos, hazardPos);

            if (fDist <= CleaningRange)
            {
                Destroy(hazard);
            }
            else if (fDist < fNearest)
            {
                fNearest = fDist;
                nearestHazard = hazard;
            }
        }

        if (nearestHazard != null)
        {
            CancelIdle();
            m_NavAgent.SetDestination(nearestHazard.transform.position);
        }
    }
}
