using UnityEngine;
using System.Collections;

public abstract class UISurveyElement : MonoBehaviour {

    public int QuestionsToSkip = 0;
    public bool Selectable = false;
        
	public virtual string GetData()
    {
        return "UNDEFINED";
    }
    
    public bool IsSelectable()
    {
        return Selectable;
    }

    public virtual bool Selected()
    {
        return false;
    }

    public virtual bool Answered()
    {
        return true;
    }

    public virtual void InitData(string strData)
    {

    }
}
