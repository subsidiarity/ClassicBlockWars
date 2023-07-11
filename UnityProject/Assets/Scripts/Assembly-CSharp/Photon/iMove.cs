using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Simple Waypoint System/iMove")]
public class iMove : MonoBehaviour
{
	public enum TimeValue
	{
		time = 0,
		speed = 1
	}

	public enum LoopType
	{
		none = 0,
		loop = 1,
		pingPong = 2,
		random = 3
	}

	public enum AxisLock
	{
		none = 0,
		X = 1,
		Y = 2,
		Z = 3
	}

	public PathManager pathContainer;

	public int currentPoint;

	public bool onStart;

	public bool moveToPath;

	public bool orientToPath;

	public float smoothRotation;

	public float sizeToAdd;

	[HideInInspector]
	public float[] StopAtPoint;

	[HideInInspector]
	public List<MessageOptions> _messages = new List<MessageOptions>();

	public TimeValue timeValue = TimeValue.speed;

	public float speed = 5f;

	public iTween.EaseType easetype = iTween.EaseType.linear;

	public LoopType looptype = LoopType.loop;

	private Transform[] waypoints;

	private bool repeat;

	public AxisLock lockAxis;

	private Vector3 startRot;

	[HideInInspector]
	public Animation anim;

	public AnimationClip walkAnim;

	public AnimationClip idleAnim;

	public bool crossfade;

	internal void Start()
	{
		if (!anim)
		{
			anim = base.gameObject.GetComponentInChildren<Animation>();
		}
		startRot = base.transform.localEulerAngles;
		if (onStart)
		{
			StartMove();
		}
	}

	public void StartMove()
	{
		if (pathContainer == null)
		{
			Debug.LogWarning(base.gameObject.name + " has no path! Please set Path Container.");
			return;
		}
		waypoints = pathContainer.waypoints;
		if (StopAtPoint == null)
		{
			StopAtPoint = new float[waypoints.Length];
		}
		else if (StopAtPoint.Length < waypoints.Length)
		{
			float[] array = new float[StopAtPoint.Length];
			Array.Copy(StopAtPoint, array, StopAtPoint.Length);
			StopAtPoint = new float[waypoints.Length];
			Array.Copy(array, StopAtPoint, array.Length);
		}
		if (_messages.Count > 0)
		{
			InitializeMessageOptions();
		}
		if (!moveToPath)
		{
			base.transform.position = waypoints[currentPoint].position + new Vector3(0f, sizeToAdd, 0f);
			StartCoroutine("NextWaypoint");
		}
		else
		{
			Move(currentPoint);
		}
	}

	internal void Move(int point)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("position", waypoints[point].position + new Vector3(0f, sizeToAdd, 0f));
		hashtable.Add("easetype", easetype);
		hashtable.Add("orienttopath", orientToPath);
		hashtable.Add("oncomplete", "NextWaypoint");
		if (orientToPath)
		{
			hashtable.Add("looktime", smoothRotation);
			if (lockAxis != 0)
			{
				hashtable.Add("onupdate", "LockAxis");
			}
		}
		if (timeValue == TimeValue.time)
		{
			hashtable.Add("time", speed);
		}
		else
		{
			hashtable.Add("speed", speed);
		}
		iTween.MoveTo(base.gameObject, hashtable);
		PlayWalk();
	}

	internal void LockAxis()
	{
		Transform transform = base.transform;
		Vector3 localEulerAngles = transform.localEulerAngles;
		switch (lockAxis)
		{
		case AxisLock.X:
			transform.localEulerAngles = new Vector3(startRot.x, localEulerAngles.y, localEulerAngles.z);
			break;
		case AxisLock.Y:
			transform.localEulerAngles = new Vector3(localEulerAngles.x, startRot.y, localEulerAngles.z);
			break;
		case AxisLock.Z:
			transform.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y, startRot.z);
			break;
		}
	}

	internal IEnumerator NextWaypoint()
	{
		StartCoroutine(SendMessages());
		if (StopAtPoint[currentPoint] > 0f)
		{
			PlayIdle();
			yield return new WaitForSeconds(StopAtPoint[currentPoint]);
		}
		switch (looptype)
		{
		case LoopType.none:
			if (currentPoint < waypoints.Length - 1)
			{
				currentPoint++;
				break;
			}
			PlayIdle();
			yield break;
		case LoopType.loop:
			if (currentPoint == waypoints.Length - 1)
			{
				currentPoint = 0;
				StartMove();
				yield break;
			}
			currentPoint++;
			break;
		case LoopType.pingPong:
			if (currentPoint == waypoints.Length - 1)
			{
				repeat = true;
			}
			else if (currentPoint == 0)
			{
				repeat = false;
			}
			if (repeat)
			{
				currentPoint--;
			}
			else
			{
				currentPoint++;
			}
			break;
		case LoopType.random:
		{
			int oldPoint = currentPoint;
			do
			{
				currentPoint = UnityEngine.Random.Range(0, waypoints.Length);
			}
			while (oldPoint == currentPoint);
			break;
		}
		}
		Move(currentPoint);
	}

	internal IEnumerator SendMessages()
	{
		if (_messages.Count != waypoints.Length)
		{
			yield break;
		}
		for (int i = 0; i < _messages[currentPoint].message.Count; i++)
		{
			if (!(_messages[currentPoint].message[i] == string.Empty))
			{
				MessageOptions mess = _messages[currentPoint];
				switch (mess.type[i])
				{
				case MessageOptions.ValueType.None:
					SendMessage(mess.message[i], SendMessageOptions.DontRequireReceiver);
					break;
				case MessageOptions.ValueType.Object:
					SendMessage(mess.message[i], mess.obj[i], SendMessageOptions.DontRequireReceiver);
					break;
				case MessageOptions.ValueType.Text:
					SendMessage(mess.message[i], mess.text[i], SendMessageOptions.DontRequireReceiver);
					break;
				case MessageOptions.ValueType.Numeric:
					SendMessage(mess.message[i], mess.num[i], SendMessageOptions.DontRequireReceiver);
					break;
				case MessageOptions.ValueType.Vector2:
					SendMessage(mess.message[i], mess.vect2[i], SendMessageOptions.DontRequireReceiver);
					break;
				case MessageOptions.ValueType.Vector3:
					SendMessage(mess.message[i], mess.vect3[i], SendMessageOptions.DontRequireReceiver);
					break;
				}
			}
		}
	}

	internal void InitializeMessageOptions()
	{
		if (_messages.Count < waypoints.Length)
		{
			for (int i = _messages.Count; i < waypoints.Length; i++)
			{
				MessageOptions item = AddMessageToOption(new MessageOptions());
				_messages.Add(item);
			}
		}
		else if (_messages.Count > waypoints.Length)
		{
			for (int num = _messages.Count - 1; num >= waypoints.Length; num--)
			{
				_messages.RemoveAt(num);
			}
		}
	}

	internal MessageOptions AddMessageToOption(MessageOptions opt)
	{
		opt.message.Add(string.Empty);
		opt.type.Add(MessageOptions.ValueType.None);
		opt.obj.Add(null);
		opt.text.Add(null);
		opt.num.Add(0f);
		opt.vect2.Add(Vector2.zero);
		opt.vect3.Add(Vector3.zero);
		return opt;
	}

	internal void PlayIdle()
	{
		if ((bool)idleAnim)
		{
			if (crossfade)
			{
				anim.CrossFade(idleAnim.name, 0.2f);
			}
			else
			{
				anim.Play(idleAnim.name);
			}
		}
	}

	internal void PlayWalk()
	{
		if ((bool)walkAnim)
		{
			if (crossfade)
			{
				anim.CrossFade(walkAnim.name, 0.2f);
			}
			else
			{
				anim.Play(walkAnim.name);
			}
		}
	}

	public void SetPath(PathManager newPath)
	{
		Stop();
		pathContainer = newPath;
		waypoints = pathContainer.waypoints;
		currentPoint = 0;
		StartMove();
	}

	public void Stop()
	{
		StopCoroutine("NextWaypoint");
		iTween.Stop(base.gameObject);
		PlayIdle();
	}

	public void Reset()
	{
		Stop();
		currentPoint = 0;
		if ((bool)pathContainer)
		{
			base.transform.position = waypoints[currentPoint].position + new Vector3(0f, sizeToAdd, 0f);
		}
	}

	public void ChangeSpeed(float value)
	{
		Stop();
		speed = value;
		StartMove();
	}

	public MessageOptions GetMessageOption(int waypointID, int messageID)
	{
		InitializeMessageOptions();
		MessageOptions messageOptions = _messages[waypointID];
		for (int i = messageOptions.message.Count; i <= messageID; i++)
		{
			AddMessageToOption(messageOptions);
		}
		return messageOptions;
	}
}
