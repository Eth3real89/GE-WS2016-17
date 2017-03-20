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

    public FARQ PlayBeamSound(Transform attachTo)
    {
        return createRequest("eff_beam", 0, -1, attachTo, true);
    }

    public FARQ PlayGrenadeExplosionSound(Transform attachTo)
    {
        return createRequest("eff_grenade_explode", 0, -1, attachTo, false);
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

    public FARQ PlayBulletSpawnSound(Transform attachTo)
    {
        return createRequest("eff_bullet_spawn", 0, -1, attachTo, false);
    }

    public FARQ PlayShockWaveSpawnSound(Transform attachTo)
    {
        return createRequest("eff_shockwave", 0, -1, attachTo, false);
    }

    public FARQ PlayLightGuardDespawnSound(Transform attachTo)
    {
        return createRequest("eff_light_guard_despawn", 0, -1, attachTo, false);
    }

    public FARQ PlayGatherLightTravellingSound(Transform attachTo)
    {
        return createRequest("eff_gather_light_travel", 0, -1, attachTo, true);
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
        return createRequest("eff_big_weapon_impact", 0, -1, attachTo, false);
    }

    public FARQ PlayThrowSpearSound(Transform attachTo)
    {
        return createRequest("eff_throw_spear", 0, -1, attachTo, false);
    }

    public FARQ PlayAxeOnGroundSound(Transform attachTo)
    {
        return createRequest("eff_axe_on_ground", 0, -1, attachTo, true);
    }

    public FARQ PlayScytheRotateUpwardsSound(Transform attachTo)
    {
        return createRequest("eff_scythe_rotate_upwards", 0, -1, attachTo, false);
    }

    public FARQ PlayScytheCrashDownSound(Transform attachTo)
    {
        return createRequest("eff_scythe_crash_down", 0, -1, attachTo, false);
    }

    public FARQ PlayHealSound(Transform attachTo)
    {
        return createRequest("eff_heal", 0, -1, attachTo, false);
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
