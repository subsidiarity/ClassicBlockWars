using Holoville.HOTween.Core;
using UnityEngine;

namespace Holoville.HOTween
{
	public class SequenceParms : ABSTweenComponentParms
	{
		internal void InitializeSequence(Sequence p_sequence)
		{
			InitializeOwner(p_sequence);
		}

		public SequenceParms Id(string p_id)
		{
			id = p_id;
			return this;
		}

		public SequenceParms IntId(int p_intId)
		{
			intId = p_intId;
			return this;
		}

		public SequenceParms AutoKill(bool p_active)
		{
			autoKillOnComplete = p_active;
			return this;
		}

		public SequenceParms UpdateType(UpdateType p_updateType)
		{
			updateType = p_updateType;
			return this;
		}

		public SequenceParms TimeScale(float p_timeScale)
		{
			timeScale = p_timeScale;
			return this;
		}

		public SequenceParms Loops(int p_loops)
		{
			return Loops(p_loops, HOTween.defLoopType);
		}

		public SequenceParms Loops(int p_loops, LoopType p_loopType)
		{
			loops = p_loops;
			loopType = p_loopType;
			return this;
		}

		public SequenceParms OnStart(TweenDelegate.TweenCallback p_function)
		{
			onStart = p_function;
			return this;
		}

		public SequenceParms OnStart(TweenDelegate.TweenCallbackWParms p_function, params object[] p_funcParms)
		{
			onStartWParms = p_function;
			onStartParms = p_funcParms;
			return this;
		}

		public SequenceParms OnUpdate(TweenDelegate.TweenCallback p_function)
		{
			onUpdate = p_function;
			return this;
		}

		public SequenceParms OnUpdate(TweenDelegate.TweenCallbackWParms p_function, params object[] p_funcParms)
		{
			onUpdateWParms = p_function;
			onUpdateParms = p_funcParms;
			return this;
		}

		public SequenceParms OnPause(TweenDelegate.TweenCallback p_function)
		{
			onPause = p_function;
			return this;
		}

		public SequenceParms OnPause(TweenDelegate.TweenCallbackWParms p_function, params object[] p_funcParms)
		{
			onPauseWParms = p_function;
			onPauseParms = p_funcParms;
			return this;
		}

		public SequenceParms OnPlay(TweenDelegate.TweenCallback p_function)
		{
			onPlay = p_function;
			return this;
		}

		public SequenceParms OnPlay(TweenDelegate.TweenCallbackWParms p_function, params object[] p_funcParms)
		{
			onPlayWParms = p_function;
			onPlayParms = p_funcParms;
			return this;
		}

		public SequenceParms OnRewinded(TweenDelegate.TweenCallback p_function)
		{
			onRewinded = p_function;
			return this;
		}

		public SequenceParms OnRewinded(TweenDelegate.TweenCallbackWParms p_function, params object[] p_funcParms)
		{
			onRewindedWParms = p_function;
			onRewindedParms = p_funcParms;
			return this;
		}

		public SequenceParms OnStepComplete(TweenDelegate.TweenCallback p_function)
		{
			onStepComplete = p_function;
			return this;
		}

		public SequenceParms OnStepComplete(TweenDelegate.TweenCallbackWParms p_function, params object[] p_funcParms)
		{
			onStepCompleteWParms = p_function;
			onStepCompleteParms = p_funcParms;
			return this;
		}

		public SequenceParms OnComplete(TweenDelegate.TweenCallback p_function)
		{
			onComplete = p_function;
			return this;
		}

		public SequenceParms OnComplete(TweenDelegate.TweenCallbackWParms p_function, params object[] p_funcParms)
		{
			onCompleteWParms = p_function;
			onCompleteParms = p_funcParms;
			return this;
		}

		public SequenceParms OnComplete(GameObject p_sendMessageTarget, string p_methodName, object p_value = null, SendMessageOptions p_options = SendMessageOptions.RequireReceiver)
		{
			onCompleteWParms = HOTween.DoSendMessage;
			onCompleteParms = new object[4] { p_sendMessageTarget, p_methodName, p_value, p_options };
			return this;
		}
	}
}
