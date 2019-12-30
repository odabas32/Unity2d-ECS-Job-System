using UnityEngine;

public class DestroyParticle : MonoBehaviour {
    //Belirtilen sürede particleyi silmek için
    public float destroyTime = 1;
    void Start () {
        Destroy (gameObject, destroyTime);
    }

    void Update () {

    }
}