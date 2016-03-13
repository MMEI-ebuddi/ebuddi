using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquippedItems 
{
    private List<BaseActionScriptableObject> m_Items;

    public EquippedItems()
    {
        m_Items = new List<BaseActionScriptableObject>();
    }

    public List<BaseActionScriptableObject> GetItems()
    {
        return m_Items;
    }

    // Attempt to equip the specified item as part of the specified list of items
    public bool EquipItem(BaseActionScriptableObject item, ActionListScriptableObject actionList)
    {
        if (!CanEquipItem(item, actionList))
            return false;

        m_Items.Add(item);

        return true;
    }

    public void RemoveItem(BaseActionScriptableObject item)
    {
        m_Items.Remove(item);
    }
    
    public bool CanEquipItem(BaseActionScriptableObject item, ActionListScriptableObject actionList)
    {
        // Make sure it's an item 
        if (!item.IsItem())

            return false;

        // Check we don't already have the item equipped
        if (m_Items != null && m_Items.Contains(item))


            return false;

        // Check the item is in the list
        if (!actionList.GetActionOrder().Contains(item))
	
            return false;

        // Check we don't have an item in this exclusive category equipped already
        if (!CanEquipCategory(item))

            return false;
        
        return true;
    }

    // Can we equip an item from the specified category, or do we already have a category item equipped?
    private bool CanEquipCategory(BaseActionScriptableObject item)
    {
        if (!item.IsExclusiveCategory())
            return true;

        foreach (BaseActionScriptableObject thisItem in m_Items)
        {
            if (thisItem.Category == item.Category)
                return false;
        }

        return true;
    }  


    // Return the currently equipped items in order of the specified list
    public List<BaseActionScriptableObject> GetOrderedItems(ActionListScriptableObject actionList)
    {
        List<BaseActionScriptableObject> items = new List<BaseActionScriptableObject>();

        // Loop through all the actions in the ordered action list and add any we have equipped to create an ordered list of equipped items
        foreach (BaseActionScriptableObject item in actionList.GetActionOrder())
        {            
            if (!item.IsItem())
                continue;
            
            if (m_Items.Contains(item))
                items.Add(item);
        }

        return items;
    }


    // Return all actions except for item actions that aren't equipped, in order.
    public List<BaseActionScriptableObject> GetOrderedActions(ActionListScriptableObject actionList)
    {
        List<BaseActionScriptableObject> items = new List<BaseActionScriptableObject>();

        foreach (BaseActionScriptableObject item in actionList.GetActionOrder())
        {
            if (item.IsItem() && IsItemEquipped(item))
                items.Add(item);

            if (!item.IsItem() && item.IsAction())
                items.Add(item);
        }

        return items;
    }

    public bool IsItemEquipped(BaseActionScriptableObject actionSO)
    {
        return (actionSO.IsItem() && m_Items.Contains(actionSO));
    }

    // Do we have all the items in the specified action list equipped?
    public bool IsFullyEquipped(ActionListScriptableObject actionList)
    {
        foreach (BaseActionScriptableObject actionSO in actionList.GetOrderedRequiredItems())
        {
            if (IsItemEquipped(actionSO))    {        
//                Debug.Log("Item equipped: " + actionSO.Name);     
			}
            else
            {
                if (actionSO.IsExclusiveCategory())
                {
                    bool bEquipped = false;

                    foreach (BaseActionScriptableObject so in actionList.GetExclusiveItemsForCategory(actionSO.Category))
                    {
                        if (IsItemEquipped(so))
                        {
                            bEquipped = true;
                            break;
                        }
                    }

                    if (!bEquipped)
                        return false;
                }
                else                
                    return false;                
            }
        }

//        Debug.Log("---------- All items equipped!");

        return true;
    }
}
