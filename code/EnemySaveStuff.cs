using Sandbox;

public sealed class EnemySaveStuff : Component
{
	[Property] public string name {get; set;}
	[Property] public List<bool> bools {get; set;}
	[Property] public List<float> floats {get; set;}
	[Property] public List<string> strings {get; set;}
}