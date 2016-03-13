using UnityEngine;
using System.Collections;

public class TestEquippedItems : MonoBehaviour {

    public ActionListScriptableObject ActionList;

    EquippedItems m_EquippedItems;

    public BaseActionScriptableObject[] Items;

	// Use this for initialization
	void Start () {
        m_EquippedItems = new EquippedItems();

        foreach (BaseActionScriptableObject item in Items)
        {
            Debug.Log("Equipped item: " + item.Name + ":" + m_EquippedItems.EquipItem(item, ActionList));
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
