using Sandbox;

public sealed class Loading : Component
{
	[Property] private string entryScene {get;set;}
	[Property] private float time {get;set;}
	[Property] private GameObject doorp {get;set;}
	protected override void OnStart()
	{
		
		swoog();
	}
	async void swoog()
	{
		
		string saveName;
		if(!FileSystem.Data.FileExists("currentSave.txt"))
		{
			FileSystem.Data.WriteAllText("currentSave.txt","defaultSave");
			saveName = "defaultSave";
		}
		else
		{
			saveName = FileSystem.Data.ReadAllText("currentSave.txt");
		}
		string dirName = $"saves-{SaveSystem.saveFolderName}/{saveName}/";
		if(!FileSystem.Data.DirectoryExists(dirName))
		{
			FileSystem.Data.CreateDirectory(dirName);
		}
		string scenetoload;
		if(!FileSystem.Data.FileExists($"{dirName}levelToLoad.txt"))
		{
			scenetoload = entryScene;
		}
		else
		{
			string swag = FileSystem.Data.ReadAllText($"{dirName}levelToLoad.txt");
			scenetoload = swag;
		}

		string doorName = scenetoload.Split("|")[1];
		for(int i = 0; i < doorp.Children.Count; i++)
		{
			if(doorp.Children[i].Name != doorName)
			{
				doorp.Children[i].Destroy();
			}
		}
		await Task.DelaySeconds(time);
		Scene.LoadFromFile(scenetoload.Split("|")[0]);
	}
}