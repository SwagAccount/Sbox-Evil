using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Sandbox;

public sealed class SurvivalFeatures : Component
{
	[Property] public Curve beatTime {get;set;}
	[Property] public Curve beatVolume {get;set;}
	[Property] public Curve satuationTabbaco {get;set;}
	[Property] public Curve resistTabbaco {get;set;}
	[Property] public Curve resistTabbacoAddicted {get;set;}
	[Property] public float Highness {get;set;}
	[Property] public float HighnessReduce {get;set;}
	[Property] public float hCamSpeed {get;set;}
	[Property] public float hCamDis {get;set;}
	[Property] public float hCamDirChangeSpeed {get;set;}
	[Property] public float hallucinationAmount {get;set;}
	[Property] public float Stamina {get;set;}
	[Property] private float StaminaReduceRun {get;set;}
	[Property] private float TimeToRegen {get;set;}
	[Property] public float StaminaReduceJump {get;set;}
	[Property] public float Tabbaco {get;set;}
	[Property] public float TabbacoReduce {get;set;}
	[Property] public float TabbacoResist {get;set;}
	[Property] public float TabbacoNeedResist {get;set;}
	[Property] public float addicted {get;set;}
	[Property] public float addictedAmount {get;set;}
	[Property] private float StaminaRegen {get;set;}
	[Property] private List<SoundPointComponent> soundPointComponents {get;set;}
	[Property] private ColorAdjustments colorAdjustments {get;set;}
	[Property] private MovingObject CameraBreathe {get;set;}
	[Property] private List<GameObject> hallucinations {get;set;}
	[Property] private Movement movement {get;set;}
	[Property] private HEALTHDETECTOR hd {get;set;}
	[Property] private PlayerDeath PlayerDeath {get;set;}
	[Property] private PlayerUI PlayerUI {get;set;}
	[Property] private GameObject cam {get;set;}
	[Property] private float BreatheBase {get;set;}
	[Property] private float BreatheMult {get;set;}
	[Property] private float BreathePosBase {get;set;}
	[Property] private float BreathePosMult {get;set;}
	[Property] float regenTime;
	[Property] FagDealer fag;
	[Property] GameObject fagDrop;
	float lastStamina;
	protected override void OnStart()
	{
		heartBeat();
		hallucination();
	}
	int soundPoint;
	async void heartBeat()
	{
		while(true)
		{
			soundPoint = soundPoint == 0 ? 1 : 0;
			soundPointComponents[soundPoint].StopSound();
			soundPointComponents[soundPoint].StartSound();

			await Task.DelaySeconds(beatTime.Evaluate(Stamina));
		}
	}
	[Property] GameObject currentHallucination;
	async void hallucination()
	{
		while(true)
		{
			bool nul = true;
			if(currentHallucination != null)
			{
				nul = !currentHallucination.IsValid;
			}
			if(Highness >= hallucinationAmount && nul)
			{
				currentHallucination = hallucinations[Game.Random.Next(0,hallucinations.Count)].Clone(Transform.Position - Transform.World.Forward*20);
				currentHallucination.Transform.Position = Transform.Position - Transform.World.Forward*20;
				currentHallucination.Transform.Rotation = Rotation.LookAt(Transform.Position - currentHallucination.Transform.Position);
			}
			await Task.DelaySeconds(20);
		}
	}
	Angles HallucinateAngles;
	float cTime = 0;
	float lastTobbaco;
	protected override void OnUpdate()
	{
		Highness = MathX.Clamp(Highness-Time.Delta*(HighnessReduce*0.1f),0,1);
		if(Highness>=hallucinationAmount)
		{
			cTime+=Time.Delta*hCamDirChangeSpeed;
			if(cTime >= 1) cTime = 0;
			if(cTime<=0)
			{
				HallucinateAngles = Angles.Random*hCamDis;
			}
		}
		else
		{
			HallucinateAngles = Angles.Zero;
		}


		
		
		cam.Transform.LocalRotation = Angles.Lerp(cam.Transform.LocalRotation,HallucinateAngles,Time.Delta*hCamSpeed);
		foreach(SoundPointComponent spc in soundPointComponents) spc.Volume = beatVolume.Evaluate(PlayerDeath.hp.hp/PlayerDeath.Health);
		if(movement.IsSprinting) Stamina -= StaminaReduceRun*Time.Delta;
		if(regenTime >= TimeToRegen) Stamina += StaminaRegen*Time.Delta;
		Stamina = MathX.Clamp(Stamina,0f,1f);
		PlayerUI.staminaPercent = Stamina;
		CameraBreathe.SpeedMult = PlayerDeath.hp.hp <= PlayerDeath.Health ? BreatheBase+(BreatheMult*(1-Stamina)) : 0;
		CameraBreathe.PosMult = PlayerDeath.hp.hp <= PlayerDeath.Health ? BreathePosBase+(BreathePosBase*((PlayerDeath.Health-(PlayerDeath.Health-PlayerDeath.hp.hp))/PlayerDeath.Health)) : 0;
		hd.healthResist = addicted > addictedAmount ? resistTabbacoAddicted.Evaluate(Tabbaco) : resistTabbaco.Evaluate(Tabbaco);

		float normalized = (hd.healthResist - TabbacoNeedResist) / (TabbacoResist - TabbacoNeedResist);
		Log.Info(normalized);
		colorAdjustments.Saturation = satuationTabbaco.Evaluate(normalized);


		if(Stamina >= lastStamina)
		{
			regenTime += Time.Delta;
		}
		else regenTime = 0;
		lastStamina = Stamina;
		
		fag.progress = Tabbaco;
		Tabbaco -= Time.Delta*TabbacoReduce;
		if(Tabbaco < 0.5f && lastTobbaco > 0.5f) setFag(false);
		if(Tabbaco > lastTobbaco) setFag(true);
		if(Tabbaco < 0) Tabbaco = 0;
		lastTobbaco = Tabbaco;
	}
	void setFag(bool to)
	{
		if(fag.GameObject.Enabled)
		{
			GameObject fagDropped = fagDrop.Clone(fag.Transform.Position,fag.Transform.Rotation);
			FagDealer fagDealer = fagDropped.Components.Get<FagDealer>();
			fagDealer.progress = fag.progress;
		}
		fag.GameObject.Enabled = to;
	}
}