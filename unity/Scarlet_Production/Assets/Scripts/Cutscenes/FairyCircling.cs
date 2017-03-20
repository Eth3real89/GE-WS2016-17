using UnityEngine;
using System.Collections;

public class FairyCircling : MonoBehaviour
{
    public Transform target;

    private float distance, rotSpeed, movSpeed;

    private void Start()
    {
        RandomizeDistance();
    }

    void Update()
    {
        Vector3 relativePos = (target.position + new Vector3(0, distance, 0)) - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);

        Quaternion current = transform.localRotation;

        transform.localRotation = Quaternion.Slerp(current, rotation, rotSpeed * Time.deltaTime);
        transform.Translate(0, 0, movSpeed * Time.deltaTime);
    }

    private void RandomizeDistance()
    {
        distance = Random.Range(0.5f, 2f);
        rotSpeed = Random.Range(1.5f, 2.5f);
        movSpeed = Random.Range(3.5f, 4.5f);

        Invoke("RandomizeDistance", Random.Range(1f, 3f));
    }
}