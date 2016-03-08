using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ActionListScriptableObject : ScriptableObject {

    public List<BaseActionScriptableObject> ActionOrder;

    public bool ShowUnusedEquipment = true;

    private List<BaseActionScriptableObject> m_ActionOrder;

    public BaseActionScriptableObject ActionWashHands;

    void OnEnable()
    {
        BuildActionList();
    }

    // Take the public list of actions and create a private list with any automatically added actions inserted
    private void BuildActionList()
    {
        m_ActionOrder = new List<BaseActionScriptableObject>();

        foreach (BaseActionScriptableObject action in ActionOrder)
        {
            m_ActionOrder.Add(action);

            //if (action.WashHandsAter)
            //    m_ActionOrder.Add(ActionWashHands);
        }
    }

    public  List<BaseActionScriptableObject> GetActionOrder()
    {
        return m_ActionOrder;
    }

    // Return only ITEM actions in this action list that are REQUIRED (non-optional)
    public List<BaseActionScriptableObject> GetOrderedRequiredItems()
    {
        List<BaseActionScriptableObject> items = new List<BaseActionScriptableObject>();

        foreach (BaseActionScriptableObject action in m_ActionOrder)
        {
            if (action.IsItem() && !action.IsOptional())
                items.Add(action);
        }

        return items;
    }

    // Return all items for the specified exclusive category
    public List<BaseActionScriptableObject> GetExclusiveItemsForCategory(BaseActionScriptableObject.EXCLUSIVE_CATEGORY category)
    {
        List<BaseActionScriptableObject> items = new List<BaseActionScriptableObject>();

        foreach (BaseActionScriptableObject action in m_ActionOrder)
        {
            if (action.IsItem() && action.Category == category)
                items.Add(action);
        }

        return items;
    }
}
