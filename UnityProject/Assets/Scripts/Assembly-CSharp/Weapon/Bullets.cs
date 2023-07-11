public class Bullets
{
	public int bulletCountToPack;

	public int bulletAllCount;

	public bool needReload;

	public Bullets(int bulletCountToPack, int allCount)
	{
		this.bulletCountToPack = bulletCountToPack;
		bulletAllCount = allCount;
	}

	public void minusOneBullet()
	{
		if (getCurrentBulletsToPack() == 1)
		{
			needReload = true;
		}
		bulletAllCount--;
	}

	public void reloadBullets()
	{
		needReload = false;
	}

	public int getRestBullets()
	{
		int num = bulletCountToPack * (bulletAllCount / bulletCountToPack);
		if (!needReload && num == bulletAllCount)
		{
			num -= bulletCountToPack;
		}
		return num;
	}

	public int getCurrentBulletsToPack()
	{
		return bulletAllCount - getRestBullets();
	}

	public string getCountBullets()
	{
		return getCurrentBulletsToPack() + "/" + getRestBullets();
	}

	public string getCountBulletsOnly()
	{
		return string.Empty + bulletAllCount;
	}
}
