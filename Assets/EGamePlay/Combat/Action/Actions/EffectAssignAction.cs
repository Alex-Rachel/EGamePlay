using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using EGamePlay.Combat;

namespace EGamePlay.Combat
{
    public class EffectAssignAbility : Entity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public bool Enable { get; set; }


        public bool TryMakeAction(out EffectAssignAction action)
        {
            if (Enable == false)
            {
                action = null;
            }
            else
            {
                action = OwnerEntity.AddChild<EffectAssignAction>();
                action.ActionAbility = this;
                action.Creator = OwnerEntity;
            }
            return Enable;
        }
    }

    /// <summary>
    /// ����Ч���ж�
    /// </summary>
    public class EffectAssignAction : Entity, IActionExecute
    {
        /// �������Ч�������ж���Դ����
        public Entity SourceAbility { get; set; }
        /// Ŀ���ж�
        public IActionExecute TargetAction { get; set; }
        public AbilityEffect AbilityEffect { get; set; }
        public AbilityItem AbilityItem { get; set; }
        public Effect EffectConfig => AbilityEffect.EffectConfig;
        /// �ж�����
        public Entity ActionAbility { get; set; }
        /// Ч�������ж�Դ
        public EffectAssignAction SourceAssignAction { get; set; }
        /// �ж�ʵ��
        public CombatEntity Creator { get; set; }
        /// Ŀ�����
        public CombatEntity Target { get; set; }
        public Entity AssignTarget { get; set; }


        /// ǰ�ô���
        private void PreProcess()
        {
            if (Target == null)
            {
                if (AssignTarget is CombatEntity combatEntity)
                {
                    Target = combatEntity;
                }
                if (AssignTarget is IActionExecute actionExecute)
                {
                    Target = actionExecute.Target;
                }
                if (AssignTarget is SkillExecution skillExecution)
                {
                    Target = skillExecution.InputTarget;
                }
            }
        }

        public void AssignEffect()
        {
            //Log.Debug($"ApplyEffectAssign {EffectConfig}");
            PreProcess();

            foreach (var item in AbilityEffect.Components.Values)
            {
                //if (item is EffectAddStatusComponent addStatusComponent)
                //{
                //    var action = addStatusComponent.GetActionExecution();
                //    action.SourceAssignAction = effectAssignAction;
                //    action.Target = effectAssignAction.Target;
                //    action.SourceAbility = effectAssignAction.SourceAbility;
                //    action.ApplyAddStatus();
                //    continue;
                //}
                if (item is IEffectTriggerSystem effectTriggerSystem)
                {
                    effectTriggerSystem.OnTriggerApplyEffect(this);
                }
            }

            PostProcess();

            FinishAction();
        }

        /// ���ô���
        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.AssignEffect, this);
            Target.TriggerActionPoint(ActionPointType.ReceiveEffect, this);

            foreach (var item in AbilityEffect.EffectConfig.Decorators)
            {
                if (item is TriggerNewEffectWhenAssignEffectDecorator effectDecorator)
                {
                    var newEffect = Creator.GetComponent<AbilityEffectComponent>().GetEffect(((int)effectDecorator.EffectApplyType) - 1);
                    newEffect.TriggerEffectCheckWithTarget(Target);
                }
            }
        }

        public void FinishAction()
        {
            Entity.Destroy(this);
        }
    }
}