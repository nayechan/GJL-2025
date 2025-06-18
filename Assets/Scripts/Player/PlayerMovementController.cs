using UnityEngine;

[RequireComponent(typeof(IMovable))]
public class PlayerMovementController : MonoBehaviour
{
    private IMovable movable;

    private void Awake()
    {
        movable = GetComponent<IMovable>();
    }

    private void Update()
    {
        if(Time.timeScale <= 0) return;
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector2 moveDir = new Vector2(horizontal, 0);
        movable.Move(moveDir);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            movable.Jump();
        }
    }
}