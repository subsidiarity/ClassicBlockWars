using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000CD RID: 205
[AddComponentMenu("NGUI/Internal/Draw Call")]
[ExecuteInEditMode]
public class UIDrawCall : MonoBehaviour
{
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x060004E2 RID: 1250 RVA: 0x0002696C File Offset: 0x00024B6C
	[Obsolete("Use UIDrawCall.activeList")]
	public static BetterList<UIDrawCall> list
	{
		get
		{
			return UIDrawCall.mActiveList;
		}
	}
	
	// Token: 0x1700006D RID: 109
	// (get) Token: 0x060004E3 RID: 1251 RVA: 0x00026974 File Offset: 0x00024B74
	public static BetterList<UIDrawCall> activeList
	{
		get
		{
			return UIDrawCall.mActiveList;
		}
	}
	
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x060004E4 RID: 1252 RVA: 0x0002697C File Offset: 0x00024B7C
	public static BetterList<UIDrawCall> inactiveList
	{
		get
		{
			return UIDrawCall.mInactiveList;
		}
	}
	
	// Token: 0x1700006F RID: 111
	// (get) Token: 0x060004E5 RID: 1253 RVA: 0x00026984 File Offset: 0x00024B84
	// (set) Token: 0x060004E6 RID: 1254 RVA: 0x0002698C File Offset: 0x00024B8C
	public int renderQueue
	{
		get
		{
			return this.mRenderQueue;
		}
		set
		{
			if (this.mRenderQueue != value)
			{
				this.mRenderQueue = value;
				if (this.mDynamicMat != null)
				{
					this.mDynamicMat.renderQueue = value;
				}
			}
		}
	}
	
	// Token: 0x17000070 RID: 112
	// (get) Token: 0x060004E7 RID: 1255 RVA: 0x000269CC File Offset: 0x00024BCC
	// (set) Token: 0x060004E8 RID: 1256 RVA: 0x000269FC File Offset: 0x00024BFC
	public int sortingOrder
	{
		get
		{
			return (!(this.mRenderer != null)) ? 0 : this.mRenderer.sortingOrder;
		}
		set
		{
			if (this.mRenderer != null && this.mRenderer.sortingOrder != value)
			{
				this.mRenderer.sortingOrder = value;
			}
		}
	}
	
	// Token: 0x17000071 RID: 113
	// (get) Token: 0x060004E9 RID: 1257 RVA: 0x00026A38 File Offset: 0x00024C38
	public int finalRenderQueue
	{
		get
		{
			return (!(this.mDynamicMat != null)) ? this.mRenderQueue : this.mDynamicMat.renderQueue;
		}
	}
	
	// Token: 0x17000072 RID: 114
	// (get) Token: 0x060004EA RID: 1258 RVA: 0x00026A64 File Offset: 0x00024C64
	public Transform cachedTransform
	{
		get
		{
			if (this.mTrans == null)
			{
				this.mTrans = base.transform;
			}
			return this.mTrans;
		}
	}
	
	// Token: 0x17000073 RID: 115
	// (get) Token: 0x060004EB RID: 1259 RVA: 0x00026A8C File Offset: 0x00024C8C
	// (set) Token: 0x060004EC RID: 1260 RVA: 0x00026A94 File Offset: 0x00024C94
	public Material baseMaterial
	{
		get
		{
			return this.mMaterial;
		}
		set
		{
			if (this.mMaterial != value)
			{
				this.mMaterial = value;
				this.mRebuildMat = true;
			}
		}
	}
	
	// Token: 0x17000074 RID: 116
	// (get) Token: 0x060004ED RID: 1261 RVA: 0x00026AB8 File Offset: 0x00024CB8
	public Material dynamicMaterial
	{
		get
		{
			return this.mDynamicMat;
		}
	}
	
	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060004EE RID: 1262 RVA: 0x00026AC0 File Offset: 0x00024CC0
	// (set) Token: 0x060004EF RID: 1263 RVA: 0x00026AC8 File Offset: 0x00024CC8
	public Texture mainTexture
	{
		get
		{
			return this.mTexture;
		}
		set
		{
			this.mTexture = value;
			if (this.mDynamicMat != null)
			{
				this.mDynamicMat.mainTexture = value;
			}
		}
	}
	
	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060004F0 RID: 1264 RVA: 0x00026AFC File Offset: 0x00024CFC
	// (set) Token: 0x060004F1 RID: 1265 RVA: 0x00026B04 File Offset: 0x00024D04
	public Shader shader
	{
		get
		{
			return this.mShader;
		}
		set
		{
			if (this.mShader != value)
			{
				this.mShader = value;
				this.mRebuildMat = true;
			}
		}
	}
	
	// Token: 0x17000077 RID: 119
	// (get) Token: 0x060004F2 RID: 1266 RVA: 0x00026B28 File Offset: 0x00024D28
	public int triangles
	{
		get
		{
			return (!(this.mMesh != null)) ? 0 : this.mTriangles;
		}
	}
	
	// Token: 0x17000078 RID: 120
	// (get) Token: 0x060004F3 RID: 1267 RVA: 0x00026B48 File Offset: 0x00024D48
	public bool isClipped
	{
		get
		{
			return this.mClipCount != 0;
		}
	}
	
	// Token: 0x060004F4 RID: 1268 RVA: 0x00026B58 File Offset: 0x00024D58
	private void CreateMaterial()
	{
		string text = ((!(this.mShader != null)) ? ((!(this.mMaterial != null)) ? "Unlit/Transparent Colored" : this.mMaterial.shader.name) : this.mShader.name);
		text = text.Replace("GUI/Text Shader", "Unlit/Text");
		if (text.Length > 2 && text[text.Length - 2] == ' ')
		{
			int num = (int)text[text.Length - 1];
			if (num > 48 && num <= 57)
			{
				text = text.Substring(0, text.Length - 2);
			}
		}
		if (text.StartsWith("HIDDEN/"))
		{
			text = text.Substring(7);
		}
		text = text.Replace(" (SoftClip)", string.Empty);
		this.mLegacyShader = false;
		this.mClipCount = this.panel.clipCount;
		Shader shader;
		if (this.mClipCount != 0)
		{
			shader = Shader.Find(string.Concat(new object[] { "HIDDEN/", text, " ", this.mClipCount }));
			if (shader == null)
			{
				Shader.Find(text + " " + this.mClipCount);
			}
			if (shader == null && this.mClipCount == 1)
			{
				this.mLegacyShader = true;
				shader = Shader.Find(text + " (SoftClip)");
			}
		}
		else
		{
			shader = Shader.Find(text);
		}
		if (this.mMaterial != null)
		{
			this.mDynamicMat = new Material(this.mMaterial);
			this.mDynamicMat.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
			this.mDynamicMat.CopyPropertiesFromMaterial(this.mMaterial);
		}
		else
		{
			this.mDynamicMat = new Material(shader);
			this.mDynamicMat.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
		}
		if (shader != null)
		{
			this.mDynamicMat.shader = shader;
		}
		else
		{
			Debug.LogError(string.Concat(new object[] { text, " shader doesn't have a clipped shader version for ", this.mClipCount, " clip regions" }));
		}
	}
	
	// Token: 0x060004F5 RID: 1269 RVA: 0x00026DA4 File Offset: 0x00024FA4
	private Material RebuildMaterial()
	{
		NGUITools.DestroyImmediate(this.mDynamicMat);
		this.CreateMaterial();
		this.mDynamicMat.renderQueue = this.mRenderQueue;
		if (this.mTexture != null)
		{
			this.mDynamicMat.mainTexture = this.mTexture;
		}
		if (this.mRenderer != null)
		{
			this.mRenderer.sharedMaterials = new Material[] { this.mDynamicMat };
		}
		return this.mDynamicMat;
	}
	
	// Token: 0x060004F6 RID: 1270 RVA: 0x00026E28 File Offset: 0x00025028
	private void UpdateMaterials()
	{
		if (this.mRebuildMat || this.mDynamicMat == null || this.mClipCount != this.panel.clipCount)
		{
			this.RebuildMaterial();
			this.mRebuildMat = false;
		}
		else if (this.mRenderer.sharedMaterial != this.mDynamicMat)
		{
			this.mRenderer.sharedMaterials = new Material[] { this.mDynamicMat };
		}
	}
	
	// Token: 0x060004F7 RID: 1271 RVA: 0x00026EB0 File Offset: 0x000250B0
	public void UpdateGeometry()
	{
		int size = this.verts.size;
		if (size > 0 && size == this.uvs.size && size == this.cols.size && size % 4 == 0)
		{
			if (this.mFilter == null)
			{
				this.mFilter = base.gameObject.GetComponent<MeshFilter>();
			}
			if (this.mFilter == null)
			{
				this.mFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (this.verts.size < 65000)
			{
				int num = (size >> 1) * 3;
				bool flag = this.mIndices == null || this.mIndices.Length != num;
				if (this.mMesh == null)
				{
					this.mMesh = new Mesh();
					this.mMesh.hideFlags = HideFlags.DontSave;
					this.mMesh.name = ((!(this.mMaterial != null)) ? "Mesh" : this.mMaterial.name);
					this.mMesh.MarkDynamic();
					flag = true;
				}
				bool flag2 = this.uvs.buffer.Length != this.verts.buffer.Length || this.cols.buffer.Length != this.verts.buffer.Length || (this.norms.buffer != null && this.norms.buffer.Length != this.verts.buffer.Length) || (this.tans.buffer != null && this.tans.buffer.Length != this.verts.buffer.Length);
				if (!flag2 && this.panel.renderQueue != UIPanel.RenderQueue.Automatic)
				{
					flag2 = this.mMesh == null || this.mMesh.vertexCount != this.verts.buffer.Length;
				}
				if (!flag2 && this.verts.size << 1 < this.verts.buffer.Length)
				{
					flag2 = true;
				}
				this.mTriangles = this.verts.size >> 1;
				if (flag2 || this.verts.buffer.Length > 65000)
				{
					if (flag2 || this.mMesh.vertexCount != this.verts.size)
					{
						this.mMesh.Clear();
						flag = true;
					}
					this.mMesh.vertices = this.verts.ToArray();
					this.mMesh.uv = this.uvs.ToArray();
					this.mMesh.colors32 = this.cols.ToArray();
					if (this.norms != null)
					{
						this.mMesh.normals = this.norms.ToArray();
					}
					if (this.tans != null)
					{
						this.mMesh.tangents = this.tans.ToArray();
					}
				}
				else
				{
					if (this.mMesh.vertexCount != this.verts.buffer.Length)
					{
						this.mMesh.Clear();
						flag = true;
					}
					this.mMesh.vertices = this.verts.buffer;
					this.mMesh.uv = this.uvs.buffer;
					this.mMesh.colors32 = this.cols.buffer;
					if (this.norms != null)
					{
						this.mMesh.normals = this.norms.buffer;
					}
					if (this.tans != null)
					{
						this.mMesh.tangents = this.tans.buffer;
					}
				}
				if (flag)
				{
					this.mIndices = this.GenerateCachedIndexBuffer(size, num);
					this.mMesh.triangles = this.mIndices;
				}
				if (flag2 || !this.alwaysOnScreen)
				{
					this.mMesh.RecalculateBounds();
				}
				this.mFilter.mesh = this.mMesh;
			}
			else
			{
				this.mTriangles = 0;
				if (this.mFilter.mesh != null)
				{
					this.mFilter.mesh.Clear();
				}
				Debug.LogError("Too many vertices on one panel: " + this.verts.size);
			}
			if (this.mRenderer == null)
			{
				this.mRenderer = base.gameObject.GetComponent<MeshRenderer>();
			}
			if (this.mRenderer == null)
			{
				this.mRenderer = base.gameObject.AddComponent<MeshRenderer>();
			}
			this.UpdateMaterials();
		}
		else
		{
			if (this.mFilter.mesh != null)
			{
				this.mFilter.mesh.Clear();
			}
			Debug.LogError("UIWidgets must fill the buffer with 4 vertices per quad. Found " + size);
		}
		this.verts.Clear();
		this.uvs.Clear();
		this.cols.Clear();
		this.norms.Clear();
		this.tans.Clear();
	}
	
	// Token: 0x060004F8 RID: 1272 RVA: 0x000273E8 File Offset: 0x000255E8
	private int[] GenerateCachedIndexBuffer(int vertexCount, int indexCount)
	{
		int i = 0;
		int count = UIDrawCall.mCache.Count;
		while (i < count)
		{
			int[] array = UIDrawCall.mCache[i];
			if (array != null && array.Length == indexCount)
			{
				return array;
			}
			i++;
		}
		int[] array2 = new int[indexCount];
		int num = 0;
		for (int j = 0; j < vertexCount; j += 4)
		{
			array2[num++] = j;
			array2[num++] = j + 1;
			array2[num++] = j + 2;
			array2[num++] = j + 2;
			array2[num++] = j + 3;
			array2[num++] = j;
		}
		if (UIDrawCall.mCache.Count > 10)
		{
			UIDrawCall.mCache.RemoveAt(0);
		}
		UIDrawCall.mCache.Add(array2);
		return array2;
	}
	
	// Token: 0x060004F9 RID: 1273 RVA: 0x000274C4 File Offset: 0x000256C4
	private void OnWillRenderObject()
	{
		this.UpdateMaterials();
		if (this.mDynamicMat == null || this.mClipCount == 0)
		{
			return;
		}
		if (!this.mLegacyShader)
		{
			UIPanel parentPanel = this.panel;
			int num = 0;
			while (parentPanel != null)
			{
				if (parentPanel.hasClipping)
				{
					float num2 = 0f;
					Vector4 drawCallClipRange = parentPanel.drawCallClipRange;
					if (parentPanel != this.panel)
					{
						Vector3 vector = parentPanel.cachedTransform.InverseTransformPoint(this.panel.cachedTransform.position);
						drawCallClipRange.x -= vector.x;
						drawCallClipRange.y -= vector.y;
						Vector3 eulerAngles = this.panel.cachedTransform.rotation.eulerAngles;
						Vector3 eulerAngles2 = parentPanel.cachedTransform.rotation.eulerAngles;
						Vector3 vector2 = eulerAngles2 - eulerAngles;
						vector2.x = NGUIMath.WrapAngle(vector2.x);
						vector2.y = NGUIMath.WrapAngle(vector2.y);
						vector2.z = NGUIMath.WrapAngle(vector2.z);
						if (Mathf.Abs(vector2.x) > 0.001f || Mathf.Abs(vector2.y) > 0.001f)
						{
							Debug.LogWarning("Panel can only be clipped properly if X and Y rotation is left at 0", this.panel);
						}
						num2 = vector2.z;
					}
					this.SetClipping(num++, drawCallClipRange, parentPanel.clipSoftness, num2);
				}
				parentPanel = parentPanel.parentPanel;
			}
		}
		else
		{
			Vector2 clipSoftness = this.panel.clipSoftness;
			Vector4 drawCallClipRange2 = this.panel.drawCallClipRange;
			Vector2 vector3 = new Vector2(-drawCallClipRange2.x / drawCallClipRange2.z, -drawCallClipRange2.y / drawCallClipRange2.w);
			Vector2 vector4 = new Vector2(1f / drawCallClipRange2.z, 1f / drawCallClipRange2.w);
			Vector2 vector5 = new Vector2(1000f, 1000f);
			if (clipSoftness.x > 0f)
			{
				vector5.x = drawCallClipRange2.z / clipSoftness.x;
			}
			if (clipSoftness.y > 0f)
			{
				vector5.y = drawCallClipRange2.w / clipSoftness.y;
			}
			this.mDynamicMat.mainTextureOffset = vector3;
			this.mDynamicMat.mainTextureScale = vector4;
			this.mDynamicMat.SetVector("_ClipSharpness", vector5);
		}
	}
	
	// Token: 0x060004FA RID: 1274 RVA: 0x00027754 File Offset: 0x00025954
	private void SetClipping(int index, Vector4 cr, Vector2 soft, float angle)
	{
		angle *= -0.017453292f;
		Vector2 vector = new Vector2(1000f, 1000f);
		if (soft.x > 0f)
		{
			vector.x = cr.z / soft.x;
		}
		if (soft.y > 0f)
		{
			vector.y = cr.w / soft.y;
		}
		if (index < UIDrawCall.ClipRange.Length)
		{
			this.mDynamicMat.SetVector(UIDrawCall.ClipRange[index], new Vector4(-cr.x / cr.z, -cr.y / cr.w, 1f / cr.z, 1f / cr.w));
			this.mDynamicMat.SetVector(UIDrawCall.ClipArgs[index], new Vector4(vector.x, vector.y, Mathf.Sin(angle), Mathf.Cos(angle)));
		}
	}
	
	// Token: 0x060004FB RID: 1275 RVA: 0x0002785C File Offset: 0x00025A5C
	private void OnEnable()
	{
		this.mRebuildMat = true;
	}
	
	// Token: 0x060004FC RID: 1276 RVA: 0x00027868 File Offset: 0x00025A68
	private void OnDisable()
	{
		this.depthStart = int.MaxValue;
		this.depthEnd = int.MinValue;
		this.panel = null;
		this.manager = null;
		this.mMaterial = null;
		this.mTexture = null;
		NGUITools.DestroyImmediate(this.mDynamicMat);
		this.mDynamicMat = null;
	}
	
	// Token: 0x060004FD RID: 1277 RVA: 0x000278BC File Offset: 0x00025ABC
	private void OnDestroy()
	{
		NGUITools.DestroyImmediate(this.mMesh);
	}
	
	// Token: 0x060004FE RID: 1278 RVA: 0x000278CC File Offset: 0x00025ACC
	public static UIDrawCall Create(UIPanel panel, Material mat, Texture tex, Shader shader)
	{
		return UIDrawCall.Create(null, panel, mat, tex, shader);
	}
	
	// Token: 0x060004FF RID: 1279 RVA: 0x000278D8 File Offset: 0x00025AD8
	private static UIDrawCall Create(string name, UIPanel pan, Material mat, Texture tex, Shader shader)
	{
		UIDrawCall uidrawCall = UIDrawCall.Create(name);
		uidrawCall.gameObject.layer = pan.cachedGameObject.layer;
		uidrawCall.baseMaterial = mat;
		uidrawCall.mainTexture = tex;
		uidrawCall.shader = shader;
		uidrawCall.renderQueue = pan.startingRenderQueue;
		uidrawCall.sortingOrder = pan.sortingOrder;
		uidrawCall.manager = pan;
		return uidrawCall;
	}
	
	// Token: 0x06000500 RID: 1280 RVA: 0x00027938 File Offset: 0x00025B38
	private static UIDrawCall Create(string name)
	{
		if (UIDrawCall.mInactiveList.size > 0)
		{
			UIDrawCall uidrawCall = UIDrawCall.mInactiveList.Pop();
			UIDrawCall.mActiveList.Add(uidrawCall);
			if (name != null)
			{
				uidrawCall.name = name;
			}
			NGUITools.SetActive(uidrawCall.gameObject, true);
			return uidrawCall;
		}
		GameObject gameObject = new GameObject(name);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		UIDrawCall uidrawCall2 = gameObject.AddComponent<UIDrawCall>();
		UIDrawCall.mActiveList.Add(uidrawCall2);
		return uidrawCall2;
	}
	
	// Token: 0x06000501 RID: 1281 RVA: 0x000279A8 File Offset: 0x00025BA8
	public static void ClearAll()
	{
		bool isPlaying = Application.isPlaying;
		int i = UIDrawCall.mActiveList.size;
		while (i > 0)
		{
			UIDrawCall uidrawCall = UIDrawCall.mActiveList[--i];
			if (uidrawCall)
			{
				if (isPlaying)
				{
					NGUITools.SetActive(uidrawCall.gameObject, false);
				}
				else
				{
					NGUITools.DestroyImmediate(uidrawCall.gameObject);
				}
			}
		}
		UIDrawCall.mActiveList.Clear();
	}
	
	// Token: 0x06000502 RID: 1282 RVA: 0x00027A1C File Offset: 0x00025C1C
	public static void ReleaseAll()
	{
		UIDrawCall.ClearAll();
		UIDrawCall.ReleaseInactive();
	}
	
	// Token: 0x06000503 RID: 1283 RVA: 0x00027A28 File Offset: 0x00025C28
	public static void ReleaseInactive()
	{
		int i = UIDrawCall.mInactiveList.size;
		while (i > 0)
		{
			UIDrawCall uidrawCall = UIDrawCall.mInactiveList[--i];
			if (uidrawCall)
			{
				NGUITools.DestroyImmediate(uidrawCall.gameObject);
			}
		}
		UIDrawCall.mInactiveList.Clear();
	}
	
	// Token: 0x06000504 RID: 1284 RVA: 0x00027A7C File Offset: 0x00025C7C
	public static int Count(UIPanel panel)
	{
		int num = 0;
		for (int i = 0; i < UIDrawCall.mActiveList.size; i++)
		{
			if (UIDrawCall.mActiveList[i].manager == panel)
			{
				num++;
			}
		}
		return num;
	}
	
	// Token: 0x06000505 RID: 1285 RVA: 0x00027AC8 File Offset: 0x00025CC8
	public static void Destroy(UIDrawCall dc)
	{
		if (dc)
		{
			if (Application.isPlaying)
			{
				if (UIDrawCall.mActiveList.Remove(dc))
				{
					NGUITools.SetActive(dc.gameObject, false);
					UIDrawCall.mInactiveList.Add(dc);
				}
			}
			else
			{
				UIDrawCall.mActiveList.Remove(dc);
				NGUITools.DestroyImmediate(dc.gameObject);
			}
		}
	}
	
	// Token: 0x04000583 RID: 1411
	private const int maxIndexBufferCache = 10;
	
	// Token: 0x04000584 RID: 1412
	private static BetterList<UIDrawCall> mActiveList = new BetterList<UIDrawCall>();
	
	// Token: 0x04000585 RID: 1413
	private static BetterList<UIDrawCall> mInactiveList = new BetterList<UIDrawCall>();
	
	// Token: 0x04000586 RID: 1414
	[HideInInspector]
	[NonSerialized]
	public int depthStart = int.MaxValue;
	
	// Token: 0x04000587 RID: 1415
	[HideInInspector]
	[NonSerialized]
	public int depthEnd = int.MinValue;
	
	// Token: 0x04000588 RID: 1416
	[HideInInspector]
	[NonSerialized]
	public UIPanel manager;
	
	// Token: 0x04000589 RID: 1417
	[HideInInspector]
	[NonSerialized]
	public UIPanel panel;
	
	// Token: 0x0400058A RID: 1418
	[HideInInspector]
	[NonSerialized]
	public bool alwaysOnScreen;
	
	// Token: 0x0400058B RID: 1419
	[HideInInspector]
	[NonSerialized]
	public BetterList<Vector3> verts = new BetterList<Vector3>();
	
	// Token: 0x0400058C RID: 1420
	[HideInInspector]
	[NonSerialized]
	public BetterList<Vector3> norms = new BetterList<Vector3>();
	
	// Token: 0x0400058D RID: 1421
	[HideInInspector]
	[NonSerialized]
	public BetterList<Vector4> tans = new BetterList<Vector4>();
	
	// Token: 0x0400058E RID: 1422
	[HideInInspector]
	[NonSerialized]
	public BetterList<Vector2> uvs = new BetterList<Vector2>();
	
	// Token: 0x0400058F RID: 1423
	[HideInInspector]
	[NonSerialized]
	public BetterList<Color32> cols = new BetterList<Color32>();
	
	// Token: 0x04000590 RID: 1424
	private Material mMaterial;
	
	// Token: 0x04000591 RID: 1425
	private Texture mTexture;
	
	// Token: 0x04000592 RID: 1426
	private Shader mShader;
	
	// Token: 0x04000593 RID: 1427
	private int mClipCount;
	
	// Token: 0x04000594 RID: 1428
	private Transform mTrans;
	
	// Token: 0x04000595 RID: 1429
	private Mesh mMesh;
	
	// Token: 0x04000596 RID: 1430
	private MeshFilter mFilter;
	
	// Token: 0x04000597 RID: 1431
	private MeshRenderer mRenderer;
	
	// Token: 0x04000598 RID: 1432
	private Material mDynamicMat;
	
	// Token: 0x04000599 RID: 1433
	private int[] mIndices;
	
	// Token: 0x0400059A RID: 1434
	private bool mRebuildMat = true;
	
	// Token: 0x0400059B RID: 1435
	private bool mLegacyShader;
	
	// Token: 0x0400059C RID: 1436
	private int mRenderQueue = 3000;
	
	// Token: 0x0400059D RID: 1437
	private int mTriangles;
	
	// Token: 0x0400059E RID: 1438
	[NonSerialized]
	public bool isDirty;
	
	// Token: 0x0400059F RID: 1439
	private static List<int[]> mCache = new List<int[]>(10);
	
	// Token: 0x040005A0 RID: 1440
	private static string[] ClipRange = new string[] { "_ClipRange0", "_ClipRange1", "_ClipRange2", "_ClipRange4" };
	
	// Token: 0x040005A1 RID: 1441
	private static string[] ClipArgs = new string[] { "_ClipArgs0", "_ClipArgs1", "_ClipArgs2", "_ClipArgs3" };
	
	// Token: 0x020000CE RID: 206
	public enum Clipping
	{
		// Token: 0x040005A3 RID: 1443
		None,
		// Token: 0x040005A4 RID: 1444
		SoftClip = 3,
		// Token: 0x040005A5 RID: 1445
		ConstrainButDontClip
	}
}
