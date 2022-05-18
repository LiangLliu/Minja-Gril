using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMaleZomble : MonoBehaviour
{
    public Vector3 targetPosition;
    public float mySpeed;
    public GameObject attackCollider;
    public int enemyLife;

    protected BoxCollider2D myBoxCollider2D;

    protected Animator myAnimator;
    protected Vector3 originPosition, tempPosition;
    protected bool isFirstIdle, isAfterBattleCheck, isAlive;

    protected GameObject myPlayer;
    protected SpriteRenderer _spriteRenderer;

    protected static readonly int Idle = Animator.StringToHash("Idle");
    protected static readonly int Attack = Animator.StringToHash("Attack");
    protected static readonly int Hurt = Animator.StringToHash("Hurt");
    protected static readonly int Die = Animator.StringToHash("Die");

    // Start is called before the first frame update
    public void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myBoxCollider2D = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        originPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        myPlayer = GameObject.Find("Player");

        isFirstIdle = true;
        isAfterBattleCheck = false;
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        MoveAndAttack();
    }

    protected virtual void MoveAndAttack()
    {
        if (!isAlive) return;
        if (Vector3.Distance(myPlayer.transform.position, transform.position) < 1.2f)
        {
            transform.localScale = myPlayer.transform.position.x <= transform.position.x 
                ? new Vector3(-1.0f, 1.0f, 1.0f) 
                : new Vector3(1.0f, 1.0f, 1.0f);

            if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
                || myAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackWait"))
            {
                return;
            }

            myAnimator.SetTrigger(Attack);
            isAfterBattleCheck = true;
            return;
        }
        else
        {
            if (isAfterBattleCheck)
            {
                if (tempPosition == targetPosition)
                {
                    StartCoroutine(TurnRight(false));
                }
                else if (tempPosition == originPosition)
                {
                    StartCoroutine(TurnRight(true));
                }

                isAfterBattleCheck = false;
            }
        }

        if (transform.position.x == targetPosition.x)
        {
            myAnimator.SetTrigger(Idle);
            tempPosition = originPosition;
            StartCoroutine(TurnRight(true));
            isFirstIdle = false;
        }
        else if (transform.position.x == originPosition.x)
        {
            if (!isFirstIdle)
            {
                myAnimator.SetTrigger(Idle);
            }

            tempPosition = targetPosition;
            StartCoroutine(TurnRight(false));
        }

        if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            transform.position = Vector3.MoveTowards(transform.position, tempPosition, mySpeed * Time.deltaTime);
        }
    }

    protected IEnumerator TurnRight(bool turnRight)
    {
        yield return new WaitForSeconds(2.0f);
        transform.localScale = turnRight ? new Vector3(1.0f, 1.0f, 1.0f) : new Vector3(-1.0f, 1.0f, 1.0f);
    }

    public void SetAttackColliderOn()
    {
        attackCollider.SetActive(true);
    }

    public void SetAttackColliderOff()
    {
        attackCollider.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PlayerAttack"))
        {
            if (enemyLife >= 1)
            {
                enemyLife--;
                myAnimator.SetTrigger(Hurt);
            }
            else
            {
                myBoxCollider2D.enabled = false;
                isAlive = false;
                myAnimator.SetTrigger(Die);
                StartCoroutine(AfterDie());
            }
        }
    }

    IEnumerator AfterDie()
    {
        yield return new WaitForSeconds(1.0f);
        _spriteRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        yield return new WaitForSeconds(1.0f);
        _spriteRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);

        yield return new WaitForSeconds(1.0f);
        Destroy(this.gameObject);
    }
}