using System.Diagnostics;
using System.Threading;
using Sandbox;

public sealed class EnemySaveSystem : Component
{
	[Property] public bool getJson {get; set;}
	[Property] public bool loadJson {get; set;}
	[Property] public string sceneName {get; set;}
	[Property] public Material happyZombieMaterial {get; set;}
	[Property] public List<GameObject> enemyDoc {get; set;}
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
	}
	bool happy;
	public void toggleHappyZombies()
	{
		foreach(GameObject child in GameObject.Children)
		{
			ZombieAI zai = child.Components.Get<ZombieAI>();
			if(zai!=null)
			{
				zai.setMaterial(happy ? null:happyZombieMaterial);
				
			}
		}
		happy = !happy;
	}
	protected override void OnUpdate()
	{
		
		if(getJson) Save();
		if(loadJson) Load();
	}
	public void Save()
	{
		enemySaveData eSD = new enemySaveData();
		eSD.posX = new List<float>();
		eSD.posY = new List<float>();
		eSD.posZ = new List<float>();
		eSD.angX = new List<float>();
		eSD.angY = new List<float>();
		eSD.angZ = new List<float>();
		eSD.name = new List<string>();
		eSD.bools = new List<string>();
		eSD.floats = new List<string>();
		eSD.strings = new List<string>();
		for(int i = 0; i < GameObject.Children.Count; i++)
		{
			eSD.posX.Add(GameObject.Children[i].Transform.LocalPosition.x);
			eSD.posY.Add(GameObject.Children[i].Transform.LocalPosition.y);
			eSD.posZ.Add(GameObject.Children[i].Transform.LocalPosition.z);
			eSD.angX.Add(GameObject.Children[i].Transform.LocalRotation.Angles().pitch);
			eSD.angY.Add(GameObject.Children[i].Transform.LocalRotation.Angles().yaw);
			eSD.angZ.Add(GameObject.Children[i].Transform.LocalRotation.Angles().roll);
			

			EnemySaveStuff eSS = GameObject.Children[i].Components.Get<EnemySaveStuff>();
			eSD.name.Add(eSS.name);
			string stringthing = string.Join(",",eSS.bools);
			if(stringthing != "")
			{
				eSD.bools.Add(stringthing);
			}
			else
			{
				eSD.bools.Add("Empty");
			}

			stringthing = string.Join(",",eSS.floats);
			if(stringthing != "")
			{
				eSD.floats.Add(stringthing);
			}
			else
			{
				eSD.floats.Add("Empty");
			}

			stringthing = string.Join(",",eSS.strings);
			if(stringthing != "")
			{
				eSD.strings.Add(stringthing);
			}
			else
			{
				eSD.strings.Add("Empty");
			}
			
		}
		string dirName = $"saves/{saveName}/{sceneName}/";
		if(!FileSystem.Data.DirectoryExists(dirName))
		{
			FileSystem.Data.CreateDirectory(dirName);
		}
		FileSystem.Data.WriteAllText($"{dirName}enemies.json", Json.Serialize(eSD));
		getJson = false;
	}
	public void Load()
	{
		string dirName = $"saves/{saveName}/{sceneName}/";
		if(!FileSystem.Data.DirectoryExists(dirName))
		{
			
			return;
			
		}
		if(!FileSystem.Data.FileExists($"{dirName}enemies.json"))
		{
			Log.Info("fuck?");
			return;
		}
		for(int i = 0; i < GameObject.Children.Count; i++ )
		{
			GameObject.Children[i].Destroy();
		}
		string shit = FileSystem.Data.ReadAllText( $"{dirName}enemies.json");
		enemySaveData eSD = Json.Deserialize<enemySaveData>(shit);
		for(int I = 0; I < eSD.posX.Count; I++)
		{
			GameObject newItems = null;
			for(int i = 0; i < enemyDoc.Count; i++)
			{
				
				string swag = enemyDoc[i].Name;
				if(swag == eSD.name[I])
				{
					newItems = enemyDoc[i];
					break;
				}
			}
			GameObject spawnedItem = newItems.Clone();
			spawnedItem.SetParent(this.GameObject);
			spawnedItem.Transform.LocalPosition = new Vector3(eSD.posX[I],eSD.posY[I],eSD.posZ[I]);
			spawnedItem.Transform.LocalRotation = new Angles(eSD.angX[I],eSD.angY[I],eSD.angZ[I]);
			EnemySaveStuff eSS = spawnedItem.Components.Get<EnemySaveStuff>();
			if(eSD.bools[I] != "Empty")
			{
				var numbers = eSD.bools[I]?.Split(',')?.Select(bool.Parse)?.ToList();
				eSS.bools = numbers;
			}
			else
			{
				eSS.bools = new List<bool>();
			}

			if(eSD.floats[I] != "Empty")
			{
				var numbers = eSD.floats[I]?.Split(',')?.Select(float.Parse)?.ToList();
				eSS.floats = numbers;
			}
			else
			{
				eSS.floats = new List<float>();
			}
			
			if(eSD.strings[I] != "Empty")
			{
				var numbers = eSD.bools[I]?.Split(',').ToList();
				eSS.strings = numbers;
			}
			else
			{
				eSS.strings = new List<string>();
			}
		}
		loadJson = false;
	}
	class enemySaveData
	{
		public List<float> posX { get; set; } 
		public List<float> posY { get; set; } 
		public List<float> posZ { get; set; } 
		public List<float> angX { get; set; } 
		public List<float> angY { get; set; } 
		public List<float> angZ { get; set; } 
		public List<string> name { get; set; } 
		public List<string> bools { get; set; } 
		public List<string> floats { get; set; } 
		public List<string> strings { get; set; } 
	}
}