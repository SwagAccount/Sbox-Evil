using System;
using Sandbox;

public sealed class DeletedObjectsSaveSystem : Component
{
	[Property] private string sceneName {get;set;}
	[Property] string saveName {get; set;}
	[Property] List<GameObject> normal {get; set;}
	protected override void OnAwake()
	{
		normal = new List<GameObject>();
		for(int i = 0; i < GameObject.Children.Count; i++)
		{
			for(int I = 0; I < GameObject.Children[i].Children.Count; I++)
			{
				normal.Add(GameObject.Children[i].Children[I]);
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
		Log.Info(saveName); 
	}
	[Property] List<bool> fart {get;set;}
	public void Save()
	{
		string dirName = $"saves/{saveName}/{sceneName}/";
		List<bool> fart = new List<bool>();
		for(int i = 0; i < normal.Count; i++)
		{
			fart.Add(false);
		}
		for(int i = 0; i < GameObject.Children.Count; i++)
		{
			for(int I = 0; I < GameObject.Children[i].Children.Count; I++)
			{
				int index = normal.IndexOf(GameObject.Children[i].Children[I]);
				if(index!=-1)
				{
					fart[index] = true;
				}
			}
		}
		FileSystem.Data.WriteAllText($"{dirName}deletables.json", Json.Serialize(fart));
	}
	public void Load()
	{
		string dirName = $"saves/{saveName}/{sceneName}/";
		if(!FileSystem.Data.DirectoryExists(dirName))
		{
			return;
		}
		if(!FileSystem.Data.FileExists($"{dirName}deletables.json"))
		{
			return;
		}
		
		string shit = FileSystem.Data.ReadAllText( $"{dirName}deletables.json");
		List<bool> fart = Json.Deserialize<List<bool>>(shit);
		for(int i = 0; i < normal.Count; i++)
		{
			if(!fart[i])
			{
				normal[i].Destroy();
			}
		}
		
	}
}