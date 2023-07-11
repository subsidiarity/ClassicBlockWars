using System;
using UnityEngine;

namespace Holoville.HOTween.Plugins.Core
{
	public class PlugVector4 : ABSTweenPlugin
	{
		internal static Type[] validPropTypes = new Type[1] { typeof(Vector4) };

		internal static Type[] validValueTypes = new Type[1] { typeof(Vector4) };

		private Vector4 typedStartVal;

		private Vector4 typedEndVal;

		private Vector4 changeVal;

		protected override object startVal
		{
			get
			{
				return _startVal;
			}
			set
			{
				if (tweenObj.isFrom && isRelative)
				{
					_startVal = (typedStartVal = typedEndVal + (Vector4)value);
				}
				else
				{
					_startVal = (typedStartVal = (Vector4)value);
				}
			}
		}

		protected override object endVal
		{
			get
			{
				return _endVal;
			}
			set
			{
				_endVal = (typedEndVal = (Vector4)value);
			}
		}

		public PlugVector4(Vector4 p_endVal)
			: base(p_endVal, false)
		{
		}

		public PlugVector4(Vector4 p_endVal, EaseType p_easeType)
			: base(p_endVal, p_easeType, false)
		{
		}

		public PlugVector4(Vector4 p_endVal, bool p_isRelative)
			: base(p_endVal, p_isRelative)
		{
		}

		public PlugVector4(Vector4 p_endVal, EaseType p_easeType, bool p_isRelative)
			: base(p_endVal, p_easeType, p_isRelative)
		{
		}

		public PlugVector4(Vector4 p_endVal, AnimationCurve p_easeAnimCurve, bool p_isRelative)
			: base(p_endVal, p_easeAnimCurve, p_isRelative)
		{
		}

		protected override float GetSpeedBasedDuration(float p_speed)
		{
			float num = changeVal.magnitude / p_speed;
			if (num < 0f)
			{
				num = 0f - num;
			}
			return num;
		}

		protected override void SetChangeVal()
		{
			if (isRelative && !tweenObj.isFrom)
			{
				changeVal = typedEndVal;
			}
			else
			{
				changeVal = new Vector4(typedEndVal.x - typedStartVal.x, typedEndVal.y - typedStartVal.y, typedEndVal.z - typedStartVal.z, typedEndVal.w - typedStartVal.w);
			}
		}

		protected override void SetIncremental(int p_diffIncr)
		{
			typedStartVal += changeVal * p_diffIncr;
		}

		protected override void DoUpdate(float p_totElapsed)
		{
			float num = ease(p_totElapsed, 0f, 1f, _duration, tweenObj.easeOvershootOrAmplitude, tweenObj.easePeriod);
			SetValue(new Vector4(typedStartVal.x + changeVal.x * num, typedStartVal.y + changeVal.y * num, typedStartVal.z + changeVal.z * num, typedStartVal.w + changeVal.w * num));
		}
	}
}
