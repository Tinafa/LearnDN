using System;
using XUtliPoolLib;
using UnityEngine;

namespace XMainClient
{
    public class XActionComp : XComponent
    {
        public static new readonly uint uuID = XCommon.singleton.XHash("XActionComp");
        public override uint ID { get { return uuID; } }

        public float moveForce = 365f;          // Amount of force added to move the player left and right.
        public float maxSpeed = 5f;             // The fastest the player can travel in the x axis.
        public AudioClip[] jumpClips;           // Array of clips for when the player jumps.
        public float jumpForce = 1000f;			// Amount of force added when the player jumps.
        
        

        //my
        private BoxCollider2D box2d = null;
        private Rigidbody2D rig2d = null;
        private Animator animator = null;
        private XFSM FSM = null;
        private Transform transform = null;
        private Transform climbCheck = null;
        private Transform groundCheck = null;          // A position marking where to check if the player is grounded.

        private bool facingRight = true;
        private bool grounded = false;          // Whether or not the player is grounded.
        private bool climb = false;
        private bool move = false;
        private bool chop = false;
        private bool idle = false;

        private float horizontal = 0;
        private float vertical = 0;

        public override void OnAttachToHost(XObject host)
        {
            base.OnAttachToHost(host);

            rig2d = _entity.EngineObject.Rigid2d;
            box2d = _entity.EngineObject.Box2d;
            animator = _entity.EngineObject.Anim;
            transform = _entity.EngineObject.transform;
            climbCheck = _entity.EngineObject.transform.FindChild("ClimbCheck");
            groundCheck = _entity.EngineObject.transform.FindChild("GroundCheck");
            FSM = CommonObjectPool<XFSM>.Get();
        }

        protected override void EventSubscribe()
        {
            base.EventSubscribe();

            RegisterEvent(XEventDefine.XEvent_Idle, OnEventIdle);
            RegisterEvent(XEventDefine.XEvent_Move, OnEventMove);
            RegisterEvent(XEventDefine.XEvent_Chop, OnEventChop);

            FSM.RegisterState(XEnum2Int<EState>.ToInt(EState.Idle), new StateFun(IdleEnter, null, IdleExit));
            FSM.RegisterState(XEnum2Int<EState>.ToInt(EState.Move), new StateFun(MoveEnter, null, MoveUpdate, MoveExit));
            FSM.RegisterState(XEnum2Int<EState>.ToInt(EState.Climb), new StateFun(ClimbEnter, null, ClimbUpdate, ClimbExit));
            FSM.RegisterState(XEnum2Int<EState>.ToInt(EState.Chop), new StateFun(ChopEnter, null, ChopExit));
        }

        public override void Attached()
        {
            FSM.ChangeState(XEnum2Int<EState>.ToInt(EState.Idle));
        }

        public override void OnDetachFromHost()
        {
            rig2d = null;
            box2d = null;
            animator = null;
            transform = null;
            climbCheck = null;
            groundCheck = null;

            horizontal = 0;
            vertical = 0;
            FSM.Clear();
            CommonObjectPool<XFSM>.Release(FSM);
            base.OnDetachFromHost();
        }

        protected virtual bool OnEventIdle(XEventArgs e)
        {
            horizontal = 0;
            vertical = 0;
            FSM.ChangeState(XEnum2Int<EState>.ToInt(EState.Idle));
            return true;
        }

        protected virtual bool OnEventMove(XEventArgs e)
        {
            XEventMove ev = e as XEventMove;
            horizontal = ev.Horizontal;
            vertical = ev.Vertical;
            if (ev.Horizontal != 0 && Grounded)
            {
                if (!Grounded)
                {
                    FSM.ChangeState(XEnum2Int<EState>.ToInt(EState.Move));
                }
            }
            else if(ev.Vertical > 0 && CanClimbUp)
            {
                if(!climb)
                {
                    FSM.ChangeState(XEnum2Int<EState>.ToInt(EState.Climb));
                }
            }
            else if(ev.Vertical < 0 && CanClimbDown)
            {
                if (!climb)
                {
                    FSM.ChangeState(XEnum2Int<EState>.ToInt(EState.Climb));
                }
            }
            animator.SetBool("grounded", Grounded);
            animator.SetBool("canClimbUp", CanClimbUp);
            animator.SetBool("canClimbDown", CanClimbDown);
            return true;
        }

        protected virtual bool OnEventChop(XEventArgs e)
        {
            //...TODO
            return true;
        }

        #region StateFuns

        private bool Grounded
        {
            get {
                grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
                return grounded;
            }
        }

        private bool CanClimbUp
        {
            get
            {
                return Physics2D.Linecast(climbCheck.position, transform.position, 1 << LayerMask.NameToLayer("Ladder"));
            }
        }

        private bool CanClimbDown
        {
            get
            {
                return Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ladder"));
            }
        }

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
        }

        //>FixedUpdate
        void MoveUpdate()
        {
            if (horizontal * rig2d.velocity.x < maxSpeed)
                rig2d.AddForce(Vector2.right * horizontal * moveForce);

            if (Mathf.Abs(rig2d.velocity.x) > maxSpeed)
                rig2d.velocity = new Vector2(Mathf.Sign(rig2d.velocity.x) * maxSpeed, rig2d.velocity.y);

            if (horizontal > 0 && !facingRight)
                Flip();
            else if (horizontal < 0 && facingRight)
                Flip();

            horizontal = 0;

            animator.SetFloat("h", rig2d.velocity.x);
            animator.SetFloat("v", rig2d.velocity.y);

            if(rig2d.velocity.x < 0.01)
            {
                FSM.ChangeState(XEnum2Int<EState>.ToInt(EState.Idle));
            }
        }

        void ClimbEnter() {
            climb = true;
        }

        void ClimbUpdate() {

            rig2d.AddForce(Vector2.up * vertical * moveForce);

            if (Mathf.Abs(rig2d.velocity.y) > maxSpeed)
                rig2d.velocity = new Vector2(rig2d.velocity.x, Mathf.Sign(rig2d.velocity.y) * maxSpeed);

            animator.SetFloat("h", rig2d.velocity.x);
            animator.SetFloat("v", rig2d.velocity.y);
        }

        void ClimbExit() {
            climb = false;
        }

        private void Flip()
        {
            facingRight = !facingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = _entity.EngineObject.LocalScale;
            theScale.x *= -1;
            _entity.EngineObject.LocalScale = theScale;
        }

        void MoveExit()
        {
            XGameManager.instance.playerTurn = true;
            move = false;
            horizontal = 0;
            vertical = 0;
        }

        void ChopEnter()
        {
            chop = true;
        }

        void ChopUpdate(float delta)
        {
        }

        void ChopExit()
        {
            chop = false;
        }

        #endregion

        public override void Update(float fDeltaT)
        {
            if (FSM != null) FSM.Update(fDeltaT);
        }

        public override void FixedUpdate()
        {
            if (FSM != null) FSM.FixedUpdate();
        }

        private void PlayAudio(Vector3 position)
        {
            _entity.Audio.PlayActionClipAt(position);
        }
    }
}
