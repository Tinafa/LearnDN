using System;
using System.Collections;
using XUtliPoolLib;
using UnityEngine;
using XDragonBones;

namespace XMainClient
{
    public class XActionComp : XComponent
    {
        public static new readonly uint uuID = XCommon.singleton.XHash("XActionComp");
        public override uint ID { get { return uuID; } }

        //DragonBones
        private const string NORMAL_ANIMATION_GROUP = "normal";
        private IXUniArmatureComp armatureComponent = null;
        private IXArmature armature = null;

        private const float NORMALIZE_MOVE_SPEED = 0.03f;
        private const float MAX_MOVE_SPEED_FRONT = 0.042f;
        private const float MAX_CLIMB_SPEED = 0.042f;
        private const float MOVE_SCALE = 1.4f;
        private const float CLIMB_SCALE = 1.4f;

        private string idleAniName = "stand";
        private string climbAniName = "walk";
        private string climbIdleAniName = "stand";
        private string walkAniName = "walk";
        private string attackAniName = "atc";

        private BoxCollider2D box2d = null;
        private XFSM FSM = null;

        /// <summary>
        ///          upClimbCheck
        /// leftCheck -- root -- rightCheck
        ///              foot
        ///           groundCheck
        /// </summary>
        /// 
        private Transform transform = null;
        private Transform upClimbCheck = null;
        private Transform root = null;
        private Transform foot = null;
        private Transform groundCheck = null;
        private Transform leftCheck = null;
        private Transform rightCheck = null;

        private int LayerGround = 0;
        private int LayerLadder = 0;
        private int LayerTurn = 0;
        private int LayerWall = 0;

        private RaycastHit2D hit;
        private bool needModify = false;
        private float modifyY = 0f;

        private IXAnimationState _walkState = null;
        private IXAnimationState _idleState = null;
        private IXAnimationState _climbState = null;
        private IXAnimationState _climbIdleState = null;
        private IXAnimationState _attackState = null;
        private Vector2 _speed = new Vector2();

        private int facingDir = -1;

        private bool grounded = false;
        private bool climb = false;
        private bool move = false;
        private bool chop = false;
        private bool idle = false;

        private float horizontal = 0;
        private float vertical = 0;

        public override void OnAttachToHost(XObject host)
        {
            base.OnAttachToHost(host);

            box2d = _entity.EngineObject.Box2d;
            transform = _entity.EngineObject.transform;
            upClimbCheck = _entity.EngineObject.transform.FindChild("UpClimbCheck");
            root = _entity.EngineObject.transform.FindChild("Root");
            foot = _entity.EngineObject.transform.FindChild("Foot");
            groundCheck = _entity.EngineObject.transform.FindChild("GroundCheck");
            leftCheck = _entity.EngineObject.transform.FindChild("LeftCheck");
            rightCheck = _entity.EngineObject.transform.FindChild("RightCheck");

            LayerGround = LayerMask.NameToLayer("Ground");
            LayerLadder = LayerMask.NameToLayer("Ladder");
            LayerTurn = LayerMask.NameToLayer("Turn");
            LayerWall = LayerMask.NameToLayer("Wall");

            armatureComponent = _entity.EngineObject.transform.GetComponent("XUnityArmatureComp") as IXUniArmatureComp;
            armature = armatureComponent.armature;
        }

        protected override void EventSubscribe()
        {
            base.EventSubscribe();

            RegisterEvent(XEventDefine.XEvent_Idle, OnEventIdle);
            RegisterEvent(XEventDefine.XEvent_Move, OnEventMove);
            RegisterEvent(XEventDefine.XEvent_Chop, OnEventChop);

            FSM = CommonObjectPool<XFSM>.Get();
            FSM.RegisterState(XEnum2Int<EState>.ToInt(EState.Idle), new StateFun(IdleEnter, null, IdleExit));
            FSM.RegisterState(XEnum2Int<EState>.ToInt(EState.Move), new StateFun(MoveEnter, MoveUpdate, MoveExit));
            FSM.RegisterState(XEnum2Int<EState>.ToInt(EState.Climb), new StateFun(ClimbEnter,ClimbUpdate, ClimbExit));
            FSM.RegisterState(XEnum2Int<EState>.ToInt(EState.Chop), new StateFun(ChopEnter, ChopUpdate, ChopExit));
        }

        public override void Attached()
        {
            FSM.ChangeState(XEnum2Int<EState>.ToInt(EState.Idle));
        }

        public override void OnDetachFromHost()
        {
            box2d = null;
            upClimbCheck = null;
            root = null;
            foot = null;
            groundCheck = null;
            leftCheck = null;
            rightCheck = null;
            needModify = false;

            horizontal = 0;
            vertical = 0;
            FSM.Clear();
            CommonObjectPool<XFSM>.Release(FSM);
            base.OnDetachFromHost();
        }

        protected virtual bool OnEventIdle(XEventArgs e)
        {
            if (idle) return true;
            if (needModify) return true;
            horizontal = 0;
            vertical = 0;
            FSM.ChangeState(XEnum2Int<EState>.ToInt(EState.Idle));
            return true;
        }

        protected virtual bool OnEventMove(XEventArgs e)
        {
            if (needModify) return true;

            XEventMove ev = e as XEventMove;
            horizontal = ev.Horizontal;
            vertical = ev.Vertical;

            bool bMove = false;
            if(horizontal != 0)
            {
                //> 在攀爬状态
                if(climb)
                {
                    //> 处于楼梯地面交叉区
                    if(IsInsection)
                    {
                        needModify = true;
                        modifyY = transform.localPosition.y-(transform.position.y - hit.transform.position.y);
                        FSM.ChangeState(XEnum2Int<EState>.ToInt(EState.Move));
                    }
                }else
                {
                    if(Grounded)
                    {
                        if(!move) FSM.ChangeState(XEnum2Int<EState>.ToInt(EState.Move));
                        bMove = true;
                    }                   
                }
            }

            //> 垂直方向输入
            if (!bMove && vertical != 0f)
            {
                if ((vertical > 0 && CanClimbUp && !climb)
                    || (vertical < 0 && CanClimbDown && !climb))
                {
                    FSM.ChangeState(XEnum2Int<EState>.ToInt(EState.Climb));
                }
            }

            return true;
        }

        protected virtual bool OnEventChop(XEventArgs e)
        {
            if (needModify) return true;
            if (chop) return true;

            FSM.ChangeState(XEnum2Int<EState>.ToInt(EState.Chop));

            return true;
        }

        #region StateFuns

        private bool IsInsection
        {
            get
            {
                hit = Physics2D.Linecast(root.position, foot.position, 1 << LayerTurn);
                return hit;
            }
        }

        private bool Grounded
        {
            get {
                grounded = Physics2D.Linecast(foot.position, groundCheck.position, 1 << LayerGround);
                return grounded;
            }
        }

        private bool CanClimbUp
        {
            get
            {
                return Physics2D.Linecast(upClimbCheck.position, root.position, 1 << LayerLadder);
            }
        }

        private bool CanClimbDown
        {
            get
            {
                return Physics2D.Linecast(foot.position, groundCheck.position, 1 << LayerLadder);
            }
        }

        /// <summary>
        /// dir 0=左 1=右 2=上 3=下 4=左右 5=上下
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private bool IsColliderWall(int dir)
        {
            switch(dir)
            {
                case 0: return Physics2D.Linecast(leftCheck.position, root.position, 1 << LayerWall);
                case 1: return Physics2D.Linecast(rightCheck.position, root.position, 1 << LayerWall);
                case 2: return Physics2D.Linecast(upClimbCheck.position, root.position, 1 << LayerWall);
                case 3: return Physics2D.Linecast(groundCheck.position, root.position, 1 << LayerWall);
                case 4: return Physics2D.Linecast(leftCheck.position, rightCheck.position, 1 << LayerWall);
                case 5: return Physics2D.Linecast(upClimbCheck.position, groundCheck.position, 1 << LayerWall);
            }
            return true;
        }

        void IdleEnter()
        {
            idle = true;

            _idleState = armature.xanimation.XFadeIn(idleAniName, -1.0f, -1, 0, NORMAL_ANIMATION_GROUP);
        }

        void IdleExit()
        {
            idle = false;
            _idleState = null;
        }

        void MoveEnter()
        {
            move = true;
            if (!needModify)
            {
                _walkState = armature.xanimation.XFadeIn(walkAniName, -1.0f, -1, 0, NORMAL_ANIMATION_GROUP);
            }
        }

        private Vector3 tpos = Vector3.zero;
        void MoveUpdate(float delta)
        {
            if(needModify)
            {
                //> 先调整Y方向高度
                float remain = transform.localPosition.y - modifyY;
                int isDown = remain > 0 ? 1 : -1;
                if (Mathf.Abs(remain) > Mathf.Epsilon)
                {
                    _speed.x = 0.0f;
                    _speed.y = MAX_CLIMB_SPEED;

                    tpos.x = transform.localPosition.x;
                    tpos.z = transform.localPosition.z;
                    tpos.y = transform.localPosition.y - isDown*Mathf.Min(_speed.y* CLIMB_SCALE, Mathf.Abs(remain));
                    transform.localPosition = tpos;
                    return;
                }
                needModify = false;
                modifyY = 0f;
                _walkState = armature.xanimation.XFadeIn(walkAniName, -1.0f, -1, 0, NORMAL_ANIMATION_GROUP);
            }

            if(horizontal == 0f)
            {
                _speed.x = 0.0f;
                _speed.y = 0.0f;
                FSM.ChangeState(XEnum2Int<EState>.ToInt(EState.Idle));
                return;
            }
 
            _speed.x = Grounded ? MAX_MOVE_SPEED_FRONT : 0;
            _speed.y = 0.0f;
            _walkState.timeScale = MOVE_SCALE;

            Vector3 position = this.transform.localPosition;
            if (_speed.x != 0.0f)
            {
                int dir = horizontal > 0 ? 1 : -1;
                if((dir > 0 && !IsColliderWall(1)) || (dir < 0 && !IsColliderWall(0)))
                {
                    position.x += dir * _speed.x * armature.xanimation.timeScale;
                }else
                {
                    //There is wall here！
                }                

                Flip(dir);
            }
            this.transform.localPosition = position;

        }

        void MoveExit()
        {
            move = false;
            horizontal = 0;
            vertical = 0;
            _walkState = null;
        }

        void ClimbEnter() {
            climb = true;
        }

        void ClimbUpdate(float delta) {
            if (vertical == 0f)
            {
                _speed.x = 0.0f;
                _speed.y = 0.0f;
                _climbState = null;
                _climbIdleState = armature.xanimation.XFadeIn(climbIdleAniName, -1.0f, -1, 0, NORMAL_ANIMATION_GROUP);
                return;
            }            
            
            _climbIdleState = null;
            if(_climbState == null)
                _climbState = armature.xanimation.XFadeIn(climbAniName, -1.0f, -1, 0, NORMAL_ANIMATION_GROUP);

            _speed.x = 0.0f;
            if ((vertical > 0f && CanClimbUp) || (vertical < 0f && CanClimbDown))
                _speed.y = MAX_CLIMB_SPEED;
            else _speed.y = 0;
            _climbState.timeScale = CLIMB_SCALE;

            Vector3 position = this.transform.localPosition;
            if (_speed.y != 0.0f)
            {
                int dir = vertical > 0f ? 1 : -1;
                if ((dir > 0 && !IsColliderWall(2)) || (dir < 0 && !IsColliderWall(3)))
                {
                    position.y += dir * _speed.y * armature.xanimation.timeScale;
                }
            }
            this.transform.localPosition = position;
        }

        void ClimbExit() {
            climb = false;
            _climbIdleState = null;
            _climbState = null;
        }

        private void Flip(int dir)
        {
            armature.flipX = dir != facingDir;
        }   

        void ChopEnter()
        {
            chop = true;
            _attackState = armature.xanimation.XFadeIn(attackAniName, -1.0f, 1, 0, NORMAL_ANIMATION_GROUP);
        }

        void ChopUpdate(float delta)
        {
            if (_attackState != null && _attackState.isCompleted)
            {
                FSM.RevertToPreviousState();
                _attackState = null;
            }
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
