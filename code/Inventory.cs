using Sandbox;

public sealed class Inventory : Component
{
	[Property] public string currentMag { get; set; }
    [Property] private WeaponDealer weaponDealer { get; set; }
    [Property] public List<string> weapons { get; set; }

    [Property] public List<GunSaveData> weaponsData { get; set; }
    [Property] public int currentWeapon;

    [Property] public List<AmmoData> ammoData { get; set; }
    [Property] public ThreeDinv threeDinv { get; set; }

    protected override void OnAwake()
    {
        IEnumerable<GameObject> balls = Scene.GetAllObjects(true);
		foreach(GameObject go in balls)
		{
			
			if(go.Tags.Has("inventory"))
			{
				threeDinv = go.Components.Get<ThreeDinv>();
				break;
			}
		}
        Log.Info(weaponsData[0].clipContent);
    }

    private void Update()
    {
        if (Input.Pressed("menu"))
        {
            currentWeapon++;
            if(currentWeapon >= weapons.Count)
            {
                currentWeapon = 0;
            }
        }
        weaponDealer.selectedGun = weapons[currentWeapon];
    }

    public int AmmoIndex(string ammoName)
    {
        for(int i = 0; i < ammoData.Count; i++)
        {
            if(ammoData[i].ammoName == ammoName)
            {
                return i;
            }
        }
        return -1;
    }
}
public class GunSaveData
{
    [Property] public int bulletType {get; set;}
    [Property] public List<int> clipContent {get; set;} = new();
    [Property] public int currentMode {get; set;}
}

[System.Serializable]
public class AmmoData
{
    [Property] public string ammoName {get; set;}
    [Property] public int ammoCount {get; set;}
}
