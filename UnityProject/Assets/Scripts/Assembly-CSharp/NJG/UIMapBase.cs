using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

namespace NJG
{
	public abstract class UIMapBase : MonoBehaviour
	{
		public enum Pivot
		{
			BottomLeft = 0,
			Left = 1,
			TopLeft = 2,
			Top = 3,
			TopRight = 4,
			Right = 5,
			BottomRight = 6,
			Bottom = 7,
			Center = 8
		}

		public NJGMapBase.ShaderType shaderType;

		public Texture maskTexture;

		public float mapBorderRadius;

		public Vector2 margin = Vector2.zero;

		public Pivot pivot = Pivot.Center;

		[SerializeField]
		public float zoom = 1f;

		[SerializeField]
		public float zoomAmount = 0.5f;

		public Transform target;

		public string targetTag = "Player";

		[SerializeField]
		public float minZoom = 1f;

		[SerializeField]
		public float maxZoom = 30f;

		[SerializeField]
		public float zoomSpeed = 1f;

		[SerializeField]
		public KeyCode zoomInKey = KeyCode.KeypadPlus;

		[SerializeField]
		public KeyCode zoomOutKey = KeyCode.KeypadMinus;

		[SerializeField]
		public bool limitBounds = true;

		[SerializeField]
		public bool rotateWithPlayer;

		[SerializeField]
		public bool mouseWheelEnabled = true;

		[SerializeField]
		public bool panning = true;

		[SerializeField]
		public EaseType panningEasing = EaseType.EaseOutCirc;

		[SerializeField]
		public float panningSpeed = 1f;

		[SerializeField]
		public float panningSensitivity = 5f;

		[SerializeField]
		public bool panningMoveBack = true;

		[SerializeField]
		public KeyCode[] keysInUse = new KeyCode[3];

		[SerializeField]
		public Vector2 panningPosition = Vector2.zero;

		public float mapAngle;

		public Vector2 scrollPosition = Vector2.zero;

		public bool drawDirectionalLines;

		public Shader linesShader;

		public int linePoints = 20;

		public Color lineColor = Color.red;

		public float lineWidth = 0.1f;

		public List<Transform> controlPoints = new List<Transform>();

		[SerializeField]
		public bool calculateBorder = true;

		[SerializeField]
		public bool tapOnMap;

		[SerializeField]
		[HideInInspector]
		protected Transform mMapTrans;

		[HideInInspector]
		[SerializeField]
		protected Vector3 mZoom = Vector3.zero;

		[HideInInspector]
		[SerializeField]
		protected List<UIMapIconBase> mList = new List<UIMapIconBase>();

		[SerializeField]
		[HideInInspector]
		protected Transform mIconRoot;

		[HideInInspector]
		[SerializeField]
		protected Transform mTrans;

		[HideInInspector]
		[SerializeField]
		protected Vector2 mMapScale;

		[SerializeField]
		[HideInInspector]
		protected Vector2 mMapHalfScale;

		[HideInInspector]
		[SerializeField]
		protected Vector2 mIconScale;

		[SerializeField]
		[HideInInspector]
		protected Matrix4x4 mMatrix;

		[SerializeField]
		[HideInInspector]
		protected Matrix4x4 rMatrix;

		[SerializeField]
		[HideInInspector]
		protected bool mMapScaleChanged = true;

		[SerializeField]
		[HideInInspector]
		protected int mDepth;

		protected Vector3 mLastScale = Vector3.zero;

		protected Vector3 mMapPos = Vector3.zero;

		protected int mLastHeight;

		private float mNextUpdate;

		protected Texture mMask;

		protected List<UIMapIconBase> mUnused = new List<UIMapIconBase>();

		protected int mCount;

		protected NJGMapBase map;

		protected bool isZooming;

		[SerializeField]
		protected Renderer mRenderer;

		[SerializeField]
		protected Color mColor = Color.white;

		protected Quaternion mapRotation;

		protected Vector3 rotationPivot = new Vector3(0.5f, 0.5f);

		[SerializeField]
		private EaseType mZoomEasing = EaseType.EaseOutExpo;

		private Vector2 mPanningMousePosLast = Vector2.zero;

		private bool mTargetWarning;

		protected Vector3 mIconEulers = Vector3.zero;

		private int mArrowSize;

		[SerializeField]
		private Vector3 mArrScale = Vector3.one;

		private Camera mUICam;

		private bool mIsPanning;

		private TweenParms mResetPan;

		private Transform mLinesRoot;

		private LineRenderer mLineRenderer;

		[SerializeField]
		protected Vector2 mMapScaleSize;

		private Vector3 mExt;

		public float mMod = 1f;

		private GameObject mFrustum;

		private Mesh mFrustumMesh;

		private Material mFrustumMat;

		private int mVertextCount;

		private int mLastVertextCount;

		private Color mLastColor;

		private float mLastWidth;

		protected Vector3 mClickPos;

		private Vector2 mWTM;

		private Vector3 mScrollPos;

		protected Transform mChild;

		private Animation mAnim;

		private bool mAnimCheck;

		private bool mVisible;

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

		public virtual Color mapColor
		{
			get
			{
				return mColor;
			}
			set
			{
				mColor = value;
				material.color = value;
			}
		}

		[SerializeField]
		public EaseType zoomEasing
		{
			get
			{
				return mZoomEasing;
			}
			set
			{
				mZoomEasing = value;
			}
		}

		public Transform iconRoot
		{
			get
			{
				if (mIconRoot == null && Application.isPlaying)
				{
					mIconRoot = NJGTools.AddChild(base.gameObject).transform;
					if (rendererTransform != null)
					{
						mIconRoot.parent = rendererTransform.parent;
						mIconRoot.localPosition = new Vector3(rendererTransform.localPosition.x, rendererTransform.localPosition.y, 1f);
						mIconRoot.localEulerAngles = rendererTransform.localEulerAngles;
					}
					mIconRoot.name = "_MapIcons";
				}
				return mIconRoot;
			}
		}

		public virtual bool isMouseOver { get; set; }

		public virtual Transform rendererTransform
		{
			get
			{
				if (planeRenderer != null && mMapTrans == null)
				{
					mMapTrans = planeRenderer.transform;
				}
				return (!(planeRenderer == null)) ? mMapTrans : base.transform;
			}
		}

		public virtual Renderer planeRenderer
		{
			get
			{
				if (mRenderer == null)
				{
					mRenderer = base.gameObject.renderer;
				}
				if (mRenderer == null)
				{
					mRenderer = base.gameObject.GetComponentInChildren<Renderer>();
				}
				return mRenderer;
			}
			set
			{
				mRenderer = value;
			}
		}

		public virtual Vector2 mapScale
		{
			get
			{
				return rendererTransform.localScale;
			}
			set
			{
				rendererTransform.hasChanged = true;
				rendererTransform.localScale = value;
			}
		}

		public virtual Vector2 mapHalfScale
		{
			get
			{
				if (rendererTransform.hasChanged)
				{
					rendererTransform.hasChanged = false;
					mMapHalfScale = mapScale * 0.5f;
				}
				return mMapHalfScale;
			}
		}

		[SerializeField]
		public virtual Material material
		{
			get
			{
				return (!Application.isPlaying) ? planeRenderer.sharedMaterial : planeRenderer.material;
			}
			set
			{
				planeRenderer.material = value;
			}
		}

		public int depth
		{
			get
			{
				material.renderQueue = 3000 + mDepth;
				return mDepth;
			}
			set
			{
				if (mDepth != value)
				{
					mDepth = value;
					material.renderQueue = 3000 + mDepth;
				}
			}
		}

		public virtual Vector3 arrowScale
		{
			get
			{
				if (mArrowSize != map.arrowSize)
				{
					mArrowSize = map.arrowSize;
					mArrScale.x = (mArrScale.y = map.arrowSize);
				}
				return mArrScale;
			}
		}

		public bool isVisible
		{
			get
			{
				return mChild.gameObject.activeInHierarchy;
			}
		}

		public bool isMouseOut
		{
			get
			{
				Vector3 mousePosition = Input.mousePosition;
				return mousePosition.x > (float)Screen.width || mousePosition.y > (float)Screen.height || mousePosition.x < 0f || mousePosition.y < 0f;
			}
		}

		public bool isPanning
		{
			get
			{
				if (!panning)
				{
					return false;
				}
				return mIsPanning;
			}
		}

		protected virtual void CleanIcons()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			int num = mList.Count;
			while (num > 0)
			{
				UIMapIconBase uIMapIconBase = mList[--num];
				if (!uIMapIconBase.isValid)
				{
					Delete(uIMapIconBase);
				}
			}
		}

		protected virtual UIMapIconBase GetEntry(NJGMapItem marker)
		{
			return null;
		}

		protected virtual void Delete(UIMapIconBase ent)
		{
			mList.Remove(ent);
			mUnused.Add(ent);
			NJGTools.SetActive(ent.gameObject, false);
		}

		protected virtual void OnEnable()
		{
			if (Application.isPlaying)
			{
				material.renderQueue = 3000 + depth;
				if (!map.queue.Contains(UpdateCode))
				{
					map.queue.Add(UpdateCode);
				}
			}
		}

		protected virtual void OnDisable()
		{
			if (Application.isPlaying && map.queue.Contains(UpdateCode))
			{
				map.queue.Remove(UpdateCode);
			}
		}

		protected virtual void OnDestroy()
		{
			if (Application.isPlaying && map.queue.Contains(UpdateCode))
			{
				map.queue.Remove(UpdateCode);
			}
		}

		protected virtual void UpdateIcon(NJGMapItem item, float x, float y)
		{
		}

		protected virtual void Awake()
		{
			mMapScaleChanged = true;
			rendererTransform.hasChanged = true;
			map = NJGMapBase.instance;
			mTrans = base.transform;
			mUICam = NJGTools.FindCameraForLayer(NJGMapBase.instance.layer);
			if (material != null)
			{
				if (shaderType == NJGMapBase.ShaderType.FOW)
				{
					material.shader = Shader.Find("NinjutsuGames/Map FOW");
				}
				else if (shaderType == NJGMapBase.ShaderType.TextureMask)
				{
					material.shader = Shader.Find("NinjutsuGames/Map TextureMask");
				}
				else if (shaderType == NJGMapBase.ShaderType.ColorMask)
				{
					material.shader = Shader.Find("NinjutsuGames/Map ColorMask");
				}
			}
			if (maskTexture == null && material != null)
			{
				maskTexture = material.GetTexture("_Mask");
			}
			if (drawDirectionalLines && Application.isPlaying)
			{
				if (linesShader == null)
				{
					linesShader = Shader.Find("Particles/Additive");
				}
				GameObject gameObject = NJGTools.AddChild(base.gameObject);
				mLinesRoot = gameObject.transform;
				mLinesRoot.parent = iconRoot;
				mLinesRoot.localPosition = Vector3.zero;
				mLinesRoot.localEulerAngles = Vector3.zero;
				mLineRenderer = gameObject.GetComponent<LineRenderer>();
				if (mLineRenderer == null)
				{
					gameObject.AddComponent<LineRenderer>();
				}
				mLineRenderer = gameObject.GetComponent<LineRenderer>();
				mLineRenderer.useWorldSpace = true;
				mLineRenderer.material = new Material(linesShader);
				mLinesRoot.name = "_Lines";
			}
		}

		protected virtual void Start()
		{
			map = NJGMapBase.instance;
			if (material == null)
			{
				if (Application.isPlaying)
				{
					Debug.LogWarning("The UITexture does not have a material assigned", this);
				}
			}
			else
			{
				if (map.generateMapTexture)
				{
					material.mainTexture = map.mapTexture;
				}
				else
				{
					material.mainTexture = NJGMapBase.instance.userMapTexture;
				}
				if (maskTexture != null)
				{
					material.SetTexture("_Mask", maskTexture);
				}
				material.color = mapColor;
			}
			if (Application.isPlaying && mChild == null)
			{
				if (cachedTransform.childCount > 0)
				{
					mChild = cachedTransform;
				}
				else
				{
					mChild = cachedTransform.GetChild(0);
				}
			}
			OnStart();
			Update();
		}

		protected virtual void UpdateCode()
		{
			Debug.Log("UpdateCode ");
		}

		private void UpdateScrollPosition()
		{
			Bounds bounds = map.bounds;
			Vector3 extents = bounds.extents;
			float num = 1f / zoom;
			if (!(target == null))
			{
				scrollPosition = Vector3.zero;
				Vector3 vector = target.position - bounds.center;
				mExt.x = 0.5f / extents.x;
				mExt.y = 0.5f / extents.y;
				mExt.z = 0.5f / extents.z;
				if (map.mapResolution == NJGMapBase.Resolution.Double)
				{
					mExt.x *= mMod;
					mExt.y *= mMod;
					mExt.z *= mMod;
				}
				scrollPosition.x = vector.x * mExt.x;
				if (map.orientation == NJGMapBase.Orientation.XZDefault)
				{
					scrollPosition.y = vector.z * mExt.z;
				}
				else
				{
					scrollPosition.y = vector.y * mExt.y;
				}
				if (panning)
				{
					scrollPosition += panningPosition;
				}
				if (limitBounds)
				{
					scrollPosition.x = Mathf.Max(0f - (1f - num) * 0.5f, scrollPosition.x);
					scrollPosition.x = Mathf.Min((1f - num) * 0.5f, scrollPosition.x);
					scrollPosition.y = Mathf.Max(0f - (1f - num) * 0.5f, scrollPosition.y);
					scrollPosition.y = Mathf.Min((1f - num) * 0.5f, scrollPosition.y);
				}
				mMapPos.x = (1f - num) * 0.5f + scrollPosition.x;
				mMapPos.y = (1f - num) * 0.5f + scrollPosition.y;
				mMapPos.z = 0f;
				mZoom.x = (mZoom.y = (mZoom.z = num));
				UpdateMatrix();
			}
		}

		protected virtual void UpdateMatrix()
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(mMapPos, Quaternion.identity, mZoom);
			if (rotateWithPlayer)
			{
				Vector3 forward = target.forward;
				forward.Normalize();
				mapAngle = ((!(Vector3.Dot(forward, Vector3.Cross(Vector3.up, Vector3.forward)) <= 0f)) ? (-1f) : 1f) * Vector3.Angle(forward, Vector3.forward);
				mapRotation = Quaternion.Euler(0f, 0f, mapAngle);
				Matrix4x4 matrix4x2 = Matrix4x4.TRS(-rotationPivot, Quaternion.identity, Vector3.one);
				Matrix4x4 matrix4x3 = Matrix4x4.TRS(Vector3.zero, mapRotation, Vector3.one);
				Matrix4x4 matrix4x4 = Matrix4x4.TRS(rotationPivot, Quaternion.identity, Vector3.one);
				rMatrix = matrix4x * matrix4x4 * matrix4x3 * matrix4x2;
				if (!mMatrix.Equals(rMatrix))
				{
					mMatrix = rMatrix;
					material.SetMatrix("_Matrix", rMatrix);
				}
				if (iconRoot != null)
				{
					mIconEulers.z = 0f - mapAngle;
					if (iconRoot.localEulerAngles != mIconEulers)
					{
						iconRoot.localEulerAngles = mIconEulers;
					}
				}
			}
			else if (!mMatrix.Equals(matrix4x))
			{
				mMatrix = matrix4x;
				material.SetMatrix("_Matrix", matrix4x);
				if (iconRoot != null && iconRoot.localEulerAngles != Vector3.zero)
				{
					iconRoot.localEulerAngles = Vector3.zero;
				}
			}
		}

		protected virtual void Update()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (target == null && !string.IsNullOrEmpty(targetTag) && GameObject.FindGameObjectWithTag(targetTag) != null)
			{
				target = GameObject.FindGameObjectWithTag(targetTag).transform;
			}
			if (isMouseOut && isMouseOver)
			{
				isMouseOver = false;
			}
			if (target != null && controlPoints.Count == 0 && !controlPoints.Contains(target))
			{
				controlPoints.Add(target);
			}
			if (target == null && !mTargetWarning)
			{
				mTargetWarning = true;
			}
			UpdateZoomKeys();
			if (mouseWheelEnabled && isMouseOver)
			{
				float axis = Input.GetAxis("Mouse ScrollWheel");
				if (axis != 0f)
				{
					if ((double)axis > 0.1)
					{
						ZoomIn(zoomAmount);
					}
					else if ((double)axis < 0.1)
					{
						ZoomOut(zoomAmount);
					}
				}
			}
			if (panning)
			{
				UpdatePanning();
			}
			int height = Screen.height;
			bool flag = mLastHeight != height;
			if (mLastScale != rendererTransform.localScale)
			{
				rendererTransform.hasChanged = true;
				mLastScale = rendererTransform.localScale;
				if (calculateBorder)
				{
					mapBorderRadius = rendererTransform.localScale.x / 2f / 4f;
				}
			}
			if (mNextUpdate < Time.time)
			{
				mLastHeight = height;
				mNextUpdate = Time.time + map.updateFrequency;
				UpdateIcons();
				CleanIcons();
				if (drawDirectionalLines)
				{
					DrawLines();
				}
				UpdateScrollPosition();
				if ((OnUpdate() || flag) && NJGMapBase.instance.renderMode == NJGMapBase.RenderMode.ScreenChange && this is UIMiniMapBase)
				{
					NJGMapBase.instance.GenerateMap();
				}
			}
		}

		protected virtual void UpdateZoomKeys()
		{
			if (Input.GetKeyDown(zoomInKey))
			{
				ZoomIn(zoomAmount);
			}
			if (Input.GetKeyDown(zoomOutKey))
			{
				ZoomOut(zoomAmount);
			}
		}

		protected virtual void OnStart()
		{
		}

		protected virtual bool OnUpdate()
		{
			return false;
		}

		protected void UpdateIcons()
		{
			int num = mList.Count;
			while (num > 0)
			{
				UIMapIconBase uIMapIconBase = mList[--num];
				uIMapIconBase.isValid = false;
				if (!drawDirectionalLines)
				{
					continue;
				}
				if (uIMapIconBase.item.cachedTransform != target)
				{
					if (uIMapIconBase.item.drawDirection)
					{
						if (!controlPoints.Contains(uIMapIconBase.cachedTransform))
						{
							controlPoints.Add(uIMapIconBase.cachedTransform);
						}
					}
					else if (controlPoints.Contains(uIMapIconBase.cachedTransform))
					{
						controlPoints.Remove(uIMapIconBase.cachedTransform);
					}
				}
				else if (controlPoints[0] != uIMapIconBase.cachedTransform)
				{
					controlPoints[0] = uIMapIconBase.cachedTransform;
				}
			}
			for (int i = 0; i < NJGMapItem.list.Count; i++)
			{
				NJGMapItem nJGMapItem = NJGMapItem.list[i];
				if (nJGMapItem.type < 1)
				{
					continue;
				}
				Vector2 pos = WorldToMap(nJGMapItem.cachedTransform.position);
				if (map.fow.enabled)
				{
					if (!nJGMapItem.isRevealed)
					{
						nJGMapItem.isRevealed = (nJGMapItem.revealFOW || NJGFOW.instance.IsVisible(pos)) && (nJGMapItem.revealFOW || NJGFOW.instance.IsExplored(pos));
						if (nJGMapItem.cachedTransform == target)
						{
							nJGMapItem.isRevealed = true;
						}
					}
				}
				else if (!nJGMapItem.isRevealed)
				{
					nJGMapItem.isRevealed = true;
				}
				if (nJGMapItem.isRevealed)
				{
					UpdateIcon(nJGMapItem, pos.x, pos.y);
				}
			}
		}

		protected virtual void UpdateFrustum()
		{
			if (!(map.cameraFrustum == null) && map.orientation != NJGMapBase.Orientation.XYSideScroller)
			{
				if (mFrustumMesh == null)
				{
					mFrustum = new GameObject("_Frustum");
					mFrustumMat = new Material(Shader.Find("NinjutsuGames/Map TextureMask"));
					mFrustum.AddComponent<MeshRenderer>().material = mFrustumMat;
					mFrustum.name = "_Frustum";
					mFrustum.transform.parent = iconRoot;
					mFrustum.transform.localEulerAngles = new Vector3(270f, 0f, 0f);
					mFrustum.transform.localPosition = Vector3.zero;
					mFrustum.transform.localScale = Vector3.one;
					mFrustum.layer = base.gameObject.layer;
					Mesh mesh = NJGTools.CreatePlane();
					mFrustum.AddComponent<MeshFilter>().mesh = mesh;
					mFrustumMesh = mesh;
				}
				Vector3[] vertices = mFrustumMesh.vertices;
				vertices[1] = map.cameraFrustum.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2, map.cameraFrustum.farClipPlane));
				vertices[2] = map.cameraFrustum.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2, map.cameraFrustum.nearClipPlane));
				vertices[3] = map.cameraFrustum.ScreenToWorldPoint(new Vector3(0f, Screen.height / 2, map.cameraFrustum.nearClipPlane));
				vertices[0] = map.cameraFrustum.ScreenToWorldPoint(new Vector3(0f, Screen.height / 2, map.cameraFrustum.farClipPlane));
				float y = ((map.orientation != 0) ? (map.bounds.max.z + 1f + 0.1f) : (map.bounds.min.y - 1f + 0.1f));
				for (int i = 0; i < 4; i++)
				{
					vertices[i].y = y;
				}
				mFrustumMesh.vertices = vertices;
				mFrustumMesh.RecalculateBounds();
				mFrustumMat.SetColor("_Color", map.cameraFrustumColor);
			}
		}

		private void UpdatePanning()
		{
			if (!panning)
			{
				panningPosition = Vector2.zero;
				return;
			}
			if (tapOnMap && Input.touchCount > 0)
			{
				if (Input.GetTouch(0).phase == TouchPhase.Began)
				{
					mPanningMousePosLast = mUICam.ScreenToViewportPoint(Input.GetTouch(0).position);
					if (HOTween.IsTweening(this))
					{
						HOTween.Kill(this);
					}
				}
				if (!mIsPanning && Input.touchCount > 0 && Vector2.Distance(mUICam.ScreenToViewportPoint(Input.GetTouch(0).position), mPanningMousePosLast) > 0.01f)
				{
					mIsPanning = true;
				}
			}
			if (!isPanning)
			{
				return;
			}
			if (Input.touchCount > 0)
			{
				Vector2 position = (Vector2)mUICam.ScreenToViewportPoint(Input.GetTouch(0).position) - mPanningMousePosLast;
				Vector2 vector = GetDirection(position) * panningSensitivity;
				panningPosition -= vector / zoom;
				mPanningMousePosLast = mUICam.ScreenToViewportPoint(Input.GetTouch(0).position);
			}
			if (Input.touchCount == 0 || (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)))
			{
				if (panningMoveBack)
				{
					ResetPanning();
				}
				else
				{
					mIsPanning = false;
				}
			}
		}

		public void ResetPanning()
		{
			if (panningPosition == Vector2.zero)
			{
				mIsPanning = false;
				return;
			}
			if (mResetPan == null)
			{
				mResetPan = new TweenParms().Prop("panningPosition", Vector2.zero).OnComplete(OnPanningComplete);
			}
			HOTween.To(this, panningSpeed, mResetPan).easeType = panningEasing;
		}

		private void OnPanningComplete()
		{
			panningPosition = Vector2.zero;
			mIsPanning = false;
		}

		private void DrawLines()
		{
			if (null == mLineRenderer || controlPoints == null || controlPoints.Count < 2)
			{
				return;
			}
			if (mLastColor != lineColor)
			{
				mLastColor = lineColor;
				mLineRenderer.SetColors(lineColor, lineColor);
			}
			if (mLastWidth != lineWidth)
			{
				mLastWidth = lineWidth;
				mLineRenderer.SetWidth(lineWidth * 0.1f, lineWidth * 0.1f);
			}
			if (linePoints < 2)
			{
				linePoints = 2;
			}
			mVertextCount = linePoints * (controlPoints.Count - 1);
			if (mLastVertextCount != mVertextCount)
			{
				mLastVertextCount = mVertextCount;
				mLineRenderer.SetVertexCount(mVertextCount);
			}
			int i = 0;
			for (int num = controlPoints.Count - 1; i < num && !(controlPoints[i] == null) && !(controlPoints[i + 1] == null) && (i <= 0 || !(controlPoints[i - 1] == null)) && (i >= controlPoints.Count - 2 || !(controlPoints[i + 2] == null)); i++)
			{
				Vector3 position = controlPoints[i].position;
				Vector3 position2 = controlPoints[i + 1].position;
				Vector3 vector = ((i <= 0) ? (controlPoints[i + 1].position - controlPoints[i].position) : (0.5f * (controlPoints[i + 1].position - controlPoints[i - 1].position)));
				Vector3 vector2 = ((i >= controlPoints.Count - 2) ? (controlPoints[i + 1].position - controlPoints[i].position) : (0.5f * (controlPoints[i + 2].position - controlPoints[i].position)));
				float num2 = 1f / (float)linePoints;
				if (i == controlPoints.Count - 2)
				{
					num2 = 1f / ((float)linePoints - 1f);
				}
				for (int j = 0; j < linePoints; j++)
				{
					float num3 = (float)j * num2;
					Vector3 position3 = (2f * num3 * num3 * num3 - 3f * num3 * num3 + 1f) * position + (num3 * num3 * num3 - 2f * num3 * num3 + num3) * vector + (-2f * num3 * num3 * num3 + 3f * num3 * num3) * position2 + (num3 * num3 * num3 - num3 * num3) * vector2;
					mLineRenderer.SetPosition(j + i * linePoints, position3);
				}
			}
		}

		public void ZoomIn(float amount)
		{
			if (zoom != maxZoom)
			{
				if (HOTween.IsTweening(this))
				{
					HOTween.Complete(this);
				}
				HOTween.To(this, zoomSpeed, "zoom", Mathf.Clamp(zoom + amount, (int)minZoom, (int)maxZoom)).easeType = zoomEasing;
			}
		}

		public void ZoomOut(float amount)
		{
			if (zoom != minZoom)
			{
				if (HOTween.IsTweening(this))
				{
					HOTween.Complete(this);
				}
				HOTween.To(this, zoomSpeed, "zoom", Mathf.Clamp(zoom - amount, (int)minZoom, (int)maxZoom)).easeType = zoomEasing;
			}
		}

		public Vector3 MapToWorld(Vector2 pos)
		{
			Bounds bounds = map.bounds;
			Vector3 extents = bounds.extents;
			Vector3 vector = (Vector3)pos + bounds.center;
			float num = mapHalfScale.x / extents.x;
			float num2 = mapHalfScale.y / ((map.orientation != 0) ? extents.y : extents.z);
			Vector3 vector2 = WorldScrollPosition();
			num *= zoom;
			num2 *= zoom;
			mClickPos.x = (vector.x + vector2.x) * num;
			mClickPos.z = ((map.orientation != 0) ? ((vector.y + vector2.y) * num2) : ((vector.z + vector2.z) * num2));
			mClickPos.y = target.position.y;
			return mClickPos;
		}

		public Vector2 WorldToMap(Vector3 worldPos)
		{
			return WorldToMap(worldPos, true);
		}

		public Vector2 WorldToMap(Vector3 worldPos, bool calculateZoom)
		{
			if (map == null)
			{
				map = NJGMapBase.instance;
			}
			Bounds bounds = map.bounds;
			Vector3 extents = bounds.extents;
			Vector3 vector = worldPos - bounds.center;
			float num = mapHalfScale.x / extents.x;
			float num2 = mapHalfScale.y / ((map.orientation != 0) ? extents.y : extents.z);
			Vector3 vector2 = WorldScrollPosition();
			if (calculateZoom)
			{
				num *= zoom;
				num2 *= zoom;
			}
			else
			{
				num *= 1f;
				num2 *= 1f;
				vector2 = Vector3.zero;
			}
			mWTM.x = (vector.x - vector2.x) * num;
			mWTM.y = ((map.orientation != 0) ? ((vector.y - vector2.y) * num2) : ((vector.z - vector2.z) * num2));
			if (map.mapResolution == NJGMapBase.Resolution.Double)
			{
				num *= mMod;
				num2 *= mMod;
			}
			return mWTM;
		}

		public Vector3 WorldScrollPosition()
		{
			Vector3 size = map.bounds.size;
			mScrollPos.x = scrollPosition.x * size.x;
			mScrollPos.y = scrollPosition.y * size.y;
			mScrollPos.z = scrollPosition.y * size.z;
			return mScrollPos;
		}

		public void Toggle()
		{
			if (mVisible)
			{
				Hide();
			}
			else
			{
				Show();
			}
		}

		public virtual void Show()
		{
			if (mVisible)
			{
				return;
			}
			if (mChild == null)
			{
				mChild = cachedTransform.GetChild(0);
			}
			if (mChild == null)
			{
				mChild = base.transform;
			}
			if (mAnim == null && !mAnimCheck)
			{
				mAnim = base.gameObject.GetComponentInChildren<Animation>();
				mAnimCheck = true;
			}
			if (mAnim != null)
			{
				NJGTools.SetActive(mChild.gameObject, true);
				mAnim[mAnim.clip.name].speed = 1f;
				mAnim[mAnim.clip.name].time = 0f;
				if (mAnim.clip != null)
				{
					mAnim.Play();
				}
			}
			else
			{
				NJGTools.SetActive(mChild.gameObject, true);
			}
			mVisible = true;
			base.enabled = true;
		}

		public virtual void Hide()
		{
			if (!mVisible)
			{
				return;
			}
			if (mChild == null)
			{
				mChild = cachedTransform.GetChild(0);
			}
			if (mChild == null)
			{
				mChild = base.transform;
			}
			if (mAnim == null && !mAnimCheck)
			{
				mAnim = base.gameObject.GetComponentInChildren<Animation>();
				mAnimCheck = true;
			}
			if (mAnim != null)
			{
				if (mAnim.clip != null)
				{
					mAnim[mAnim.clip.name].speed = -1f;
					mAnim[mAnim.clip.name].time = mAnim[mAnim.clip.name].length;
					mAnim.Play();
					StartCoroutine(DisableOnFinish());
				}
			}
			else
			{
				NJGTools.SetActive(mChild.gameObject, false);
			}
			mVisible = false;
			base.enabled = false;
		}

		private IEnumerator DisableOnFinish()
		{
			yield return new WaitForSeconds(mAnim[mAnim.clip.name].length);
			NJGTools.SetActive(mChild.gameObject, false);
		}

		public Vector2 GetDirection(Vector2 position)
		{
			return mapRotation * position;
		}

		public Vector3 GetDirection(Vector3 position)
		{
			return mapRotation * position;
		}
	}
}
