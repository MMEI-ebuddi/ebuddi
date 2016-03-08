using UnityEngine;
using System.Collections;

public class TriggerTimer : BaseTrigger {
        
    public float Time = 5f;

    bool m_bExpired = false;

	public override bool Triggered()
	{
        return m_bExpired;
	}

    void OnEnable()
    {
        Invoke("SetTriggered", Time);
    }

    void SetTriggered()
    {
        m_bExpired = true;
    }


}
