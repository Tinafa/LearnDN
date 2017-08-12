using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMainClient
{
    public class Player : Actor
    {
        public AudioClip moveSound1;
        public float moveSpeed = 10f;
        public float maxVelocity = 20f;

        private Animator animator;
        
        private bool isToRight = true;

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 touchOrigin = -Vector2.one;
#endif

        protected override void Start()
        {
            animator = GetComponent<Animator>();

            base.Start();
        }

        private void OnDisable()
        {

        }

        protected override void Update()
        {
            base.Update();

            if (move) return;

            int horizontal = 0;
            int vertical = 0;

#if true || UNITY_STANDALONE || UNITY_WEBPLAYER
            horizontal = (int)(Input.GetAxisRaw("Horizontal"));
            vertical = (int)(Input.GetAxisRaw("Vertical"));
            if (horizontal != 0)
            {
                vertical = 0;
            }
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            if (Input.touchCount > 0){
                Touch myTouch = Input.touches[0];
                if (myTouch.phase == TouchPhase.Began)
				{
					touchOrigin = myTouch.position;
				}
                else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					Vector2 touchEnd = myTouch.position;
					
					float x = touchEnd.x - touchOrigin.x;
					
					float y = touchEnd.y - touchOrigin.y;
					
					touchOrigin.x = -1;
					
					if (Mathf.Abs(x) > Mathf.Abs(y))
						horizontal = x > 0 ? 1 : -1;
					else
						vertical = y > 0 ? 1 : -1;
				}
            }
#endif
            if (horizontal != 0 || vertical != 0)
            {
                if((isToRight && horizontal<0)||(!isToRight && horizontal > 0))
                {
                    flip();
                }
                AttemptMove<IDamage>(horizontal, vertical);
            }
        }

       /* protected override bool Move(int xDir, int yDir, out RaycastHit2D hit)
        {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);

            boxCollider.enabled = false;
            hit = Physics2D.Linecast(start, end, blockingLayer);
            boxCollider.enabled = true;

            if (hit.transform == null)
            {
                int sign = xDir >= 0 ? 1 : -1;

                rb2d.AddForce(sign * transform.right * moveSpeed);
                if (Mathf.Abs(rb2d.velocity.x) > maxVelocity)
                {
                    int sign2 = rb2d.velocity.x >= 0 ? 1 : -1;
                    rb2d.velocity = new Vector2(sign2 * maxVelocity, rb2d.velocity.y);
                }
            }

            return false;
        }*/

        protected void flip()
        {
            isToRight = !isToRight;
            Vector3 vec = transform.localScale;
            vec.x *= -1;
            transform.localScale = vec;
        }

        protected override bool AttemptMove<T>(int xDir, int yDir)
        {
            bool canMove = base.AttemptMove<T>(xDir, yDir);

            if (canMove)
            {
                SoundManager.instance.PlaySingle(moveSound1);
            }

            CheckIfGameOver();
            return canMove;
        }

        void CheckIfGameOver()
        {

        }

        protected override void OnCantMove<T>(T component)
        {
            IDamage hitWall = component as IDamage;
            if(hitWall != null)
            {
                hitWall.Damage();
            }
            animator.SetTrigger("playerChop");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Exit")
            {
                //Invoke("Restart", restartLevelDelay);
                enabled = false;
            }
        }

        public void SetWalkClip(AudioClip clip)
        {
            moveSound1 = clip;
        }
    }
}
