using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMainClient
{
    public abstract class XActor : XCharcacter
    {
        public float moveTime = 0.1f; //m/s
        public int blockingLayer;
        public int atk = 1;

        protected BoxCollider2D boxCollider;
        protected Rigidbody2D rb2d;

        private bool move = false;
        private bool chop = false;
        private bool idle = false;

        private float inverseMoveTime;

        private bool isToRight = true;


        private XBaseObject cachedDamageUnit = null;
        private Vector2 cachedEnd;

        public bool IsMoving
        {
            get
            {
                return move;
            }
        }

        protected override void Start()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            rb2d = GetComponent<Rigidbody2D>();
            inverseMoveTime = 1f / moveTime;

            blockingLayer = 1<<LayerMask.NameToLayer("BlockingLayer");

            InitState();

            base.Start();
        }

        protected virtual void InitState()
        {
            RegisterState(EnumInt32ToInt.Convert<EState>(EState.Idle), new StateFun(IdleEnter, null, IdleExit));
            RegisterState(EnumInt32ToInt.Convert<EState>(EState.Move), new StateFun(MoveEnter, null, MoveExit));
            RegisterState(EnumInt32ToInt.Convert<EState>(EState.Chop), new StateFun(ChopEnter, null, ChopExit));
        }

        #region StateFuns

        void IdleEnter()
        {
            idle = true;
        }

        void IdleExit()
        {
            idle = false;
        }

        void MoveEnter()
        {
            XGameManager.instance.playerTurn = false;
            move = true;
            StartCoroutine(SmoothMovement(cachedEnd));
        }

        void MoveExit()
        {
            XGameManager.instance.playerTurn = true;
            move = false;
        }

        void ChopEnter()
        {
            chop = true;
            OnChopEnter();
        }

        void ChopUpdate(float delta)
        {
            //OnChopUpdate(delta);
        }

        void ChopExit()
        {
            cachedDamageUnit = null;
            chop = false;
        }

        #endregion


        #region virtual
        protected virtual void OnChopEnter() { }
        protected virtual void OnChopUpdate(float delta) { }
        #endregion

        #region Call For Spr
        public void OnChopDamage()
        {
            XEventChopDamage evt = XEventPool<XEventChopDamage>.GetEvent();
            evt.Firer = cachedDamageUnit;
            evt.Attacker = this;
            evt.DamageNum = atk;
            XEventMgr.singleton.FireEvent(evt);

            //Chop End!
            ChangeState(EnumInt32ToInt.Convert<EState>(EState.Idle));
        }
        #endregion

        #region Events
        protected override bool OnEventIdle(XEventArgs e)
        {
            ChangeState(EnumInt32ToInt.Convert<EState>(EState.Idle));
            return true;
        }

        protected override bool OnEventMove(XEventArgs e)
        {
            if (move)
                return true;

            XEventMove ev = e as XEventMove;
            if ((isToRight && ev.Horizontal < 0) || (!isToRight && ev.Horizontal > 0))
            {
                flip();
            }

            RaycastHit2D  hit;

            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(ev.Horizontal, ev.Vertical);

            boxCollider.enabled = false;
            hit = Physics2D.Linecast(start, end, blockingLayer);
            boxCollider.enabled = true;

            if (hit.transform == null)
            {
                cachedEnd = end;
                ChangeState(EnumInt32ToInt.Convert<EState>(EState.Move));
            }
            else
            {
                XBaseObject hitComponent = hit.transform.GetComponent<XBaseObject>() as XBaseObject;

                if ( hitComponent != null)
                {
                    XEventChop evt = XEventPool<XEventChop>.GetEvent();
                    cachedDamageUnit = hitComponent;
                    OnEventChop(evt);
                }
            }

            return true;
        }

        protected override bool OnEventChop(XEventArgs e)
        {
            if(!chop)
                ChangeState(EnumInt32ToInt.Convert<EState>(EState.Chop));

            return true;
        }

        #endregion

        protected void flip()
        {
            isToRight = !isToRight;
            Vector3 vec = transform.localScale;
            vec.x *= -1;
            transform.localScale = vec;
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
            
            ChangeState(EnumInt32ToInt.Convert<EState>(EState.Idle));
        }
        
    }
}
