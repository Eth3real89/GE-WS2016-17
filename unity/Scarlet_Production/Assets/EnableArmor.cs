using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableArmor : MonoBehaviour
{
    public void Prepare()
    {
        GetComponent<Animator>().SetTrigger("Prepare");
    }
}
