using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeSoundEffects : MonoBehaviour {

    public void BladeAttackStart() {
        FancyAudioEffectsSoundPlayer.Instance.PlayBladeSlashSound(transform);
    }

}