using System;
using System.Diagnostics;
using System.Dynamic;
using Microsoft.VisualBasic;
using Sandbox;

public sealed class WeaponDealer : Component
{
	[Property] public bool forceUnqequip {get; set;}
	[Property] private List<string> bulletIgnore{ get; set; }
	[Property] private GameObject empty { get; set; }
	[Property] private FollowTransform leftHand { get; set; }
	[Property] private FollowTransform rightHand { get; set; }
	[Property] public GameObject noGunHandLPos { get; set; }
	[Property] public GameObject noGunHandRPos { get; set; }
	[Property] private GameObject gunSound { get; set; }
	[Property] private BulletHoleDB bHoleDB { get; set; }
	[Property] private Inventory inv { get; set; }
	[Property] private MovementLocker ml { get; set; }
	[Property] private CameraComponent cam { get; set; }
	[Property] private float swaySmooth { get; set; }  = 0.5f;
	[Property] private float cameraBlockDis { get; set; }  = 50f;
	private float mainFov { get; set; }  = 100f;
	[Property] private float runFovAdd { get; set; }  = 10f;
	[Property] private float bulletHoleDuration { get; set; }  = 30f;
	[Property] private List<GameObject> guns { get; set; }  = new();
	[Property] private List<string> ignoredTags { get; set; }  = new();
	[Property] public GunValues gunValues;
	[Property] private Movement movement { get; set; } 
	[Property] public string selectedGun;
	string lastSG;
    bool startreload;
    Vector3 gpos;
	Angles gang;
	GameObject findGun(string Name)
	{
		for(int i = 0; i < guns.Count; i++)
		{
			if(guns[i].Name == Name) return guns[i];
		}
		return null;
	}
    [Property ] bool pauseGun = false;
    private Settings settings {get;set;}
	protected override void OnStart()
	{
		IEnumerable<GameObject> balls = Scene.GetAllObjects(true);
		foreach(GameObject go in balls)
		{
            if(go.Tags.Has("settings"))
			{
				settings = go.Components.Get<Settings>();
				break;
			}
        }
	}
	protected override void OnUpdate()
	{
        
        
        mainFov = settings.fovValue;
        if(!ml.locked && selectedGun == "")
        {
            leftHand.followed = noGunHandLPos;
            rightHand.followed = noGunHandRPos;
        }
        selectedGun = forceUnqequip ? "" :inv.weapons[inv.currentWeapon];
        if(!pauseGun)
        {
            if(gunValues!=null)
            {

                gunInput();
                GunPosition();
                if(gunValues.LoadedParam != "" && gunValues.Model != null) gunValues.Model.Set(gunValues.LoadedParam, inv.weaponsData[inv.currentWeapon].clipContent.Count > 0);
            }
            GunSelection();
        }
	}
	int ammoIndex;
	bool isReloading;
    bool cantShoot;
	float targetFov;
	int shotsFired = 0;
    Vector3 recoilOffsetPos;
    Angles recoilOffsetRot;
    
	async void gunInput()
	{
        
		ammoIndex = inv.AmmoIndex(gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].bulletType].ammoType);
		GunSaveData gunSaveData = inv.weaponsData[inv.currentWeapon];
		if (Input.Pressed("changeAmmoType") && !isReloading)
        {
            gunSaveData.bulletType++;
            if (gunSaveData.bulletType >= gunValues.bulletStats.Count)
            {
                gunSaveData.bulletType = 0;
            }
        }
        if (Input.Pressed("changeMode"))
        {
            gunSaveData.currentMode++;
            if (gunSaveData.currentMode >= gunValues.modes.Count)
            {
                gunSaveData.currentMode = 0;
            }
        }
		inv.weaponsData[inv.currentWeapon] = gunSaveData;
        if (gunValues.hasFakeProjectile)
        {
            gunValues.fakeProjectile.Enabled = inv.weaponsData[inv.currentWeapon].clipContent.Count > 0 || isReloading;
        }
		if (!gunValues.notReloadable)
        {
            if(!cantShoot && Input.Pressed("reload") && inv.ammoData[ammoIndex].ammoCount > 0)
            {
                Log.Info("Try Reload");
                toReload();
            }
        }
        if (gunValues.canShoot && Input.Pressed("attack1") && inv.weaponsData[inv.currentWeapon].clipContent.Count >= gunValues.modes[inv.weaponsData[inv.currentWeapon].currentMode].ammoNeeded && !cantShoot)
        {
            isReloading = false;
            if(gunValues.Model != null) gunValues.Model.Set(gunValues.fireParam, true);
            await Task.DelayRealtimeSeconds(gunValues.modes[inv.weaponsData[inv.currentWeapon].currentMode].timeBeforeShooting);
            Shoot();
        }
        else if (gunValues.modes[inv.weaponsData[inv.currentWeapon].currentMode].buttonHold && gunValues.canShoot && Input.Down("attack1") && inv.weaponsData[inv.currentWeapon].clipContent.Count >= gunValues.modes[inv.weaponsData[inv.currentWeapon].currentMode].ammoNeeded&& !cantShoot)
        {
            isReloading = false;
            if(gunValues.Model != null) gunValues.Model.Set(gunValues.fireParam, true);
            await Task.DelayRealtimeSeconds(gunValues.modes[inv.weaponsData[inv.currentWeapon].currentMode].timeBeforeShooting);
            Shoot();
        }
        
	}
	async void toReload()
    {
        Log.Info("Reloading");
        if (!gunValues.cantOverReload && inv.weaponsData[inv.currentWeapon].clipContent.Count >= gunValues.clipSize)
        {
            return;
        }
        isReloading = true;
		if(inv.weaponsData[inv.currentWeapon].clipContent.Count > 0)
        {
            if(gunValues.Model != null) gunValues.Model.Set(gunValues.reloadCParam, true);
        }
        else
        {
            if(gunValues.Model != null) gunValues.Model.Set(gunValues.reloadNoCParam, true);
        }
        await Task.DelayRealtimeSeconds(gunValues.reloadWarmTime);
        Reload();
    }
	public double GetRandomNumberInRange(Random random,double minNumber, double maxNumber)
	{
		return random.NextDouble() * (maxNumber - minNumber) + minNumber;
	}
	async void Reload()
    {
        Log.Info("Reload animation");
        cantShoot = false;
        shotsFired = 0;
        float length = gunValues.reloadNoCTime;
        if(inv.weaponsData[inv.currentWeapon].clipContent.Count > 0)
        {
            length = gunValues.reloadCTime;
        }
        if (isReloading)
		{
			await Task.DelayRealtimeSeconds(length);
			ResetReload();
		}
    }
    void Recoil()
    {
        Random r = new Random();
        float randomx = (float)GetRandomNumberInRange(r,gunValues.minMaxRecoilPos[0].x, gunValues.minMaxRecoilPos[1].x);
        float randomy = (float)GetRandomNumberInRange(r,gunValues.minMaxRecoilPos[0].y, gunValues.minMaxRecoilPos[1].y);
        float randomz = (float)GetRandomNumberInRange(r,gunValues.minMaxRecoilPos[0].z, gunValues.minMaxRecoilPos[1].z);
        float randompitch = (float)GetRandomNumberInRange(r,gunValues.minMaxRecoilRot[0].pitch, gunValues.minMaxRecoilRot[1].pitch);
        float  randomyaw = (float)GetRandomNumberInRange(r,gunValues.minMaxRecoilRot[0].yaw, gunValues.minMaxRecoilRot[1].yaw);
        float randomroll = (float)GetRandomNumberInRange(r,gunValues.minMaxRecoilRot[0].roll, gunValues.minMaxRecoilRot[1].roll);
        recoilOffsetPos += new Vector3(randomx,randomy,randomz);
        recoilOffsetRot += new Angles(randompitch,randomyaw,randomroll);
    }
	private Vector3 GetShotDirection()
    {
        
        Vector3 shotDirection = gunValues.tip.Transform.World.Forward;
        if (gunValues.shootFromCam)
        {
            shotDirection = cam.Transform.World.Forward;
        }
        if (gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].spreadX + gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].spreadY > 0f)
        {
			Random r = new Random();    
            float randomPosX = (float)GetRandomNumberInRange(r,-gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].spreadX, gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].spreadX); 
            float randomPosY = (float)GetRandomNumberInRange(r,-gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].spreadY, gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].spreadY);

            shotDirection += gunValues.tip.Transform.World.Up * randomPosY;
            shotDirection += gunValues.tip.Transform.World.Right * randomPosX;
        }
        return shotDirection;
    }
	async void Shoot()
    {
        if (inv.weaponsData[inv.currentWeapon].clipContent.Count <= 0 && !gunValues.notReloadable)
        {
            toReload();
            return;
        }
        if (!cantShoot)
        {
            shotsFired = 0;
        }
        else
        {
            if(gunValues.Model != null) gunValues.Model.Set(gunValues.fireParam, true);
        }
        cantShoot = true;
        Recoil();
        if(gunValues.flash!=null)
        {
            GameObject mF = gunValues.flash.Clone();
            mF.Transform.Position = gunValues.tip.Transform.World.Position;
            mF.Transform.Rotation = gunValues.tip.Transform.Rotation;
        }
        if(gunValues.sound!=null)
        {
            SoundPointComponent sP = gunSound.Clone().Components.Get<SoundPointComponent>();
            sP.Transform.Position = Transform.World.Position;
            sP.SoundEvent = gunValues.sound;
            sP.StartSound();
        }
        for (int i = 0; i < gunValues.bulletStats[(int)inv.weaponsData[inv.currentWeapon].clipContent[0]].shotsPer; i++)
        {
            Log.Info("twat");
            Vector3 rayDirection = GetShotDirection();
            if (!gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].isProjectile)
            {
                
                Vector3 rayPosition = gunValues.tip.Transform.World.Position;
                if(gunValues.shootFromCam) rayPosition = cam.Transform.Position;

				var sTR = Scene.Trace.Ray(rayPosition,rayPosition+(rayDirection * gunValues.range)).Size(1f).WithoutTags(bulletIgnore.ToArray()).UseHitboxes().Run();
                if(sTR.GameObject != null || sTR.Hitbox != null)
                {
                    float dm = 1;
                    GameObject bh = bHoleDB.FindBulletHoleByMaterial(sTR.Surface.ResourceName);
                    if(bh!=null)
                    {
                        
                        GameObject bulletHole = bh.Clone();
                        bulletHole.Transform.Position = sTR.EndPosition;
                        bulletHole.Transform.Rotation = Rotation.LookAt(-sTR.Normal);
                        GameObject parent = sTR.GameObject;
                        if ( sTR.Hitbox != null)
                        {
                            IEnumerable<string> tags = sTR.Hitbox.Tags.TryGetAll();
                            string tag = "1";
                            foreach(string s in tags)
                            {
                                tag = s;
                                Log.Info($"Damage Multiplier of {s}");
                                break; 
                            }
                            dm = tag.ToFloat();
                            try
                            {
                                SkinnedModelRenderer renderer = sTR.Hitbox.GameObject.Components.Get<SkinnedModelRenderer>();
                                parent = renderer.GetBoneObject( sTR.Hitbox.Bone );
                            }catch{}
                            
                        }

                        bulletHole.SetParent(parent);
                    }
                    
                    Log.Info($"hit :{sTR.GameObject.Name}");
                    HEALTHDETECTOR healthScript = sTR.GameObject.Components.Get<HEALTHDETECTOR>();
                    if (healthScript != null && !sTR.GameObject.Tags.Has("player"))
                    {
                        healthScript.hp += gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].damage*dm;
                        healthScript.bleedAmount += gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].bleedDamage*dm;
                    }
                    //sTR.Surface.ResourceName
                    //bulletHole.transform.parent = bulletHoleParent.transform;
                    

                    Rigidbody rb = sTR.GameObject.Components.Get<Rigidbody>();
                    if (rb != null)
                    {
                        rb.ApplyForceAt(sTR.EndPosition,rayDirection * gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].shotsForce);             
                    }
					if(sTR.Body != null)
                    {
                        sTR.Body.ApplyImpulseAt( sTR.HitPosition, sTR.Direction * gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].shotsForce * 0.0001f * sTR.Body.Mass.Clamp( 0, 200 ) );
                    }
                }
            }
            else
            {
                
                //GameObject projectile = Instantiate(gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].projectile, gunValues.tip.position, Quaternion.identity);
				GameObject projectile = gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].projectile.Clone();
                
				projectile.Transform.Position = gunValues.tip.Transform.World.Position;
                projectile.Transform.Rotation = Rotation.LookAt(rayDirection);
                if (!gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].selfPropelled) projectile.Components.Get<Rigidbody>().ApplyForce(rayDirection * gunValues.bulletStats[inv.weaponsData[inv.currentWeapon].clipContent[0]].projectileSpeed);
            }
        }
        
        shotsFired++;

        for(int b = 0; b < gunValues.modes[inv.weaponsData[inv.currentWeapon].currentMode].ammoUse; b++) inv.weaponsData[inv.currentWeapon].clipContent.RemoveAt(0);
        
		/*
        if (gunValues.muzzleFlash != null)
        {
            GameObject Muzzle = Instantiate(gunValues.muzzleFlash, gunValues.cosmeticTip.position, gunValues.cosmeticTip.rotation);
            Muzzle.GetComponent<followTransform>().followed = gunValues.cosmeticTip;
        }
        if (gunValues.audioClip != null)
        {
            GameObject sound = new GameObject("GunSound");
            AudioSource source = sound.AddComponent<AudioSource>();
            source.clip = gunValues.audioClip;
            source.volume = gunValues.audioVolume;
            source.pitch = Random.Range(gunValues.pitchRange.x, gunValues.pitchRange.y);
            source.spatialBlend = 1;
            sound.transform.position = gunValues.tip.position;
            DELETEAFTER deleteAfter = sound.AddComponent<DELETEAFTER>();
            deleteAfter.time = gunValues.audioClip.length + 0.1f;
            source.Play();
        }
        */
        if (shotsFired < gunValues.modes[inv.weaponsData[inv.currentWeapon].currentMode].shotsPerShoot)
        {
			await Task.DelaySeconds(gunValues.shootTime);
            Shoot();
        }
        else
        {
			await Task.DelaySeconds(gunValues.shootTime);
            ResetShoot();
        }

    }
	private void ResetShoot()
    {
        cantShoot = false;
        if (inv.weaponsData[inv.currentWeapon].clipContent.Count <= 0 && gunValues.letGoAtZeroAmmo)
        {
            gunValues = null;
            inv.weapons[inv.currentWeapon] = "";
            inv.weaponsData[inv.currentWeapon] = new GunSaveData();
            inv.threeDinv.deleteItem(inv.threeDinv.currentEquip);
        }
    }
	private void ResetReload()
    {
        Log.Info("Finish Reload");
        if (isReloading)
        {
            for (int i = 0; i < gunValues.reloadMount; i++)
            {
                if(inv.weaponsData[inv.currentWeapon].clipContent.Count < gunValues.clipSize && inv.ammoData[ammoIndex].ammoCount > 0)
                {
                    inv.weaponsData[inv.currentWeapon].clipContent.Add(inv.weaponsData[inv.currentWeapon].bulletType);
                    inv.ammoData[ammoIndex].ammoCount--;
                }
            }
            if (inv.weaponsData[inv.currentWeapon].clipContent.Count >= gunValues.clipSize-(gunValues.reloadTillFull ? 0 : 1) || inv.ammoData[ammoIndex].ammoCount <= 0)
            {
                inv.weaponsData[inv.currentWeapon] = inv.weaponsData[inv.currentWeapon];
                isReloading = false;
                if(gunValues.stopReload != "" && gunValues.stopReload != null) gunValues.Model.Set(gunValues.stopReload, true);
            }
            else
            {
                Reload();
            }
        }
    }

    Angles sway;
	void GunPosition()
	{
		Vector3 targetPos = Vector3.Zero;
		Angles targetAngles = Angles.Zero;
 		if(movement.IsSprinting)
		{
			targetFov = mainFov + runFovAdd;
			targetPos = gunValues.tRun.Transform.LocalPosition;
			targetAngles = gunValues.tRun.Transform.LocalRotation;
		}
		else if(Input.Down("attack2"))
		{
			targetFov = mainFov * gunValues.fovMult;
			targetPos = gunValues.tAim.Transform.LocalPosition;
			targetAngles = gunValues.tAim.Transform.LocalRotation;
		}
		else
		{
			targetFov = mainFov;
			targetPos = gunValues.tIdle.Transform.LocalPosition;
			targetAngles = gunValues.tIdle.Transform.LocalRotation;
		}
        float rayDis = Vector3.DistanceBetween(gunValues.Transform.World.Position,gunValues.tip.Transform.World.Position)+1;
        var trace = Scene.Trace.Ray(
            gunValues.Transform.Position-(movement.Transform.World.Forward*5),
            gunValues.Transform.Position+(movement.Transform.World.Forward*rayDis))
            .Size(1f)
            .IgnoreGameObject(inv.GameObject)
            .UseHitboxes()
            .Run();
       
        float A = MathF.Sqrt(rayDis+(rayDis+1));
        
        float angle = (trace.Hit && !float.IsNaN(-MathF.Acos(trace.Distance/(rayDis+1))) && !float.IsNaN(-MathF.Acos(trace.Distance/(rayDis+1)))) ? ((-MathF.Acos(trace.Distance/(rayDis+1)) * 180 / MathF.PI) - cam.GameObject.Parent.Transform.LocalRotation.Angles().pitch) : 0f;
        
        recoilOffsetPos = Vector3.Lerp(recoilOffsetPos,Vector3.Zero,gunValues.recoilReset*Time.Delta);
        recoilOffsetRot = Angles.Lerp(recoilOffsetRot,Angles.Zero,gunValues.recoilReset*Time.Delta);
        cam.FieldOfView = MathX.Lerp(cam.FieldOfView, targetFov,gunValues.posSpeed*Time.Delta);
		gpos = Vector3.Lerp(gpos, targetPos, gunValues.posSpeed*Time.Delta);
		gang = Angles.Lerp(gang, targetAngles, gunValues.rotSpeed*Time.Delta);
        sway = Angles.Lerp(sway,new Angles(Input.AnalogLook.pitch*gunValues.swayPitch*settings.MouseSens,Input.AnalogLook.yaw*settings.MouseSens*gunValues.swayYaw,0),swaySmooth*Time.Delta);
        GameObject.Children[0].Transform.LocalPosition = gpos+recoilOffsetPos;
        GameObject.Children[0].Transform.LocalRotation = gang+recoilOffsetRot+(gunValues.avoidAxis*angle)+sway;
	}
    async void changeGun()
    {
        if(gunValues!=null)
        {
            pauseGun = true;
            if(gunValues.Model != null) gunValues.Model.Set(gunValues.unDeployParam, true);
            await Task.DelayRealtimeSeconds(gunValues.deployTime);
        }
        leftHand.followed = noGunHandLPos;
        rightHand.followed = noGunHandRPos;
        if(GameObject.Children.Count > 0) GameObject.Children[0].Destroy();
        if(selectedGun != "")
        {
            GameObject newGun = findGun(selectedGun).Clone();
            newGun.Parent = GameObject;
            newGun.Transform.LocalPosition = Vector3.Zero;
            newGun.Transform.LocalRotation = Angles.Zero;
            gunValues = newGun.Components.Get<GunValues>();
            followHands();
        }
        else
        {
            gunValues = null;
        }
        pauseGun = false;
		
    }
    public void followHands()
    {
        if(gunValues!=null)
        {
            leftHand.followed = gunValues.leftHand;
            rightHand.followed = gunValues.rightHand;
        }
        
    }
	void GunSelection()
	{
		if(lastSG != selectedGun || Input.Down("Slot1"))
		{
			changeGun();
		}
		lastSG = selectedGun;
	}
}