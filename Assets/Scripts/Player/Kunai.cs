using UnityEngine;

public class Kunai : MonoBehaviour
{
    private GameObject _player;
    private Rigidbody2D _rigidbody2D;

    public float kunaiSpeed = 10f;

    private void Awake()
    {
        _player = GameObject.Find("Player");
        _rigidbody2D = GetComponent<Rigidbody2D>();

        if (_player.transform.localScale.x == 1.0f)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            _rigidbody2D.AddForce(Vector2.right * kunaiSpeed, ForceMode2D.Impulse);
        }
        else if (_player.transform.localScale.x == -1.0f)
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            _rigidbody2D.AddForce(Vector2.left * kunaiSpeed, ForceMode2D.Impulse);
        }

        Destroy(gameObject, 5.0f);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.isTrigger && !col.CompareTag("StopPoint"))
        {
            Destroy(this.gameObject);
        }
    }
}