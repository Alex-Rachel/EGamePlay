﻿using System.Collections.Generic;
using System;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 条件事件管理组件，条件事件是指全局或个体某个状态条件达成触发的事件
    /// 在这里管理条件事件的添加监听、移除监听、触发流程
    /// </summary>
    public sealed class ConditionEventComponent : Component
    {
        private Dictionary<Action, Entity> ConditionEvents { get; set; } = new Dictionary<Action, Entity>();


        public void AddListener(ConditionEventType conditionType, Action action, object paramObj = null)
        {
            switch (conditionType)
            {
                case ConditionEventType.WhenInTimeNoDamage:
                    {
                        if (float.TryParse((string)paramObj, out var time))
                        {
                            var condition = Entity.AddChild<ConditionWhenInTimeNoDamage>(time);
                            condition.StartListen(action);
                            ConditionEvents.Add(action, condition);
                        }
                        break;
                    }
                case ConditionEventType.WhenHPLower:
                    break;
                case ConditionEventType.WhenHPPctLower:
                    break;
                case ConditionEventType.WhenIntervalTime:
                    {
                        if (float.TryParse((string)paramObj, out var time))
                        {
                            var condition = Entity.AddChild<ConditionTimeInterval>(time);
                            condition.StartListen(action);
                            ConditionEvents.Add(action, condition);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        public void RemoveListener(ConditionEventType conditionType, Action action)
        {
            if (ConditionEvents.ContainsKey(action))
            {
                Entity.Destroy(ConditionEvents[action]);
                ConditionEvents.Remove(action);
            }
        }
    }
}