using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyControl : MonoBehaviour {

    private GameControl _gameControl;

    [SerializeField] private int health;
    private ParticleSystem.MainModule pMain;

    [Header ("Particles")]
    public GameObject[] Particles;

    void Start () {

        _gameControl = GameObject.FindGameObjectWithTag ("gamecontrol").GetComponent<GameControl> ();
        health = _gameControl.BodyList.Find (c => c.transform == gameObject.transform).health; // Circle nin toplam canını gamecontrol de bulunan bodylist den aldık
    }

    void OnTriggerEnter2D (Collider2D col) {
        //çarpışma ve yok olma olayları
        if (col.gameObject.tag == "body") {

            if (health <= 0) {
                createParticle (false);
                StartCoroutine ("DestroyAndDivision");
            }

            createParticle (true);
            health--;

        }
    }

    private void createParticle (bool isCollision) {

        if (isCollision) {
            GameObject collisionParticle = Instantiate (Particles[0], gameObject.transform.position, Quaternion.identity);
            pMain = collisionParticle.GetComponent<ParticleSystem> ().main;
            pMain.startSize = transform.localScale.x;
            collisionParticle.transform.parent = gameObject.transform;
        } else {
            GameObject destroyParticle = Instantiate (Particles[1], gameObject.transform.position, Quaternion.identity);
            pMain = destroyParticle.GetComponent<ParticleSystem> ().main;
            pMain.startSize = transform.localScale.x;
            destroyParticle.transform.parent = gameObject.transform;
        }
    }

    IEnumerator DestroyAndDivision () {//yeni circle spawnlanması için gamecontrolde bulunan divisionBody fonksiyonunu çağırdık
        yield return new WaitForSeconds (0.1f);

        Destroy (gameObject);

        _gameControl.divisionBody ();
    }

}