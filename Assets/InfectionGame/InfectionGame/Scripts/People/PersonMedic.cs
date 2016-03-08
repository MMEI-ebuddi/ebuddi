using UnityEngine;
using System.Collections;
using System;

public class  PersonMedic : BasePerson 
{
    public override void Clicked()
    {
        base.Clicked();

        if (!IsInfected() && RoundManager.Instance.IsPPEAllowed())
            SetWearingPPE();        
    }  
}
