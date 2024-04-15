using Sandbox;

public sealed class Onlyfirsttime : Component
{
	[Property] string sceneName {get;set;}
	protected override void OnStart()
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
		List<string> vistedMaps = new List<string>();
		if(FileSystem.Data.FileExists($"saves/{saveName}/vistedMaps.json"))
		{
			string shit = FileSystem.Data.ReadAllText( $"saves/{saveName}/vistedMaps.json");
			vistedMaps = Json.Deserialize<List<string>>(shit);
		}
		foreach(string s in vistedMaps)
		{
			if (s == sceneName)
			{
				GameObject.Destroy();
			}
		}
	}
}