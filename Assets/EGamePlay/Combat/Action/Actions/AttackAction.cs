﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using ET;

namespace EGamePlay.Combat
{
    public class AttackActionAbility : Entity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public CombatEntity ParentEntity { get => GetParent<CombatEntity>(); }
        public bool Enable { get; set; }


        public bool TryMakeAction(out AttackAction action)
        {
            if (Enable == false)
            {
                action = null;
            }
            else
            {
                action = OwnerEntity.AddChild<AttackAction>();
                action.ActionAbility = this;
                action.Creator = OwnerEntity;
            }
            return Enable;
        }

        //public void TryActivateAbility() => ActivateAbility();

        //public void ActivateAbility() => Enable = true;

        //public void DeactivateAbility() { }

        //public void EndAbility() { }

        //public Entity CreateExecution()
        //{
        //    var execution = OwnerEntity.MakeAction<AttackAction>();
        //    execution.ActionAbility = this;
        //    return execution;
        //}

        //public bool TryMakeAction(out AttackAction abilityExecution)
        //{
        //    if (Enable == false)
        //    {
        //        abilityExecution = null;
        //    }
        //    else
        //    {
        //        abilityExecution = CreateExecution() as AttackAction;
        //    }
        //    return Enable;
        //}
    }

    /// <summary>
    /// 普攻行动
    /// </summary>
    public class AttackAction : Entity, IActionExecution
    {
        /// 行动能力
        public Entity ActionAbility { get; set; }
        /// 效果赋给行动源
        public EffectAssignAction SourceAssignAction { get; set; }
        /// 行动实体
        public CombatEntity Creator { get; set; }
        /// 目标对象
        public CombatEntity Target { get; set; }


        public void FinishAction()
        {
            Entity.Destroy(this);
        }

        //前置处理
        private void PreProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PreGiveAttack, this);
            Target.TriggerActionPoint(ActionPointType.PreReceiveAttack, this);
        }

        public async ETTask ApplyAttackAwait()
        {
            PreProcess();

            await TimeHelper.WaitAsync(1000);

            ApplyAttack();

            await TimeHelper.WaitAsync(300);

            PostProcess();

            FinishAction();
        }

        public void ApplyAttack()
        {
            var attackExecution = Creator.AttackAbility.CreateExecution();
            attackExecution.AttackAction = this;
            attackExecution.BeginExecute();
        }

        //后置处理
        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PostGiveAttack, this);
            Target.TriggerActionPoint(ActionPointType.PostReceiveAttack, this);
        }
    }
}