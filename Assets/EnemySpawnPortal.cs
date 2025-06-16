using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawnPortal : MonoBehaviour
{
    [SerializeField] private GameObject enemyGameObj;
    [SerializeField] private int count;
    [SerializeField] private float startDelay = 0.3f, delay = 0.1f;
    [SerializeField] private float maxRadius = 1.0f;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);
        for (int i = 0; i < count; i++)
        {
            yield return Spawn();
            yield return new WaitForSeconds(delay);
        }
        GetComponent<Animator>().SetTrigger("Destroy");
    }

    public IEnumerator Spawn()
    {
        Vector2 randomness = Random.insideUnitCircle * maxRadius;
        yield return InstantiateAsync(enemyGameObj, transform.position + (Vector3)randomness, Quaternion.identity);
    }
}
