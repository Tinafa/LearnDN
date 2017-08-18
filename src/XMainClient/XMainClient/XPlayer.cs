using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMainClient
{
    public class XPlayer : XActor
    {
        public AudioClip moveSound1;
        public float moveSpeed = 10f;
        public float maxVelocity = 20f;

        private Animator animator;

        protected override void Start()
        {
            animator = GetComponent<Animator>();
            Init();
            base.Start();
        }

        private void Init()
        {
        }

        private void OnDisable()
        {

        }       

        void CheckIfGameOver()
        {

        }

        protected override void OnChopEnter()
        {
            animator.SetTrigger("playerChop");
        }

        protected override void OnChopUpdate(float delta)
        {
            AnimatorStateInfo stateinfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateinfo.IsName("Base Layer.PlayerChop") && stateinfo.normalizedTime>=1)
            {
                ChangeState(EnumInt32ToInt.Convert<EState>(EState.Idle));
            }
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
