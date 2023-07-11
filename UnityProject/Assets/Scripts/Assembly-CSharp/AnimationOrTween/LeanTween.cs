using System;
using System.Collections;
using UnityEngine;

public class LeanTween : MonoBehaviour
{
	public delegate float DelFunc(float fromVect, float toVect, float ratioPassed);

	public static bool throwErrors = true;

	private static LeanTweenDescr[] tweens;

	private static int tweenMaxSearch = 0;

	private static int maxTweens = 400;

	private static int frameRendered = -1;

	private static GameObject tweenEmpty;

	private static float dtEstimated;

	private static float dt;

	private static float dtActual;

	private static LeanTweenDescr tween;

	private static int i;

	private static int j;

	private static AnimationCurve punch = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.112586f, 0.9976035f), new Keyframe(0.3120486f, -0.1720615f), new Keyframe(0.4316337f, 0.07030682f), new Keyframe(0.5524869f, -0.03141804f), new Keyframe(0.6549395f, 0.003909959f), new Keyframe(0.770987f, -0.009817753f), new Keyframe(0.8838775f, 0.001939224f), new Keyframe(1f, 0f));

	private static Hashtable emptyHash = new Hashtable();

	private static Action onUpdateF;

	private static Transform trans;

	private static float timeTotal;

	private static int tweenAction;

	private static Hashtable optionalItems;

	private static AnimationCurve animationCurve;

	private static float ratioPassed;

	private static float from;

	private static float to;

	private static float val;

	private static Vector3 newVect;

	private static bool isTweenFinished;

	private static GameObject target;

	private static GameObject customTarget;

	public static int startSearch = 0;

	public static int lastMax = 0;

	public static void init()
	{
		init(maxTweens);
	}

	public static void init(int maxSimultaneousTweens)
	{
		if (tweens == null)
		{
			maxTweens = maxSimultaneousTweens;
			tweens = new LeanTweenDescr[maxTweens];
			tweenEmpty = new GameObject();
			tweenEmpty.name = "~LeanTween";
			tweenEmpty.AddComponent(typeof(LeanTween));
			tweenEmpty.isStatic = true;
			UnityEngine.Object.DontDestroyOnLoad(tweenEmpty);
			for (int i = 0; i < maxTweens; i++)
			{
				tweens[i] = new LeanTweenDescr();
			}
		}
	}

	public static void reset()
	{
		tweens = null;
	}

	public void Update()
	{
		update();
	}

	public static void update()
	{
		if (frameRendered == Time.frameCount)
		{
			return;
		}
		init();
		dtEstimated = ((Application.targetFrameRate <= 0) ? (1f / 60f) : (1f / (float)Application.targetFrameRate));
		dtActual = Time.deltaTime * Time.timeScale;
		for (int i = 0; i < tweenMaxSearch && i < maxTweens; i++)
		{
			if (!tweens[i].toggle)
			{
				continue;
			}
			tween = tweens[i];
			trans = tween.trans;
			timeTotal = tween.time;
			tweenAction = (int)tween.type;
			animationCurve = tween.animationCurve;
			optionalItems = tween.optional;
			dt = dtActual;
			if (tween.useEstimatedTime)
			{
				dt = dtEstimated;
			}
			else if (tween.useFrames)
			{
				dt = 1f;
			}
			if (trans == null)
			{
				removeTween(i);
				continue;
			}
			isTweenFinished = false;
			if (tween.passed + dt > timeTotal && tween.direction > 0f)
			{
				isTweenFinished = true;
				tween.passed = timeTotal;
			}
			else if (tween.direction < 0f && tween.passed - dt < 0f)
			{
				isTweenFinished = true;
				tween.passed = float.Epsilon;
			}
			if (((double)tween.passed == 0.0 && (double)tweens[i].delay == 0.0) || ((double)tween.passed > 0.0 && !tween.hasInitiliazed))
			{
				tween.hasInitiliazed = true;
				switch ((TweenAction)tweenAction)
				{
				case TweenAction.MOVE:
					tween.from = trans.position;
					break;
				case TweenAction.MOVE_X:
					tween.from.x = trans.position.x;
					break;
				case TweenAction.MOVE_Y:
					tween.from.x = trans.position.y;
					break;
				case TweenAction.MOVE_Z:
					tween.from.x = trans.position.z;
					break;
				case TweenAction.MOVE_LOCAL_X:
					tweens[i].from.x = trans.localPosition.x;
					break;
				case TweenAction.MOVE_LOCAL_Y:
					tweens[i].from.x = trans.localPosition.y;
					break;
				case TweenAction.MOVE_LOCAL_Z:
					tweens[i].from.x = trans.localPosition.z;
					break;
				case TweenAction.SCALE_X:
					tween.from.x = trans.localScale.x;
					break;
				case TweenAction.SCALE_Y:
					tween.from.x = trans.localScale.y;
					break;
				case TweenAction.SCALE_Z:
					tween.from.x = trans.localScale.z;
					break;
				case TweenAction.ALPHA:
					tween.from.x = trans.gameObject.renderer.material.color.a;
					break;
				case TweenAction.MOVE_LOCAL:
					tween.from = trans.localPosition;
					break;
				case TweenAction.MOVE_CURVED:
					tween.from.x = 0f;
					break;
				case TweenAction.MOVE_CURVED_LOCAL:
					tween.from.x = 0f;
					break;
				case TweenAction.ROTATE:
					tween.from = trans.eulerAngles;
					tween.to.x = closestRot(tween.from.x, tween.to.x);
					tween.to.y = closestRot(tween.from.y, tween.to.y);
					tween.to.z = closestRot(tween.from.z, tween.to.z);
					break;
				case TweenAction.ROTATE_X:
					tween.from.x = trans.eulerAngles.x;
					tween.to.x = closestRot(tween.from.x, tween.to.x);
					break;
				case TweenAction.ROTATE_Y:
					tween.from.x = trans.eulerAngles.y;
					tween.to.x = closestRot(tween.from.x, tween.to.x);
					break;
				case TweenAction.ROTATE_Z:
					tween.from.x = trans.eulerAngles.z;
					tween.to.x = closestRot(tween.from.x, tween.to.x);
					break;
				case TweenAction.ROTATE_AROUND:
					tween.optional["last"] = 0f;
					tween.optional["origRotation"] = trans.eulerAngles;
					break;
				case TweenAction.ROTATE_LOCAL:
					tween.from = trans.localEulerAngles;
					tween.to.x = closestRot(tween.from.x, tween.to.x);
					tween.to.y = closestRot(tween.from.y, tween.to.y);
					tween.to.z = closestRot(tween.from.z, tween.to.z);
					break;
				case TweenAction.SCALE:
					tween.from = trans.localScale;
					break;
				case TweenAction.GUI_MOVE:
					tween.from = new Vector3(tween.ltRect.rect.x, tween.ltRect.rect.y, 0f);
					break;
				case TweenAction.GUI_SCALE:
					tween.from = new Vector3(tween.ltRect.rect.width, tween.ltRect.rect.height, 0f);
					break;
				case TweenAction.GUI_ALPHA:
					tween.from.x = tween.ltRect.alpha;
					break;
				case TweenAction.GUI_ROTATE:
					if (!tween.ltRect.rotateEnabled)
					{
						tween.ltRect.rotateEnabled = true;
						tween.ltRect.resetForRotation();
					}
					tween.from.x = tween.ltRect.rotation;
					break;
				}
				tween.diff.x = tween.to.x - tween.from.x;
				tween.diff.y = tween.to.y - tween.from.y;
				tween.diff.z = tween.to.z - tween.from.z;
			}
			if (tween.delay <= 0f)
			{
				ratioPassed = tween.passed / timeTotal;
				if ((double)ratioPassed > 1.0)
				{
					ratioPassed = 1f;
				}
				if (tweenAction >= 0 && tweenAction <= 17)
				{
					if (animationCurve != null)
					{
						val = tweenOnCurve(tween, ratioPassed);
					}
					else
					{
						switch (tween.tweenType)
						{
						case LeanTweenType.linear:
							val = tween.from.x + tween.diff.x * ratioPassed;
							break;
						case LeanTweenType.easeOutQuad:
							val = easeOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
							break;
						case LeanTweenType.easeInQuad:
							val = easeInQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
							break;
						case LeanTweenType.easeInOutQuad:
							val = easeInOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
							break;
						case LeanTweenType.easeInCubic:
							val = easeInCubic(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeOutCubic:
							val = easeOutCubic(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInOutCubic:
							val = easeInOutCubic(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInQuart:
							val = easeInQuart(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeOutQuart:
							val = easeOutQuart(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInOutQuart:
							val = easeInOutQuart(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInQuint:
							val = easeInQuint(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeOutQuint:
							val = easeOutQuint(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInOutQuint:
							val = easeInOutQuint(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInSine:
							val = easeInSine(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeOutSine:
							val = easeOutSine(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInOutSine:
							val = easeInOutSine(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInExpo:
							val = easeInExpo(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeOutExpo:
							val = easeOutExpo(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInOutExpo:
							val = easeInOutExpo(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInCirc:
							val = easeInCirc(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeOutCirc:
							val = easeOutCirc(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInOutCirc:
							val = easeInOutCirc(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInBounce:
							val = easeInBounce(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeOutBounce:
							val = easeOutBounce(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInOutBounce:
							val = easeInOutBounce(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInBack:
							val = easeInBack(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeOutBack:
							val = easeOutBack(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInOutBack:
							val = easeInOutElastic(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInElastic:
							val = easeInElastic(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeOutElastic:
							val = easeOutElastic(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.easeInOutElastic:
							val = easeInOutElastic(tween.from.x, tween.to.x, ratioPassed);
							break;
						case LeanTweenType.punch:
							tween.animationCurve = punch;
							tween.to.x = tween.from.x + tween.to.x;
							val = tweenOnCurve(tween, ratioPassed);
							break;
						default:
							val = tween.from.x + tween.diff.x * ratioPassed;
							break;
						}
					}
					if (tweenAction == 0)
					{
						trans.position = new Vector3(val, trans.position.y, trans.position.z);
					}
					else if (tweenAction == 1)
					{
						trans.position = new Vector3(trans.position.x, val, trans.position.z);
					}
					else if (tweenAction == 2)
					{
						trans.position = new Vector3(trans.position.x, trans.position.y, val);
					}
					if (tweenAction == 3)
					{
						trans.localPosition = new Vector3(val, trans.localPosition.y, trans.localPosition.z);
					}
					else if (tweenAction == 4)
					{
						trans.localPosition = new Vector3(trans.localPosition.x, val, trans.localPosition.z);
					}
					else if (tweenAction == 5)
					{
						trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, val);
					}
					else if (tweenAction == 6)
					{
						if (tween.path.orientToPath)
						{
							tween.path.place(trans, val);
						}
						else
						{
							trans.position = tween.path.point(val);
						}
					}
					else if (tweenAction == 7)
					{
						if (tween.path.orientToPath)
						{
							tween.path.placeLocal(trans, val);
						}
						else
						{
							trans.localPosition = tween.path.point(val);
						}
					}
					else if (tweenAction == 8)
					{
						trans.localScale = new Vector3(val, trans.localScale.y, trans.localScale.z);
					}
					else if (tweenAction == 9)
					{
						trans.localScale = new Vector3(trans.localScale.x, val, trans.localScale.z);
					}
					else if (tweenAction == 10)
					{
						trans.localScale = new Vector3(trans.localScale.x, trans.localScale.y, val);
					}
					else if (tweenAction == 11)
					{
						trans.eulerAngles = new Vector3(val, trans.eulerAngles.y, trans.eulerAngles.z);
					}
					else if (tweenAction == 12)
					{
						trans.eulerAngles = new Vector3(trans.eulerAngles.x, val, trans.eulerAngles.z);
					}
					else if (tweenAction == 13)
					{
						trans.eulerAngles = new Vector3(trans.eulerAngles.x, trans.eulerAngles.y, val);
					}
					else if (tweenAction == 14)
					{
						float num = (float)tween.optional["last"];
						float angle = val - num;
						if (isTweenFinished)
						{
							Vector3 eulerAngles = (Vector3)tween.optional["origRotation"];
							trans.eulerAngles = eulerAngles;
							trans.RotateAround(trans.TransformPoint((Vector3)tween.optional["point"]), (Vector3)tween.optional["axis"], tween.to.x);
						}
						else
						{
							trans.RotateAround(trans.TransformPoint((Vector3)tween.optional["point"]), (Vector3)tween.optional["axis"], angle);
							tween.optional["last"] = val;
						}
					}
					else if (tweenAction == 15)
					{
						Material[] materials = trans.gameObject.renderer.materials;
						foreach (Material material in materials)
						{
							material.color = new Color(material.color.r, material.color.g, material.color.b, val);
						}
					}
				}
				else if (tweenAction >= 18)
				{
					if (animationCurve != null)
					{
						newVect = tweenOnCurveVector(tween, ratioPassed);
					}
					else if (tween.tweenType == LeanTweenType.linear)
					{
						newVect.x = tween.from.x + tween.diff.x * ratioPassed;
						newVect.y = tween.from.y + tween.diff.y * ratioPassed;
						newVect.z = tween.from.z + tween.diff.z * ratioPassed;
					}
					else if (tween.tweenType >= LeanTweenType.linear)
					{
						switch (tween.tweenType)
						{
						case LeanTweenType.easeOutQuad:
							newVect = new Vector3(easeOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed), easeOutQuadOpt(tween.from.y, tween.diff.y, ratioPassed), easeOutQuadOpt(tween.from.z, tween.diff.z, ratioPassed));
							break;
						case LeanTweenType.easeInQuad:
							newVect = new Vector3(easeInQuadOpt(tween.from.x, tween.diff.x, ratioPassed), easeInQuadOpt(tween.from.y, tween.diff.y, ratioPassed), easeInQuadOpt(tween.from.z, tween.diff.z, ratioPassed));
							break;
						case LeanTweenType.easeInOutQuad:
							newVect = new Vector3(easeInOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed), easeInOutQuadOpt(tween.from.y, tween.diff.y, ratioPassed), easeInOutQuadOpt(tween.from.z, tween.diff.z, ratioPassed));
							break;
						case LeanTweenType.easeInCubic:
							newVect = new Vector3(easeInCubic(tween.from.x, tween.to.x, ratioPassed), easeInCubic(tween.from.y, tween.to.y, ratioPassed), easeInCubic(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeOutCubic:
							newVect = new Vector3(easeOutCubic(tween.from.x, tween.to.x, ratioPassed), easeOutCubic(tween.from.y, tween.to.y, ratioPassed), easeOutCubic(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInOutCubic:
							newVect = new Vector3(easeInOutCubic(tween.from.x, tween.to.x, ratioPassed), easeInOutCubic(tween.from.y, tween.to.y, ratioPassed), easeInOutCubic(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInQuart:
							newVect = new Vector3(easeInQuart(tween.from.x, tween.to.x, ratioPassed), easeInQuart(tween.from.y, tween.to.y, ratioPassed), easeInQuart(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeOutQuart:
							newVect = new Vector3(easeOutQuart(tween.from.x, tween.to.x, ratioPassed), easeOutQuart(tween.from.y, tween.to.y, ratioPassed), easeOutQuart(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInOutQuart:
							newVect = new Vector3(easeInOutQuart(tween.from.x, tween.to.x, ratioPassed), easeInOutQuart(tween.from.y, tween.to.y, ratioPassed), easeInOutQuart(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInQuint:
							newVect = new Vector3(easeInQuint(tween.from.x, tween.to.x, ratioPassed), easeInQuint(tween.from.y, tween.to.y, ratioPassed), easeInQuint(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeOutQuint:
							newVect = new Vector3(easeOutQuint(tween.from.x, tween.to.x, ratioPassed), easeOutQuint(tween.from.y, tween.to.y, ratioPassed), easeOutQuint(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInOutQuint:
							newVect = new Vector3(easeInOutQuint(tween.from.x, tween.to.x, ratioPassed), easeInOutQuint(tween.from.y, tween.to.y, ratioPassed), easeInOutQuint(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInSine:
							newVect = new Vector3(easeInSine(tween.from.x, tween.to.x, ratioPassed), easeInSine(tween.from.y, tween.to.y, ratioPassed), easeInSine(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeOutSine:
							newVect = new Vector3(easeOutSine(tween.from.x, tween.to.x, ratioPassed), easeOutSine(tween.from.y, tween.to.y, ratioPassed), easeOutSine(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInOutSine:
							newVect = new Vector3(easeInOutSine(tween.from.x, tween.to.x, ratioPassed), easeInOutSine(tween.from.y, tween.to.y, ratioPassed), easeInOutSine(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInExpo:
							newVect = new Vector3(easeInExpo(tween.from.x, tween.to.x, ratioPassed), easeInExpo(tween.from.y, tween.to.y, ratioPassed), easeInExpo(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeOutExpo:
							newVect = new Vector3(easeOutExpo(tween.from.x, tween.to.x, ratioPassed), easeOutExpo(tween.from.y, tween.to.y, ratioPassed), easeOutExpo(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInOutExpo:
							newVect = new Vector3(easeInOutExpo(tween.from.x, tween.to.x, ratioPassed), easeInOutExpo(tween.from.y, tween.to.y, ratioPassed), easeInOutExpo(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInCirc:
							newVect = new Vector3(easeInCirc(tween.from.x, tween.to.x, ratioPassed), easeInCirc(tween.from.y, tween.to.y, ratioPassed), easeInCirc(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeOutCirc:
							newVect = new Vector3(easeOutCirc(tween.from.x, tween.to.x, ratioPassed), easeOutCirc(tween.from.y, tween.to.y, ratioPassed), easeOutCirc(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInOutCirc:
							newVect = new Vector3(easeInOutCirc(tween.from.x, tween.to.x, ratioPassed), easeInOutCirc(tween.from.y, tween.to.y, ratioPassed), easeInOutCirc(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInBounce:
							newVect = new Vector3(easeInBounce(tween.from.x, tween.to.x, ratioPassed), easeInBounce(tween.from.y, tween.to.y, ratioPassed), easeInBounce(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeOutBounce:
							newVect = new Vector3(easeOutBounce(tween.from.x, tween.to.x, ratioPassed), easeOutBounce(tween.from.y, tween.to.y, ratioPassed), easeOutBounce(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInOutBounce:
							newVect = new Vector3(easeInOutBounce(tween.from.x, tween.to.x, ratioPassed), easeInOutBounce(tween.from.y, tween.to.y, ratioPassed), easeInOutBounce(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInBack:
							newVect = new Vector3(easeInBack(tween.from.x, tween.to.x, ratioPassed), easeInBack(tween.from.y, tween.to.y, ratioPassed), easeInBack(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeOutBack:
							newVect = new Vector3(easeOutBack(tween.from.x, tween.to.x, ratioPassed), easeOutBack(tween.from.y, tween.to.y, ratioPassed), easeOutBack(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInOutBack:
							newVect = new Vector3(easeInOutBack(tween.from.x, tween.to.x, ratioPassed), easeInOutBack(tween.from.y, tween.to.y, ratioPassed), easeInOutBack(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInElastic:
							newVect = new Vector3(easeInElastic(tween.from.x, tween.to.x, ratioPassed), easeInElastic(tween.from.y, tween.to.y, ratioPassed), easeInElastic(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeOutElastic:
							newVect = new Vector3(easeOutElastic(tween.from.x, tween.to.x, ratioPassed), easeOutElastic(tween.from.y, tween.to.y, ratioPassed), easeOutElastic(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.easeInOutElastic:
							newVect = new Vector3(easeInOutElastic(tween.from.x, tween.to.x, ratioPassed), easeInOutElastic(tween.from.y, tween.to.y, ratioPassed), easeInOutElastic(tween.from.z, tween.to.z, ratioPassed));
							break;
						case LeanTweenType.punch:
							tween.animationCurve = punch;
							tween.to.x = tween.from.x + tween.to.x;
							tween.to.y = tween.from.y + tween.to.y;
							tween.to.z = tween.from.z + tween.to.z;
							if (tweenAction == 20 || tweenAction == 21)
							{
								tween.to.x = closestRot(tween.from.x, tween.to.x);
								tween.to.y = closestRot(tween.from.y, tween.to.y);
								tween.to.z = closestRot(tween.from.z, tween.to.z);
							}
							newVect = tweenOnCurveVector(tween, ratioPassed);
							break;
						}
					}
					else
					{
						newVect.x = tween.from.x + tween.diff.x * ratioPassed;
						newVect.y = tween.from.y + tween.diff.y * ratioPassed;
						newVect.z = tween.from.z + tween.diff.z * ratioPassed;
					}
					if (tweenAction == 18)
					{
						trans.position = newVect;
					}
					else if (tweenAction == 19)
					{
						trans.localPosition = newVect;
					}
					else if (tweenAction == 20)
					{
						if (tween.hasPhysics)
						{
							trans.gameObject.rigidbody.MoveRotation(Quaternion.Euler(newVect));
						}
						else
						{
							trans.eulerAngles = newVect;
						}
					}
					else if (tweenAction == 21)
					{
						trans.localEulerAngles = newVect;
					}
					else if (tweenAction == 22)
					{
						trans.localScale = newVect;
					}
					else if (tweenAction == 24)
					{
						tween.ltRect.rect = new Rect(newVect.x, newVect.y, tween.ltRect.rect.width, tween.ltRect.rect.height);
					}
					else if (tweenAction == 25)
					{
						tween.ltRect.rect = new Rect(tween.ltRect.rect.x, tween.ltRect.rect.y, newVect.x, newVect.y);
					}
					else if (tweenAction == 26)
					{
						tween.ltRect.alpha = newVect.x;
					}
					else if (tweenAction == 27)
					{
						tween.ltRect.rotation = newVect.x;
					}
				}
				if (tween.optional != null)
				{
					object obj = optionalItems["onUpdate"];
					if (obj != null)
					{
						Hashtable arg = (Hashtable)optionalItems["onUpdateParam"];
						if (tweenAction == 23)
						{
							if (obj.GetType() == typeof(string))
							{
								string methodName = obj as string;
								customTarget = ((optionalItems["onUpdateTarget"] == null) ? trans.gameObject : (optionalItems["onUpdateTarget"] as GameObject));
								customTarget.BroadcastMessage(methodName, newVect);
							}
							else if (obj.GetType() == typeof(Action<Vector3, Hashtable>))
							{
								Action<Vector3, Hashtable> action = (Action<Vector3, Hashtable>)obj;
								action(newVect, arg);
							}
							else
							{
								Action<Vector3> action2 = (Action<Vector3>)obj;
								action2(newVect);
							}
						}
						else if (obj.GetType() == typeof(string))
						{
							string methodName2 = obj as string;
							if (optionalItems["onUpdateTarget"] != null)
							{
								customTarget = optionalItems["onUpdateTarget"] as GameObject;
								customTarget.BroadcastMessage(methodName2, val);
							}
							else
							{
								trans.gameObject.BroadcastMessage(methodName2, val);
							}
						}
						else if (obj.GetType() == typeof(Action<float, Hashtable>))
						{
							Action<float, Hashtable> action3 = (Action<float, Hashtable>)obj;
							action3(val, arg);
						}
						else
						{
							Action<float> action4 = (Action<float>)obj;
							action4(val);
						}
					}
				}
			}
			if (isTweenFinished)
			{
				Action action5 = null;
				if (tweenAction == 27)
				{
					tween.ltRect.rotateFinished = true;
				}
				if (tween.loopType == LeanTweenType.once || tween.loopCount == 1)
				{
					string text = string.Empty;
					object obj2 = null;
					if (tween.optional != null && (bool)tween.trans)
					{
						if (optionalItems["onComplete"] != null)
						{
							if (optionalItems["onComplete"].GetType() == typeof(string))
							{
								text = optionalItems["onComplete"] as string;
							}
							else
							{
								action5 = (Action)optionalItems["onComplete"];
							}
						}
						obj2 = optionalItems["onCompleteParam"];
					}
					removeTween(i);
					if (action5 != null)
					{
						action5();
					}
					else
					{
						if (!(text != string.Empty))
						{
							continue;
						}
						if (optionalItems["onCompleteTarget"] != null)
						{
							customTarget = optionalItems["onCompleteTarget"] as GameObject;
							if (obj2 != null)
							{
								customTarget.BroadcastMessage(text, obj2);
							}
							else
							{
								customTarget.BroadcastMessage(text);
							}
						}
						else if (obj2 != null)
						{
							trans.gameObject.BroadcastMessage(text, obj2);
						}
						else
						{
							trans.gameObject.BroadcastMessage(text);
						}
					}
				}
				else
				{
					if (tween.loopCount >= 1)
					{
						tween.loopCount--;
					}
					if (tween.loopType == LeanTweenType.clamp)
					{
						tween.passed = float.Epsilon;
					}
					else if (tween.loopType == LeanTweenType.pingPong)
					{
						tween.direction = 0f - tween.direction;
					}
				}
			}
			else if (tween.delay <= 0f)
			{
				tween.passed += dt * tween.direction;
			}
			else
			{
				tween.delay -= dt;
				if (tween.delay < 0f)
				{
					tween.passed = 0f;
					tween.delay = 0f;
				}
			}
		}
		frameRendered = Time.frameCount;
	}

	private static void removeTween(int i)
	{
		tweens[i].toggle = false;
		tweens[i].optional = null;
		startSearch = i;
		if (i + 1 >= tweenMaxSearch)
		{
			startSearch = 0;
			tweenMaxSearch--;
		}
	}

	private static int pushNewTween(GameObject gameObject, Vector3 to, float time, TweenAction tweenAction, Hashtable optional)
	{
		init(maxTweens);
		if (gameObject == null)
		{
			return -1;
		}
		j = 0;
		i = startSearch;
		while (j < maxTweens)
		{
			if (i >= maxTweens - 1)
			{
				i = 0;
			}
			if (!tweens[i].toggle)
			{
				if (i + 1 > tweenMaxSearch)
				{
					tweenMaxSearch = i + 1;
				}
				startSearch = i + 1;
				break;
			}
			j++;
			if (j >= maxTweens)
			{
				string message = "LeanTween - You have run out of available spaces for tweening. To avoid this error increase the number of spaces to available for tweening when you initialize the LeanTween class ex: LeanTween.init( " + maxTweens * 2 + " );";
				if (throwErrors)
				{
					Debug.LogError(message);
				}
				else
				{
					Debug.Log(message);
				}
				return -1;
			}
			i++;
		}
		tween = tweens[i];
		tween.toggle = true;
		tween.trans = gameObject.transform;
		tween.to = to;
		tween.time = time;
		tween.passed = 0f;
		tween.type = tweenAction;
		tween.optional = optional;
		tween.delay = 0f;
		tween.id = i;
		tween.useEstimatedTime = false;
		tween.useFrames = false;
		tween.animationCurve = null;
		tween.tweenType = LeanTweenType.linear;
		tween.loopType = LeanTweenType.once;
		tween.direction = 1f;
		tween.hasInitiliazed = false;
		tween.hasPhysics = gameObject.rigidbody != null;
		if (optional != null)
		{
			object obj = optional["ease"];
			int num = 0;
			if (obj != null)
			{
				tween.tweenType = LeanTweenType.notUsed;
				if (obj.GetType() == typeof(LeanTweenType))
				{
					tween.tweenType = (LeanTweenType)(int)obj;
				}
				else if (obj.GetType() == typeof(AnimationCurve))
				{
					tween.animationCurve = optional["ease"] as AnimationCurve;
				}
				else
				{
					tween.tweenFunc = optional["ease"].ToString();
					if (tween.tweenFunc.Equals("easeOutQuad"))
					{
						tween.tweenType = LeanTweenType.easeOutQuad;
					}
					else if (tween.tweenFunc.Equals("easeInQuad"))
					{
						tween.tweenType = LeanTweenType.easeInQuad;
					}
					else if (tween.tweenFunc.Equals("easeInOutQuad"))
					{
						tween.tweenType = LeanTweenType.easeInOutQuad;
					}
				}
				num++;
			}
			if (optional["rect"] != null)
			{
				tween.ltRect = (LTRect)optional["rect"];
				num++;
			}
			if (optional["path"] != null)
			{
				tween.path = (LTBezierPath)optional["path"];
				num++;
			}
			if (optional["delay"] != null)
			{
				tween.delay = (float)optional["delay"];
				num++;
			}
			if (optional["useEstimatedTime"] != null)
			{
				tween.useEstimatedTime = (bool)optional["useEstimatedTime"];
				num++;
			}
			if (optional["useFrames"] != null)
			{
				tween.useFrames = (bool)optional["useFrames"];
				num++;
			}
			if (optional["loopType"] != null)
			{
				tween.loopType = (LeanTweenType)(int)optional["loopType"];
				num++;
			}
			if (optional["repeat"] != null)
			{
				tween.loopCount = (int)optional["repeat"];
				if (tween.loopType == LeanTweenType.once)
				{
					tween.loopType = LeanTweenType.clamp;
				}
				num++;
			}
			if (optional.Count <= num)
			{
				tween.optional = null;
			}
		}
		else
		{
			tween.optional = null;
		}
		return tweens[i].id;
	}

	public static Vector3[] add(Vector3[] a, Vector3 b)
	{
		Vector3[] array = new Vector3[a.Length];
		for (i = 0; i < a.Length; i++)
		{
			array[i] = a[i] + b;
		}
		return array;
	}

	public static Hashtable h(object[] arr)
	{
		if (arr.Length % 2 == 1)
		{
			string message = "LeanTween - You have attempted to create a Hashtable with an odd number of values.";
			if (throwErrors)
			{
				Debug.LogError(message);
			}
			else
			{
				Debug.Log(message);
			}
			return null;
		}
		Hashtable hashtable = new Hashtable();
		for (i = 0; i < arr.Length; i += 2)
		{
			hashtable.Add(arr[i] as string, arr[i + 1]);
		}
		return hashtable;
	}

	public static float closestRot(float from, float to)
	{
		float num = 0f - (360f - to);
		float num2 = 360f + to;
		float num3 = Mathf.Abs(to - from);
		float num4 = Mathf.Abs(num - from);
		float num5 = Mathf.Abs(num2 - from);
		if (num3 < num4 && num3 < num5)
		{
			return to;
		}
		if (num4 < num5)
		{
			return num;
		}
		return num2;
	}

	public static void cancel(GameObject gameObject)
	{
		Transform transform = gameObject.transform;
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].trans == transform)
			{
				removeTween(i);
			}
		}
	}

	public static void cancel(GameObject gameObject, int id)
	{
		Transform transform = gameObject.transform;
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].trans == transform && tweens[i].id == id)
			{
				removeTween(i);
			}
		}
	}

	public static void cancel(LTRect ltRect, int id)
	{
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].id == id && tweens[i].ltRect == ltRect)
			{
				removeTween(i);
			}
		}
	}

	public static LeanTweenDescr description(int id)
	{
		if (tweens[id] != null && tweens[id].id == id)
		{
			return tweens[id];
		}
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].id == id)
			{
				return tweens[i];
			}
		}
		return null;
	}

	public static void pause(GameObject gameObject, int id)
	{
		Transform transform = gameObject.transform;
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].trans == transform && tweens[i].id == id)
			{
				if (tweens[i].optional == null || tweens[i].optional.Count == 0)
				{
					tweens[i].optional = new Hashtable();
				}
				tweens[i].optional["directionSaved"] = tweens[i].direction;
				tweens[i].direction = 0f;
			}
		}
	}

	public static void pause(GameObject gameObject)
	{
		Transform transform = gameObject.transform;
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].trans == transform)
			{
				if (tweens[i].optional == null || tweens[i].optional.Count == 0)
				{
					tweens[i].optional = new Hashtable();
				}
				tweens[i].optional["directionSaved"] = tweens[i].direction;
				tweens[i].direction = 0f;
			}
		}
	}

	public static void resume(GameObject gameObject, int id)
	{
		Transform transform = gameObject.transform;
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].trans == transform && tweens[i].id == id)
			{
				tweens[i].direction = (float)tweens[i].optional["directionSaved"];
			}
		}
	}

	public static void resume(GameObject gameObject)
	{
		Transform transform = gameObject.transform;
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].trans == transform)
			{
				tweens[i].direction = (float)tweens[i].optional["directionSaved"];
			}
		}
	}

	public static bool isTweening(GameObject gameObject)
	{
		Transform transform = gameObject.transform;
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].toggle && tweens[i].trans == transform)
			{
				return true;
			}
		}
		return false;
	}

	public static bool isTweening(LTRect ltRect)
	{
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].toggle && tweens[i].ltRect == ltRect)
			{
				return true;
			}
		}
		return false;
	}

	public static void drawBezierPath(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		Vector3 vector = a;
		Vector3 vector2 = -a + 3f * (b - c) + d;
		Vector3 vector3 = 3f * (a + c) - 6f * b;
		Vector3 vector4 = 3f * (b - a);
		for (float num = 1f; num <= 30f; num += 1f)
		{
			float num2 = num / 30f;
			Vector3 vector5 = ((vector2 * num2 + vector3) * num2 + vector4) * num2 + a;
			Gizmos.DrawLine(vector, vector5);
			vector = vector5;
		}
	}

	public static int value(string callOnUpdate, float from, float to, float time, Hashtable optional)
	{
		return value(tweenEmpty, callOnUpdate, from, to, time, optional);
	}

	public static int value(GameObject gameObject, string callOnUpdate, float from, float to, float time)
	{
		return value(gameObject, callOnUpdate, from, to, time, new Hashtable());
	}

	public static int value(GameObject gameObject, string callOnUpdate, float from, float to, float time, object[] optional)
	{
		return value(gameObject, callOnUpdate, from, to, time, h(optional));
	}

	public static int value(GameObject gameObject, Action<float> callOnUpdate, float from, float to, float time)
	{
		return value(gameObject, callOnUpdate, from, to, time, new Hashtable());
	}

	public static int value(GameObject gameObject, Action<float> callOnUpdate, float from, float to, float time, object[] optional)
	{
		return value(gameObject, callOnUpdate, from, to, time, h(optional));
	}

	public static int value(GameObject gameObject, Action<float, Hashtable> callOnUpdate, float from, float to, float time, object[] optional)
	{
		return value(gameObject, callOnUpdate, from, to, time, h(optional));
	}

	public static int value(GameObject gameObject, string callOnUpdate, float from, float to, float time, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["onUpdate"] = callOnUpdate;
		int num = pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CALLBACK, optional);
		tweens[num].from = new Vector3(from, 0f, 0f);
		return num;
	}

	public static int value(GameObject gameObject, Action<float> callOnUpdate, float from, float to, float time, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["onUpdate"] = callOnUpdate;
		int num = pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CALLBACK, optional);
		tweens[num].from = new Vector3(from, 0f, 0f);
		return num;
	}

	public static int value(GameObject gameObject, Action<float, Hashtable> callOnUpdate, float from, float to, float time, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["onUpdate"] = callOnUpdate;
		int num = pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CALLBACK, optional);
		tweens[num].from = new Vector3(from, 0f, 0f);
		return num;
	}

	public static int value(GameObject gameObject, string callOnUpdate, Vector3 from, Vector3 to, float time, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["onUpdate"] = callOnUpdate;
		int num = pushNewTween(gameObject, to, time, TweenAction.VALUE3, optional);
		tweens[num].from = from;
		return num;
	}

	public static int value(GameObject gameObject, string callOnUpdate, Vector3 from, Vector3 to, float time, object[] optional)
	{
		return value(gameObject, callOnUpdate, from, to, time, h(optional));
	}

	public static int value(GameObject gameObject, Action<Vector3> callOnUpdate, Vector3 from, Vector3 to, float time, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["onUpdate"] = callOnUpdate;
		int num = pushNewTween(gameObject, to, time, TweenAction.VALUE3, optional);
		tweens[num].from = from;
		return num;
	}

	public static int value(GameObject gameObject, Action<Vector3, Hashtable> callOnUpdate, Vector3 from, Vector3 to, float time, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["onUpdate"] = callOnUpdate;
		int num = pushNewTween(gameObject, to, time, TweenAction.VALUE3, optional);
		tweens[num].from = from;
		return num;
	}

	public static int value(GameObject gameObject, Action<Vector3> callOnUpdate, Vector3 from, Vector3 to, float time, object[] optional)
	{
		return value(gameObject, callOnUpdate, from, to, time, h(optional));
	}

	public static int value(GameObject gameObject, Action<Vector3, Hashtable> callOnUpdate, Vector3 from, Vector3 to, float time, object[] optional)
	{
		return value(gameObject, callOnUpdate, from, to, time, h(optional));
	}

	public static int rotate(GameObject gameObject, Vector3 to, float time)
	{
		return rotate(gameObject, to, time, emptyHash);
	}

	public static int rotate(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.ROTATE, optional);
	}

	public static int rotate(GameObject gameObject, Vector3 to, float time, object[] optional)
	{
		return rotate(gameObject, to, time, h(optional));
	}

	public static int rotate(LTRect ltRect, float to, float time, Hashtable optional)
	{
		init();
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["rect"] = ltRect;
		return pushNewTween(tweenEmpty, new Vector3(to, 0f, 0f), time, TweenAction.GUI_ROTATE, optional);
	}

	public static int rotate(LTRect ltRect, float to, float time, object[] optional)
	{
		return rotate(ltRect, to, time, h(optional));
	}

	public static int rotateX(GameObject gameObject, float to, float time)
	{
		return rotateX(gameObject, to, time, emptyHash);
	}

	public static int rotateX(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_X, optional);
	}

	public static int rotateX(GameObject gameObject, float to, float time, object[] optional)
	{
		return rotateX(gameObject, to, time, h(optional));
	}

	public static int rotateY(GameObject gameObject, float to, float time)
	{
		return rotateY(gameObject, to, time, emptyHash);
	}

	public static int rotateY(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_Y, optional);
	}

	public static int rotateY(GameObject gameObject, float to, float time, object[] optional)
	{
		return rotateY(gameObject, to, time, h(optional));
	}

	public static int rotateZ(GameObject gameObject, float to, float time)
	{
		return rotateZ(gameObject, to, time, emptyHash);
	}

	public static int rotateZ(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_Z, optional);
	}

	public static int rotateZ(GameObject gameObject, float to, float time, object[] optional)
	{
		return rotateZ(gameObject, to, time, h(optional));
	}

	public static int rotateLocal(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.ROTATE_LOCAL, optional);
	}

	public static int rotateLocal(GameObject gameObject, Vector3 to, float time, object[] optional)
	{
		return rotateLocal(gameObject, to, time, h(optional));
	}

	public static int rotateAround(GameObject gameObject, Vector3 axis, float add, float time, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["axis"] = axis;
		if (optional["point"] == null)
		{
			optional["point"] = Vector3.zero;
		}
		return pushNewTween(gameObject, new Vector3(add, 0f, 0f), time, TweenAction.ROTATE_AROUND, optional);
	}

	public static int rotateAround(GameObject gameObject, Vector3 axis, float add, float time, object[] optional)
	{
		return rotateAround(gameObject, axis, add, time, h(optional));
	}

	public static int moveX(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_X, optional);
	}

	public static int moveX(GameObject gameObject, float to, float time, object[] optional)
	{
		return moveX(gameObject, to, time, h(optional));
	}

	public static int moveX(GameObject gameObject, float to, float time)
	{
		return moveX(gameObject, to, time, emptyHash);
	}

	public static int moveY(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_Y, optional);
	}

	public static int moveY(GameObject gameObject, float to, float time, object[] optional)
	{
		return moveY(gameObject, to, time, h(optional));
	}

	public static int moveY(GameObject gameObject, float to, float time)
	{
		return moveY(gameObject, to, time, emptyHash);
	}

	public static int moveZ(GameObject gameObject, float to, float time)
	{
		return moveZ(gameObject, to, time, emptyHash);
	}

	public static int moveZ(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_Z, optional);
	}

	public static int moveZ(GameObject gameObject, float to, float time, object[] optional)
	{
		return moveZ(gameObject, to, time, h(optional));
	}

	public static int move(GameObject gameObject, Vector3 to, float time)
	{
		return move(gameObject, to, time, emptyHash);
	}

	public static int move(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.MOVE, optional);
	}

	public static int move(GameObject gameObject, Vector3 to, float time, object[] optional)
	{
		return move(gameObject, to, time, h(optional));
	}

	public static int move(GameObject gameObject, Vector3[] to, float time, Hashtable optional)
	{
		if (to.Length < 4)
		{
			string message = "LeanTween - When passing values for a vector path, you must pass four or more values!";
			if (throwErrors)
			{
				Debug.LogError(message);
			}
			else
			{
				Debug.Log(message);
			}
			return -1;
		}
		if (to.Length % 4 != 0)
		{
			string message2 = "LeanTween - When passing values for a vector path, they must be in sets of four: controlPoint1, controlPoint2, endPoint2, controlPoint2, controlPoint2...";
			if (throwErrors)
			{
				Debug.LogError(message2);
			}
			else
			{
				Debug.Log(message2);
			}
			return -1;
		}
		init();
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		LTBezierPath lTBezierPath = new LTBezierPath(to);
		if (optional["orientToPath"] != null)
		{
			lTBezierPath.orientToPath = true;
		}
		optional["path"] = lTBezierPath;
		return pushNewTween(gameObject, new Vector3(1f, 0f, 0f), time, TweenAction.MOVE_CURVED, optional);
	}

	public static int move(GameObject gameObject, Vector3[] to, float time, object[] optional)
	{
		return move(gameObject, to, time, h(optional));
	}

	public static int move(LTRect ltRect, Vector2 to, float time, Hashtable optional)
	{
		init();
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["rect"] = ltRect;
		return pushNewTween(tweenEmpty, to, time, TweenAction.GUI_MOVE, optional);
	}

	public static int move(LTRect ltRect, Vector3 to, float time, object[] optional)
	{
		return move(ltRect, to, time, h(optional));
	}

	public static int move(LTRect ltRect, Vector2 to, float time)
	{
		return move(ltRect, to, time, null);
	}

	public static int moveLocal(GameObject gameObject, Vector3 to, float time)
	{
		return moveLocal(gameObject, to, time, emptyHash);
	}

	public static int moveLocal(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.MOVE_LOCAL, optional);
	}

	public static int moveLocal(GameObject gameObject, Vector3 to, float time, object[] optional)
	{
		return moveLocal(gameObject, to, time, h(optional));
	}

	public static int moveLocal(GameObject gameObject, Vector3[] to, float time, Hashtable optional)
	{
		if (to.Length < 4)
		{
			string message = "LeanTween - When passing values for a vector path, you must pass four or more values!";
			if (throwErrors)
			{
				Debug.LogError(message);
			}
			else
			{
				Debug.Log(message);
			}
			return -1;
		}
		if (to.Length % 4 != 0)
		{
			string message2 = "LeanTween - When passing values for a vector path, they must be in sets of four: controlPoint1, controlPoint2, endPoint2, controlPoint2, controlPoint2...";
			if (throwErrors)
			{
				Debug.LogError(message2);
			}
			else
			{
				Debug.Log(message2);
			}
			return -1;
		}
		init();
		if (optional == null)
		{
			optional = new Hashtable();
		}
		LTBezierPath lTBezierPath = new LTBezierPath(to);
		if (optional["orientToPath"] != null)
		{
			lTBezierPath.orientToPath = true;
		}
		optional["path"] = lTBezierPath;
		return pushNewTween(gameObject, new Vector3(1f, 0f, 0f), time, TweenAction.MOVE_CURVED_LOCAL, optional);
	}

	public static int moveLocal(GameObject gameObject, Vector3[] to, float time, object[] optional)
	{
		return moveLocal(gameObject, to, time, h(optional));
	}

	public static int moveLocalX(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_X, optional);
	}

	public static int moveLocalX(GameObject gameObject, float to, float time, object[] optional)
	{
		return moveLocalX(gameObject, to, time, h(optional));
	}

	public static int moveLocalY(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_Y, optional);
	}

	public static int moveLocalY(GameObject gameObject, float to, float time, object[] optional)
	{
		return moveLocalY(gameObject, to, time, h(optional));
	}

	public static int moveLocalZ(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_Z, optional);
	}

	public static int moveLocalZ(GameObject gameObject, float to, float time, object[] optional)
	{
		return moveLocalZ(gameObject, to, time, h(optional));
	}

	public static int scale(GameObject gameObject, Vector3 to, float time)
	{
		return scale(gameObject, to, time, emptyHash);
	}

	public static int scale(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.SCALE, optional);
	}

	public static int scale(GameObject gameObject, Vector3 to, float time, object[] optional)
	{
		return scale(gameObject, to, time, h(optional));
	}

	public static int scale(LTRect ltRect, Vector2 to, float time, Hashtable optional)
	{
		init();
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["rect"] = ltRect;
		return pushNewTween(tweenEmpty, to, time, TweenAction.GUI_SCALE, optional);
	}

	public static int scale(LTRect ltRect, Vector2 to, float time, object[] optional)
	{
		return scale(ltRect, to, time, h(optional));
	}

	public static int scale(LTRect ltRect, Vector2 to, float time)
	{
		return scale(ltRect, to, time, emptyHash);
	}

	public static int alpha(LTRect ltRect, float to, float time, Hashtable optional)
	{
		init();
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		ltRect.alphaEnabled = true;
		optional["rect"] = ltRect;
		return pushNewTween(tweenEmpty, new Vector3(to, 0f, 0f), time, TweenAction.GUI_ALPHA, optional);
	}

	public static int alpha(LTRect ltRect, float to, float time, object[] optional)
	{
		return alpha(ltRect, to, time, h(optional));
	}

	public static int scaleX(GameObject gameObject, float to, float time)
	{
		return scaleX(gameObject, to, time, emptyHash);
	}

	public static int scaleX(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_X, optional);
	}

	public static int scaleX(GameObject gameObject, float to, float time, object[] optional)
	{
		return scaleX(gameObject, to, time, h(optional));
	}

	public static int scaleY(GameObject gameObject, float to, float time)
	{
		return scaleY(gameObject, to, time, emptyHash);
	}

	public static int scaleY(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_Y, optional);
	}

	public static int scaleY(GameObject gameObject, float to, float time, object[] optional)
	{
		return scaleY(gameObject, to, time, h(optional));
	}

	public static int scaleZ(GameObject gameObject, float to, float time)
	{
		return scaleZ(gameObject, to, time, emptyHash);
	}

	public static int scaleZ(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_Z, optional);
	}

	public static int scaleZ(GameObject gameObject, float to, float time, object[] optional)
	{
		return scaleZ(gameObject, to, time, h(optional));
	}

	public static int delayedCall(float delayTime, string callback)
	{
		return delayedCall(tweenEmpty, delayTime, callback, new Hashtable());
	}

	public static int delayedCall(float delayTime, Action callback)
	{
		return delayedCall(tweenEmpty, delayTime, callback);
	}

	public static int delayedCall(float delayTime, string callback, Hashtable optional)
	{
		return delayedCall(tweenEmpty, delayTime, callback, optional);
	}

	public static int delayedCall(float delayTime, Action callback, object[] optional)
	{
		return delayedCall(tweenEmpty, delayTime, callback, h(optional));
	}

	public static int delayedCall(GameObject gameObject, float delayTime, string callback, object[] optional)
	{
		return delayedCall(gameObject, delayTime, callback, h(optional));
	}

	public static int delayedCall(GameObject gameObject, float delayTime, Action callback, object[] optional)
	{
		return delayedCall(gameObject, delayTime, callback, h(optional));
	}

	public static int delayedCall(GameObject gameObject, float delayTime, string callback)
	{
		return delayedCall(gameObject, delayTime, callback, new Hashtable());
	}

	public static int delayedCall(GameObject gameObject, float delayTime, string callback, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["onComplete"] = callback;
		return pushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, optional);
	}

	public static int delayedCall(GameObject gameObject, float delayTime, Action callback, Hashtable optional)
	{
		if (optional == null)
		{
			optional = new Hashtable();
		}
		optional["onComplete"] = callback;
		return pushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, optional);
	}

	public static int delayedCall(GameObject gameObject, float delayTime, Action callback)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["onComplete"] = callback;
		Debug.Log("callback:" + callback);
		return pushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, hashtable);
	}

	public static int alpha(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ALPHA, optional);
	}

	public static int alpha(GameObject gameObject, float to, float time, object[] optional)
	{
		return alpha(gameObject, to, time, h(optional));
	}

	public static int alpha(GameObject gameObject, float to, float time)
	{
		return alpha(gameObject, to, time, emptyHash);
	}

	private static float tweenOnCurve(LeanTweenDescr tweenDescr, float ratioPassed)
	{
		return tweenDescr.from.x + (tweenDescr.to.x - tweenDescr.from.x) * tweenDescr.animationCurve.Evaluate(ratioPassed);
	}

	private static Vector3 tweenOnCurveVector(LeanTweenDescr tweenDescr, float ratioPassed)
	{
		return new Vector3(tweenDescr.from.x + (tweenDescr.to.x - tweenDescr.from.x) * tweenDescr.animationCurve.Evaluate(ratioPassed), tweenDescr.from.y + (tweenDescr.to.y - tweenDescr.from.y) * tweenDescr.animationCurve.Evaluate(ratioPassed), tweenDescr.from.z + (tweenDescr.to.z - tweenDescr.from.z) * tweenDescr.animationCurve.Evaluate(ratioPassed));
	}

	public static float easeOutQuadOpt(float start, float diff, float ratioPassed)
	{
		return (0f - diff) * ratioPassed * (ratioPassed - 2f) + start;
	}

	public static float easeInQuadOpt(float start, float diff, float ratioPassed)
	{
		return diff * ratioPassed * ratioPassed + start;
	}

	public static float easeInOutQuadOpt(float start, float diff, float ratioPassed)
	{
		ratioPassed /= 0.5f;
		if (ratioPassed < 1f)
		{
			return diff / 2f * ratioPassed * ratioPassed + start;
		}
		ratioPassed -= 1f;
		return (0f - diff) / 2f * (ratioPassed * (ratioPassed - 2f) - 1f) + start;
	}

	public static float linear(float start, float end, float val)
	{
		return Mathf.Lerp(start, end, val);
	}

	public static float clerp(float start, float end, float val)
	{
		float num = 0f;
		float num2 = 360f;
		float num3 = Mathf.Abs((num2 - num) / 2f);
		float num4 = 0f;
		float num5 = 0f;
		if (end - start < 0f - num3)
		{
			num5 = (num2 - start + end) * val;
			return start + num5;
		}
		if (end - start > num3)
		{
			num5 = (0f - (num2 - end + start)) * val;
			return start + num5;
		}
		return start + (end - start) * val;
	}

	public static float spring(float start, float end, float val)
	{
		val = Mathf.Clamp01(val);
		val = (Mathf.Sin(val * (float)Math.PI * (0.2f + 2.5f * val * val * val)) * Mathf.Pow(1f - val, 2.2f) + val) * (1f + 1.2f * (1f - val));
		return start + (end - start) * val;
	}

	public static float easeInQuad(float start, float end, float val)
	{
		end -= start;
		return end * val * val + start;
	}

	public static float easeOutQuad(float start, float end, float val)
	{
		end -= start;
		return (0f - end) * val * (val - 2f) + start;
	}

	public static float easeInOutQuad(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		if (val < 1f)
		{
			return end / 2f * val * val + start;
		}
		val -= 1f;
		return (0f - end) / 2f * (val * (val - 2f) - 1f) + start;
	}

	public static float easeInCubic(float start, float end, float val)
	{
		end -= start;
		return end * val * val * val + start;
	}

	public static float easeOutCubic(float start, float end, float val)
	{
		val -= 1f;
		end -= start;
		return end * (val * val * val + 1f) + start;
	}

	public static float easeInOutCubic(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		if (val < 1f)
		{
			return end / 2f * val * val * val + start;
		}
		val -= 2f;
		return end / 2f * (val * val * val + 2f) + start;
	}

	public static float easeInQuart(float start, float end, float val)
	{
		end -= start;
		return end * val * val * val * val + start;
	}

	public static float easeOutQuart(float start, float end, float val)
	{
		val -= 1f;
		end -= start;
		return (0f - end) * (val * val * val * val - 1f) + start;
	}

	public static float easeInOutQuart(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		if (val < 1f)
		{
			return end / 2f * val * val * val * val + start;
		}
		val -= 2f;
		return (0f - end) / 2f * (val * val * val * val - 2f) + start;
	}

	public static float easeInQuint(float start, float end, float val)
	{
		end -= start;
		return end * val * val * val * val * val + start;
	}

	public static float easeOutQuint(float start, float end, float val)
	{
		val -= 1f;
		end -= start;
		return end * (val * val * val * val * val + 1f) + start;
	}

	public static float easeInOutQuint(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		if (val < 1f)
		{
			return end / 2f * val * val * val * val * val + start;
		}
		val -= 2f;
		return end / 2f * (val * val * val * val * val + 2f) + start;
	}

	public static float easeInSine(float start, float end, float val)
	{
		end -= start;
		return (0f - end) * Mathf.Cos(val / 1f * ((float)Math.PI / 2f)) + end + start;
	}

	public static float easeOutSine(float start, float end, float val)
	{
		end -= start;
		return end * Mathf.Sin(val / 1f * ((float)Math.PI / 2f)) + start;
	}

	public static float easeInOutSine(float start, float end, float val)
	{
		end -= start;
		return (0f - end) / 2f * (Mathf.Cos((float)Math.PI * val / 1f) - 1f) + start;
	}

	public static float easeInExpo(float start, float end, float val)
	{
		end -= start;
		return end * Mathf.Pow(2f, 10f * (val / 1f - 1f)) + start;
	}

	public static float easeOutExpo(float start, float end, float val)
	{
		end -= start;
		return end * (0f - Mathf.Pow(2f, -10f * val / 1f) + 1f) + start;
	}

	public static float easeInOutExpo(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		if (val < 1f)
		{
			return end / 2f * Mathf.Pow(2f, 10f * (val - 1f)) + start;
		}
		val -= 1f;
		return end / 2f * (0f - Mathf.Pow(2f, -10f * val) + 2f) + start;
	}

	public static float easeInCirc(float start, float end, float val)
	{
		end -= start;
		return (0f - end) * (Mathf.Sqrt(1f - val * val) - 1f) + start;
	}

	public static float easeOutCirc(float start, float end, float val)
	{
		val -= 1f;
		end -= start;
		return end * Mathf.Sqrt(1f - val * val) + start;
	}

	public static float easeInOutCirc(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		if (val < 1f)
		{
			return (0f - end) / 2f * (Mathf.Sqrt(1f - val * val) - 1f) + start;
		}
		val -= 2f;
		return end / 2f * (Mathf.Sqrt(1f - val * val) + 1f) + start;
	}

	public static float easeInBounce(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		return end - easeOutBounce(0f, end, num - val) + start;
	}

	public static float easeOutBounce(float start, float end, float val)
	{
		val /= 1f;
		end -= start;
		if (val < 0.36363637f)
		{
			return end * (7.5625f * val * val) + start;
		}
		if (val < 0.72727275f)
		{
			val -= 0.54545456f;
			return end * (7.5625f * val * val + 0.75f) + start;
		}
		if ((double)val < 0.9090909090909091)
		{
			val -= 0.8181818f;
			return end * (7.5625f * val * val + 0.9375f) + start;
		}
		val -= 21f / 22f;
		return end * (7.5625f * val * val + 63f / 64f) + start;
	}

	public static float easeInOutBounce(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		if (val < num / 2f)
		{
			return easeInBounce(0f, end, val * 2f) * 0.5f + start;
		}
		return easeOutBounce(0f, end, val * 2f - num) * 0.5f + end * 0.5f + start;
	}

	public static float easeInBack(float start, float end, float val)
	{
		end -= start;
		val /= 1f;
		float num = 1.70158f;
		return end * val * val * ((num + 1f) * val - num) + start;
	}

	public static float easeOutBack(float start, float end, float val)
	{
		float num = 1.70158f;
		end -= start;
		val = val / 1f - 1f;
		return end * (val * val * ((num + 1f) * val + num) + 1f) + start;
	}

	public static float easeInOutBack(float start, float end, float val)
	{
		float num = 1.70158f;
		end -= start;
		val /= 0.5f;
		if (val < 1f)
		{
			num *= 1.525f;
			return end / 2f * (val * val * ((num + 1f) * val - num)) + start;
		}
		val -= 2f;
		num *= 1.525f;
		return end / 2f * (val * val * ((num + 1f) * val + num) + 2f) + start;
	}

	public static float easeInElastic(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		float num2 = num * 0.3f;
		float num3 = 0f;
		float num4 = 0f;
		if (val == 0f)
		{
			return start;
		}
		val /= num;
		if (val == 1f)
		{
			return start + end;
		}
		if (num4 == 0f || num4 < Mathf.Abs(end))
		{
			num4 = end;
			num3 = num2 / 4f;
		}
		else
		{
			num3 = num2 / ((float)Math.PI * 2f) * Mathf.Asin(end / num4);
		}
		val -= 1f;
		return 0f - num4 * Mathf.Pow(2f, 10f * val) * Mathf.Sin((val * num - num3) * ((float)Math.PI * 2f) / num2) + start;
	}

	public static float easeOutElastic(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		float num2 = num * 0.3f;
		float num3 = 0f;
		float num4 = 0f;
		if (val == 0f)
		{
			return start;
		}
		val /= num;
		if (val == 1f)
		{
			return start + end;
		}
		if (num4 == 0f || num4 < Mathf.Abs(end))
		{
			num4 = end;
			num3 = num2 / 4f;
		}
		else
		{
			num3 = num2 / ((float)Math.PI * 2f) * Mathf.Asin(end / num4);
		}
		return num4 * Mathf.Pow(2f, -10f * val) * Mathf.Sin((val * num - num3) * ((float)Math.PI * 2f) / num2) + end + start;
	}

	public static float easeInOutElastic(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		float num2 = num * 0.3f;
		float num3 = 0f;
		float num4 = 0f;
		if (val == 0f)
		{
			return start;
		}
		val /= num / 2f;
		if (val == 2f)
		{
			return start + end;
		}
		if (num4 == 0f || num4 < Mathf.Abs(end))
		{
			num4 = end;
			num3 = num2 / 4f;
		}
		else
		{
			num3 = num2 / ((float)Math.PI * 2f) * Mathf.Asin(end / num4);
		}
		if (val < 1f)
		{
			val -= 1f;
			return -0.5f * (num4 * Mathf.Pow(2f, 10f * val) * Mathf.Sin((val * num - num3) * ((float)Math.PI * 2f) / num2)) + start;
		}
		val -= 1f;
		return num4 * Mathf.Pow(2f, -10f * val) * Mathf.Sin((val * num - num3) * ((float)Math.PI * 2f) / num2) * 0.5f + end + start;
	}
}
