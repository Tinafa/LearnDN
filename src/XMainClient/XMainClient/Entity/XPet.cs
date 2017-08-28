using System;
using System.Collections;
using UnityEngine;
using XUtliPoolLib;

namespace XMainClient
{
    class XPet : XCharcacter
    {
        public float moveTime = 0.5f; // s

        private bool move = false;
        //private bool idle = false;

        private float inverseMoveTime;

        //private bool isToRight = true;

        Vector3 cachedEnd = Vector3.zero;
        Transform cachedTrans = null;


        protected override void Start()
        {
            cachedTrans = gameObject.transform;
            inverseMoveTime = 1f / moveTime;

            InitState();
            base.Start();
        }

        protected virtual void InitState()
        {
            RegisterEvent(XEventDefine.XEvent_MoveTo, OnEventMoveTo);

            RegisterState(EnumInt32ToInt.Convert<EState>(EState.Idle), new StateFun(IdleEnter, null, IdleExit));
            RegisterState(EnumInt32ToInt.Convert<EState>(EState.MoveTo), new StateFun(MoveToEnter, null, MoveToExit));
        }

        void IdleEnter()
        {
            //idle = true;
        }

        void IdleExit()
        {
            //idle = false;
        }

        void MoveToEnter()
        {
            if(!move)StartCoroutine(SmoothMovement());
            move = true;
        }

        void MoveToExit()
        {
            move = false;
        }

        protected virtual bool OnEventMoveTo(XEventArgs e)
        {
            XEventMoveTo ev = e as XEventMoveTo;
            cachedEnd = new Vector3(ev.PosX, ev.PosY, 0f);
            ChangeState(EnumInt32ToInt.Convert<EState>(EState.MoveTo));
            return true;
        }

        protected override bool OnEventIdle(XEventArgs e)
        {
            ChangeState(EnumInt32ToInt.Convert<EState>(EState.Idle));
            return true;
        }

        protected override bool OnEventMove(XEventArgs e)
        {
            XEventMove ev = e as XEventMove;
            cachedEnd = new Vector3(gameObject.transform.localPosition.x + ev.Horizontal, gameObject.transform.localPosition.y+ev.Vertical, 0f);
            ChangeState(EnumInt32ToInt.Convert<EState>(EState.Move));
            return true;
        }

        protected override bool OnEventChop(XEventArgs e)
        {
            return true;
        }


        protected IEnumerator SmoothMovement()
        {
            //float sqrRemainingDistance = (transform.position - cachedEnd).sqrMagnitude;

            while (Mathf.Abs(transform.localPosition.x-cachedEnd.x) > (1f+float.Epsilon) || Mathf.Abs(transform.localPosition.y - cachedEnd.y) > (1f + float.Epsilon))
            {
                Vector3 newPosition = Vector3.MoveTowards(cachedTrans.localPosition, cachedEnd, inverseMoveTime * Time.deltaTime);
                cachedTrans.localPosition = newPosition;

                //sqrRemainingDistance = (transform.position - cachedEnd).sqrMagnitude;
                yield return null;
            }

            ChangeState(EnumInt32ToInt.Convert<EState>(EState.Idle));
        }

        public void Think()
        {
           // XDebug.singleton.AddGreenLog("Crab 有话说！！");
        }
    }
}
