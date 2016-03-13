using UnityEngine;
using System.Collections;

public class TriggerAllDestroyed : BaseTrigger {

    public GameObject[] Objects;
	
	public override bool Triggered()
	{
		foreach (GameObject go in Objects)
        {
            if (go != null)
                return false;
        }

        return true;
	}
}
