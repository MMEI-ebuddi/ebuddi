using System;
using System.Collections.Generic;
using UnityEngine;
using TMSupport;



//TODO Remove this class if possible. Moving into per scene camera controller with a base slass and scene subclasses
public class OurCamera : MonoBehaviour
{
	private static OurCamera	sInstance;
	public static OurCamera		Instance { get { return sInstance; } }

	[Header("General")]
	public Camera			m_MainCamera;
	public Camera			m_BaseCamera;
	public Camera			m_RackCamera;
	public Camera			m_AvatarCamera;
	public Camera			m_BuddyCamera;

	[Header("Scene1")]
	public Camera			m_HeadZoomCamera;

	[Header("Scene2")]
	public Camera			m_AlternateBaseCamera;

	[Header("Scene3")]
	public Camera			m_BucketCamera;
	public Camera			m_BinCamera;



	public enum eCameraFocus
	{
		Main = 0,
		Rack,
		Avatar,
		Buddy,
		MainAlternate,
		Bucket,
		Bin,
		HeadZoom,
		MAX,
	}
	eCameraFocus		m_CameraFocus;
	eCameraFocus		m_NewCameraFocus;

	Transform			m_Transform;
	float				m_Timer;
	float				m_CurrentTime;

	List<Vector3>		m_Positions;
	List<Vector3>		m_Rotations;

	Func<float, float, float, float, float>		m_EaseFunction;

	Vector3				m_CameraOffset;

	private Camera UI3DCamera;


	void OnDestroy()
	{
		TMMessenger.Instance.RemoveFromMessageQueue(this);
	}

	void Awake()
	{
		sInstance = this;

		m_Transform = GetComponent<Transform>();
		m_Positions = new List<Vector3>(4);
		m_Rotations = new List<Vector3>(4);
		m_CameraOffset = Vector3.zero;

		AddCamera(m_BaseCamera);
		AddCamera(m_RackCamera);
		AddCamera(m_AvatarCamera);
		AddCamera(m_BuddyCamera);
		AddCamera(m_AlternateBaseCamera);
		AddCamera(m_BucketCamera);
		AddCamera(m_BinCamera);
		AddCamera(m_HeadZoomCamera);

		MoveTo(eCameraFocus.Main, 0);

		if (GameObject.Find("3DUICamera") != null) {
			UI3DCamera = GameObject.Find("3DUICamera").GetComponent<Camera>();
		}
	}



	private void AddCamera(Camera NewCamera)
	{
		if (NewCamera != null)
		{
			m_Positions.Add(NewCamera.transform.position);
			m_Rotations.Add(NewCamera.transform.rotation.eulerAngles);
			NewCamera.gameObject.SetActive(false);
		}
		else
		{
			m_Positions.Add(Vector3.zero);
			m_Rotations.Add(Vector3.zero);
		}
	}

	public bool MoveTo(eCameraFocus Focus, float Time)
	{
		bool		NewFocus = m_CameraFocus != Focus;

		if(Time == 0)
		{
			m_NewCameraFocus = m_CameraFocus = Focus;
			m_CurrentTime = 0;
			SetPositionAndRotation(Focus);
		}
		else
		{
			m_NewCameraFocus = Focus;
			m_CurrentTime = m_Timer = Time;
		}
		return(NewFocus);
	}

	void SetPositionAndRotation(eCameraFocus Focus)
	{
		SetPositionAndRotation(m_Positions[(int)Focus] + m_CameraOffset, m_Rotations[(int)Focus]);
	}

	private void SetPositionAndRotation(Vector3 CameraPosition, Vector3 CameraRotation)
	{
		m_Transform.position = CameraPosition;
		m_Transform.rotation = Quaternion.Euler(CameraRotation);
	}

	void Update()
	{

		if(m_NewCameraFocus != m_CameraFocus)
		{
			m_CurrentTime -= Time.deltaTime;
			if(m_CurrentTime <= 0)
			{
				m_CameraFocus = m_NewCameraFocus;
				SetPositionAndRotation(m_CameraFocus);
				TMSupport.TMMessenger.Send("CameraCutFinished".GetHashCode());
			}
			else
			{
				float		Interp = m_CurrentTime / m_Timer;

				m_EaseFunction = GoTweenUtils.easeFunctionForType(GoEaseType.QuadInOut);
				Interp = m_EaseFunction(Interp, 0, 1, 1);

				Vector3		InterpPosition = Vector3.Lerp(m_Positions[(int)m_NewCameraFocus], m_Positions[(int)m_CameraFocus], Interp);
				Vector3		InterpRotation = Vector3.Lerp(m_Rotations[(int)m_NewCameraFocus], m_Rotations[(int)m_CameraFocus], Interp);

				SetPositionAndRotation(InterpPosition, InterpRotation);
			}
		}

	}

	internal void SetCameraInterp(eCameraFocus StartFocus, eCameraFocus EndFocus, float Interp)
	{
		m_EaseFunction = GoTweenUtils.easeFunctionForType(GoEaseType.QuadInOut);
		Interp = m_EaseFunction(Interp, 0, 1, 1);

		Vector3		InterpPosition = Vector3.Lerp(m_Positions[(int)StartFocus], m_Positions[(int)EndFocus], Interp);
		Vector3		InterpRotation = Vector3.Lerp(m_Rotations[(int)StartFocus], m_Rotations[(int)EndFocus], Interp);

		SetPositionAndRotation(InterpPosition, InterpRotation);
	}

	internal void SetCameraOffset(Vector3 CameraOffset)
	{
		m_CameraOffset = CameraOffset;
	}



	/// <summary>
	/// Blurs whe camera view
	/// </summary>
	public void BlurOn()
    {
        #if (!UNITY_ANDROID && !UNITY_IOS)
		iTween.ValueTo(this.gameObject, iTween.Hash("from", 0, "to", 1f, "time", 0.75f, "onupdate", "AnimateBlur"));
        #endif
	}

	public void BlurOff()
    {
        #if (!UNITY_ANDROID && !UNITY_IOS)
		iTween.ValueTo(this.gameObject, iTween.Hash("from", 1f, "to", 0, "time", 0.75f, "onupdate", "AnimateBlur"));
        #endif
	}
	






}
