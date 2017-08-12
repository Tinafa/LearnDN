using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMainClient
{
    public abstract class Actor : Charcacter
    {
        public float moveTime = 0.1f; //m/s
        public LayerMask blockingLayer;

        public static new readonly uint uuID = XCommon.Singleton.XHash("Actor");
        public override uint ID { get { return uuID; } }

        protected BoxCollider2D boxCollider;
        protected Rigidbody2D rb2d;

        [HideInInspector]
        public bool move = false;
        private float inverseMoveTime;

        protected virtual void Start()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            rb2d = GetComponent<Rigidbody2D>();
            inverseMoveTime = 1f / moveTime;

            blockingLayer = LayerMask.NameToLayer("BlockingLayer");
        }

        protected virtual bool Move(int xDir, int yDir, out RaycastHit2D hit)
        {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);

            boxCollider.enabled = false;
            hit = Physics2D.Linecast(start, end, 1<<blockingLayer);
            boxCollider.enabled = true;

            if (hit.transform == null)
            {
                GameManager.instance.playerTurn = false;
                move = true;
                StartCoroutine(SmoothMovement(end));
            }

            return false;
        }

        protected IEnumerator SmoothMovement(Vector3 end)
        {
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            while(sqrRemainingDistance > float.Epsilon)
            {
                Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime*Time.deltaTime);
                rb2d.MovePosition(newPosition);

                sqrRemainingDistance = (transform.position - end).sqrMagnitude;
                yield return null;
            }
            
            GameManager.instance.playerTurn = true;
            move = false;
        }

        protected virtual bool AttemptMove<T>(int xDir, int yDir) where T : IDamage
        {
            RaycastHit2D hit;
            bool canMove = Move(xDir, yDir, out hit);

            if (hit.transform == null) return canMove;

            IDamage hitComponent = hit.transform.GetComponent<IDamage>() as IDamage;

            Component hitComponent2 = hit.transform.GetComponent("Wall");
            if(!canMove && hitComponent != null)
            {
                OnCantMove<IDamage>(hitComponent);
            }
            return canMove;
        }

        protected abstract void OnCantMove<T>(T componet) where T : IDamage;
        
    }
}
