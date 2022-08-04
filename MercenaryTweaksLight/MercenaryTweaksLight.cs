using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MercenaryTweaksLight
{
    //This is an example plugin that can be put in BepInEx/plugins/ExamplePlugin/ExamplePlugin.dll to test out.
    //It's a small plugin that adds a relatively simple item to the game, and gives you that item whenever you press F2.

    //This attribute specifies that we have a dependency on R2API, as we're using it to add our item to the game.
    //You don't need this if you're not using R2API in your plugin, it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency(R2API.R2API.PluginGUID)]

    //This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    //We will be using 2 modules from R2API: ItemAPI to add our item and LanguageAPI to add our language tokens.
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI), nameof(ContentAddition), nameof(PrefabAPI))]

    //This is the main declaration of our plugin class. BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    //BaseUnityPlugin itself inherits from MonoBehaviour, so you can use this as a reference for what you can declare and use in your plugin class: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class MercenaryTweaksLight : BaseUnityPlugin
    {
        //The Plugin GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config).
        //If we see this PluginGUID as it is on thunderstore, we will deprecate this mod. Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "pbobbert";
        public const string PluginName = "MercenaryTweaksLight";
        public const string PluginVersion = "1.0.0";

        public int jumpsonkill;
        public bool slayerult;
        public bool slayerslash;
        public float slashwidthsize;
        public float slashdepthsize;
        public float slashheightsize;//this ability is just way to hard to hit with, which makes it way worse than the default for quickly taking out targets. 
        //If I made the projectile massive maybe it would solve it? Maybe a finite range in which it activates?

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            //There are three options for the config file to load.
            jumpsonkill = base.Config.Bind<int>(new ConfigDefinition("01 - Main", "Jumps restored on kill"), 1, new ConfigDescription("On kill, restore the following amount of jumps. Does not increase your maximum.")).Value;
            //slayerult = base.Config.Bind<bool>(new ConfigDefinition("01 - Main", "Slayer Eviscerate"), true, new ConfigDescription("Adds the Slayer tag to your eviscerate skill, which causes addition damage to low targets.")).Value;
            slayerslash = base.Config.Bind<bool>(new ConfigDefinition("01 - Main", "Slayer Slicing Winds"), true, new ConfigDescription("Adds the Slayer tag to your sclicing winds skill, which causes addition damage to low targets.")).Value;
            slashwidthsize = base.Config.Bind<float>(new ConfigDefinition("01 - Main", "Slicing Winds Size Width axis"), 7, new ConfigDescription("Increases the projectile size of slicing winds, making aiming it more akin to eviserate.")).Value;
            slashdepthsize = base.Config.Bind<float>(new ConfigDefinition("01 - Main", "Slicing Winds Size Depth axis"), 0.28f, new ConfigDescription("Increases the projectile size of slicing winds, making aiming it more akin to eviserate.")).Value;
            slashheightsize = base.Config.Bind<float>(new ConfigDefinition("01 - Main", "Slicing Winds Size Height axis"), 0.48f, new ConfigDescription("Increases the projectile size of slicing winds, making aiming it more akin to eviserate.")).Value;


            // run the subroutines
            if (jumpsonkill > 0)
            {
                GlobalEventManager.onCharacterDeathGlobal += restorejumpsonkill;
            }

            //handle the in game descriptions here so they take effect immediately
            if (slayerult)
            {
            }
            if (slayerslash)
            {
                string desc = "<style=cIsDamage>Slayer</style>. Fire a wind of blades that attack up to <style=cIsDamage>3</style> enemies for <style=cIsDamage>8x100% damage</style>, and leaving them <style=cIsUtility>Exposed</style>.";
                LanguageAPI.Add("MERC_SPECIAL_ALT1_DESCRIPTION", desc);
            }

            On.EntityStates.Merc.Evis.OnEnter += (orig, self) =>
            {
                //need to figure out what the On. does
                //EntityStates.Merc.Weapon
                //On.EntityStates.Merc.Evis
                //On.EntityStates.Merc.Evis.OnEnter += (orig, self) =>
                //{
                //    //increase the hitbox of the projectile
                //    self.hiteffectpre
                //    self.GetComponent<RoR2.Projectile.ProjectileImpactExplosion>().childrenProjectilePrefab.GetComponent<ProjectileDamage>().damageType = DamageType.BonusToLowHealth;

                //    //increase the aoe of the skill (currently set as the default, for reference)
                //    //self.projectilePrefab.GetComponent<RoR2.Projectile.ProjectileImpactExplosion>().childrenProjectilePrefab.transform.GetChild(0).localScale = new Vector3(15, 15, 15);

                //    //add the 'slayer' deal extra damage to low targets.
                //    self.projectilePrefab.GetComponent<RoR2.Projectile.ProjectileImpactExplosion>().childrenProjectilePrefab.GetComponent<ProjectileDamage>().damageType = DamageType.BonusToLowHealth;
                //    orig(self);
                //};
                if (slayerult)
                {
                }
                    orig(self);
            };

            //GameObject evisProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/EvisProjectile.prefab").WaitForCompletion();
            //evisProjectile.transform.localScale = new Vector3(10, 10, 10);
            On.EntityStates.Merc.Weapon.ThrowEvisProjectile.OnEnter += (orig, self) =>
            {
                //increase the hitbox of the projectile
                self.projectilePrefab.GetComponent<BoxCollider>().size = new Vector3(slashwidthsize, slashheightsize, slashdepthsize);
                //default values //self.projectilePrefab.GetComponent<BoxCollider>().size = new Vector3(1.92"f, 0.24f, 0.28f);

                //increase the aoe of the skill (currently set as the default, for reference)
                //self.projectilePrefab.GetComponent<RoR2.Projectile.ProjectileImpactExplosion>().childrenProjectilePrefab.transform.GetChild(0).localScale = new Vector3(15, 15, 15);

                //add the 'slayer' deal extra damage to low targets.
                if (slayerslash)
                {
                    self.projectilePrefab.GetComponent<RoR2.Projectile.ProjectileImpactExplosion>().childrenProjectilePrefab.GetComponent<ProjectileDamage>().damageType = DamageType.BonusToLowHealth;
                    orig(self);
                }
            };
        }

        //this function gets added to the onCharacterDeathGlobal code. I
        //runs a check to see if the thing that died was killed by a mercenary, if it was it restores jumps to the mercenary.
        //todo make it so the jump is only granted upon killing a marked target as an altenative to excluding eviserate kills?
        //todo right now the way I exlude the eviserate kills is report.damageInfo.inflictor != null, which seems janky at best
        public void restorejumpsonkill(DamageReport report)
        {
            if ((bool)report.attackerBody && report.attackerBody.bodyIndex == BodyCatalog.FindBodyIndex("MercBody") && report.damageInfo.inflictor != null && report.attackerBody.characterMotor.jumpCount > 0)
            {
                if (report.attackerBody.characterMotor.jumpCount >= jumpsonkill)
                {
                    report.attackerBody.characterMotor.jumpCount -= jumpsonkill;
                }
                else 
                {
                    report.attackerBody.characterMotor.jumpCount = 0;//I dont want to know what happends if you set jumpCount to a negative value
                }

            }
            //Debug.LogWarning(report.attacker);
            //Debug.LogWarning(report.attackerBody);
            //Debug.LogWarning(report.attackerMaster);
            //Debug.LogWarning(report.attackerOwnerBodyIndex);
            //Debug.LogWarning(report.damageDealt);
            //Debug.LogWarning(report.damageInfo);
            //Debug.LogWarning(report.damageInfo.attacker);
            //Debug.LogWarning(report.damageInfo.damageType);
            //Debug.LogWarning(report.damageInfo.inflictor);
        }
    }
}
