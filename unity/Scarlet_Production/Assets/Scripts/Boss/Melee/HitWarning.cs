using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HitWarning : MonoBehaviour {

    public abstract void ShowWarning(int attackAnimation);

    public abstract void HideWarning(int attackAnimation);

}
