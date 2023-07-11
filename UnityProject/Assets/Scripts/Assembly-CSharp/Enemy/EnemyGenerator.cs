using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
	private static EnemyGenerator instance;

	public List<string> enemyList;

	private Dictionary<string, GameObject> enemyAvailableList;

	public List<GameObject> enemiesInHell;

	public List<GameObject> enemyNearList;

	public bool allEnemiesAggressive;

	private int CLOSE_DISTANCE = 100;

	private PreloadSector[] allPreloadSectors;

	public GameObject navMeshAllzone;

	public GameObject spisokVragov;

	public GameObject enemyRespawnPoints;

	public BoxCollider[] arrNavMeshzone;

	public List<EnemyBehavior> listEnemy = new List<EnemyBehavior>();

	public static EnemyGenerator Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(EnemyGenerator)) as EnemyGenerator;
			}
			return instance;
		}
	}

	private void initEnemyAvailableList()
	{
		Debug.Log("Initing enemy list.");

		enemyAvailableList = new Dictionary<string, GameObject>();

		Object dog = Resources.LoadAll("enemies/enemyDog")[0];
		enemyAvailableList.Add(dog.name, dog as GameObject);

		Object human = Resources.LoadAll("enemies/enemyHuman")[0];
		enemyAvailableList.Add(human.name, human as GameObject);
	}

	private void initHell()
	{
		enemiesInHell = new List<GameObject>();
		for (int i = 0; i < settings.maxKolVragov; i++)
		{
			Debug.Log("Adding Entity from hell.");
			GameObject value;
			if (enemyAvailableList.TryGetValue(getRandomEnemy(), out value))
			{
				GameObject gameObject = (GameObject)Object.Instantiate(value, getRandomChild(enemyRespawnPoints.transform).position, Quaternion.identity);
				gameObject.SetActive(false);
				gameObject.transform.parent = spisokVragov.transform;
				enemiesInHell.Add(gameObject);
			}
		}
	}

	private Vector3 getPointNearPlayer()
	{
		List<Transform> list = new List<Transform>();
		Transform[] componentsInChildren = enemyRespawnPoints.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (Vector3.Distance(GameController.thisScript.playerScript.transform.position, transform.transform.position) <= (float)CLOSE_DISTANCE)
			{
				list.Add(transform.transform);
			}
		}
		if (list.Count == 0)
		{
			return Vector3.zero;
		}
		Transform transform2 = list[Random.Range(0, list.Count)];
		return transform2.position;
	}

	private EnemyType GetRandomPossibleEnemyType(Vector3 position)
	{
		float num = 1000f;
		PreloadSector preloadSector = null;
		PreloadSector[] array = allPreloadSectors;
		foreach (PreloadSector preloadSector2 in array)
		{
			float num2 = Vector3.Distance(position, preloadSector2.transform.position);
			if (num2 < num)
			{
				num = num2;
				preloadSector = preloadSector2;
			}
		}
		if (preloadSector != null && preloadSector.possibleEnemyTypes.Length != 0)
		{
			return preloadSector.possibleEnemyTypes[Random.Range(0, preloadSector.possibleEnemyTypes.Length)];
		}
		return EnemyType.Casual;
	}

	public void GoToHell(GameObject enemy)
	{
		enemiesInHell.Add(enemy);
		enemy.SetActive(false);
		if (enemy.GetComponent<EnemyBehavior>().enemyWeapon != null)
		{
			Object.Destroy(enemy.GetComponent<EnemyBehavior>().enemyWeapon);
		}
		listEnemy.Remove(enemy.GetComponent<EnemyBehavior>());
		settings.tekKolVragov--;
	}

	private GameObject InstantiateFromHell()
	{
		GameObject gameObject = enemiesInHell[enemiesInHell.Count - 1];
		enemiesInHell.Remove(gameObject);
		gameObject.transform.position = getPointNearPlayer();
		gameObject.SetActive(true);
		return gameObject;
	}

	private void Awake()
	{
		if (navMeshAllzone == null)
		{
			navMeshAllzone = GameObject.FindGameObjectWithTag("NavMeshZone");
		}
		if (enemyRespawnPoints == null)
		{
			enemyRespawnPoints = GameObject.FindGameObjectWithTag("NavMeshZone");
		}
		instance = this;
		enemyList = new List<string>();
		createArrNavMeshzone();
	}

	private void Start()
	{
		if (settings.offlineMode)
		{
			enemyNearList = new List<GameObject>();
			allPreloadSectors = GameObject.FindGameObjectWithTag("Sectors_Preloader").GetComponentsInChildren<PreloadSector>();
			initEnemyAvailableList();
			initHell();
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void Update()
	{
		AddEnemiesToScene();
	}

	public void createArrNavMeshzone()
	{
		settings.tekKolVragov = 0;
		arrNavMeshzone = navMeshAllzone.GetComponentsInChildren<BoxCollider>();
	}

	public Transform getRandomChild(Transform parent)
	{
		Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>();
		int num = Random.Range(0, componentsInChildren.Length);
		return componentsInChildren[num];
	}

	private string getRandomEnemy()
	{
		int num = Random.Range(0, 100);
		if (num <= 15)
		{
			return "enemyDog";
		}
		return "enemyHuman";
	}

	public void makeHellNotAggressive()
	{
		settings.tekKolVragov = 0;
		allEnemiesAggressive = false;
		foreach (EnemyBehavior item in listEnemy)
		{
			Object.Destroy(item.gameObject);
		}
		listEnemy.Clear();
		initHell();
	}

	public void makeHellAggressive()
	{
		settings.tekKolVragov = 0;
		allEnemiesAggressive = true;
		foreach (EnemyBehavior item in listEnemy)
		{
			Object.Destroy(item.gameObject);
		}
		listEnemy.Clear();
		initHell();
	}

	private void AddEnemiesToScene()
	{
		if (settings.tekKolVragov < settings.maxKolVragov && settings.isLearned)
		{
			GameObject gameObject = InstantiateFromHell();
			EnemyBehavior component = gameObject.GetComponent<EnemyBehavior>();
			if (!allEnemiesAggressive)
			{
				component.SwitchEnemyType(GetRandomPossibleEnemyType(component.transform.position));
			}
			else
			{
				component.SwitchEnemyType(EnemyType.Bandit);
			}
			listEnemy.Add(component);
			settings.tekKolVragov++;
		}
	}

	public void removeEnemy(EnemyBehavior curPlayer)
	{
		listEnemy.Remove(curPlayer);
		settings.tekKolVragov--;
	}
}
