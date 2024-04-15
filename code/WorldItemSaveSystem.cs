using System.Diagnostics;
using System.Text.Json;
using Sandbox;
using Sandbox.Services;

public sealed class WorldItemSaveSystem : Component
{
	[Property] private bool getJson {get; set;} 
	[Property] private string sceneName {get; set;} 
	[Property] private bool loadtest {get; set;} 
	[Property] private ThreeDinv inv {get; set;} 
	string saveName;
	protected override void OnAwake()
	{
		if(!FileSystem.Data.FileExists("currentSave.txt"))
		{
			FileSystem.Data.WriteAllText("currentSave.txt","defaultSave");
			saveName = "defaultSave";
		}
		else
		{
			saveName = FileSystem.Data.ReadAllText("currentSave.txt");
		}
		if(getJson) Save();
		if(loadtest) Load();
	}
	public void Save()
	{
		worldItemSaveData wISD = new worldItemSaveData();
		wISD.posX = new List<float>();
		wISD.posY = new List<float>();
		wISD.posZ = new List<float>();
		wISD.angX = new List<float>();
		wISD.angY = new List<float>();
		wISD.angZ = new List<float>();
		wISD.name = new List<string>();
		wISD.bulletType = new List<int>();
		wISD.clipContent = new List<string>();
		wISD.currentMode = new List<int>();
		wISD.stat = new List<bool>();
		wISD.parentIndex = new List<int>();
		for(int I = 0; I < GameObject.Children.Count; I++)
		{
			for(int i = 0; i < GameObject.Children[I].Children.Count; i++)
			{
				Log.Info(GameObject.Children[I].Children[i].Name);
				wISD.parentIndex.Add(I);
				wISD.posX.Add(GameObject.Children[I].Children[i].Transform.LocalPosition.x);
				wISD.posY.Add(GameObject.Children[I].Children[i].Transform.LocalPosition.y);
				wISD.posZ.Add(GameObject.Children[I].Children[i].Transform.LocalPosition.z);
				wISD.angX.Add(GameObject.Children[I].Children[i].Transform.LocalRotation.Angles().pitch);
				wISD.angY.Add(GameObject.Children[I].Children[i].Transform.LocalRotation.Angles().yaw);
				wISD.angZ.Add(GameObject.Children[I].Children[i].Transform.LocalRotation.Angles().roll);
				WorldItemScript wis = GameObject.Children[I].Children[i].Components.Get<WorldItemScript>();
				wISD.name.Add(wis.name);
				wISD.bulletType.Add(wis.gunSaveData.bulletType);
				string clipc = string.Join(",",wis.gunSaveData.clipContent);
				if(clipc != "")
				{
					wISD.clipContent.Add(clipc);
				}
				else
				{
					wISD.clipContent.Add("Empty");
				}
				wISD.currentMode.Add(wis.gunSaveData.currentMode);
				wISD.stat.Add(wis.stat);
			}
		}
		
		string dirName = $"saves-{SaveSystem.saveFolderName}/{saveName}/{sceneName}/";
		if(!FileSystem.Data.DirectoryExists(dirName))
		{
			FileSystem.Data.CreateDirectory(dirName);
		}
		if(!FileSystem.Data.FileExists($"{dirName}worldItems.json"))
		{
			//FileSystem.Data.($"{dirName}worldItems.json");
		}
		FileSystem.Data.WriteAllText($"{dirName}worldItems.json", Json.Serialize(wISD));
		Log.Info("ass");
	}
	public void Load()
	{
		for(int i = 0; i < GameObject.Children.Count; i++ )
		{
			for(int I = 0; I < GameObject.Children[i].Children.Count; I++) GameObject.Children[i].Children[I].Destroy();
		}
		string dirName = $"saves-{SaveSystem.saveFolderName}/{saveName}/{sceneName}/";
		if(!FileSystem.Data.DirectoryExists(dirName))
		{
			return;
		}
		if(!FileSystem.Data.FileExists($"{dirName}worldItems.json"))
		{
			return;
		}
		string shit = FileSystem.Data.ReadAllText( $"{dirName}worldItems.json");
		worldItemSaveData wISD = Json.Deserialize<worldItemSaveData>(shit);
		for(int I = 0; I < wISD.stat.Count; I++)
		{
			GameObject neworldItems = null;
			for(int i = 0; i < inv.worlditemdoc.Count; i++)
			{
				string swag = inv.worlditemdoc[i].Name.Substring(0,inv.worlditemdoc[i].Name.Length-5);
				if(swag == wISD.name[I])
				{
					neworldItems = inv.worlditemdoc[i];
					break;
				}
			}
			GameObject spawnedItem = neworldItems.Clone();
			WorldItemScript wis = spawnedItem.Components.Get<WorldItemScript>();
			spawnedItem.SetParent(this.GameObject);
			spawnedItem.SetParent(GameObject.Children[wISD.parentIndex[I]]);
			spawnedItem.Transform.LocalPosition = new Vector3(wISD.posX[I],wISD.posY[I],wISD.posZ[I]);
			spawnedItem.Transform.LocalRotation = new Angles(wISD.angX[I],wISD.angY[I],wISD.angZ[I]);
			wis.name = wISD.name[I];
			wis.gunSaveData = new GunSaveData();
			wis.gunSaveData.bulletType = wISD.bulletType[I];
			wis.gunSaveData.currentMode = wISD.currentMode[I];
			if(wISD.clipContent[I] != "Empty")
			{
				var numbers = wISD.clipContent[I]?.Split(',')?.Select(int.Parse)?.ToList();
				wis.gunSaveData.clipContent = numbers;
			}
			else
			{
				wis.gunSaveData.clipContent = new List<int>();
			}
			wis.stat = wISD.stat[I];
			
		}
		

	}
	class worldItemSaveData
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
		public List<bool> stat { get; set; } 
		public List<int> parentIndex { get; set; } 
	}
}