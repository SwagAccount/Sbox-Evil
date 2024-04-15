using System;
using System.Diagnostics;
using Sandbox;

public sealed class BulletHoleDB : Component
{
	[Property] public List<BulletHoleData> bulletHoles {get;set;} = new();
	[Property] public int defaultHole {get;set;}
	public class BulletHoleData
    {
        [Property] public List<string> bulletholeNames {get; set;}  = new();
        [Property] public List<GameObject> bulletHoles {get; set;} = new();
    }
    public int GetRandomNumberInRange(Random random,int minNumber, int maxNumber)
	{
		return (int)(random.NextInt64() * (maxNumber - minNumber) + minNumber);
	}
    public GameObject FindBulletHoleByMaterial(string mat)
    {
        int i = 0;
        if(mat != null)
        {
            foreach (BulletHoleData item in bulletHoles)
            {
                bool Yes = false;
                foreach(string s in item.bulletholeNames)
                {
                    if(s == mat)
                    {
                        Log.Info($"Found Material {mat}");
                        Yes = true;
                        break;
                    }
                }
                if (Yes)
                {
                    Random r = new Random();
                    if(item.bulletHoles.Count > 0) return item.bulletHoles[0]; 
                    else return null;
                }
                i++;
            }
        }
        else
        {
            Random r = new Random();
            return bulletHoles[defaultHole].bulletHoles[0];
        }
        Random R = new Random();
        return bulletHoles[defaultHole].bulletHoles[0];
    }
}