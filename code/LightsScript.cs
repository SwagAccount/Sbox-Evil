using System.Diagnostics;
using Sandbox;

public sealed class LightsScript : Component
{
	[Property] private List<PointLight> pointLights {get; set;}
	[Property] private List<float> pointLightsRad {get; set;}
	[Property] private List<string> SceneNames {get; set;}
	string saveName;
	[Property] public List<string> levelLight {get;set;}
	[Property] public bool checkLights {get;set;}
	protected override void OnAwake()
	{
		getLightss();
		//checklight(false);
		if(!FileSystem.Data.FileExists("currentSave.txt"))
		{
			FileSystem.Data.WriteAllText("currentSave.txt","defaultSave");
			saveName = "defaultSave";
		}
		else
		{
			saveName = FileSystem.Data.ReadAllText("currentSave.txt");
		}
		checklight(levelLight.Contains(SceneNames[0]));
	}
	protected override void OnUpdate()
	{
		if(checkLights) checklight(false);
	}
	void checklight(bool on)
	{
		int i = 0;
		foreach(PointLight p in pointLights)
		{
			p.Radius = on ? pointLightsRad[i] : 0;
			i++;
		}
	}
	void getLightss()
	{
		pointLights = new List<PointLight>();
		pointLightsRad = new List<float>();
		foreach(GameObject g in GameObject.Children)
		{
			ModelRenderer modelRenderer = g.Components.Get<ModelRenderer>();
			pointLights.Add(g.Children[0].Components.Get<PointLight>());
			pointLightsRad.Add(g.Children[0].Components.Get<PointLight>().Radius);
		}
	}
	public void addLight(bool on)
	{
		foreach(string sceneN in SceneNames)
		{
			if(on && !levelLight.Contains(sceneN)) levelLight.Add(sceneN);
			else if (levelLight.Contains(sceneN)) levelLight.Remove(sceneN);
		}
		checklight(on);
	}
	public void Save()
	{
		string dirName = $"saves-{SaveSystem.saveFolderName}/{saveName}/";
		Log.Info("FUCKING CUNT");
		FileSystem.Data.WriteAllText($"{dirName}lights.json", Json.Serialize(levelLight));
	}
	public void Load()
	{
		Log.Info("Shit");
		string dirName = $"saves-{SaveSystem.saveFolderName}/{saveName}/";
		levelLight = new List<string>();
		if(!FileSystem.Data.DirectoryExists(dirName))
		{
			return;
		}
		if(!FileSystem.Data.FileExists($"{dirName}lights.json"))
		{
			checklight(false);
			return;
			
		}
		
		string shit = FileSystem.Data.ReadAllText( $"{dirName}lights.json");
		levelLight = Json.Deserialize<List<string>>(shit);
		Log.Info(levelLight.Contains(SceneNames[0]));
		checklight(levelLight.Contains(SceneNames[0]));
	}
}