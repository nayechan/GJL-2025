using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemySpawnPortal : MonoBehaviour
{
    [SerializeField] private GameObject portalGameObj;
    [SerializeField] private GameObject enemyGameObj;
    [field: SerializeField] public int MaxCount { get; set; }
    [SerializeField] private float startDelay = 0.3f, delay = 0.1f;
    [SerializeField] private float maxRadius = 1.0f;

    private bool isActivated = false;
    
    public UnityEvent onDestroy;

    private void OnDestroy()
    {
        onDestroy?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActivated && other.tag == "Player")
        {
            isActivated = true;
            portalGameObj.SetActive(true);
            StartCoroutine(ActivatePortal());
        }
    }

    // Start is called before the first frame update
    IEnumerator ActivatePortal()
    {
        yield return new WaitForSeconds(startDelay);
        for (int i = 0; i < MaxCount; i++)
        {
            yield return Spawn();
            yield return new WaitForSeconds(delay);
        }
        portalGameObj.GetComponent<Animator>().SetTrigger("Destroy");
    }

    public IEnumerator Spawn()
    {
        Vector2 randomness = Random.insideUnitCircle * maxRadius;
        yield return InstantiateAsync(enemyGameObj, transform.position + (Vector3)randomness, Quaternion.identity);
    }
}
