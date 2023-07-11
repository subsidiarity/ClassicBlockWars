using System.Collections;
using UnityEngine;

public class LeanTweenDescr
{
	public bool toggle;

	public Transform trans;

	public LTRect ltRect;

	public Vector3 from;

	public Vector3 to;

	public Vector3 diff;

	public LTBezierPath path;

	public float time;

	public bool useEstimatedTime;

	public bool useFrames;

	public bool hasInitiliazed;

	public bool hasPhysics;

	public float passed;

	public TweenAction type;

	public Hashtable optional;

	public float delay;

	public string tweenFunc;

	public LeanTweenType tweenType;

	public AnimationCurve animationCurve;

	public int id;

	public LeanTweenType loopType;

	public int loopCount;

	public float direction;

	public string TweenToString()
	{
		return string.Concat("gameObject:", trans.gameObject, " toggle:", toggle, " passed:", passed, " time:", time, " delay:", delay, " from:", from, " to:", to, " type:", type, " useEstimatedTime:", useEstimatedTime, " id:", id);
	}
}
