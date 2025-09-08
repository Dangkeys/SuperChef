using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerPhysics : MonoBehaviour
{
    public float pushForce = 5f;
    CharacterController cc;

    void Awake() => cc = GetComponent<CharacterController>();
    void OnControllerColliderHit(ControllerColliderHit hit) {
        var rb = hit.rigidbody;
        if (rb == null || rb.isKinematic) return;

        // Only push mostly-horizontal impacts
        var pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
        rb.AddForce(pushDir.normalized * pushForce, ForceMode.Impulse);
    }
}
