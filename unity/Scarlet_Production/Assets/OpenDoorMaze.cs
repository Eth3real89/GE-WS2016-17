using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorMaze : Interactor
{
    public override void Interact()
    {
        GetComponent<LoadBossFight>().LoadNextScene();
    }
}
