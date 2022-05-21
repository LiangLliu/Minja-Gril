using System;
using System.Collections;
using UnityEngine;

namespace Enemy.EnemyPumpkinMan
{
    public class EnemyPumpkinMan : MonoBehaviour
    {
        private bool _isAlive, _isIdle, _isJunpAttack, _isJumpUp, _isSlideAttack, _isHurt, _canBeHurt;

        public int life;
        public float attackDistance;
        public float jumpHeight;
        public float jumpOnSpeed;
        public float jumpDownSpeed;
        public float slideSpeed;
        public float fallDownSpeed;

        private GameObject _player;
        private Animator _animator;
        private BoxCollider2D _boxCollider2D;
        private Vector3 _slideTargetPosition;
        private SpriteRenderer _mySr;
        private AudioSource _audioSource;

        private static readonly int JumpUp = Animator.StringToHash("JumpUp");
        private static readonly int JumpDown = Animator.StringToHash("JumpDown");
        private static readonly int Slide = Animator.StringToHash("Slide");
        private static readonly int Hurt = Animator.StringToHash("Hurt");
        private static readonly int Die = Animator.StringToHash("Die");

        // Start is called before the first frame update
        protected void Awake()
        {
            _player = GameObject.Find("Player");
            _animator = GetComponent<Animator>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _mySr = GetComponent<SpriteRenderer>();
            _audioSource = GetComponent<AudioSource>();

            _isAlive = true;
            _isIdle = true;
            _isJunpAttack = false;
            _isJumpUp = true;
            _isSlideAttack = false;
            _isHurt = false;
            _canBeHurt = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (_isAlive)
            {
                if (_isIdle)
                {
                    LookAtPlayer();
                    if (Vector3.Distance(_player.transform.position, transform.position) <= attackDistance)
                    {
                        _isIdle = false;
                        StartCoroutine(IdleToSlideAttack());
                    }
                    else
                    {
                        _isIdle = false;
                        StartCoroutine(IdleToJumpAttack());
                    }
                }
                else if (_isJunpAttack)
                {
                    if (_isJumpUp)
                    {
                        var myTarget = new Vector3(_player.transform.position.x, jumpHeight, transform.position.z);
                        transform.position =
                            Vector3.MoveTowards(transform.position, myTarget, jumpOnSpeed * Time.deltaTime);
                        _animator.SetBool(JumpUp, true);
                    }
                    else
                    {
                        _animator.SetBool(JumpUp, false);
                        _animator.SetBool(JumpDown, true);

                        var myTarget = new Vector3(transform.position.x, -2.85f, transform.position.z);
                        transform.position =
                            Vector3.MoveTowards(transform.position, myTarget, jumpDownSpeed * Time.deltaTime);
                    }

                    if (transform.position.y == jumpHeight)
                    {
                        _isJumpUp = false;
                    }
                    else if (transform.position.y == -2.85f)
                    {
                        _isJunpAttack = false;
                        StartCoroutine(JumpDownToIdle());
                    }
                }
                else if (_isSlideAttack)
                {
                    LookAtPlayer();
                    _animator.SetBool(Slide, true);

                    transform.position =
                        Vector3.MoveTowards(transform.position, _slideTargetPosition, slideSpeed * Time.deltaTime);

                    if (transform.position == _slideTargetPosition)
                    {
                        _boxCollider2D.offset = EnemyPumpkinManConstant.OriginalBoxColliderOffset;
                        _boxCollider2D.size = EnemyPumpkinManConstant.OriginalBoxColliderSize;

                        _animator.SetBool(Slide, false);
                        _isSlideAttack = false;
                        _isIdle = true;
                    }
                }
                else if (_isHurt)
                {
                    var myTargetPosition = new Vector3(transform.position.x, -2.85f, transform.position.z);
                    transform.position =
                        Vector3.MoveTowards(transform.position, myTargetPosition, fallDownSpeed * Time.deltaTime);
                }
            }
            else
            {
                var myTargetPosition = new Vector3(transform.position.x, -2.85f, transform.position.z);
                transform.position =
                    Vector3.MoveTowards(transform.position, myTargetPosition, fallDownSpeed * Time.deltaTime);
            }
        }

        void LookAtPlayer()
        {
            transform.localScale = _player.transform.position.x > transform.position.x
                ? new Vector3(1.0f, 1.0f, 1.0f)
                : new Vector3(-1.0f, 1.0f, 1.0f);
        }

        IEnumerator IdleToSlideAttack()
        {
            yield return new WaitForSeconds(1.0f);
            _boxCollider2D.offset = EnemyPumpkinManConstant.SlideBoxColliderOffset;
            _boxCollider2D.size = EnemyPumpkinManConstant.SlideBoxColliderSize;
            _slideTargetPosition =
                new Vector3(_player.transform.position.x, transform.position.y, transform.position.z);
            LookAtPlayer();
            _isSlideAttack = true;
        }

        IEnumerator IdleToJumpAttack()
        {
            yield return new WaitForSeconds(1.0f);
            _isJunpAttack = true;
        }

        IEnumerator JumpDownToIdle()
        {
            yield return new WaitForSeconds(0.5f);
            _isIdle = true;
            _isJumpUp = true;
            _animator.SetBool(JumpUp, false);
            _animator.SetBool(JumpDown, false);
        }

        IEnumerator SetAnimatorHurtFalse()
        {
            yield return new WaitForSeconds(0.5f);
            _animator.SetBool(Hurt, false);
            _animator.SetBool(JumpUp, false);
            _animator.SetBool(JumpDown, false);
            _animator.SetBool(Slide, false);
            _isHurt = false;
            _isIdle = true;

            _mySr.material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

            yield return new WaitForSeconds(2.0f);
            _canBeHurt = true;
            _mySr.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("PlayerAttack"))
            {
                if (_canBeHurt)
                {
                    _audioSource.PlayOneShot(_audioSource.clip);
                    life--;
                    if (life >= 1)
                    {
                        _isIdle = false;
                        _isJunpAttack = false;
                        _isSlideAttack = false;

                        _isHurt = true;

                        StopCoroutine(IdleToSlideAttack());
                        StopCoroutine(IdleToJumpAttack());
                        StopCoroutine(JumpDownToIdle());

                        _animator.SetBool(Hurt, true);
                        StartCoroutine(SetAnimatorHurtFalse());
                    }
                    else
                    {
                        // die
                        _isAlive = false;
                        _boxCollider2D.enabled = false;
                        StopAllCoroutines();
                        _animator.SetBool(Die, true);
                    }

                    _canBeHurt = false;
                }
            }
        }
    }
}