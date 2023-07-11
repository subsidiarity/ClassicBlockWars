using UnityEngine;

[AddComponentMenu("NJG MiniMap/NGUI/Interaction/Tooltip")]
public class UICustomTooltip : MonoBehaviour
{
	private static UICustomTooltip mInstance;

	public Camera uiCamera;

	public UILabel text;

	public UISprite background;

	public float appearSpeed = 10f;

	public bool scalingTransitions = true;

	public bool followMousePosition = true;

	public Vector2 backgroundPadding = new Vector2(4f, 4f);

	public Vector2 positionOffset = new Vector2(0f, 30f);

	private Transform mTrans;

	private float mTarget;

	private float mCurrent;

	private Vector3 mPos;

	private Vector3 mSize;

	private UIWidget[] mWidgets;

	private void Awake()
	{
		mInstance = this;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Start()
	{
		mTrans = base.transform;
		NGUITools.SetActive(base.gameObject, true);
		mWidgets = GetComponentsInChildren<UIWidget>();
		mPos = mTrans.localPosition;
		mSize = mTrans.localScale;
		if (uiCamera == null)
		{
			uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		SetAlpha(0f);
	}

	private void Update()
	{
		if (mCurrent != mTarget)
		{
			mCurrent = Mathf.Lerp(mCurrent, mTarget, Time.deltaTime * appearSpeed);
			if (Mathf.Abs(mCurrent - mTarget) < 0.001f)
			{
				mCurrent = mTarget;
			}
			SetAlpha(mCurrent * mCurrent);
			if (scalingTransitions)
			{
				Vector3 vector = mSize * 0.25f;
				vector.y = 0f - vector.y;
				Vector3 localScale = Vector3.one * (1.5f - mCurrent * 0.5f);
				Vector3 localPosition = Vector3.Lerp(mPos - vector, mPos, mCurrent);
				mTrans.localPosition = localPosition;
				mTrans.localScale = localScale;
			}
		}
		if (followMousePosition && mCurrent > 0f)
		{
			UpdatePosition();
		}
	}

	private void SetAlpha(float val)
	{
		int i = 0;
		for (int num = mWidgets.Length; i < num; i++)
		{
			UIWidget uIWidget = mWidgets[i];
			Color color = uIWidget.color;
			color.a = val;
			uIWidget.color = color;
		}
	}

	private void SetText(string content, Color bgColor)
	{
		if (content != null && !string.IsNullOrEmpty(content))
		{
			mTarget = 1f;
			text.text = content;
			if (background != null)
			{
				Transform transform = text.transform;
				Vector3 localScale = transform.localScale;
				mSize = text.printedSize;
				mSize.x *= localScale.x;
				mSize.y *= localScale.y;
				mSize.x += background.border.x + background.border.z + backgroundPadding.x;
				mSize.y += background.border.y + background.border.w + backgroundPadding.y;
				mSize.z = 1f;
				transform.localPosition = new Vector3(transform.localPosition.x, backgroundPadding.y * 2f, transform.localPosition.z);
				background.width = (int)mSize.x;
				background.height = (int)mSize.y;
			}
			UpdatePosition();
		}
		else
		{
			mTarget = 0f;
		}
		SetAlpha(mCurrent * mCurrent);
	}

	private void UpdatePosition()
	{
		mPos = Input.mousePosition;
		if (uiCamera != null)
		{
			mPos.x = Mathf.Clamp01(mPos.x / (float)Screen.width);
			mPos.y = Mathf.Clamp01(mPos.y / (float)Screen.height);
			float num = uiCamera.orthographicSize / mTrans.parent.lossyScale.y;
			float num2 = (float)Screen.height * 0.5f / num;
			Vector2 vector = new Vector2(num2 * mSize.x / (float)Screen.width, num2 * mSize.y / (float)Screen.height);
			mPos.x = Mathf.Min(mPos.x, 1f - vector.x);
			mPos.y = Mathf.Max(mPos.y, vector.y);
			mTrans.position = uiCamera.ViewportToWorldPoint(mPos);
			mPos = mTrans.localPosition;
			mPos.x = (int)(mPos.x + positionOffset.x);
			mPos.y = (int)(mPos.y + positionOffset.y);
			mTrans.localPosition = mPos;
		}
		else
		{
			if (mPos.x + mSize.x > (float)Screen.width)
			{
				mPos.x = (float)Screen.width - mSize.x;
			}
			if (mPos.y - mSize.y < 0f)
			{
				mPos.y = mSize.y;
			}
			mPos.x -= (float)Screen.width * 0.5f;
			mPos.y -= (float)Screen.height * 0.5f;
		}
	}

	public static void Hide()
	{
		if (mInstance != null)
		{
			mInstance.mTarget = 0f;
		}
	}

	public static void Show(string contentText)
	{
		if (mInstance != null)
		{
			mInstance.SetText(contentText, Color.white);
		}
	}

	public static void Show(string contentText, Color color)
	{
		if (mInstance != null)
		{
			mInstance.SetText(contentText, color);
		}
	}
}
