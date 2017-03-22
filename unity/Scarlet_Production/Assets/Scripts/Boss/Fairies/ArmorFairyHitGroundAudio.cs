using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorFairyHitGroundAudio : MonoBehaviour {

    public void HitGround()
    {
        FancyAudioEffectsSoundPlayer.Instance.PlayArmorHitGroundSound(transform);
    }

    public void GroundImpact()
    {
        FancyAudioEffectsSoundPlayer.Instance.PlayArmorHitGroundSound(transform);
    }
}
