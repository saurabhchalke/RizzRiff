using System.Collections;
using UnityEngine;

public class BoxBehaviour : MonoBehaviour
{

    void DestroyBox() => StartCoroutine(DestroyBoxCoroutine());

    IEnumerator DestroyBoxCoroutine()
    {
        GetComponent<Rigidbody>().useGravity = true;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
