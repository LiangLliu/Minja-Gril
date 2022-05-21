using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float mySpeed = 5f;
    public float jumpForce;
    public GameObject attackCollider, kunaiPreFab;

    [HideInInspector] public Animator myAnimator;

    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int AttackThrow = Animator.StringToHash("AttackThrow");

    private Rigidbody2D _myRigidbody2D;
    private float _kunaiDistance;
    private SpriteRenderer _spriteRenderer;
    
    public AudioClip[] audioClips;

    private AudioSource _audioSource;

    private int _playerLife;

    [HideInInspector] public bool isJumpPressed, canJump, isAttack, isHart, canBeHurt;
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int Dead = Animator.StringToHash("Dead");

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        _myRigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();

        isJumpPressed = false;
        canJump = true;
        isAttack = false;
        isHart = false;
        canBeHurt = true;
        _playerLife = 3;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump && !isHart)
        {
            isJumpPressed = true;
            canJump = false;
        }

        // 攻击
        if (Input.GetKeyDown(KeyCode.T))
        {
            myAnimator.SetTrigger(Attack);
            isAttack = true;
            canJump = false;
        }

        // 投掷
        if (Input.GetKeyDown(KeyCode.G))
        {
            myAnimator.SetTrigger(AttackThrow);
            isAttack = true;
            canJump = false;
        }
    }

    public void PlaySwordEffect()
    {
        _audioSource.PlayOneShot(audioClips[3]);
    }

    public void PlayKunaiEffect()
    {
        _audioSource.PlayOneShot(audioClips[2]);
    }

    private void FixedUpdate()
    {
        var horizontalMove = Input.GetAxisRaw("Horizontal");

        if (isAttack || isHart)
        {
            horizontalMove = 0;
        }

        if (horizontalMove > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (horizontalMove < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        myAnimator.SetFloat(Run, Mathf.Abs(horizontalMove));

        if (isJumpPressed)
        {
            _myRigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumpPressed = false;
            myAnimator.SetBool(Jump, true);
        }

        if (!isHart)
        {
            _myRigidbody2D.velocity = new Vector2(horizontalMove * mySpeed, _myRigidbody2D.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            OnEnemy();
        }     
        
        if (col.CompareTag("Item"))
        {
            _audioSource.PlayOneShot(audioClips[1]);
            Destroy(col.gameObject);
        }
    }

    private void OnEnemy()
    {
        if (!isHart && canBeHurt)
        {
            _audioSource.PlayOneShot(audioClips[0]);
            
            _playerLife--;
            if (_playerLife >= 1)
            {
                isHart = true;
                canBeHurt = false;

                SetSpriteRendererAlpha(0.5f);

                myAnimator.SetBool(Hurt, true);

                if (transform.localScale.x == 1.0f)
                {
                    _myRigidbody2D.velocity = new Vector2(-2.5f, 10.0f);
                }
                else if (transform.localScale.x == -1.0f)
                {
                    _myRigidbody2D.velocity = new Vector2(2.5f, 10.0f);
                }

                StartCoroutine(nameof(SetIsHurtFalse));
            }
            else
            {
                isHart = true;
                isAttack = true;
                _myRigidbody2D.velocity = new Vector2(0f, 0f);
                myAnimator.SetBool(Dead, true);
            }
            
        }
    }

    IEnumerator SetIsHurtFalse()
    {
        yield return new WaitForSeconds(1.0f);
        isHart = false;
        myAnimator.SetBool(Hurt, false);

        // 无敌状态
        yield return new WaitForSeconds(1.0f);
        canBeHurt = true;
        SetSpriteRendererAlpha(1.0f);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            OnEnemy();
        }
    }

    private void SetSpriteRendererAlpha(float alpha)
    {
        var color = _spriteRenderer.color;
        color = new Color(color.r, color.g, color.b, alpha);
        _spriteRenderer.color = color;
    }

    public void SetAttackFalse()
    {
        isAttack = false;
        canJump = true;
        myAnimator.ResetTrigger(Attack);
        myAnimator.ResetTrigger(AttackThrow);
    }

    public void ForIsHurtSetting()
    {
        isAttack = false;
        myAnimator.ResetTrigger(Attack);
        myAnimator.ResetTrigger(AttackThrow);
        SetAttackColliderOff();
    }

    public void SetAttackColliderOn()
    {
        attackCollider.SetActive(true);
    }

    public void SetAttackColliderOff()
    {
        attackCollider.SetActive(false);
    }

    public void KunaiInstantiate()
    {
        if (transform.localScale.x == 1.0f)
        {
            _kunaiDistance = 1.0f;
        }
        else if (transform.localScale.x == -1.0f)
        {
            _kunaiDistance = -1.0f;
        }

        var temp = new Vector3(transform.position.x + _kunaiDistance, transform.position.y, transform.position.z);
        Instantiate(kunaiPreFab, temp, Quaternion.identity);
    }
}