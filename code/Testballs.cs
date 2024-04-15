using Sandbox;

public sealed class Testballs : Component
{
	[Property] public List<testStruct> testStructs { get; set; }
	public struct testStruct
	{
		public int bulletType;
	}
}