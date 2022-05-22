using UnityEngine;

namespace Enemy.EnemyFemaleZomble
{
    public class EnemyFemaleZomble : EnemyMaleZomble.EnemyMaleZomble
    {
        public float runSpeed;

        private bool _isBattleMode;

        protected override void Awake()
        {
            base.Awake();

            _isBattleMode = true;
        }

        protected override void MoveAndAttack()
        {
            if (!isAlive) return;
            if (_isBattleMode)
            {
                if (Vector3.Distance(myPlayer.transform.position, transform.position) < 4.0f)
                {
                    transform.localScale = myPlayer.transform.position.x <= transform.position.x
                        ? new Vector3(-1.0f, 1.0f, 1.0f)
                        : new Vector3(1.0f, 1.0f, 1.0f);

                    var newtarget = new Vector3(myPlayer.transform.position.x, transform.position.y,
                        transform.position.z);
                    if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                    {
                        transform.position =
                            Vector3.MoveTowards(transform.position, newtarget, runSpeed * Time.deltaTime);
                    }

                    isAfterBattleCheck = true;
                    return;
                }

                if (isAfterBattleCheck)
                {
                    if (transform.position.x > tempPosition.x || transform.position.x < tempPosition.x)
                    {
                        transform.localPosition = transform.position.x > tempPosition.x
                            ? new Vector3(-1.0f, 1.0f, 1.0f)
                            : new Vector3(1.0f, 1.0f, 1.0f);
                    }
                    else
                    {
                        if (tempPosition == targetPosition)
                        {
                            StartCoroutine(TurnRight(false));
                        }
                        else if (tempPosition == originPosition)
                        {
                            StartCoroutine(TurnRight(true));
                        }
                    }

                    isAfterBattleCheck = false;
                }
            }
            else
            {
                transform.localPosition = transform.position.x > tempPosition.x
                    ? new Vector3(-1.0f, 1.0f, 1.0f)
                    : new Vector3(1.0f, 1.0f, 1.0f);

                if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                {
                    transform.position =
                        Vector3.MoveTowards(transform.position, tempPosition, mySpeed * Time.deltaTime);
                }

                if (transform.position == tempPosition)
                {
                    _isBattleMode = true;
                }

                return;
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

        protected override void OnTriggerEnter2D(Collider2D col)
        {
            base.OnTriggerEnter2D(col);

            if (col.CompareTag("StopPoint"))
            {
                _isBattleMode = false;
            }
            
            if (col.CompareTag("PlayerAttack"))
            {
                _isBattleMode = true;
            }
        }
    }
}