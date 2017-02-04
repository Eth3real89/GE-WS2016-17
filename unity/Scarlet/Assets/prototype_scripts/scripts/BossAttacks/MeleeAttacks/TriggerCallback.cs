using UnityEngine;
using System.Collections;

public interface TriggerCallback {

    void OnTriggerEnter(Collider other);
    void OnTriggerStay(Collider other);
    void OnTriggerLeave(Collider other);

}
