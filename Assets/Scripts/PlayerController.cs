using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float MoveSpeed = 10f;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("[PlayerController] No Rigidbody found on " + gameObject.name + ". Disabling PlayerController.");
            enabled = false;
        }
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        var movement = new Vector3(horizontal, 0f, vertical);
        if (movement.sqrMagnitude > 1f)
        {
            movement.Normalize();
        }

        _rigidbody.velocity = new Vector3(movement.x * MoveSpeed, _rigidbody.velocity.y, movement.z * MoveSpeed);
    }
}
