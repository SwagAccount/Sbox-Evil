using System;
using System.Diagnostics;
using System.Net.Mail;
using Sandbox;
using Sandbox.UI;

public sealed class ZombieAI : Component
{
	[Category("universal")][Property] private List<Material> materials {get; set;}
	[Category("universal")][Property] private attackTypes attackType {get; set;}
	[Category("universal")][Property] private float seeDis {get; set;}
	[Category("universal")][Property] private float alertDis {get; set;}
	[Category("universal")][Property] private NavMeshAgent agent {get; set;}
	[Category("universal")][Property] public EnemySaveStuff eSS {get; set;}
	[Category("universal")][Property] private float attackdis {get; set;}
	[Category("universal")][Property] private float approachDis {get; set;}
	[Category("universal")][Property] private SkinnedModelRenderer model {get; set;}
	[Category("universal")][Property] private ModelPhysics modelPhysics {get; set;}
	[Category("universal")][Property] private ModelHitboxes modelHitboxes {get;set;}
	[Category("universal")][Property] private float startwalkvel {get; set;}
	[Category("universal")][Property] private float slowwalkspeed {get; set;}
	[Category("universal")][Property] private float fastwalkspeed {get; set;}
	[Category("grabber")][Property] private float runtype {get; set;}
	[Category("grabber")][Property] private float qtAcc {get; set;}
	[Category("grabber")][Property] private float qtDistance {get; set;}
	[Category("grabber")][Property] private float qtMouseRemove {get; set;} = 1f;
	[Category("grabber")][Property] private float StaminaReduce {get; set;} = 1f;
	[Category("grabber")][Property] private float HealthReduce {get; set;} = 12.5f;
	[Category("spitter")][Property] private GameObject spit {get; set;}
	[Category("spitter")][Property] private GameObject spitPoint {get; set;}
	[Category("spitter")][Property] private float spitForce {get; set;}
	[Category("spitter")][Property] private float spitRate {get; set;}
	bool canSpit;
	bool fastwalk;
	[Category("universal")][Property] private GameObject player;
	enum attackTypes
	{
		grabber,
		spitter
	}
	public float GetRandomNumberInRange(Random random,float minNumber,float maxNumber)
	{
		return random.Next(0,1000)/1000 ;
	}
	MovementLocker ml;
	SurvivalFeatures sf;
	HEALTHDETECTOR phd;
	ZombieUI zombieUI;
	public void setMaterial(Material material = null)
	{
		if(material == null)
		{
			Random r = new Random((int)MathF.Round(eSS.floats[0]*1000));
			material = materials[r.Next(0,materials.Count)];
		}
		model.MaterialOverride = material;
	}
	protected override void OnAwake()
	{
		eSS.floats[0] = Game.Random.Next(0,1000)/1000f;
	}
	protected override void OnStart()
	{
		canSpit = true;
		IEnumerable<GameObject> playerballs = Scene.GetAllObjects(true);
		foreach(GameObject go in playerballs)
		{
			
			if(go.Tags.Has("player"))
			{
				player = go;
				break;
			}
		}
		ml = player.Components.Get<MovementLocker>();
		sf = player.Components.Get<SurvivalFeatures>();
		phd = player.Components.Get<HEALTHDETECTOR>();
		zombieUI = player.Components.Get<ZombieUI>();
		fastwalk = eSS.floats[0] > runtype;
		setMaterial();
	}
	[Property]float Velocity;
	[Property]bool attack;
	[Property]bool noFastWalk;
	[Property] float attackt;
	[Property] float attackdt;
	[Property] private float attackTime {get; set;}
	[Property] private float attackDelay {get; set;}
	[Property] private Vector2 quickTimeCount{get; set;}
	[Property] private GameObject camT;
	[Property] private GameObject pT;
	[Property] public float qtReturn {get; set;}
	[Property] public float size;
	[Property] public float Health {get;set;}
	[Property] public float ScreamTime {get;set;}
	[Property] public HEALTHDETECTOR hd {get;set;}
	[Property] public SoundPointComponent soundPointComponent {get; set;}
	[Property] public SoundPointComponent soundPointComponentVoice {get; set;}
	bool lastSeen;
	bool screaming;
	async void Scream()
	{
		model.Set("Scream",true);
		soundPointComponent.StartSound();
		await Task.DelaySeconds(ScreamTime);
		screaming = false;
	}
	void notifyNearByZombies()
	{
		for(int i = 0; i < GameObject.Parent.Children.Count; i++)
		{
			ZombieAI zombieAI = GameObject.Parent.Children[i].Components.Get<ZombieAI>();
			if(zombieAI!=null)
			{
				float dis = Vector3.DistanceBetween(GameObject.Parent.Children[i].Transform.Position,Transform.Position);
				if(dis <= alertDis)
				{
					Log.Info("FUCK");
					zombieAI.eSS.bools[0] = true;
				}
			}
		}
	}
	float lasthp;
	protected override void OnUpdate()
	{
		Velocity = agent.Velocity.Abs().x+agent.Velocity.Abs().y+agent.Velocity.Abs().z;
		if(lasthp < hd.hp) eSS.bools[0] = true;
		float dis = Vector3.DistanceBetween(Transform.Position, player.Transform.Position);
		if(!eSS.bools[0] && dis<seeDis)
		{
			var tr = Scene.Trace.Ray(Transform.Position+Transform.World.Up*60,player.Transform.Position+Transform.World.Up*32).Run();
			if(tr.GameObject == player)
			{
				eSS.bools[0] = true;
				if(eSS.bools[1])notifyNearByZombies();
			}
		}
		if(hd.hp >= Health)
		{
			eSS.floats[1] = hd.hp;
			
			hd.Enabled = false;
			
			Vector3 position = model.Transform.Position;
			Rotation rotation = model.Transform.Rotation;

			model.GameObject.SetParent(null);

			model.Transform.Position = position;
			model.Transform.Rotation = rotation;

			
			
			modelHitboxes.Destroy();
			soundPointComponent.StopSound();
			soundPointComponent.Enabled = false;
			soundPointComponentVoice.StopSound();
			soundPointComponentVoice.Enabled = false;
			
			Components.Get<ZombieAI>().Enabled = false;
			
			return;
		}
		if(eSS.bools[0] && !lastSeen && eSS.bools[1])
		{
			screaming = true;
			Scream();
		}
		
		if(eSS.bools[0] && !screaming)
		{
			if(dis > approachDis)
			{
				agent.MoveTo(player.Transform.Position);
			}
			else
			{
				agent.Stop();
			}
			var tr = Scene.Trace.Ray(Transform.Position,player.Transform.Position+Transform.World.Up*32).Run();
			if(!attack)
			{
				attackt = 0;
				attackdt += Time.Delta;
				if(!noFastWalk) model.Set("Fast Walk", fastwalk);
				agent.MaxSpeed = fastwalk ? fastwalkspeed : slowwalkspeed;
				
				if(dis <= attackdis)
				{
					if(attackdt > attackDelay && !ml.locked && tr.GameObject == player)
					{
						attack = true;
						if(attackType == attackTypes.grabber) soundPointComponent.StartSound();
						size=1;
					}
				}

				model.Set("Walking", (agent.Velocity.Abs().x+agent.Velocity.Abs().y+agent.Velocity.Abs().z) > startwalkvel);
			}
			else
			{
				switch(attackType)
				{
					case attackTypes.grabber:
						size -= Input.Down("attack1") ? Time.Delta*qtMouseRemove : 0;
						size = MathX.Lerp(size,1,Time.Delta*qtReturn);
						sf.Stamina -= StaminaReduce*Time.Delta;
						phd.hp += HealthReduce*Time.Delta;
						zombieUI.actv = true;
						zombieUI.size = size*100;
						attackdt = 0;
						ml.locked = true;
						ml.playerTarget = pT;
						ml.cameraTarget = camT;
						model.Set("Attacking", true);
						model.Set("Walking",false);
						attackt += Time.Delta;
						if(attackt > attackTime || size < qtDistance || sf.Stamina <= 0)
						{
							if(sf.Stamina <= 0)phd.hp=1000;
							attack = false;
							ml.locked = false;
							zombieUI.actv = false;
						}
						break;
					case attackTypes.spitter:
						if(canSpit) Spit();
						if(attackdt < attackDelay || ml.locked || tr.GameObject != player) attack = false;
						break;
				}
				
			}
		}
		else
		{
			agent.Stop();
		}
		lastSeen = eSS.bools[0];
		lasthp = hd.hp;
	}
	async void Spit()
	{
		canSpit = false;
		soundPointComponent.StopSound();
		soundPointComponent.StartSound();
		GameObject sp = spit.Clone(spitPoint.Transform.Position,spitPoint.Transform.Rotation);
		sp.Transform.Rotation = Rotation.LookAt(player.Transform.Position+(Vector3.Up*70)-spitPoint.Transform.Position);
		Rigidbody rb = sp.Components.Get<Rigidbody>();
		rb.Velocity = sp.Transform.World.Forward*spitForce;
		await Task.DelaySeconds(spitRate);
		canSpit=true;
	}
}

public static class MathHelper
{
    public static float ToDegrees(float radians)
    {
        return radians * (180f / (float)Math.PI);
    }
}