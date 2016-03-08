using UnityEngine;
using System.Collections;
 
public class CameraFacingBillboard : MonoBehaviour
{
	Camera		m_Camera;
	Transform	m_Transform;
	Transform	m_CameraTransform;

	void Awake()
	{
        Debug.Log("<color=\"#BB22FF\"><b>CFB Awake </b> </color>", this);
		m_Camera = Camera.main;
		m_Transform = GetComponent<Transform>();
		m_CameraTransform = m_Camera.GetComponent<Transform>();
	}
	
	void Update()
	{
        if (m_Transform == null || m_CameraTransform == null)
        {
            m_Camera = Camera.main;
            m_Transform = GetComponent<Transform>();
            m_CameraTransform = m_Camera.GetComponent<Transform>();
            Debug.Log("<color=\"#BB22FF\"><b>CFB issue </b> </color>",this);
			return;
        }
		
		m_Transform.LookAt( m_Transform.position + m_CameraTransform.rotation * Vector3.forward, m_CameraTransform.rotation * Vector3.up );
	}
}