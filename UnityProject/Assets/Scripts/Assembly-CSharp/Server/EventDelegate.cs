using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class EventDelegate
{
	[Serializable]
	public class Parameter
	{
		public UnityEngine.Object obj;

		public string field;

		[NonSerialized]
		public bool cached;

		[NonSerialized]
		public Type expectedType = typeof(void);

		[NonSerialized]
		public PropertyInfo propInfo;

		[NonSerialized]
		public FieldInfo fieldInfo;

		public object value
		{
			get
			{
				if (!cached)
				{
					cached = true;
					fieldInfo = null;
					propInfo = null;
					if (obj != null && !string.IsNullOrEmpty(field))
					{
						Type type = obj.GetType();
						propInfo = type.GetProperty(field);
						if (propInfo == null)
						{
							fieldInfo = type.GetField(field);
						}
					}
				}
				if (propInfo != null)
				{
					return propInfo.GetValue(obj, null);
				}
				if (fieldInfo != null)
				{
					return fieldInfo.GetValue(obj);
				}
				return obj;
			}
		}

		public Type type
		{
			get
			{
				if (obj == null)
				{
					return typeof(void);
				}
				return obj.GetType();
			}
		}
	}

	public delegate void Callback();

	[SerializeField]
	private MonoBehaviour mTarget;

	[SerializeField]
	private string mMethodName;

	[SerializeField]
	private Parameter[] mParameters;

	public bool oneShot;

	private Callback mCachedCallback;

	private bool mRawDelegate;

	private bool mCached;

	private MethodInfo mMethod;

	private object[] mArgs;

	private static int s_Hash = "EventDelegate".GetHashCode();

	public MonoBehaviour target
	{
		get
		{
			return mTarget;
		}
		set
		{
			mTarget = value;
			mCachedCallback = null;
			mRawDelegate = false;
			mCached = false;
			mMethod = null;
			mParameters = null;
		}
	}

	public string methodName
	{
		get
		{
			return mMethodName;
		}
		set
		{
			mMethodName = value;
			mCachedCallback = null;
			mRawDelegate = false;
			mCached = false;
			mMethod = null;
			mParameters = null;
		}
	}

	public Parameter[] parameters
	{
		get
		{
			if (!mCached)
			{
				Cache();
			}
			return mParameters;
		}
	}

	public bool isValid
	{
		get
		{
			if (!mCached)
			{
				Cache();
			}
			return (mRawDelegate && mCachedCallback != null) || (mTarget != null && !string.IsNullOrEmpty(mMethodName));
		}
	}

	public bool isEnabled
	{
		get
		{
			if (!mCached)
			{
				Cache();
			}
			if (mRawDelegate && mCachedCallback != null)
			{
				return true;
			}
			if (mTarget == null)
			{
				return false;
			}
			MonoBehaviour monoBehaviour = mTarget;
			return monoBehaviour == null || monoBehaviour.enabled;
		}
	}

	public EventDelegate()
	{
	}

	public EventDelegate(Callback call)
	{
		Set(call);
	}

	public EventDelegate(MonoBehaviour target, string methodName)
	{
		Set(target, methodName);
	}

	private static string GetMethodName(Callback callback)
	{
		return callback.Method.Name;
	}

	private static bool IsValid(Callback callback)
	{
		return callback != null && callback.Method != null;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return !isValid;
		}
		if (obj is Callback)
		{
			Callback callback = obj as Callback;
			if (callback.Equals(mCachedCallback))
			{
				return true;
			}
			MonoBehaviour monoBehaviour = callback.Target as MonoBehaviour;
			return mTarget == monoBehaviour && string.Equals(mMethodName, GetMethodName(callback));
		}
		if (obj is EventDelegate)
		{
			EventDelegate eventDelegate = obj as EventDelegate;
			return mTarget == eventDelegate.mTarget && string.Equals(mMethodName, eventDelegate.mMethodName);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return s_Hash;
	}

	private void Set(Callback call)
	{
		Clear();
		if (call != null && IsValid(call))
		{
			mTarget = call.Target as MonoBehaviour;
			if (mTarget == null)
			{
				mRawDelegate = true;
				mCachedCallback = call;
				mMethodName = null;
			}
			else
			{
				mMethodName = GetMethodName(call);
				mRawDelegate = false;
			}
		}
	}

	public void Set(MonoBehaviour target, string methodName)
	{
		Clear();
		mTarget = target;
		mMethodName = methodName;
	}

	private void Cache()
	{
		mCached = true;
		if (mRawDelegate || (mCachedCallback != null && !(mCachedCallback.Target as MonoBehaviour != mTarget) && !(GetMethodName(mCachedCallback) != mMethodName)) || !(mTarget != null) || string.IsNullOrEmpty(mMethodName))
		{
			return;
		}
		mMethod = mTarget.GetType().GetMethod(mMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (mMethod == null)
		{
			Debug.LogError("Could not find method '" + mMethodName + "' on " + mTarget.GetType(), mTarget);
			return;
		}
		if (mMethod.ReturnType != typeof(void))
		{
			Debug.LogError(string.Concat(mTarget.GetType(), ".", mMethodName, " must have a 'void' return type."), mTarget);
			return;
		}
		ParameterInfo[] array = mMethod.GetParameters();
		if (array.Length == 0)
		{
			mCachedCallback = (Callback)Delegate.CreateDelegate(typeof(Callback), mTarget, mMethodName);
			mArgs = null;
			mParameters = null;
			return;
		}
		if (mParameters == null || mParameters.Length != array.Length)
		{
			mParameters = new Parameter[array.Length];
			int i = 0;
			for (int num = mParameters.Length; i < num; i++)
			{
				mParameters[i] = new Parameter();
			}
		}
		int j = 0;
		for (int num2 = mParameters.Length; j < num2; j++)
		{
			mParameters[j].expectedType = array[j].ParameterType;
		}
	}

	public bool Execute()
	{
		if (!mCached)
		{
			Cache();
		}
		if (mCachedCallback != null)
		{
			mCachedCallback();
			return true;
		}
		if (mMethod != null)
		{
			if (mParameters == null || mParameters.Length == 0)
			{
				mMethod.Invoke(mTarget, null);
			}
			else
			{
				if (mArgs == null || mArgs.Length != mParameters.Length)
				{
					mArgs = new object[mParameters.Length];
				}
				int i = 0;
				for (int num = mParameters.Length; i < num; i++)
				{
					mArgs[i] = mParameters[i].value;
				}
				try
				{
					mMethod.Invoke(mTarget, mArgs);
				}
				catch (ArgumentException ex)
				{
					string message = ex.Message;
					message += "\nExpected: ";
					ParameterInfo[] array = mMethod.GetParameters();
					if (array.Length == 0)
					{
						message += "no arguments";
					}
					else
					{
						message += array[0];
						for (int j = 1; j < array.Length; j++)
						{
							message = message + ", " + array[j].ParameterType;
						}
					}
					message += "\nGot: ";
					if (mParameters.Length == 0)
					{
						message += "no arguments";
					}
					else
					{
						message += mParameters[0].type;
						for (int k = 1; k < mParameters.Length; k++)
						{
							message = message + ", " + mParameters[k].type;
						}
					}
					message += "\n";
					Debug.LogError(message);
				}
				int l = 0;
				for (int num2 = mArgs.Length; l < num2; l++)
				{
					mArgs[l] = null;
				}
			}
			return true;
		}
		return false;
	}

	public void Clear()
	{
		mTarget = null;
		mMethodName = null;
		mRawDelegate = false;
		mCachedCallback = null;
		mParameters = null;
		mCached = false;
		mMethod = null;
		mArgs = null;
	}

	public override string ToString()
	{
		if (mTarget != null)
		{
			string text = mTarget.GetType().ToString();
			int num = text.LastIndexOf('.');
			if (num > 0)
			{
				text = text.Substring(num + 1);
			}
			if (!string.IsNullOrEmpty(methodName))
			{
				return text + "." + methodName;
			}
			return text + ".[delegate]";
		}
		return (!mRawDelegate) ? null : "[delegate]";
	}

	public static void Execute(List<EventDelegate> list)
	{
		if (list == null)
		{
			return;
		}
		int num = 0;
		while (num < list.Count)
		{
			EventDelegate eventDelegate = list[num];
			if (eventDelegate != null)
			{
				eventDelegate.Execute();
				if (num >= list.Count)
				{
					break;
				}
				if (list[num] != eventDelegate)
				{
					continue;
				}
				if (eventDelegate.oneShot)
				{
					list.RemoveAt(num);
					continue;
				}
			}
			num++;
		}
	}

	public static bool IsValid(List<EventDelegate> list)
	{
		if (list != null)
		{
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.isValid)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void Set(List<EventDelegate> list, Callback callback)
	{
		if (list != null)
		{
			list.Clear();
			list.Add(new EventDelegate(callback));
		}
	}

	public static void Set(List<EventDelegate> list, EventDelegate del)
	{
		if (list != null)
		{
			list.Clear();
			list.Add(del);
		}
	}

	public static void Add(List<EventDelegate> list, Callback callback)
	{
		Add(list, callback, false);
	}

	public static void Add(List<EventDelegate> list, Callback callback, bool oneShot)
	{
		if (list != null)
		{
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.Equals(callback))
				{
					return;
				}
			}
			EventDelegate eventDelegate2 = new EventDelegate(callback);
			eventDelegate2.oneShot = oneShot;
			list.Add(eventDelegate2);
		}
		else
		{
			Debug.LogWarning("Attempting to add a callback to a list that's null");
		}
	}

	public static void Add(List<EventDelegate> list, EventDelegate ev)
	{
		Add(list, ev, ev.oneShot);
	}

	public static void Add(List<EventDelegate> list, EventDelegate ev, bool oneShot)
	{
		if (ev.mRawDelegate || ev.target == null || string.IsNullOrEmpty(ev.methodName))
		{
			Add(list, ev.mCachedCallback, oneShot);
		}
		else if (list != null)
		{
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.Equals(ev))
				{
					return;
				}
			}
			EventDelegate eventDelegate2 = new EventDelegate(ev.target, ev.methodName);
			eventDelegate2.oneShot = oneShot;
			if (ev.mParameters != null && ev.mParameters.Length > 0)
			{
				eventDelegate2.mParameters = new Parameter[ev.mParameters.Length];
				for (int j = 0; j < ev.mParameters.Length; j++)
				{
					eventDelegate2.mParameters[j] = ev.mParameters[j];
				}
			}
			list.Add(eventDelegate2);
		}
		else
		{
			Debug.LogWarning("Attempting to add a callback to a list that's null");
		}
	}

	public static bool Remove(List<EventDelegate> list, Callback callback)
	{
		if (list != null)
		{
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.Equals(callback))
				{
					list.RemoveAt(i);
					return true;
				}
			}
		}
		return false;
	}

	public static bool Remove(List<EventDelegate> list, EventDelegate ev)
	{
		if (list != null)
		{
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.Equals(ev))
				{
					list.RemoveAt(i);
					return true;
				}
			}
		}
		return false;
	}
}
