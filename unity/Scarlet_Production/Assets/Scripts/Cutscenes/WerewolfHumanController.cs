using UnityEngine;

public class WerewolfHumanController : MonoBehaviour
{
    public void InteractBridge()
    {
        GetComponent<Animator>().SetTrigger("Interact");
    }

    public void Cower()
    {
        GetComponent<Animator>().SetTrigger("Cower");
    }

    public void HandToMoon()
    {
        GetComponent<Animator>().SetTrigger("HandToMoon");
    }

    public void Talk()
    {
        new FARQ().ClipName("werewolf").StartTime(0.3f).EndTime(7.2f).Location(Camera.main.transform).Play();
    }

    public void Disappear()
    {
        Destroy(gameObject);
    }
}
