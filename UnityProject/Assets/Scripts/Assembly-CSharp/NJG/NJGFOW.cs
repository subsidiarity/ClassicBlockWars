using System.Diagnostics;
using System.Threading;
using NJG;
using UnityEngine;

public class NJGFOW : MonoBehaviour
{
	public enum State
	{
		Blending = 0,
		NeedUpdate = 1,
		UpdateTexture0 = 2,
		UpdateTexture1 = 3
	}

	public class Revealer
	{
		public bool isActive;

		public Vector2 pos = Vector2.zero;

		public int revealDistance = 10;
	}

	private const string FOW_ID = "NJGFOW";

	private static NJGFOW mInst;

	private static FastList<Revealer> mRevealers = new FastList<Revealer>();

	private static FastList<Revealer> mAdded = new FastList<Revealer>();

	private static FastList<Revealer> mRemoved = new FastList<Revealer>();

	private float mBlendFactor;

	private State mState;

	private Texture2D mTexture0;

	private Texture2D hiddenTexture;

	private Vector2 mOrigin;

	private Color32[] mBuffer0;

	private Color32[] mBuffer1;

	private Color32[] mBuffer2;

	private Color32[] mBuffer3;

	private NJGMapBase map;

	private float mNextUpdate;

	private Thread mThread;

	public static NJGFOW instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = Object.FindObjectOfType(typeof(NJGFOW)) as NJGFOW;
				if (mInst == null)
				{
					GameObject gameObject = new GameObject("_NJGFOW");
					gameObject.hideFlags = HideFlags.HideInHierarchy;
					mInst = gameObject.AddComponent<NJGFOW>();
				}
			}
			return mInst;
		}
	}

	private void Awake()
	{
		map = NJGMapBase.instance;
	}

	public void Init()
	{
		if (hiddenTexture == null)
		{
			hiddenTexture = new Texture2D(4, 4);
		}
		if (mBuffer3 == null)
		{
			mBuffer3 = new Color32[16];
		}
		int i = 0;
		for (int num = mBuffer3.Length; i < num; i++)
		{
			mBuffer3[i] = map.fow.fogColor;
		}
		hiddenTexture.SetPixels32(mBuffer3);
		hiddenTexture.Apply();
		if (map.fow.fowSystem == NJGMapBase.FOW.FOWSystem.BuiltInFOW)
		{
			if (mTexture0 == null)
			{
				mTexture0 = new Texture2D(map.fow.textureSize, map.fow.textureSize, TextureFormat.ARGB32, false);
				mTexture0.wrapMode = TextureWrapMode.Clamp;
			}
			mOrigin = Vector2.zero;
			mOrigin.x -= (float)map.fow.textureSize * 0.5f;
			mOrigin.y -= (float)map.fow.textureSize * 0.5f;
			int num2 = map.fow.textureSize * map.fow.textureSize;
			if (mBuffer0 == null)
			{
				mBuffer0 = new Color32[num2];
			}
			if (mBuffer1 == null)
			{
				mBuffer1 = new Color32[num2];
			}
			if (mBuffer2 == null)
			{
				mBuffer2 = new Color32[num2];
			}
			int j = 0;
			for (int num3 = mBuffer0.Length; j < num3; j++)
			{
				mBuffer0[j] = Color.clear;
				mBuffer1[j] = Color.clear;
				mBuffer2[j] = Color.clear;
			}
			UpdateBuffer();
			UpdateTexture();
			if (UIMiniMapBase.inst != null)
			{
				UIMiniMapBase.inst.material.SetTexture("_Revealed", mTexture0);
				UIMiniMapBase.inst.material.SetTexture("_Hidden", hiddenTexture);
			}
			if (UIWorldMapBase.inst != null)
			{
				UIWorldMapBase.inst.material.SetTexture("_Revealed", mTexture0);
				UIWorldMapBase.inst.material.SetTexture("_Hidden", hiddenTexture);
			}
			mNextUpdate = Time.time + map.fow.updateFrequency;
			if (mThread == null)
			{
				mThread = new Thread(ThreadUpdate);
				mThread.Start();
			}
		}
		else if (map.fow.fowSystem != NJGMapBase.FOW.FOWSystem.TasharenFOW)
		{
		}
	}

	private void OnDestroy()
	{
		if (mThread != null)
		{
			mThread.Abort();
			while (mThread.IsAlive)
			{
				Thread.Sleep(1);
			}
			mThread = null;
		}
	}

	private void ThreadUpdate()
	{
		Stopwatch stopwatch = new Stopwatch();
		while (true)
		{
			if (mState == State.NeedUpdate)
			{
				stopwatch.Reset();
				stopwatch.Start();
				UpdateBuffer();
				stopwatch.Stop();
				mState = State.UpdateTexture0;
			}
			Thread.Sleep(1);
		}
	}

	private void Update()
	{
		if (map == null)
		{
			return;
		}
		if (map.fow.textureBlendTime > 0f)
		{
			mBlendFactor = Mathf.Clamp01(mBlendFactor + Time.deltaTime / map.fow.textureBlendTime);
		}
		else
		{
			mBlendFactor = 1f;
		}
		if (mState == State.Blending)
		{
			float time = Time.time;
			if (mNextUpdate < time)
			{
				mNextUpdate = time + map.fow.updateFrequency;
				mState = State.NeedUpdate;
			}
		}
		else if (mState != State.NeedUpdate)
		{
			UpdateTexture();
		}
	}

	public static Revealer CreateRevealer()
	{
		Revealer revealer = new Revealer();
		revealer.isActive = false;
		lock (mAdded)
		{
			mAdded.Add(revealer);
			return revealer;
		}
	}

	public static void DeleteRevealer(Revealer rev)
	{
		lock (mRemoved)
		{
			mRemoved.Add(rev);
		}
	}

	public byte[] GetRevealedBuffer()
	{
		int num = map.fow.textureSize * map.fow.textureSize;
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = mBuffer1[i].g;
		}
		return array;
	}

	public void RevealFOW(byte[] arr)
	{
		int num = map.fow.textureSize * map.fow.textureSize;
		if (arr.Length != num)
		{
			UnityEngine.Debug.LogError("Buffer size mismatch. Fog is " + num + ", but passed array is " + arr.Length);
			return;
		}
		if (mBuffer0 == null)
		{
			mBuffer0 = new Color32[num];
			mBuffer1 = new Color32[num];
		}
		for (int i = 0; i < num; i++)
		{
			mBuffer0[i].g = arr[i];
			mBuffer1[i].g = arr[i];
		}
	}

	public void RevealFOW(string fowData)
	{
		int num = map.fow.textureSize * map.fow.textureSize;
		string[] array = fowData.Split('|');
		if (array.Length != num)
		{
			UnityEngine.Debug.LogError("Buffer size mismatch. Fog is " + num + ", but passed array is " + array.Length);
			return;
		}
		if (mBuffer0 == null)
		{
			mBuffer0 = new Color32[num];
			mBuffer1 = new Color32[num];
		}
		for (int i = 0; i < num; i++)
		{
			mBuffer0[i].g = byte.Parse(array[i]);
			mBuffer1[i].g = byte.Parse(array[i]);
		}
	}

	private string SerializeFOW()
	{
		int num = map.fow.textureSize * map.fow.textureSize;
		string[] array = new string[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = string.Empty + mBuffer1[i].g;
		}
		return string.Join("|", array);
	}

	private void Save(string gameName)
	{
		string value = SerializeFOW();
		if (!string.IsNullOrEmpty(value))
		{
			PlayerPrefs.SetString(gameName + "NJGFOW", value);
		}
	}

	private void Load(string gameName)
	{
		RevealFOW(PlayerPrefs.GetString(gameName + "NJGFOW", null));
	}

	public void ResetFOW()
	{
		if (map.fow.fowSystem == NJGMapBase.FOW.FOWSystem.BuiltInFOW)
		{
			for (int i = 0; i < NJGMapItem.list.Count; i++)
			{
				NJGMapItem nJGMapItem = NJGMapItem.list[i];
				if (nJGMapItem.cachedTransform != UIMiniMapBase.inst.target)
				{
					nJGMapItem.isRevealed = false;
				}
			}
		}
		Init();
	}

	private void UpdateTexture()
	{
		if (mState == State.UpdateTexture0)
		{
			mTexture0.SetPixels32(mBuffer0);
			mTexture0.Apply();
			mState = State.UpdateTexture1;
			mBlendFactor = 0f;
		}
		else if (mState == State.UpdateTexture1)
		{
			mState = State.Blending;
		}
	}

	private void UpdateBuffer()
	{
		if (mAdded.size > 0)
		{
			lock (mAdded)
			{
				while (mAdded.size > 0)
				{
					int num = mAdded.size - 1;
					mRevealers.Add(mAdded.buffer[num]);
					mAdded.RemoveAt(num);
				}
			}
		}
		if (mRemoved.size > 0)
		{
			lock (mRemoved)
			{
				while (mRemoved.size > 0)
				{
					int num2 = mRemoved.size - 1;
					mRevealers.Remove(mRemoved.buffer[num2]);
					mRemoved.RemoveAt(num2);
				}
			}
		}
		float t = ((!(map.fow.textureBlendTime > 0f)) ? 1f : Mathf.Clamp01(mBlendFactor + map.elapsed / map.fow.textureBlendTime));
		if (mBuffer0 != null)
		{
			int i = 0;
			for (int num3 = mBuffer0.Length; i < num3; i++)
			{
				mBuffer0[i] = Color32.Lerp(mBuffer0[i], mBuffer1[i], t);
				mBuffer0[i].r = 0;
			}
		}
		float worldToTex = map.fow.textureSize / map.fow.textureSize;
		for (int j = 0; j != mRevealers.size; j++)
		{
			Revealer revealer = mRevealers[j];
			if (revealer.isActive)
			{
				RevealAtPosition(revealer, worldToTex);
			}
		}
		for (int k = 0; k != map.fow.blurIterations; k++)
		{
			BlurVisibility();
		}
		RevealMap();
	}

	private void BlurVisibility()
	{
		for (int i = 0; i < map.fow.textureSize; i++)
		{
			int num = i * map.fow.textureSize;
			int num2 = i - 1;
			if (num2 < 0)
			{
				num2 = 0;
			}
			int num3 = i + 1;
			if (num3 == map.fow.textureSize)
			{
				num3 = i;
			}
			num2 *= map.fow.textureSize;
			num3 *= map.fow.textureSize;
			for (int j = 0; j < map.fow.textureSize; j++)
			{
				int num4 = j - 1;
				if (num4 < 0)
				{
					num4 = 0;
				}
				int num5 = j + 1;
				if (num5 == map.fow.textureSize)
				{
					num5 = j;
				}
				int num6 = j + num;
				int r = mBuffer1[num6].r;
				r += mBuffer1[num4 + num].r;
				r += mBuffer1[num5 + num].r;
				r += mBuffer1[j + num2].r;
				r += mBuffer1[j + num3].r;
				r += mBuffer1[num4 + num2].r;
				r += mBuffer1[num5 + num2].r;
				r += mBuffer1[num4 + num3].r;
				r += mBuffer1[num5 + num3].r;
				Color32 color = mBuffer2[num6];
				color.r = (byte)(r / 9);
				mBuffer2[num6] = color;
				if (map.fow.trailEffect)
				{
					mBuffer2[num6].a = 0;
					mBuffer2[num6].g = 0;
					mBuffer2[num6].b = 0;
				}
			}
		}
		Color32[] array = mBuffer1;
		mBuffer1 = mBuffer2;
		mBuffer2 = array;
	}

	private void RevealAtPosition(Revealer r, float worldToTex)
	{
		Vector2 vector = r.pos - mOrigin;
		int num = Mathf.RoundToInt((vector.x - (float)r.revealDistance) * worldToTex);
		int num2 = Mathf.RoundToInt((vector.y - (float)r.revealDistance) * worldToTex);
		int num3 = Mathf.RoundToInt((vector.x + (float)r.revealDistance) * worldToTex);
		int num4 = Mathf.RoundToInt((vector.y + (float)r.revealDistance) * worldToTex);
		int value = Mathf.RoundToInt(vector.x * worldToTex);
		int value2 = Mathf.RoundToInt(vector.y * worldToTex);
		int textureSize = map.fow.textureSize;
		value = Mathf.Clamp(value, 0, textureSize - 1);
		value2 = Mathf.Clamp(value2, 0, textureSize - 1);
		int num5 = Mathf.RoundToInt((float)(r.revealDistance * r.revealDistance) * worldToTex * worldToTex);
		for (int i = num2; i < num4; i++)
		{
			if (i <= -1 || i >= textureSize)
			{
				continue;
			}
			int num6 = i * textureSize;
			for (int j = num; j < num3; j++)
			{
				if (j > -1 && j < textureSize)
				{
					int num7 = j - value;
					int num8 = i - value2;
					int num9 = num7 * num7 + num8 * num8;
					if (num9 < num5)
					{
						mBuffer1[j + num6].r = byte.MaxValue;
					}
				}
			}
		}
	}

	private void RevealMap()
	{
		for (int i = 0; i < map.fow.textureSize; i++)
		{
			int num = i * map.fow.textureSize;
			for (int j = 0; j < map.fow.textureSize; j++)
			{
				int num2 = j + num;
				Color32 color = mBuffer1[num2];
				if (color.g < color.r)
				{
					color.g = color.r;
					mBuffer1[num2] = color;
				}
			}
		}
	}

	public bool IsVisible(Vector2 pos)
	{
		if (mBuffer0 == null)
		{
			return false;
		}
		pos -= mOrigin;
		float num = (float)map.fow.textureSize / (float)map.fow.textureSize;
		int value = Mathf.RoundToInt(pos.x * num);
		int value2 = Mathf.RoundToInt(pos.y * num);
		value = Mathf.Clamp(value, 0, map.fow.textureSize - 1);
		value2 = Mathf.Clamp(value2, 0, map.fow.textureSize - 1);
		int num2 = value + value2 * map.fow.textureSize;
		return mBuffer1[num2].r > 0 || mBuffer0[num2].r > 0;
	}

	public bool IsExplored(Vector2 pos)
	{
		if (mBuffer0 == null)
		{
			return false;
		}
		pos -= mOrigin;
		float num = (float)map.fow.textureSize / (float)map.fow.textureSize;
		int value = Mathf.RoundToInt(pos.x * num);
		int value2 = Mathf.RoundToInt(pos.y * num);
		value = Mathf.Clamp(value, 0, map.fow.textureSize - 1);
		value2 = Mathf.Clamp(value2, 0, map.fow.textureSize - 1);
		return mBuffer0[value + value2 * map.fow.textureSize].g > 0;
	}
}
