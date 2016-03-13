using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMSupport;

public class Patient : MonoBehaviour
{
	Animator			m_Animator;

	enum eAction
	{
		Cough = 0,
		Moan,
		MAX
	}

	float				m_CountdownToAction;

	void Awake()
	{
		m_Animator = GetComponent<Animator>();
		GetNextTime();
	}

	void GetNextTime()
	{
		m_CountdownToAction = Random.Range(5.0f, 12.0f);
	}

	void Update()
	{
		m_CountdownToAction -= Time.deltaTime;
		if(m_CountdownToAction <= 0)
		{
			eAction			ActionToUse = (eAction)Random.Range(0, (int)eAction.MAX);

			m_Animator.SetTrigger(ActionToUse.ToString());
			GetNextTime();
		}
	}
}
