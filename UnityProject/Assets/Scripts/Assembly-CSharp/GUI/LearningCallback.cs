using UnityEngine;

public class LearningCallback : MonoBehaviour
{
	private Learning lObject;

	private bool isInvoked;

	public void setLearningObject(Learning lobj)
	{
		lObject = lobj;
	}

	private void OnPress(bool isDown)
	{
		if (isDown && lObject != null)
		{
			Invoke("nextItem", lObject.items[lObject.currentItem].delay);
			isInvoked = true;
		}
	}

	private void nextItem()
	{
		if (lObject != null && lObject.items[lObject.currentItem].itemTransform.gameObject.Equals(base.gameObject))
		{
			lObject.nextItem();
		}
	}
}
