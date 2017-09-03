using UnityEngine;
using System.Collections;
using DragonBones;

public class Player : MonoBehaviour {

    private UnityArmatureComponent _armatureComponent = null;
    private Armature _armature = null;
    private Rigidbody2D _rid2d = null;
    private BoxCollider2D _box2d = null;
    private UnityEngine.Transform upLadderCheck = null;
    private UnityEngine.Transform downLadderCheck = null;
    private UnityEngine.Transform leftCheck = null;
    private UnityEngine.Transform rightCheck = null;
    private UnityEngine.Transform leftCheck_d = null;
    private UnityEngine.Transform rightCheck_d = null;
    private UnityEngine.Transform wall_left = null;
    private UnityEngine.Transform wall_right = null;
    private int ClimbLayer = 0;

    private const string NORMAL_ANIMATION_GROUP = "normal";

    private const float NORMALIZE_MOVE_SPEED = 0.03f;
    private const float MAX_MOVE_SPEED_FRONT = NORMALIZE_MOVE_SPEED * 1.4f;
    private const float MAX_CLIMB_SPEED = NORMALIZE_MOVE_SPEED * 1f;

    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;

    //private int _faceDir = -1;
    private int _moveDir = 0;
    private int _climbDir = 0;
    private int _aimDir = 0;

    private DragonBones.AnimationState _walkState = null;
    private DragonBones.AnimationState _attackState = null;
    private Vector2 _speed = new Vector2();


	// Use this for initialization
	void Start () {
        _armatureComponent = this.GetComponentInChildren<UnityArmatureComponent>();
        _armature = _armatureComponent.armature;
        _rid2d = this.GetComponent<Rigidbody2D>();
        _box2d = this.GetComponent<BoxCollider2D>();
        upLadderCheck = this.transform.FindChild("upLadderCheck");
        downLadderCheck = this.transform.FindChild("downLadderCheck");
        leftCheck = this.transform.FindChild("leftCheck");
        leftCheck_d = this.transform.FindChild("leftCheck_d");
        rightCheck = this.transform.FindChild("rightCheck");
        rightCheck_d = this.transform.FindChild("rightCheck_d");
        wall_left = this.transform.FindChild("wall_left");
        wall_right = this.transform.FindChild("wall_right");
        ClimbLayer = 1 << LayerMask.NameToLayer("Ladder");

        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Ladder"),LayerMask.NameToLayer("Player"));

        _armatureComponent.AddEventListener(EventObject.FADE_IN_COMPLETE, _animationEventHandler);
        _armatureComponent.AddEventListener(EventObject.FADE_OUT_COMPLETE, _animationEventHandler);
	}
	
	// Update is called once per frame
	void Update () {

        // Input 
        var isLeft = Input.GetKey(left);
        var isRight = Input.GetKey(right);


        bool flag = true;

        if (isLeft == isRight)
        {
            _move(0);
        }
        else if (isLeft)
        {
            if (IsCanMoveLeft() && !IsLeftWall)
                _move(-1);
            else flag = false;
        }
        else
        {
            if (IsCanMoveRight() && !IsRightWall)
                _move(1);
            else flag = false;
        }
        
        if(!isLeft && !isRight)
        {
            bool isUp = Input.GetKey(up);
            bool isDown = Input.GetKey(down);
            if (isUp == isDown)
            {
                _climb(0);                
            }else if(isUp)
            {
                if (IsCanClimbUp())
                {
                    _climb(1);
                }
                else
                {
                    flag = false;
                }
            }else
            {
                if (IsCanClimbDown())
                {
                    _climb(-1);
                }
                else
                {
                    flag = false;
                }
            }
        }

        if (flag) _updatePosition();
	}

    private bool IsLeftWall
    {
        get
        {
            return Physics2D.Linecast(wall_left.position, transform.position, 1 << LayerMask.NameToLayer("Wall"));
        }
    }

    private bool IsRightWall
    {
        get
        {
            return Physics2D.Linecast(transform.position, wall_right.position, 1 << LayerMask.NameToLayer("Wall"));
        }
    }

    private bool IsCanMoveLeft()
    {
        return Physics2D.Linecast(leftCheck.position, leftCheck_d.position, 1 << LayerMask.NameToLayer("Ground"));
    }

    private bool IsCanMoveRight()
    {
        return Physics2D.Linecast(rightCheck.position, rightCheck_d.position, 1 << LayerMask.NameToLayer("Ground"));
    }

    private bool IsCanClimbUp()
    {
        return Physics2D.Linecast(upLadderCheck.position, this.transform.position, 1 << LayerMask.NameToLayer("Ladder"));
    }
    private bool IsCanClimbDown()
    {
        return Physics2D.Linecast(this.transform.position, downLadderCheck.position, ClimbLayer);
    }

    private void _updateAnimation()
    {
        if (_moveDir == 0 && _climbDir == 0)
        {
            _speed.x = 0.0f;
            _speed.y = 0.0f;
            _armature.animation.FadeIn("stand", -1.0f, -1, 0, NORMAL_ANIMATION_GROUP);
            _walkState = null;
        }
        else
        {
            if (_walkState == null)
            {
                _walkState = _armature.animation.FadeIn("walk", -1.0f, -1, 0, NORMAL_ANIMATION_GROUP);
            }

            _walkState.timeScale = MAX_MOVE_SPEED_FRONT / NORMALIZE_MOVE_SPEED;

            _speed.x = MAX_MOVE_SPEED_FRONT * _moveDir;
            _speed.y = MAX_CLIMB_SPEED * _climbDir;
        }
    }

    private void _updatePosition()
    {
        if (_speed.x == 0.0f && _speed.y == 0.0f)
        {
            return;
        }

        Vector3 position = this.transform.localPosition;
        if (_speed.x != 0.0f)
        {
            position.x += _speed.x * _armature.animation.timeScale;
        }

        if (_speed.y != 0.0f)
        {
            position.y += _speed.y * _armature.animation.timeScale;
        }

        this.transform.localPosition = position;
    }

    private void _animationEventHandler(string type, EventObject eventObject)
    {

    }

    private void _move(int dir)
    {
        if (_moveDir == dir)
        {
            return;
        }

        if(dir > 0)
        {
            _armature.flipX = true;
        }
        else if(dir < 0){
            _armature.flipX = false;
        }
        _moveDir = dir;        

        _updateAnimation();
    }
    

    private void _climb(int dir)
    {
        if (_climbDir == dir)
        {
            return;
        }

        _climbDir = dir;

        _updateAnimation();
    }
}
