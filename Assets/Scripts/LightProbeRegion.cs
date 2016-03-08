#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;




public class LightProbeRegion : MonoBehaviour
{
	internal float width = 32.0f;
	internal float height = 32.0f;
	internal float depth = 32.0f;
	public float hDensity = 10.0f;
	public float wDensity = 10.0f;
	public float dDensity = 10.0f;
	public bool Distribute = false;
	public bool ShowDensityField = false;
	
	void Start()
	{
	}
	
	[MenuItem("triffCo/Add Light Probe Region")]
	static void Init()
	{
		GameObject LPUniverse = GameObject.Find("Light Probe Universe");
		if (LPUniverse == null)
		{
			LPUniverse = new GameObject("Light Probe Universe");
		}
		GameObject NewRegion = new GameObject("Light Probe Region");
		NewRegion.transform.parent = LPUniverse.transform;
		NewRegion.AddComponent<LightProbeRegion>();
	}
	
	void Update()
	{
	}
	
	public void ClearMesh()
	{
		DestroyLightProbeGroup();
	}
	
	void DestroyLightProbeGroup()
	{
		LightProbeGroup LPG = GetComponent<LightProbeGroup>();
		if( LPG != null )
			DestroyImmediate( LPG );
	}

	public void GenerateMesh()
	{
		DestroyLightProbeGroup();

		LightProbeGroup LPG = gameObject.AddComponent<LightProbeGroup>();
		Vector3[] lpPositions;
		
		Vector3 scale = transform.localScale;
		
		int nodecount = 0;
		int nodes;
		if (Distribute == false)
		{
			nodes = Mathf.FloorToInt(((height*scale.y/hDensity)+1)*((width*scale.x/wDensity)+1)*((depth*scale.z/dDensity)+1));
			//Debug.Log("Objective Spacing Found " + nodes.ToString());
			lpPositions = new Vector3[nodes];
			
			for(float yloc = -(height*scale.y/2); yloc <= (height*scale.y/2); yloc += hDensity)
			{
				for(float xloc = -(width*scale.x/2); xloc <= (width*scale.x/2); xloc += wDensity)
				{
					for(float zloc = -(depth*scale.z/2); zloc <= (depth*scale.z/2); zloc += dDensity)
					{
						if (nodecount >= nodes)
						{
							//Debug.LogWarning("Objective Spacing Iterated more cycles than we have nodes, Node Counter at " + nodecount.ToString());
							break;
						}
						
						lpPositions[nodecount].Set(xloc/scale.x, yloc/scale.y, zloc/scale.z);
						
						nodecount += 1;
					}
					if (nodecount >= nodes)
					{
						//Debug.LogWarning("Objective Spacing Aborting Count");
						break;
					}
				}
				if (nodecount >= nodes)
				{
					//Debug.LogWarning("OObjective Spacing Count Aborted");
					break;
				}
			}
		}
		else
		{
			nodes = Mathf.FloorToInt((hDensity+1)*(wDensity+1)*(dDensity+1));
			//Debug.Log("Distributive Spacing Found " + nodes.ToString());
			lpPositions = new Vector3[nodes];
			int iterCount = 0;
			
			for(float yloc = -(height/2); yloc <= (height/2); yloc += height/hDensity)
			{
				for(float xloc = -(width/2); xloc <= (width/2); xloc += width/wDensity)
				{
					for(float zloc = -(depth/2); zloc <= (depth/2); zloc += depth/dDensity)
					{
						iterCount += 1;
						if (nodecount >= nodes)
						{
							//Debug.LogWarning("Distributive Spacing Iterated more cycles than we have nodes, Node Counter at " + nodecount.ToString());
							break;
						}
						
						lpPositions[nodecount].Set(xloc, yloc, zloc);
						
						nodecount += 1;
					}
					if (nodecount >= nodes)
					{
						//Debug.LogWarning("Distributive Spacing Aborting Count");
						break;
					}
				}
				if (nodecount >= nodes)
				{
					//Debug.LogWarning("Distributive Spacing Count Aborted");
					break;
				}
			}
			//Debug.Log("Iterated through " + iterCount.ToString() + " nodes");
		}
		
		LPG.probePositions = lpPositions;
	}
	
	void OnDrawGizmos()
	{
		Vector3 scale = transform.localScale;
		
		Gizmos.color = Color.blue;
		Gizmos.matrix = transform.localToWorldMatrix;
		
		Gizmos.DrawWireCube(new Vector3(0,0,0),
							new Vector3(width, height, depth));
		
		if (ShowDensityField)
		{
			if (Distribute == false)
			{
				
				for(float yloc = -(height*scale.y/2); yloc <= (height*scale.y/2); yloc += hDensity)
				{
					for(float xloc = -(width*scale.x/2); xloc <= (width*scale.x/2); xloc += wDensity)
					{
						for(float zloc = -(depth*scale.z/2); zloc <= (depth*scale.z/2); zloc += dDensity)
						{
							// X lines
							Gizmos.DrawLine(new Vector3((width/2), yloc/scale.y, zloc/scale.z),
											new Vector3(-(width/2), yloc/scale.y, zloc/scale.z));
							// Z lines
							Gizmos.DrawLine(new Vector3(xloc/scale.x, yloc/scale.y, (depth/2)),
											new Vector3(xloc/scale.x, yloc/scale.y, -(depth/2)));
							// Y lines
							Gizmos.DrawLine(new Vector3(xloc/scale.x, (height/2), zloc/scale.z),
											new Vector3(xloc/scale.x, -(height/2), zloc/scale.z));
						}
					}
					
				}
			}
			else
			{
				for(float yloc = -(height/2); yloc <= (height/2); yloc += height/hDensity)
				{
					for(float xloc = -(width/2); xloc <= (width/2); xloc += width/wDensity)
					{
						for(float zloc = -(width/2); zloc <= (width/2); zloc += depth/dDensity)
						{
							// X lines
							Gizmos.DrawLine(new Vector3((width/2), yloc, zloc), new Vector3(-(width/2), yloc, zloc));
							
							// Z lines
							Gizmos.DrawLine(new Vector3(xloc, yloc, (depth/2)), new Vector3(xloc, yloc, -(depth/2)));
							
							// Y lines
							Gizmos.DrawLine(new Vector3(xloc, (height/2), zloc), new Vector3(xloc, -(height/2), zloc));
						}
					}
				}
			}
		}
	}
	
	void OnDestroy()
	{
		//DestroyLightProbeGroup();		getting an error on build due to multiple calls to DestroyImmediate
	}
}

#endif