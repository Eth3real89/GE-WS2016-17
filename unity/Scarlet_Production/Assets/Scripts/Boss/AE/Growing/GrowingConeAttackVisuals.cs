using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GrowingConeAttackVisuals {

    void ShowAttack();
    void HideAttack();
    void SetAngle(float angle);
    float GetAngle();
    void SetRadius(float radius);

    void ScaleTo(Vector3 scale);
    void UpdateVisuals();
}
