using System.Threading.Tasks;
using Sandbox;

public sealed class Skitzo : Component
{
	public List<string> SchizophrenicMessages = new List<string>
    {
        "Watching",
        "Calling you?",
        "The walls speak",
        "fmgnslfds",
        "They never left. They are all around",
        "cannot hide",
        "too late to leave",
        "all your secrets are known",
        "stop running",
        "eyes in the dark",
        "It hurts but it's not real",
        "They speak through silence",
        ",",
        "Hide",
        "Home.",
        "they are close",
        "safety",
        "Turn around. Slowly",
        "behind",
        "answer.",
        "light.",
        "There's more than just you in your head",
        "They’ve marked you as their own."
    };
	protected override void OnStart()
	{
		repeat();
	}

	async void repeat()
	{
		ConsoleSystem.Run( "playermessage", SchizophrenicMessages[Game.Random.Next(0,SchizophrenicMessages.Count)] );
		await Task.DelaySeconds(15*(Game.Random.Next(500, 1000)/1000f));
		repeat();
	}
}