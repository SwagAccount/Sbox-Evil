using System;
using Sandbox;
using Sandbox.ActionGraphs;

public sealed class SaveSystem : Component
{
	public const string saveFolderName = "preC1";
	[Property] private string sceneName {get;set;}
	[Property] private WorldItemSaveSystem worldItemSaveSystem {get; set;}
	[Property] private EnemySaveSystem enemySaveSystem {get; set;}
	[Property] private Settings Settings {get; set;}
	[Property] private LightsScript lightsScript {get; set;}
	[Property] private ProgressSave progressSave {get; set;}
	[Property] private DeletedObjectsSaveSystem deletedObjectsSaveSystem {get; set;}
	[Property] private InventorySafeSystem inventorySafeSystem {get; set;}
	[Property] private GameObject player {get; set;}
	[Property] List<string> vistedMaps {get;set;}
	string saveName;
	protected override void OnAwake()
	{
		bool gotplayer = false;
		IEnumerable<GameObject> balls = Scene.GetAllObjects(true);
		foreach(GameObject go in balls)
		{
			if(go.Tags.Has("player") && !gotplayer)
			{
				gotplayer =true;
				player = go;
			}
			WorldItemSaveSystem wss = go.Components.Get<WorldItemSaveSystem>();
			EnemySaveSystem ess = go.Components.Get<EnemySaveSystem>();
			LightsScript ls = go.Components.Get<LightsScript>();
			ProgressSave ps = go.Components.Get<ProgressSave>();
			DeletedObjectsSaveSystem doss = go.Components.Get<DeletedObjectsSaveSystem>();
			Settings s = go.Components.Get<Settings>();
			InventorySafeSystem iss = go.Components.Get<InventorySafeSystem>();
			if(wss!=null) worldItemSaveSystem = wss;
			if(ess!=null) enemySaveSystem = ess;
			if(ls!=null) lightsScript = ls;
			if(ps!=null) progressSave = ps;
			if(doss!=null) deletedObjectsSaveSystem = doss;
			if(s!=null) Settings = s;
			if(iss!=null) inventorySafeSystem = iss;
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
		vistedMaps = new List<string>();
		if(FileSystem.Data.FileExists($"saves-{saveFolderName}/{saveName}/vistedMaps.json"))
		{
			string shit = FileSystem.Data.ReadAllText( $"saves-{saveFolderName}/{saveName}/vistedMaps.json");
			vistedMaps = Json.Deserialize<List<string>>(shit);
		}
		foreach(string s in vistedMaps)
		{
			if (s == sceneName)
			{
				worldItemSaveSystem.Load();
				enemySaveSystem.Load();
				lightsScript.Load();
				if(progressSave!=null) progressSave.Load();
				if(deletedObjectsSaveSystem!=null) deletedObjectsSaveSystem.Load();
			}
		}
		Settings.Load();
		LoadPlayer();
		inventorySafeSystem.Load();
	}
	public void AddLocation(string name)
	{
		if(vistedMaps.Contains(name)) return;
		vistedMaps.Add(name);
		FileSystem.Data.WriteAllText($"saves-{saveFolderName}/{saveName}/vistedMaps.json", Json.Serialize(vistedMaps));
	}
	public void loadLevel(string scene, Vector3 playerPos, Angles playerAngles)
	{
		
		worldItemSaveSystem.Save();
		enemySaveSystem.Save();
		lightsScript.Save();
		Settings.Save();
		if(progressSave!=null) progressSave.Save();
		if(deletedObjectsSaveSystem!=null) deletedObjectsSaveSystem.Save();
		inventorySafeSystem.Save();
		FileSystem.Data.WriteAllText($"saves-{saveFolderName}/{saveName}/levelToLoad.txt",scene);
		SavePlayer(playerPos,playerAngles);
		Scene.LoadFromFile("scenes/loading.scene");
	}
	void SavePlayer(Vector3 playerPos, Angles playerAngles)
	{
		AddLocation(sceneName);

		

		playerSaveData pSD = new playerSaveData();
		
		pSD.posX = playerPos.x;
		pSD.posY = playerPos.y;
		pSD.posZ = playerPos.z;

		pSD.angX = playerAngles.pitch;
		pSD.angY = playerAngles.yaw;
		pSD.angZ = playerAngles.roll;

		HEALTHDETECTOR hD = player.Components.Get<HEALTHDETECTOR>();
		pSD.hp = hD.hp;
		pSD.bleedAmount = hD.bleedAmount;
		SurvivalFeatures sF = player.Components.Get<SurvivalFeatures>();
		pSD.stamina = sF.Stamina;
		pSD.addicted = sF.addicted;
		pSD.Tabbaco = sF.Tabbaco;
		Inventory inv = player.Components.Get<Inventory>();
		pSD.ammos = new List<int>();
		for(int i = 0; i < inv.ammoData.Count; i++)
		{
			pSD.ammos.Add(inv.ammoData[i].ammoCount);
		}
		string dirName = $"saves-{saveFolderName}/{saveName}/";
		if(!FileSystem.Data.DirectoryExists(dirName))
		{
			FileSystem.Data.CreateDirectory(dirName);
		}
		FileSystem.Data.WriteAllText($"{dirName}playerSave.json", Json.Serialize(pSD));
	}
	void LoadPlayer()
	{
		Scene.NavMesh.Generate(Scene.PhysicsWorld);

		string dirName = $"saves-{saveFolderName}/{saveName}/";
		if(FileSystem.Data.DirectoryExists(dirName) && FileSystem.Data.FileExists($"{dirName}playerSave.json"))
		{
			string shit = FileSystem.Data.ReadAllText( $"{dirName}playerSave.json");
			playerSaveData pSD = Json.Deserialize<playerSaveData>(shit);
			player.Transform.Position = new Vector3(pSD.posX,pSD.posY,pSD.posZ);
			player.Transform.Rotation = new Angles(pSD.angX,pSD.angY,pSD.angZ);
			HEALTHDETECTOR hp = player.Components.Get<HEALTHDETECTOR>();
			Log.Info(player);
			hp.hp = pSD.hp;
			hp.bleedAmount = pSD.bleedAmount;
			SurvivalFeatures sF = player.Components.Get<SurvivalFeatures>();
			sF.Stamina = pSD.stamina;
			sF.addicted = pSD.addicted;
			sF.Tabbaco = pSD.Tabbaco;
			Inventory inv = player.Components.Get<Inventory>();
			for(int i = 0; i < pSD.ammos.Count; i++)
			{
				inv.ammoData[i].ammoCount = pSD.ammos[i];
			}
		}
	}
	class playerSaveData
	{
		public float posX { get; set; } 
		public float posY { get; set; } 
		public float posZ { get; set; } 
		public float angX { get; set; } 
		public float angY { get; set; } 
		public float angZ { get; set; } 
		public float hp { get; set; } 
		public float bleedAmount { get; set; } 
		public float stamina { get; set; } 
		public float Tabbaco { get; set; } 
		public float addicted { get; set; } 
		public List<int> ammos { get; set; } 
	}

	
}