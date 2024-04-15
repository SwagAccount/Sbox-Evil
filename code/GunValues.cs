using Sandbox;

public sealed class GunValues : Component
{
	[Property] public SoundEvent sound {get; set;}
	[Property] public List<Vector3> minMaxRecoilPos {get; set;} = new();
	[Property] public List<Angles> minMaxRecoilRot {get; set;} = new();
	[Property] public Vector3 avoidAxis {get; set;}
	[Property] public float range {get; set;} = 52493f;
	[Property] public float recoilReset {get; set;} = 20f;
	[Property] public float swayPitch {get; set;} = 1f;
	[Property] public float swayYaw {get; set;} = 1f;
	[Property] public bool canShoot {get; set;}
	[Property] public bool shootFromCam {get; set;}
	[Property] public GameObject leftHand {get; set;}
	[Property] public GameObject rightHand {get; set;}
	[Property] public GameObject flash {get; set;}
	[Property] public GameObject tip {get; set;}
	[Property] public GameObject tIdle {get; set;}
	[Property] public GameObject tAim {get; set;}
	[Property] public GameObject tRun {get; set;}
	[Property] public float posSpeed {get; set;}
	[Property] public float rotSpeed {get; set;}
	[Property] public List<Mode> modes { get; set; }  = new();
	[Property] public List<bulletStat> bulletStats { get; set; }  = new();
	[Property] public float fovMult {get; set;} = 0.66f;
	[Property] public bool runReload {get; set;}
	[Property] public float sensMult {get; set;} = 1f;
    [Property] public int clipSize{get; set;}
    [Property] public int reloadMount {get; set;}
    [Property] public float reloadWarmTime {get; set;}
    [Property] public bool notReloadable {get; set;}
    [Property] public bool reloadTillFull {get; set;}
    [Property] public bool cantOverReload {get; set;}
    [Property] public bool letGoAtZeroAmmo {get; set;}
	[Property] public string stopReload {get; set;}
	[Property] public string fireParam {get; set;}
	[Property] public string reloadCParam {get; set;}
	[Property] public string reloadNoCParam {get; set;}
    [Property] public string unDeployParam {get; set;}
    [Property] public string LoadedParam {get; set;}
	[Property] public SkinnedModelRenderer Model {get; set;}
	[Property] public bool hasFakeProjectile {get; set;}
	[Property] public GameObject fakeProjectile {get; set;}
	[Property] public float reloadCTime {get; set;}
	[Property] public float reloadNoCTime {get; set;}
	[Property] public float shootTime {get; set;}
    [Property] public float deployTime {get; set;}

    

	[System.Serializable]
    public class Mode
    {
        [Property] public string modeName {get; set;}
        [Property] public int ammoUse {get; set;}
        [Property] public float timeBetweenEachShot {get; set;}
        [Property] public float timeBeforeShooting {get; set;}
        [Property] public bool burst {get; set;}
        [Property] public bool buttonHold {get; set;}
        [Property] public float shotsPerShoot {get; set;}
        [Property] public int ammoNeeded {get; set;}
    }
	[System.Serializable]
    public class bulletStat
    {

        [Property] public string ammoType {get; set;}
        //[Header("Stats")]
        [Property] public float shotsForce {get; set;}
        [Property] public float shotsPer {get; set;}
        [Property] public float spreadX {get; set;}
        [Property] public float spreadY {get; set;}
        //[Header("Hitscan Only")]
        [Property] public float damage {get; set;}
        [Property] public float bleedDamage {get; set;}
        //[Header("Projectile Values")]
        [Property] public bool isProjectile {get; set;}
        [Property] public GameObject projectile {get; set;}
        [Property] public bool selfPropelled {get; set;}
        [Property]public float projectileSpeed {get; set;}

    }
	protected override void OnUpdate()
	{

	}
}