using Sandbox;
using System;

public sealed class ProgressSave : Component
{
	[Property] private string sceneName {get;set;}
	[Property] string saveName {get; set;}
	[Property] List<List<float>> rogress {get; set;}
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
		Log.Info(saveName); 
	}
	public void Save()
	{
		string dirName = $"saves/{saveName}/{sceneName}/";
		List<List<float>> progresses = new List<List<float>>();
		int I= 0; 
		foreach(GameObject g in GameObject.Children)
		{
			Progress progress = g.Components.Get<Progress>();
			progresses.Add(new List<float>());
			for(int i = 0; i < progress.progress.Count; i++)
			{
				progresses[I].Add(progress.progress[i]);
			}
			I++;
		}
		FileSystem.Data.WriteAllText($"{dirName}Interactables.json", Json.Serialize(progresses));
	}
	public void Load()
	{
		Log.Info(saveName);
		string dirName = $"saves/{saveName}/{sceneName}/";
		
		if(!FileSystem.Data.DirectoryExists(dirName))
		{
			return;
		}
		if(!FileSystem.Data.FileExists($"{dirName}Interactables.json"))
		{
			
			return;
			
		}
		
		string shit = FileSystem.Data.ReadAllText( $"{dirName}Interactables.json");
		rogress = Json.Deserialize<List<List<float>>>(shit);
		for(int i = 0; i < rogress.Count; i++)
		{
			Progress p = GameObject.Children[i].Components.Get<Progress>();
			p.progress = rogress[i];
		}
	}
}