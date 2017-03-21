using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitGroundAudio : MonoBehaviour {

    public void HitGround()
    {
        FancyAudioEffectsSoundPlayer.Instance.PlayBodyHitGroundSound(transform);
    }
}
