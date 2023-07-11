using System;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;
using UnityEngine;

[AddComponentMenu("Simple Waypoint System/hoMove")]
public class hoMove : MonoBehaviour
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

	public PathManager pathContainer;

	public PathType pathtype = PathType.Curved;

	public bool onStart;

	public bool moveToPath;

	public bool closePath;

	public bool orientToPath;

	public bool local;

	public float lookAhead;

	public float sizeToAdd;

	[HideInInspector]
	public float[] StopAtPoint;

	[HideInInspector]
	public List<MessageOptions> _messages = new List<MessageOptions>();

	public TimeValue timeValue = TimeValue.speed;

	public float speed = 5f;

	public EaseType easetype;

	public LoopType looptype = LoopType.loop;

	private Transform[] waypoints;

	[HideInInspector]
	public int currentPoint;

	private bool repeat;

	public Axis lockAxis;

	public Axis lockPosition;

	[HideInInspector]
	public Animation anim;

	public AnimationClip walkAnim;

	public AnimationClip idleAnim;

	public bool crossfade;

	public Tweener tween;

	private Vector3[] wpPos;

	private TweenParms tParms;

	private PlugVector3Path plugPath;

	private System.Random rand = new System.Random();

	private int[] rndArray;

	private int rndIndex;

	private bool waiting;

	private PhotonView photonView;

	private float originSpeed;

	internal void Start()
	{
		photonView = PhotonView.Get(this);
		if (!photonView.isMine)
		{
			base.enabled = false;
			return;
		}
		if (!anim)
		{
			anim = base.gameObject.GetComponentInChildren<Animation>();
		}
		if (onStart)
		{
			StartMove();
		}
	}

	internal void InitWaypoints()
	{
		wpPos = new Vector3[waypoints.Length];
		for (int i = 0; i < wpPos.Length; i++)
		{
			wpPos[i] = waypoints[i].position + new Vector3(0f, sizeToAdd, 0f);
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
		originSpeed = speed;
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
		StartCoroutine(Move());
	}

	internal IEnumerator Move()
	{
		if (moveToPath)
		{
			yield return StartCoroutine(MoveToPath());
		}
		else
		{
			InitWaypoints();
			base.transform.position = waypoints[currentPoint].position + new Vector3(0f, sizeToAdd, 0f);
			if (orientToPath && currentPoint < wpPos.Length - 1)
			{
				base.transform.LookAt(wpPos[currentPoint + 1]);
			}
		}
		if (looptype == LoopType.random)
		{
			StartCoroutine(ReachedEnd());
			yield break;
		}
		CreateTween();
		StartCoroutine(NextWaypoint());
	}

	internal IEnumerator MoveToPath()
	{
		int max = ((waypoints.Length <= 4) ? waypoints.Length : 4);
		wpPos = new Vector3[max];
		for (int i = 1; i < max; i++)
		{
			wpPos[i] = waypoints[i - 1].position + new Vector3(0f, sizeToAdd, 0f);
		}
		wpPos[0] = base.transform.position;
		CreateTween();
		if (tween.isPaused)
		{
			tween.Play();
		}
		yield return StartCoroutine(tween.UsePartialPath(-1, 1).WaitForCompletion());
		moveToPath = false;
		tween.Kill();
		tween = null;
		InitWaypoints();
	}

	internal void CreateTween()
	{
		PlayWalk();
		plugPath = new PlugVector3Path(wpPos, true, pathtype);
		if (orientToPath || lockAxis != 0)
		{
			plugPath.OrientToPath(lookAhead, lockAxis);
		}
		if (lockPosition != 0)
		{
			plugPath.LockPosition(lockPosition);
		}
		if (closePath)
		{
			plugPath.ClosePath(true);
		}
		tParms = new TweenParms();
		if (local)
		{
			tParms.Prop("localPosition", plugPath);
		}
		else
		{
			tParms.Prop("position", plugPath);
		}
		tParms.AutoKill(false);
		tParms.Pause(true);
		tParms.Loops(1);
		if (timeValue == TimeValue.speed)
		{
			tParms.SpeedBased();
			tParms.Ease(EaseType.Linear);
		}
		else
		{
			tParms.Ease(easetype);
		}
		tween = HOTween.To(base.transform, originSpeed, tParms);
		if (originSpeed != speed)
		{
			ChangeSpeed(speed);
		}
	}

	internal IEnumerator NextWaypoint()
	{
		for (int point = 0; point < wpPos.Length - 1; point++)
		{
			StartCoroutine(SendMessages());
			if (StopAtPoint[currentPoint] > 0f)
			{
				yield return StartCoroutine(WaitDelay());
			}
			while (waiting)
			{
				yield return null;
			}
			PlayWalk();
			tween.Play();
			yield return StartCoroutine(tween.UsePartialPath(point, point + 1).WaitForCompletion());
			if (repeat)
			{
				currentPoint--;
			}
			else if (looptype == LoopType.random)
			{
				rndIndex++;
				currentPoint = rndArray[rndIndex];
			}
			else
			{
				currentPoint++;
			}
		}
		if (looptype != LoopType.pingPong && looptype != LoopType.random)
		{
			StartCoroutine(SendMessages());
			if (StopAtPoint[currentPoint] > 0f)
			{
				yield return StartCoroutine(WaitDelay());
			}
		}
		StartCoroutine(ReachedEnd());
	}

	internal IEnumerator WaitDelay()
	{
		tween.Pause();
		PlayIdle();
		float timer = Time.time + StopAtPoint[currentPoint];
		while (!waiting && Time.time < timer)
		{
			yield return null;
		}
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

	internal IEnumerator ReachedEnd()
	{
		switch (looptype)
		{
		case LoopType.none:
			tween.Kill();
			tween = null;
			PlayIdle();
			yield break;
		case LoopType.loop:
			if (closePath)
			{
				tween.Play();
				PlayWalk();
				yield return StartCoroutine(tween.UsePartialPath(currentPoint, -1).WaitForCompletion());
			}
			currentPoint = 0;
			break;
		case LoopType.pingPong:
			tween.Kill();
			tween = null;
			if (!repeat)
			{
				repeat = true;
				for (int i = 0; i < wpPos.Length; i++)
				{
					wpPos[i] = waypoints[waypoints.Length - 1 - i].position + new Vector3(0f, sizeToAdd, 0f);
				}
			}
			else
			{
				InitWaypoints();
				repeat = false;
			}
			CreateTween();
			break;
		case LoopType.random:
		{
			rndIndex = 0;
			InitWaypoints();
			if (tween != null)
			{
				tween.Kill();
				tween = null;
			}
			rndArray = new int[wpPos.Length];
			for (int j = 0; j < rndArray.Length; j++)
			{
				rndArray[j] = j;
			}
			int m = wpPos.Length;
			while (m > 1)
			{
				int l = rand.Next(m--);
				Vector3 temp = wpPos[m];
				wpPos[m] = wpPos[l];
				wpPos[l] = temp;
				int tmpI = rndArray[m];
				rndArray[m] = rndArray[l];
				rndArray[l] = tmpI;
			}
			Vector3 first = wpPos[0];
			int rndFirst = rndArray[0];
			for (int k = 0; k < wpPos.Length; k++)
			{
				if (rndArray[k] == currentPoint)
				{
					rndArray[k] = rndFirst;
					wpPos[0] = wpPos[k];
					wpPos[k] = first;
				}
			}
			rndArray[0] = currentPoint;
			CreateTween();
			break;
		}
		}
		StartCoroutine(NextWaypoint());
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
		StopAllCoroutines();
		HOTween.Kill(base.transform);
		plugPath = null;
		tween = null;
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

	public void Pause()
	{
		waiting = true;
		HOTween.Pause(base.transform);
		PlayIdle();
	}

	public void Resume()
	{
		waiting = false;
		HOTween.Play(base.transform);
		PlayWalk();
	}

	public void ChangeSpeed(float value)
	{
		float timeScale = ((timeValue != TimeValue.speed) ? (originSpeed / value) : (value / originSpeed));
		speed = value;
		tween.timeScale = timeScale;
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
