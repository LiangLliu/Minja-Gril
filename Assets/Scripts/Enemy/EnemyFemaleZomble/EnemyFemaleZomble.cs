using UnityEngine;

namespace Enemy.EnemyFemaleZomble
{
    public class EnemyFemaleZomble : EnemyMaleZomble.EnemyMaleZomble
    {
        public float runSpeed;

        protected override void MoveAndAttack()
        {
            if (!isAlive) return;
            if (Vector3.Distance(myPlayer.transform.position, transform.position) < 4.0f)
            {
                transform.localScale = myPlayer.transform.position.x <= transform.position.x
                    ? new Vector3(-1.0f, 1.0f, 1.0f)
                    : new Vector3(1.0f, 1.0f, 1.0f);

                var newtarget = new Vector3(myPlayer.transform.position.x, transform.position.y, transform.position.z);
                if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                {
                    transform.position = Vector3.MoveTowards(transform.position, newtarget, runSpeed * Time.deltaTime);
                }

                isAfterBattleCheck = true;
                return;
            }

            if (isAfterBattleCheck)
            {
                if (transform.position.x > tempPosition.x || transform.position.x < tempPosition.x)
                {
                    if (transform.position.x > tempPosition.x)
                    {
                        transform.localPosition = new Vector3(-1.0f, 1.0f, 1.0f);
                    }
                    else if (transform.position.x < tempPosition.x)
                    {
                        transform.localPosition = new Vector3(1.0f, 1.0f, 1.0f);
                    }
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
    }
}