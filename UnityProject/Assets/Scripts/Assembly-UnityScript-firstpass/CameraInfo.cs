using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Camera Info")]
public class CameraInfo : MonoBehaviour
{
	public DepthTextureMode currentDepthMode;

	public RenderingPath currentRenderPath;

	public int recognizedPostFxCount;

	public virtual void Main()
	{
	}
}
