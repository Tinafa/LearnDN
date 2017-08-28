using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMainClient
{
    public class XPlayer : XActor
    {

        public List<EffectUpdateHander> effectHandlers;
        private List<int> removeHandlers;
        public XAttributes attrComp;

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
            effectHandlers = new List<EffectUpdateHander>();
            removeHandlers = new List<int>();
        }

        public void SetAttr(XAttributes comp)
        {
            attrComp = comp;
            comp.SetHost(this);
        }

        private void OnDisable()
        {

        }       

        void CheckIfGameOver()
        {

        }

        protected override void Update()
        {
            for(int i= removeHandlers.Count-1; i>=0;--i)
            {
                effectHandlers.RemoveAt(i);
            }
            removeHandlers.Clear();
            for (int i=0;i< effectHandlers.Count;++i)
            {
                if(!effectHandlers[i](Time.deltaTime))
                {
                    removeHandlers.Add(i);
                }
            }
                
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
            }else if(other.tag == "Food")
            {
                XRottenApple item = new XRottenApple();
                item.Use(this.attrComp);
                other.gameObject.SetActive(false);
            }
        }

        public void SetWalkClip(AudioClip clip)
        {
            moveSound1 = clip;
        }

        public void UseItem(XUseItem item)
        {
            if(item != null)
            {
                item.Use(this.attrComp);
            }
        }

        public void AddEffects(EffectUpdateHander handler)
        {
            effectHandlers.Add(handler);
        }
    }
}
