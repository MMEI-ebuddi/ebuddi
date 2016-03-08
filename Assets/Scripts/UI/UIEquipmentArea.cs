using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIEquipmentArea : MonoBehaviour {

	public List<Equipment> equipment = new List<Equipment>();
	public bool equipmentHighlighted = false;



	public void HighlightCorrectEquipment() {

		List<UIHighlightable> correctEquipment = new List<UIHighlightable>();

		foreach (Equipment e in equipment) {
			if (EquipmentManager.Instance.CanChooseEquipment(e.ItemSO)){
				if (e.GetComponent<UIHighlightable>() !=  null) {
					correctEquipment.Add(e.GetComponent<UIHighlightable>());
				}
			}
		}

		UIHighlightManager.instance.HighlightObjects(correctEquipment);
		equipmentHighlighted = true;
	}


}
