using System.Collections;
using NJG;
using UnityEngine;

[ExecuteInEditMode]
public class NJGMapRenderer : MonoBehaviour
{
	private static NJGMapRenderer mInst;

	public int mapImageIndex;

	private Vector2 lastSize;

	private Vector2 mSize;

	private Transform mTrans;

	private bool canRender = true;

	private bool mGenerated;

	private bool mWarning;

	private bool mReaded;

	private bool mApplied;

	private float lastRender;

	private NJGMapBase map;

	public static NJGMapRenderer instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = Object.FindObjectOfType(typeof(NJGMapRenderer)) as NJGMapRenderer;
				if (mInst == null)
				{
					GameObject gameObject = new GameObject("_NJGMapRenderer");
					gameObject.transform.parent = NJGMapBase.instance.transform;
					gameObject.layer = LayerMask.NameToLayer("TransparentFX");
					mInst = gameObject.AddComponent<NJGMapRenderer>();
				}
			}
			return mInst;
		}
	}

	public Transform cachedTransform
	{
		get
		{
			if (mTrans == null)
			{
				mTrans = base.transform;
			}
			return mTrans;
		}
	}

	private void Awake()
	{
		map = NJGMapBase.instance;
		if (map == null)
		{
			Debug.LogWarning("Can't render map photo. NJGMiniMap instance not found.");
			NJGTools.Destroy(base.gameObject);
			return;
		}
		if (base.gameObject.GetComponent<Camera>() == null)
		{
			base.gameObject.AddComponent<Camera>();
		}
		Render();
	}

	private void Start()
	{
		if (map.boundLayers.value == 0)
		{
			Debug.LogWarning("Can't render map photo. You have not choosen any layer for bounds calculation. Go to the NJGMiniMap inspector.", map);
			NJGTools.DestroyImmediate(base.gameObject);
			return;
		}
		if (map.renderLayers.value == 0)
		{
			Debug.LogWarning("Can't render map photo. You have not choosen any layer for rendering. Go to the NJGMiniMap inspector.", map);
			NJGTools.DestroyImmediate(base.gameObject);
			return;
		}
		ConfigCamera();
		if (!Application.isPlaying)
		{
			Render();
		}
	}

	private void ConfigCamera()
	{
		map.UpdateBounds();
		Bounds bounds = map.bounds;
		base.GetComponent<Camera>().depth = -100f;
		base.GetComponent<Camera>().backgroundColor = map.cameraBackgroundColor;
		base.GetComponent<Camera>().cullingMask = map.renderLayers;
		base.GetComponent<Camera>().clearFlags = (CameraClearFlags)map.cameraClearFlags;
		base.GetComponent<Camera>().orthographic = true;
		float orthographicSize = 0f;
		if (map.orientation == NJGMapBase.Orientation.XYSideScroller)
		{
			base.GetComponent<Camera>().farClipPlane = bounds.size.z * 1.1f;
			orthographicSize = bounds.extents.y;
			base.GetComponent<Camera>().aspect = bounds.size.x / bounds.size.y;
		}
		else if (map.orientation == NJGMapBase.Orientation.XZDefault)
		{
			base.GetComponent<Camera>().farClipPlane = bounds.size.y * 1.1f;
			orthographicSize = bounds.extents.z;
			base.GetComponent<Camera>().aspect = bounds.size.x / bounds.size.z;
		}
		base.GetComponent<Camera>().farClipPlane = base.GetComponent<Camera>().farClipPlane * 5f;
		base.GetComponent<Camera>().orthographicSize = orthographicSize;
		if (map.orientation == NJGMapBase.Orientation.XZDefault)
		{
			cachedTransform.eulerAngles = new Vector3(90f, 0f, 0f);
			if (map.mapResolution == NJGMapBase.Resolution.Double)
			{
				for (int i = 0; i < 4; i++)
				{
					switch (i)
					{
					case 0:
						cachedTransform.position = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y + 1f, bounds.center.z - bounds.extents.z);
						break;
					case 1:
						cachedTransform.position = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y + 1f, bounds.center.z - bounds.extents.z);
						break;
					case 2:
						cachedTransform.position = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y + 1f, bounds.center.z + bounds.extents.z);
						break;
					case 3:
						cachedTransform.position = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y + 1f, bounds.center.z + bounds.extents.z);
						break;
					}
					Debug.Log(string.Concat("cachedTransform.position ", cachedTransform.position, " / mapImageIndex ", mapImageIndex));
					base.GetComponent<Camera>().enabled = true;
					mapImageIndex = i;
				}
			}
			else
			{
				cachedTransform.position = new Vector3(bounds.max.x - bounds.extents.x, bounds.size.y * 2f, bounds.center.z);
				base.GetComponent<Camera>().enabled = true;
			}
		}
		else
		{
			cachedTransform.eulerAngles = new Vector3(0f, 0f, 0f);
			cachedTransform.position = new Vector3(bounds.max.x - bounds.extents.x, bounds.center.y, 0f - (Mathf.Abs(bounds.min.z) + Mathf.Abs(bounds.max.z) + 10f));
		}
	}

	private IEnumerator OnPostRender()
	{
		if (!Application.isPlaying || NJGMapBase.instance.renderMode == NJGMapBase.RenderMode.Dynamic)
		{
			ConfigCamera();
		}
		if (mGenerated && map.optimize && Application.isPlaying && !mWarning)
		{
			mWarning = true;
			Debug.LogWarning("Can't Re-generate the map texture since 'optimize' is activated");
			canRender = false;
		}
		else if (canRender)
		{
			if (map.mapTexture == null)
			{
				mSize = map.mapSize;
				if (map.mapResolution == NJGMapBase.Resolution.Double)
				{
					mSize = map.mapSize * 2f;
				}
				map.mapTexture = new Texture2D((int)mSize.x, (int)mSize.y, (TextureFormat)map.textureFormat, map.generateMipmaps);
				map.mapTexture.name = "_NJGMapTexture";
				map.mapTexture.filterMode = map.mapFilterMode;
				map.mapTexture.wrapMode = map.mapWrapMode;
				lastSize = mSize;
			}
			if (!mReaded || !Application.isPlaying)
			{
				if (map.generateMapTexture && canRender)
				{
					if (NJGMapBase.instance.renderMode != 0)
					{
						mSize = map.mapSize;
						if (map.mapResolution == NJGMapBase.Resolution.Double)
						{
							mSize = map.mapSize * 2f;
						}
						if (mSize.x >= lastSize.x || mSize.y >= lastSize.y)
						{
							lastSize = mSize;
							map.mapTexture.Resize((int)mSize.x, (int)mSize.y);
						}
					}
					if (map.mapResolution == NJGMapBase.Resolution.Double)
					{
						Bounds bounds = map.bounds;
						for (int i = 0; i < 4; i++)
						{
							switch (i)
							{
							case 0:
								map.mapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, map.mapSize.y), 0, 0, map.generateMipmaps);
								cachedTransform.position = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y + 1f, bounds.center.z - bounds.extents.z);
								break;
							case 1:
								cachedTransform.position = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y + 1f, bounds.center.z - bounds.extents.z);
								map.mapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, map.mapSize.y), (int)map.mapSize.x, 0, map.generateMipmaps);
								break;
							case 2:
								cachedTransform.position = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y + 1f, bounds.center.z + bounds.extents.z);
								map.mapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, map.mapSize.y), (int)map.mapSize.x, (int)map.mapSize.y, map.generateMipmaps);
								break;
							case 3:
								cachedTransform.position = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y + 1f, bounds.center.z + bounds.extents.z);
								mReaded = true;
								map.mapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, map.mapSize.y), 0, (int)map.mapSize.y, map.generateMipmaps);
								break;
							}
							Debug.Log(string.Concat("mapImageIndex ", i, " / map.mapSize ", map.mapSize, " / cachedTransform.position ", cachedTransform.position, " / mReaded ", mReaded));
						}
					}
					else
					{
						mReaded = true;
						map.mapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, map.mapSize.y), 0, 0, map.generateMipmaps);
					}
				}
				else if (map.userMapTexture != null)
				{
					map.userMapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, map.mapSize.y), 0, 0, map.generateMipmaps);
				}
			}
			if (!mApplied)
			{
				mApplied = true;
				if (map.generateMapTexture)
				{
					if (map.optimize)
					{
						map.mapTexture.Compress(true);
						canRender = false;
					}
					map.mapTexture.Apply(map.generateMipmaps, map.optimize);
				}
				else
				{
					map.userMapTexture.Apply(map.generateMipmaps, map.optimize);
				}
			}
			yield return new WaitForEndOfFrame();
			if (canRender && !mGenerated)
			{
				if (Application.isPlaying)
				{
					mGenerated = true;
				}
				map.SetTexture((!map.generateMapTexture) ? map.userMapTexture : map.mapTexture);
			}
			if (base.GetComponent<Camera>().enabled && Application.isPlaying)
			{
				base.GetComponent<Camera>().enabled = false;
			}
		}
		lastRender = Time.time + 1f;
	}

	public void Render()
	{
		if (!(Time.time >= lastRender))
		{
			return;
		}
		if (Application.isPlaying)
		{
			lastRender = Time.time + 1f;
		}
		mReaded = false;
		mApplied = false;
		mGenerated = false;
		mWarning = false;
		if (!map.optimize)
		{
			canRender = true;
		}
		if (map.mapSize.x == 0f || map.mapSize.y == 0f)
		{
			map.mapSize = new Vector2(Screen.width, Screen.height);
		}
		if (map.generateMapTexture)
		{
			if (map.userMapTexture != null)
			{
				NJGTools.Destroy(map.userMapTexture);
				map.userMapTexture = null;
			}
			if (map.mapTexture == null)
			{
				mSize = map.mapSize;
				if (map.mapResolution == NJGMapBase.Resolution.Double)
				{
					mSize = map.mapSize * 2f;
				}
				map.mapTexture = new Texture2D((int)mSize.x, (int)mSize.y, (TextureFormat)map.textureFormat, map.generateMipmaps);
				map.mapTexture.name = "_NJGMapTexture";
				map.mapTexture.filterMode = map.mapFilterMode;
				map.mapTexture.wrapMode = map.mapWrapMode;
				lastSize = mSize;
			}
		}
		else if (!Application.isPlaying)
		{
			if (map.mapTexture != null)
			{
				NJGTools.DestroyImmediate(map.mapTexture);
				map.mapTexture = null;
			}
			map.userMapTexture = new Texture2D((int)map.mapSize.x, (int)map.mapSize.y, (TextureFormat)map.textureFormat, map.generateMipmaps);
			map.userMapTexture.name = "_NJGTempTexture";
			map.userMapTexture.filterMode = map.mapFilterMode;
			map.userMapTexture.wrapMode = map.mapWrapMode;
		}
		ConfigCamera();
		base.GetComponent<Camera>().enabled = true;
	}
}
