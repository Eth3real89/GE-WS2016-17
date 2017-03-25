using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FancyAudioEffectsSoundPlayer : MonoBehaviour {

    private static FancyAudioEffectsSoundPlayer _Instance;
    public static FancyAudioEffectsSoundPlayer Instance
    {
        get
        {
            return _Instance;
        }
    }

    private void Start()
    {
        if (_Instance == null)
            _Instance = this;
    }

    public FARQ PlayParriedSound(Transform attachTo)
    {
        return createRequest("eff_parried", 0, -1, attachTo, false, 1f);
    }

    public FARQ PlayBeamStartSound(Transform attachTo)
    {
        return createRequest("eff_beam_start", 0, -1, attachTo, false, 1f);
    }

    public FARQ PlayBeamLoopSound(Transform attachTo)
    {
        return createRequest("eff_beam_loop", 0, -1, attachTo, true, 1f);
    }

    public FARQ PlayBeamEndSound(Transform attachTo)
    {
        return createRequest("eff_beam_end", 0, -1, attachTo, false, 1f);
    }

    public FARQ PlayGrenadeExplosionSound(Transform attachTo)
    {
        return createRequest("eff_grenade_explode", 0, -1, attachTo, false, 1f);
    }

    public FARQ PlayPistolsReloadSound(Transform attachTo)
    {
        return createRequest("eff_pistols_reload", 0, -1, attachTo, false, 1f);
    }

    public FARQ PlayRifleReloadSound(Transform attachTo)
    {
        return createRequest("eff_rifle_reload", 0, -1, attachTo, false, 1f);
    }


    public FARQ PlayPistolsShotSound(Transform attachTo)
    {
        return createRequestUnlessPlaying("eff_pistols_shot", 0, 0.48f, attachTo, false, 1f);
    }

    public FARQ PlayMagInsertSound(Transform attachTo)
    {
        return createRequest("eff_mag_insert", 0, -1, attachTo, false, 1f);
    }

    public FARQ PlayRifleShotSound(Transform attachTo)
    {
        return createRequest("eff_rifle_shot", 0, -1, attachTo, false, 1f);
    }

    public FARQ PlayBulletSpawnSound(Transform attachTo, float volume = 1f)
    {
        return createRequest("eff_bullet_spawn", 3f, 3.9f, attachTo, false, volume);
    }

    public FARQ PlayShockWaveSpawnSound(Transform attachTo)
    {
        return createRequest("eff_shockwave", 0.65f, -1, attachTo, false, 1f);
    }

    public FARQ PlayLightGuardSpawnSound(Transform attachTo)
    {
        return createRequest("eff_light_guard_spawn", 0, 1, attachTo, false, 1f);
    }

    public FARQ PlayLightGuardDespawnSound(Transform attachTo)
    {
        return createRequest("eff_light_guard_despawn", 5, -1, attachTo, false, 1f);
    }

    public FARQ PlayGatherLightTravellingSound(Transform attachTo)
    {
        return createRequest("eff_gather_light_travel", 0, -1, attachTo, true, 1f);
    }

    public FARQ PlayHoverDashSound(Transform attachTo)
    {
        return createRequest("eff_hover_dash", 0, -1, attachTo, false);
    }

    public FARQ PlayWeaponSpawnSound(Transform attachTo)
    {
        return createRequest("eff_weapon_spawn", 0, -1, attachTo, false);
    }

    public FARQ PlayBigWeaponImpactSound(Transform attachTo)
    {
        return createRequest("eff_big_weapon_impact", 0, -1, attachTo, false, 1f);
    }

    public FARQ PlayThrowSpearSound(Transform attachTo)
    {
        return createRequest("eff_throw_spear", 0, -1, attachTo, false, 1f);
    }

    public FARQ PlayAxeOnGroundSound(Transform attachTo)
    {
        return createRequest("eff_axe_on_ground", 0, -1, attachTo, true, 1f);
    }

    public FARQ PlayScytheRotateUpwardsSound(Transform attachTo)
    {
        return createRequest("eff_scythe_rotate_upwards", 0.2f, -1, attachTo, true, 1f);
    }

    public FARQ PlayScytheCrashDownSound(Transform attachTo)
    {
        return createRequest("eff_scythe_crash_down", 0.7f, -1, attachTo, false, 1f);
    }

    public FARQ PlayHealSound(Transform attachTo)
    {
        return createRequest("eff_heal", 0, -1, attachTo, false, 1f);
    }

    public FARQ PlayBladeSlashSound(Transform attachTo)
    {
        return createRequest("eff_blade_slash", 0, -1, attachTo, false, 1f);
    }

    public FARQ PlayBodyHitGroundSound(Transform attachTo)
    {
        return createRequest("eff_body_hit_ground", 0, -1, attachTo, false, 1f);
    }

    public FARQ PlayArmorHitGroundSound(Transform attachTo)
    {
        return createRequest("eff_armor_hit_ground", 0, -1, attachTo, false, 1f);
    }

    public FARQ PlayBulletBlockedAudio(Transform attachTo)
    {
        return createRequest("eff_bullet_blocked", 0.6f, -1, attachTo, false, 1f);
    }

    protected FARQ createRequest(string clipName, float start, float end, Transform attachTo, bool loop, float volume = 0f)
    {
        FARQ f = new FARQ().ClipName(clipName).StartTime(start).EndTime(end).Location(attachTo).Loop(loop).Volume(volume);
        f.Play();
        return f;
    }

    protected FARQ createRequestUnlessPlaying(string clipName, float start, float end, Transform attachTo, bool loop, float volume = 0f)
    {
        FARQ f = new FARQ().ClipName(clipName).StartTime(start).EndTime(end).Location(attachTo).Loop(loop).Volume(volume);
        f.PlayUnlessPlaying();
        return f;
    }

}
