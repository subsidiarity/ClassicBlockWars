using UnityEngine;

public class Learning : MonoBehaviour
{
	public GameObject[] enableAtTheEnd;

	public Item[] items;

	public int currentItem;

	private GameObject activeElement;

	private Transform finger;

	public UILabel fieldMessage;

	private void Start()
	{
		finger = base.transform.Find("Finger");
		if (finger != null)
		{
			finger.gameObject.SetActive(false);
		}
		if (settings.isLearned)
		{
			base.gameObject.SetActive(false);
		}
		else if (settings.offlineMode)
		{
			Invoke("startLearning", 0.5f);
		}
	}

	private void Update()
	{
	}

	private void startLearning()
	{
		currentItem = 0;
		GameObject[] array = enableAtTheEnd;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		Item[] array2 = items;
		foreach (Item item in array2)
		{
			item.itemTransform.GetComponent<LearningCallback>().setLearningObject(this);
			item.itemTransform.gameObject.SetActive(false);
		}
		items[currentItem].itemTransform.gameObject.SetActive(true);
		showFinger(items[currentItem]);
	}

	private void saveLearning()
	{
		settings.isLearned = true;
		Save.SaveBool("isLearned", true);
	}

	private void finishLearning()
	{
		fieldMessage.text = string.Empty;
		Invoke("hideMessage", 2f);
	}

	private void hideMessage()
	{
		fieldMessage.gameObject.SetActive(false);
		base.gameObject.SetActive(false);
	}

	private void showFinger(Item wht)
	{
		if (wht.isLeft)
		{
			finger.rotation = Quaternion.Euler(0f, 0f, -60f);
		}
		else
		{
			finger.rotation = Quaternion.Euler(0f, 0f, 60f);
		}
		finger.gameObject.SetActive(true);
		finger.transform.position = wht.itemTransform.position;
		fieldMessage.text = wht.itemText;
	}

	public void nextItem()
	{
		if (currentItem + 1 < items.Length)
		{
			currentItem++;
			items[currentItem].itemTransform.gameObject.SetActive(true);
			finger.gameObject.SetActive(false);
			showFinger(items[currentItem]);
		}
		else if (!settings.isLearned)
		{
			GameObject[] array = enableAtTheEnd;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(true);
			}
			fieldMessage.text = "Okey, now you are ready!";
			finger.gameObject.SetActive(false);
			saveLearning();
			Invoke("finishLearning", 2f);
		}
	}
}
