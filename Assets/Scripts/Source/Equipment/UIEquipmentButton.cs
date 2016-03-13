using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMSupport;

public class UIEquipmentButton : MonoBehaviour 
{
	public EquipmentManager.eEquipment		m_Equipment;
	public Sprite							m_Sprite;

	void Start () 
	{
	}

	public void OnClicked()
	{
        Debug.Log("--------- UIEquipmentButton::OnClicked() - REFACTOR: NEEDS UPDATING TO CALL CLICKED WITH THIS ITEM'S SO.");
		//EquipmentManager.Instance.Clicked(m_Equipment);
	}
}
