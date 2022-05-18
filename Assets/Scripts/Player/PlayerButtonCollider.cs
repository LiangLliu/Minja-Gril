using UnityEngine;

public class PlayerButtonCollider : MonoBehaviour
{
    private Player _playerScript;
    private static readonly int Jump = Animator.StringToHash("Jump");

    private void Awake()
    {
        _playerScript = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Ground"))
        {

            _playerScript.canJump = true;
            _playerScript.myAnimator.SetBool(Jump, false);
        }
    }
}