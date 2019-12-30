using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class GameControl : MonoBehaviour {

    public Transform pfBody;
    public int maxBody = 50;

    public GameObject refDivisionParticle;

    public TextMeshProUGUI txtTotal;

    [Header ("Body Settings")]
    public int maxHealth = 5;

    public List<Body> BodyList;

    private void Start () {
        //Başlangıçta belirttiğimiz kadar rasgele konumlara circle spawnladık
        BodyList = new List<Body> ();
        for (int i = 0; i < maxBody; i++) {
            Transform bodyTransform = Instantiate (pfBody, new Vector3 (UnityEngine.Random.Range (-8f, 8f), UnityEngine.Random.Range (-5f, 5f)), Quaternion.identity);

            float bodyScale = UnityEngine.Random.Range (0.2f, 1f);
            bodyTransform.transform.localScale = new Vector3 (bodyScale, bodyScale, 0);

            bodyTransform.transform.parent = GameObject.FindWithTag ("bodies").transform;

            BodyList.Add (new Body {
                transform = bodyTransform,
                    health = maxHealth,
                    lScale = bodyScale,
                    moveY = UnityEngine.Random.Range (1f, 2f)
            });
        }
    }

    private void Update () {
        float startTime = Time.realtimeSinceStartup;

        //Objenin sürekli hareket halinde olmasını sağlamak için gerekli tanımları yaptık
        NativeArray<float> moveYArray = new NativeArray<float> (BodyList.Count, Allocator.TempJob);
        TransformAccessArray transformAccessArray = new TransformAccessArray (BodyList.Count);

        for (int i = 0; i < BodyList.Count; i++) {
            moveYArray[i] = BodyList[i].moveY;
            transformAccessArray.Add (BodyList[i].transform);
        }

        ReallyToughParallelJobTransforms reallyToughParallelJobTransforms = new ReallyToughParallelJobTransforms {
            deltaTime = Time.deltaTime,
            moveYArray = moveYArray,
        };

        JobHandle jobHandle = reallyToughParallelJobTransforms.Schedule (transformAccessArray);
        jobHandle.Complete ();

        for (int i = 0; i < BodyList.Count; i++) {

            BodyList[i].moveY = moveYArray[i];
        }

        moveYArray.Dispose ();
        transformAccessArray.Dispose ();

        txtTotal.text = "TOTAL\n" + GameObject.FindGameObjectWithTag ("bodies").transform.childCount.ToString (); //Obje sayısını güncel tuttuk
    }

//Yeni obje spawnı
    public void divisionBody () {

        StartCoroutine ("rndSpawnBody");

    }

    IEnumerator rndSpawnBody () {

        float bodyScale = UnityEngine.Random.Range (0.2f, 1f);
        Vector3 vcRndBody = new Vector3 (UnityEngine.Random.Range (-8f, 8f), UnityEngine.Random.Range (-5f, 5f));
        GameObject divisionParticle = Instantiate (refDivisionParticle, vcRndBody, Quaternion.identity);
        ParticleSystem.MainModule pMain = divisionParticle.GetComponent<ParticleSystem> ().main;
        pMain.startSize = bodyScale;

        yield return new WaitForSeconds (2);

        Transform bodyTransform = Instantiate (pfBody, vcRndBody, Quaternion.identity);
        bodyTransform.transform.localScale = new Vector3 (bodyScale, bodyScale, 0);
        bodyTransform.transform.parent = GameObject.FindWithTag ("bodies").transform;
        BodyList.Add (new Body {
            transform = bodyTransform,
                health = maxHealth,
                lScale = bodyScale,
                moveY = UnityEngine.Random.Range (1f, 2f)
        });

    }
}

[BurstCompile]
public struct ReallyToughParallelJobTransforms : IJobParallelForTransform {

    public NativeArray<float> moveYArray;
    [ReadOnly] public float deltaTime;

    public void Execute (int index, TransformAccess transform) {
        transform.position += new Vector3 (0, moveYArray[index] * deltaTime, 0f);
        if (transform.position.y > 4.5f) {
            moveYArray[index] = -math.abs (moveYArray[index]);
        }
        if (transform.position.y < -4.5f) {
            moveYArray[index] = +math.abs (moveYArray[index]);
        }

        float value = 0f;
        for (int i = 0; i < 1000; i++) {
            value = math.exp10 (math.sqrt (value));
        }
    }

}