using System.Dynamic;
using Sandbox;

public sealed class InventorySafeSystem : Component
{
	[Property] private ThreeDinv inv {get;set;}
	[Property] private string sceneName {get;set;}
	[Property] private bool getJson {get;set;}
	[Property] private bool loadJson {get;set;}
	string saveName;
	protected override void OnAwake()
	{
		IEnumerable<GameObject> balls = Scene.GetAllObjects(true);
		foreach(GameObject go in balls)
		{
			ThreeDinv tdi = go.Components.Get<ThreeDinv>();
			if(tdi!=null)
			{
				inv = tdi;
				break;
			}
		}
		if(!FileSystem.Data.FileExists("currentSave.txt"))
		{
			FileSystem.Data.WriteAllText("currentSave.txt","defaultSave");
			saveName = "defaultSave";
		}
		else
		{
			saveName = FileSystem.Data.ReadAllText("currentSave.txt");
		}
		
	}
	protected override void OnUpdate()
	{
		if(getJson) Save();
		if(loadJson) Load();
	}
	public void Save()
	{
		getJson = false;
		inv.invParent.Enabled = true;
		invItemSaveData iTSD = new invItemSaveData();
		iTSD.posX = new List<float>();
		iTSD.posY = new List<float>();
		iTSD.posZ = new List<float>();
		iTSD.angX = new List<float>();
		iTSD.angY = new List<float>();
		iTSD.angZ = new List<float>();
		iTSD.name = new List<string>();
		iTSD.bulletType = new List<int>();
		iTSD.clipContent = new List<string>();
		iTSD.currentMode = new List<int>();
		iTSD.currentMode = new List<int>();
		iTSD.currentGunIndex = -1;
		int holyFuckLois = 0;
		for(int i = 0; i < inv.invParent.Children.Count; i++)
		{
			ItemDetails iD = inv.invParent.Children[i].Components.Get<ItemDetails>();
			if(iD!=null)
			{
				iTSD.posX.Add(inv.invParent.Children[i].Transform.LocalPosition.x);
				iTSD.posY.Add(inv.invParent.Children[i].Transform.LocalPosition.y);
				iTSD.posZ.Add(inv.invParent.Children[i].Transform.LocalPosition.z);
				iTSD.angX.Add(inv.invParent.Children[i].Transform.LocalRotation.Angles().pitch);
				iTSD.angY.Add(inv.invParent.Children[i].Transform.LocalRotation.Angles().yaw);
				iTSD.angZ.Add(inv.invParent.Children[i].Transform.LocalRotation.Angles().roll);
				iTSD.name.Add(iD.name);
				iTSD.bulletType.Add(iD.gunSaveData.bulletType);
				string clipc = string.Join(",",iD.gunSaveData.clipContent);
				if(clipc != "")
				{
					iTSD.clipContent.Add(clipc);
				}
				else
				{
					iTSD.clipContent.Add("Empty");
				}
				iTSD.currentMode.Add(iD.gunSaveData.currentMode);
				
				if(iD == inv.currentEquip)
				{
					iTSD.currentGunIndex = holyFuckLois;
				}
				holyFuckLois++;
				
			}
			
		}
		string dirName = $"saves/{saveName}/";
		if(!FileSystem.Data.DirectoryExists(dirName))
		{
			FileSystem.Data.CreateDirectory(dirName);
		}
		FileSystem.Data.WriteAllText($"{dirName}inventoryItems.json", Json.Serialize(iTSD));
		inv.invParent.Enabled =false;
	}
	public void Load()
	{
		inv.invParent.Enabled = true;
		
		string dirName = $"saves/{saveName}/";
		if(!FileSystem.Data.DirectoryExists(dirName))
		{
			return;
		}
		if(!FileSystem.Data.FileExists($"{dirName}inventoryItems.json"))
		{
			return;
		}
		for(int i = 0; i < GameObject.Children.Count; i++ )
		{
			if(GameObject.Children[i].Components.Get<ItemDetails>()!=null) GameObject.Children[i].Destroy();
		}

		inv.currentEquip = null;
		string shit = FileSystem.Data.ReadAllText( $"{dirName}inventoryItems.json");
		invItemSaveData iISD = Json.Deserialize<invItemSaveData>(shit);
		inv.items = new List<ItemDetails>();
		for(int I = 0; I < iISD.posX.Count; I++)
		{
			GameObject newItems = null;
			for(int i = 0; i < inv.worlditemdoc.Count; i++)
			{
				string swag = inv.itemdoc[i].Name.Substring(0,inv.itemdoc[i].Name.Length-4);
				if(swag == iISD.name[I])
				{
					newItems = inv.itemdoc[i];
					break;
				}
			}
			GameObject spawnedItem = newItems.Clone();
			spawnedItem.SetParent(inv.invParent);
			spawnedItem.Transform.LocalPosition = new Vector3(iISD.posX[I],iISD.posY[I],iISD.posZ[I]);
			spawnedItem.Transform.LocalRotation = new Angles(iISD.angX[I],iISD.angY[I],iISD.angZ[I]);
			ItemDetails wis = spawnedItem.Components.Get<ItemDetails>();
			inv.items.Add(wis);
			wis.name = iISD.name[I];
			wis.gunSaveData = new GunSaveData();
			wis.gunSaveData.bulletType = iISD.bulletType[I];
			wis.gunSaveData.currentMode = iISD.currentMode[I];
			if(iISD.clipContent[I] != "Empty")
			{
				var numbers = iISD.clipContent[I]?.Split(',')?.Select(int.Parse)?.ToList();
				wis.gunSaveData.clipContent = numbers;
			}
			else
			{
				wis.gunSaveData.clipContent = new List<int>();
			}
			if(I == iISD.currentGunIndex)
			{
				inv.equipWeapon(wis);
			}
			
		}
		inv.invParent.Enabled = false;
		loadJson = false;
	}

	class invItemSaveData
	{
		public List<float> posX { get; set; } 
		public List<float> posY { get; set; } 
		public List<float> posZ { get; set; } 
		public List<float> angX { get; set; } 
		public List<float> angY { get; set; } 
		public List<float> angZ { get; set; } 
		public List<string> name { get; set; } 
		public List<int> bulletType { get; set; } 
		public List<string> clipContent { get; set; } 
		public List<int> currentMode { get; set; } 
		public int currentGunIndex { get; set; } 
	}
}