using UnityEngine;

public class PoliceManager : MonoBehaviour
{
	private static PoliceManager instance;

	private WarningLevel currentWarningLevel;

	private int chasingTime = 15;

	private GameObject wLabel;

	public static PoliceManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(PoliceManager)) as PoliceManager;
			}
			return instance;
		}
	}

	private void Blink()
	{
		if (wLabel != null)
		{
			TweenAlpha.Begin(wLabel, 0.5f, 1f);
			Utils.Instance.SafeInvoke(delegate
			{
				TweenAlpha.Begin(wLabel, 0.5f, 0.1f);
			}, 0.5f, true);
		}
	}

	private void Start()
	{
		wLabel = GameObject.FindGameObjectWithTag("GUI_PoliceWarning");
		wLabel.SetActive(false);
	}

	public void StopChasing()
	{
		wLabel.SetActive(false);
		CancelInvoke("Blink");
		foreach (EnemyBehavior item in EnemyGenerator.Instance.listEnemy)
		{
			if (item != null && item.playerTarget != null && item.isPolice)
			{
				item.isAggressive = false;
				item.SwitchEnemyState(EnemyState.Passive);
			}
		}
		SwitchWarningLevel(WarningLevel.Normal);
	}

	public void UpdateChasingTimer()
	{
		CancelInvoke("StopChasing");
		Invoke("StopChasing", chasingTime + 10);
		SwitchWarningLevel(WarningLevel.Chasing);
	}

	public void StartChasing()
	{
		wLabel.SetActive(true);
		Invoke("StopChasing", chasingTime);
		InvokeRepeating("Blink", 0f, 1f);
		foreach (EnemyBehavior item in EnemyGenerator.Instance.listEnemy)
		{
			if (item.isPolice)
			{
				item.getDamage(0);
			}
		}
	}

	public void SwitchWarningLevel(WarningLevel w)
	{
		switch (w)
		{
		case WarningLevel.Normal:
			currentWarningLevel = w;
			break;
		case WarningLevel.Chasing:
			if (currentWarningLevel != WarningLevel.Chasing)
			{
				currentWarningLevel = w;
				StartChasing();
			}
			break;
		}
	}

	private void Update()
	{
		WarningLevel warningLevel = currentWarningLevel;
		if (warningLevel != 0 && warningLevel == WarningLevel.Chasing && GameController.thisScript.playerScript != null && GameController.thisScript.playerScript.isDead)
		{
			StopChasing();
		}
	}
}
