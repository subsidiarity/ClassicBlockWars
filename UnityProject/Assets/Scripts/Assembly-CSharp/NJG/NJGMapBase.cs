using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace NJG
{
	[ExecuteInEditMode]
	public class NJGMapBase : MonoBehaviour
	{
		[Serializable]
		public class FOW
		{
			public enum FOWSystem
			{
				BuiltInFOW = 0,
				TasharenFOW = 1
			}

			public bool enabled;

			public FOWSystem fowSystem;

			public bool trailEffect;

			public float textureBlendTime = 0.5f;

			public float updateFrequency = 0.15f;

			public Color fogColor = Color.black;

			public int revealDistance = 10;

			public int textureSize = 200;

			public bool debug;

			public int blurIterations = 2;
		}

		[Serializable]
		public class MapItemType
		{
			public bool enableInteraction = true;

			public string type = "New Marker";

			public string sprite;

			public string selectedSprite;

			public bool useCustomSize;

			public bool useCustomBorderSize;

			public int size = 32;

			public int borderSize = 32;

			public Color color = Color.white;

			public bool animateOnVisible = true;

			public bool showOnAction;

			public bool loopAnimation;

			public float fadeOutAfterDelay;

			public bool rotate = true;

			public bool updatePosition = true;

			public bool haveArrow;

			public string arrowSprite;

			public bool folded = true;

			public int depth;

			public bool deleteRequest;

			public int arrowOffset = 20;

			public int arrowDepth = 5;

			public bool arrowRotate = true;

			public void OnSelectSprite(string spriteName)
			{
				sprite = spriteName;
			}

			public void OnSelectBorderSprite(string spriteName)
			{
				selectedSprite = spriteName;
			}

			public void OnSelectArrowSprite(string spriteName)
			{
				arrowSprite = spriteName;
			}
		}

		[Serializable]
		public class MapLevel
		{
			public string level = "Level";

			public List<MapZone> zones = new List<MapZone>();

			public bool folded = true;

			public bool itemsFolded = true;

			public bool deleteRequest;
		}

		[Serializable]
		public class MapZone
		{
			public string type = "New Zone";

			public Color color = Color.white;

			public float fadeOutAfterDelay = 3f;

			public bool folded = true;

			public int depth;

			public bool deleteRequest;
		}

		public enum SettingsScreen
		{
			General = 0,
			Icons = 1,
			FOW = 2,
			Zones = 3,
			_LastDoNotUse = 4
		}

		public enum Resolution
		{
			Normal = 0,
			Double = 1
		}

		public enum RenderMode
		{
			Once = 0,
			ScreenChange = 1,
			Dynamic = 2
		}

		public enum NJGTextureFormat
		{
			ARGB32 = 5,
			RGB24 = 3
		}

		public enum NJGCameraClearFlags
		{
			Skybox = 1,
			Depth = 3,
			Color = 2,
			Nothing = 4
		}

		public enum ShaderType
		{
			TextureMask = 0,
			ColorMask = 1,
			FOW = 2
		}

		[SerializeField]
		public enum Orientation
		{
			XZDefault = 0,
			XYSideScroller = 1
		}

		public const string VERSION = "1.5";

		[SerializeField]
		public UIMiniMapBase miniMap;

		[SerializeField]
		public UIWorldMapBase worldMap;

		private static NJGMapBase mInst;

		private static GameObject mZRoot;

		public Action<string> onWorldNameChanged;

		[SerializeField]
		public FOW fow;

		[SerializeField]
		public bool showBounds = true;

		[SerializeField]
		public Color zoneColor = Color.white;

		public List<MapItemType> mapItems = new List<MapItemType>(new MapItemType[1]
		{
			new MapItemType
			{
				type = "None"
			}
		});

		public List<MapLevel> levels = new List<MapLevel>();

		[SerializeField]
		public RenderMode renderMode;

		[SerializeField]
		public Resolution mapResolution;

		[SerializeField]
		public float dynamicRenderTime = 1f;

		public Orientation orientation;

		public SettingsScreen screen;

		[SerializeField]
		public LayerMask renderLayers = 1;

		[SerializeField]
		public LayerMask boundLayers = 1;

		public int iconSize = 16;

		public int borderSize = 16;

		public int arrowSize = 16;

		public float updateFrequency = 0.01f;

		public bool setBoundsManually;

		[SerializeField]
		public Vector3 manualBounds = new Vector3(10f, 10f, 10f);

		[SerializeField]
		public Bounds bounds;

		public bool typesFolded;

		public bool zonesFolded;

		public Texture2D mapTexture;

		public Texture2D userMapTexture;

		public bool generateMapTexture;

		public bool generateAtStart = true;

		public Camera cameraFrustum;

		public Color cameraFrustumColor = new Color(255f, 255f, 255f, 50f);

		public bool useTextureGenerated;

		[SerializeField]
		public FilterMode mapFilterMode = FilterMode.Bilinear;

		[SerializeField]
		public TextureWrapMode mapWrapMode = TextureWrapMode.Clamp;

		[SerializeField]
		public NJGTextureFormat textureFormat = NJGTextureFormat.ARGB32;

		[SerializeField]
		public NJGCameraClearFlags cameraClearFlags = NJGCameraClearFlags.Skybox;

		public Color cameraBackgroundColor = Color.red;

		public bool transparentTexture;

		public bool optimize;

		public bool generateMipmaps;

		public int renderOffset = 10;

		public int layer;

		public List<Action> queue = new List<Action>();

		protected Camera mCam;

		private Vector2 mSize = new Vector2(1024f, 1024f);

		private Bounds mBounds;

		[SerializeField]
		private string mWorldName = "My Epic World";

		private string mLastWorldName;

		private Vector3 mMapOrigin = Vector2.zero;

		private Vector3 mMapEulers = Vector2.zero;

		private float mOrtoSize;

		private float mAspect;

		private Thread mThread;

		private float mElapsed;

		private Terrain[] mTerrains;

		public static NJGMapBase instance
		{
			get
			{
				if (mInst == null)
				{
					mInst = UnityEngine.Object.FindObjectOfType(typeof(NJGMapBase)) as NJGMapBase;
				}
				return mInst;
			}
		}

		public static GameObject zonesRoot
		{
			get
			{
				if (mZRoot == null)
				{
					mZRoot = GameObject.Find("_MapZones");
				}
				return mZRoot;
			}
			set
			{
				mZRoot = value;
			}
		}

		[SerializeField]
		public string worldName
		{
			get
			{
				return mWorldName;
			}
			set
			{
				mWorldName = value;
				if (mLastWorldName != mWorldName)
				{
					mLastWorldName = mWorldName;
					if (onWorldNameChanged != null)
					{
						onWorldNameChanged(mWorldName);
					}
				}
			}
		}

		public virtual bool isMouseOver
		{
			get
			{
				return UIMiniMapBase.inst == null || UIMiniMapBase.inst.isMouseOver || UIWorldMapBase.inst == null || UIWorldMapBase.inst.isMouseOver;
			}
		}

		[SerializeField]
		public Vector3 mapOrigin
		{
			get
			{
				if (NJGMapRenderer.instance != null)
				{
					mMapOrigin = NJGMapRenderer.instance.cachedTransform.position;
				}
				return mMapOrigin;
			}
		}

		[SerializeField]
		public Vector3 mapEulers
		{
			get
			{
				if (NJGMapRenderer.instance != null)
				{
					mMapEulers = NJGMapRenderer.instance.cachedTransform.eulerAngles;
				}
				return mMapEulers;
			}
		}

		[SerializeField]
		public float ortoSize
		{
			get
			{
				if (NJGMapRenderer.instance != null)
				{
					mOrtoSize = NJGMapRenderer.instance.GetComponent<Camera>().orthographicSize;
				}
				return mOrtoSize;
			}
		}

		[SerializeField]
		public float aspect
		{
			get
			{
				if (NJGMapRenderer.instance != null)
				{
					mAspect = NJGMapRenderer.instance.GetComponent<Camera>().aspect;
				}
				return mAspect;
			}
		}

		[SerializeField]
		public Vector2 mapSize
		{
			get
			{
				if (Application.isPlaying)
				{
					mSize.x = Screen.width;
					mSize.y = Screen.height;
				}
				return mSize;
			}
			set
			{
				mSize = value;
			}
		}

		public float elapsed
		{
			get
			{
				return mElapsed;
			}
		}

		[SerializeField]
		public string[] mapItemTypes
		{
			get
			{
				List<string> list = new List<string>();
				int i = 0;
				for (int count = mapItems.Count; i < count; i++)
				{
					list.Add(mapItems[i].type);
				}
				return (list.Count != 0) ? list.ToArray() : new string[1] { "No types defined" };
			}
		}

		private void Awake()
		{
			if (fow.textureSize < 200)
			{
				fow.textureSize = 200;
			}
			if (miniMap == null)
			{
				miniMap = UnityEngine.Object.FindObjectOfType(typeof(UIMiniMapBase)) as UIMiniMapBase;
			}
			if (worldMap == null)
			{
				worldMap = UnityEngine.Object.FindObjectOfType(typeof(UIWorldMapBase)) as UIWorldMapBase;
			}
			if (Application.isPlaying)
			{
				if (mapTexture != null)
				{
					NJGTools.Destroy(mapTexture);
				}
				if (generateAtStart)
				{
					GenerateMap();
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (showBounds)
			{
				Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
				Gizmos.DrawWireCube(bounds.center, bounds.size);
			}
		}

		public void GenerateMap()
		{
			if ((Application.isPlaying && generateMapTexture) || (!Application.isPlaying && !generateMapTexture))
			{
				NJGMapRenderer.instance.Render();
			}
		}

		private void Start()
		{
			if (onWorldNameChanged != null)
			{
				onWorldNameChanged(worldName);
			}
			UpdateBounds();
			if (Application.isPlaying)
			{
				if (fow.enabled)
				{
					NJGFOW.instance.Init();
				}
				InvokeRepeating("UpdateBounds", 0.1f, 0.5f);
				Invoke("stopUpdateBounds", 3f);
			}
		}

		private void stopUpdateBounds()
		{
			CancelInvoke("UpdateBounds");
		}

		private void ThreadUpdate()
		{
			Stopwatch stopwatch = new Stopwatch();
			while (true)
			{
				stopwatch.Reset();
				stopwatch.Start();
				queue.ForEach(delegate(Action a)
				{
					a();
				});
				stopwatch.Stop();
				mElapsed = 0.001f * (float)stopwatch.ElapsedMilliseconds;
				Thread.Sleep(1);
			}
		}

		public void SetTexture(Texture2D tex)
		{
			if (UIMiniMapBase.inst != null)
			{
				UIMiniMapBase.inst.material.mainTexture = tex;
			}
			if (UIWorldMapBase.inst != null)
			{
				UIWorldMapBase.inst.material.mainTexture = tex;
			}
		}

		public static bool IsInRenderLayers(GameObject obj, LayerMask mask)
		{
			return (mask.value & (1 << obj.layer)) > 0;
		}

		public void UpdateBounds()
		{
			if (setBoundsManually)
			{
				mBounds = new Bounds(manualBounds * 0.5f, manualBounds);
				bounds = mBounds;
				return;
			}
			bool flag = false;
			int num = 0;
			mTerrains = UnityEngine.Object.FindObjectsOfType(typeof(Terrain)) as Terrain[];
			bool flag2 = mTerrains != null;
			if (flag2)
			{
				flag2 = mTerrains.Length > 1;
			}
			if (flag2)
			{
				int i = 0;
				for (num = mTerrains.Length; i < num; i++)
				{
					Terrain terrain = mTerrains[i];
					MeshRenderer component = terrain.GetComponent<MeshRenderer>();
					if (!flag)
					{
						mBounds = default(Bounds);
						flag = true;
					}
					if (component != null)
					{
						mBounds.Encapsulate(component.bounds);
						continue;
					}
					TerrainCollider component2 = terrain.GetComponent<TerrainCollider>();
					if (component2 != null)
					{
						mBounds.Encapsulate(component2.bounds);
						continue;
					}
					UnityEngine.Debug.LogError("Could not get measure bounds of terrain.", this);
					return;
				}
			}
			else if (Terrain.activeTerrain != null)
			{
				Terrain activeTerrain = Terrain.activeTerrain;
				MeshRenderer component3 = activeTerrain.GetComponent<MeshRenderer>();
				if (!flag)
				{
					mBounds = default(Bounds);
					flag = true;
				}
				if (component3 != null)
				{
					mBounds.Encapsulate(component3.bounds);
				}
				else
				{
					TerrainCollider component4 = activeTerrain.GetComponent<TerrainCollider>();
					if (!(component4 != null))
					{
						UnityEngine.Debug.LogError("Could not get measure bounds of terrain.", this);
						return;
					}
					mBounds.Encapsulate(component4.bounds);
				}
			}
			GameObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
			if (array != null)
			{
				int i = 0;
				for (num = array.Length; i < num; i++)
				{
					GameObject gameObject = array[i];
					if (gameObject.layer == base.gameObject.layer || !IsInRenderLayers(gameObject, boundLayers))
					{
						continue;
					}
					if (!flag)
					{
						mBounds = new Bounds(gameObject.transform.position, new Vector3(1f, 1f, 1f));
						flag = true;
					}
					Renderer renderer = gameObject.GetComponent<Renderer>();
					if (renderer != null)
					{
						mBounds.Encapsulate(renderer.bounds);
						continue;
					}
					Collider collider = gameObject.GetComponent<Collider>();
					if (collider != null)
					{
						mBounds.Encapsulate(collider.bounds);
					}
				}
			}
			if (!flag)
			{
				UnityEngine.Debug.Log("Could not find terrain nor any other bounds in scene", this);
				mBounds = new Bounds(base.gameObject.transform.position, new Vector3(1f, 1f, 1f));
			}
			mBounds.Expand(new Vector3(renderOffset, 0f, renderOffset));
			if (mapResolution == Resolution.Double)
			{
			}
			bounds = mBounds;
		}

		public string[] GetZones(string level)
		{
			List<string> list = new List<string>();
			if (levels != null)
			{
				int i = 0;
				for (int count = levels.Count; i < count; i++)
				{
					if (levels[i].level == level)
					{
						int j = 0;
						for (int count2 = levels[i].zones.Count; j < count2; j++)
						{
							list.Add(levels[i].zones[j].type);
						}
					}
				}
			}
			return (list.Count != 0) ? list.ToArray() : new string[1] { "No Zones defined" };
		}

		public string[] GetLevels()
		{
			List<string> list = new List<string>();
			if (levels != null)
			{
				int i = 0;
				for (int count = levels.Count; i < count; i++)
				{
					list.Add(levels[i].level);
				}
			}
			return (list.Count != 0) ? list.ToArray() : new string[1] { "No Levels defined" };
		}

		public Color GetZoneColor(string level, string zone)
		{
			Color white = Color.white;
			int i = 0;
			for (int count = levels.Count; i < count; i++)
			{
				if (!(levels[i].level == level))
				{
					continue;
				}
				int j = 0;
				for (int count2 = levels[i].zones.Count; j < count2; j++)
				{
					if (levels[i].zones[j].type.Equals(zone))
					{
						return levels[i].zones[j].color;
					}
				}
			}
			return white;
		}

		public bool GetInteraction(int type)
		{
			return Get(type) != null && Get(type).enableInteraction;
		}

		public Color GetColor(int type)
		{
			return (Get(type) != null) ? Get(type).color : Color.white;
		}

		public bool GetAnimateOnVisible(int type)
		{
			return Get(type) != null && Get(type).animateOnVisible;
		}

		public bool GetAnimateOnAction(int type)
		{
			return Get(type) != null && Get(type).showOnAction;
		}

		public bool GetLoopAnimation(int type)
		{
			return Get(type) != null && Get(type).loopAnimation;
		}

		public bool GetHaveArrow(int type)
		{
			return Get(type) != null && Get(type).haveArrow;
		}

		public float GetFadeOutAfter(int type)
		{
			return (Get(type) != null) ? Get(type).fadeOutAfterDelay : 0f;
		}

		public bool GetRotate(int type)
		{
			return Get(type) != null && Get(type).rotate;
		}

		public bool GetArrowRotate(int type)
		{
			return Get(type) != null && Get(type).arrowRotate;
		}

		public bool GetUpdatePosition(int type)
		{
			return Get(type) != null && Get(type).updatePosition;
		}

		public int GetSize(int type)
		{
			return (Get(type) != null) ? Get(type).size : 0;
		}

		public int GetBorderSize(int type)
		{
			return (Get(type) != null) ? Get(type).borderSize : 0;
		}

		public bool GetCustom(int type)
		{
			return Get(type) != null && Get(type).useCustomSize;
		}

		public bool GetCustomBorder(int type)
		{
			return Get(type) != null && Get(type).useCustomBorderSize;
		}

		public int GetDepth(int type)
		{
			return (Get(type) != null) ? Get(type).depth : 0;
		}

		public int GetArrowDepth(int type)
		{
			return (Get(type) != null) ? Get(type).arrowDepth : 0;
		}

		public int GetArrowOffset(int type)
		{
			return (Get(type) != null) ? Get(type).arrowOffset : 0;
		}

		public MapItemType Get(int type)
		{
			if (type == -1)
			{
				return null;
			}
			if (type > mapItems.Count)
			{
				return null;
			}
			MapItemType mapItemType = mapItems[type];
			return (mapItemType != null) ? mapItemType : null;
		}
	}
}
