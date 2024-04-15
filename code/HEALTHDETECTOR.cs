using Sandbox;

public sealed class HEALTHDETECTOR : Component
{
	[Property] public float hp;
    [Property] public float bleedAmount;
    [Property] public float healthResist;
    [Property] public float regenAmount;
    [Property] public float bleedMult = 2f;
    [Property] public EnemySaveStuff eSSLink;
    [Property] public int eSSIndex;
	protected override void OnStart()
	{
        if(eSSLink != null)
        {
            hp = eSSLink.floats[eSSIndex];
        }
	}
    float lastHP;
	protected override void OnUpdate()
	{
        if(hp!=lastHP)
        {
            hp+=(lastHP-hp)*healthResist;
        }
        if(eSSLink != null) eSSLink.floats[eSSIndex] = hp;
		hp += bleedAmount * Time.Delta * bleedMult;
        hp -= regenAmount * Time.Delta;
        if (hp < 0) hp = 0;
        lastHP = hp;
	}
}