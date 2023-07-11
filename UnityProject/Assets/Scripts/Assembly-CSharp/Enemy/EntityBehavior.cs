using Photon;

public abstract class EntityBehavior : MonoBehaviour
{
	public bool isDead;

	public abstract void dead();

	public abstract void getDamage(int damage);

	public virtual bool isAlive()
	{
		return !isDead;
	}
}
