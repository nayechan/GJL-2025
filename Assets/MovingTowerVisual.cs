using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTowerVisual : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * -2f);
        if(transform.position.y < -5f) 
            transform.Translate(Vector3.up * 10f);
    }
}
