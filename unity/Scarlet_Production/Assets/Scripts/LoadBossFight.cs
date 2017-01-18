using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadBossFight : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        LevelManager.Instance.m_ControlMode = LevelManager.ControlMode.Combat;
        SceneManager.LoadScene("werewolf_battle_dev");

        LevelManager.Instance.QuickLoadFix();
    }
}
