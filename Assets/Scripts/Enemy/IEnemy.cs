using UnityEditor;
using UnityEngine;

public abstract class IEnemy : MonoBehaviour
{
    public void OnDeathAnimationFinish()
    {
        Destroy(gameObject);
    }

    protected virtual void Die()
    {
        Debug.Log("Enemy Died");
        // 사망 처리 (사망 애니메이션, 제거, 보상 등)
        StageManager.Instance.OnMobKilled(this);
        GetComponent<Animator>().SetTrigger("Death");
        foreach (Collider2D collider2d in GetComponents<Collider2D>())
        {
            collider2d.isTrigger = true;
        }
        GetComponent<Rigidbody2D>().gravityScale = 2;
        enabled = false;
    }
}