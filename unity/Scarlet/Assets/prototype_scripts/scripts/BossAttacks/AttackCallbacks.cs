using UnityEngine;
using System.Collections;

public interface AttackCallbacks {

    void OnAttackStart(Attack a);
    void OnAttackEnd(Attack a);

    void OnAttackParried(Attack a);
    void OnAttackCancelled(Attack a);

}
