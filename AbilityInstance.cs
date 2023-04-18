using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using JsonFx.Json;
using App;

namespace GameLogic
{
    public class AbilityInstance : IJsonData,IPoolable
    {
        public string Uuid;
        public string Id;
        public AbilityState AbilityState;
        public float DurationRemain;
        public float RandomValue;
        public float ActiveTime;
        public float LimitOverLayCount = 1;
        public float StackingCount;
        public float ActiveCount;
        public float TriggerIntervalTime;
        public float Duration;
        public double OverrideValue;
        public int SkillId;
        public bool IsRefreshAttribute;
        public double MaxValue;
        public double MinValue;
        public AbilityCreateFrom AbilityCreateFrom;

        public List<string> ActionAddedSkillUuidIds = new List<string>();
        public string FromAbilityUuid;//uuid.由动作产生的能力.方面移除重复产生的能力

        public float DurationForAddAbility;
        [JsonIgnore]
        public float AddAbilityDuration { get { return this.Config.AbilityActionData.AddAbilityOverrideDuration; } }

        [JsonIgnore]
        public GameObject gameObject => throw new NotImplementedException();

        [JsonIgnore]
        public UnitInstance AttachUnit;

        [JsonIgnore]
        public UnitInstance CreateFromUnit;
        public UnitInstance TriggerFromUnit;

        [JsonIgnore]
        public AbilityConfig Config;


        public struct CheckParams
        {
            public UnitInstance TargetUnit;
            public Dictionary<DamageType, double> DamageDict;

        }

        private CheckParams mCheckParams;
        //public AbilityActionAttribute AbilityActionAttribute = new AbilityActionAttribute();

        //[JsonIgnore]
        //public List<Dictionary<BaseAttributeType, double>> ActionAttributeRateList = new List<Dictionary<BaseAttributeType, double>>();
        //[JsonIgnore]
        //public List<Dictionary<BaseAttributeType, double>> ActionAttributeValueList = new List<Dictionary<BaseAttributeType, double>>();

        //[JsonIgnore]
        //public Dictionary<AbilityCondtionType, Func<UnitInstance, AbilityConditionData, bool>> abilityConditionFunMapping;

        public AbilityInstance()
        {

        }

        public AbilityInstance(AbilityInstance another)
        {
            this.Uuid                    = Guid.NewGuid().ToString("N");
            this.Id                      = another.Id;
            this.AbilityState            = another.AbilityState;
            this.DurationRemain          = another.DurationRemain;
            this.ActiveTime              = another.ActiveTime;
            this.LimitOverLayCount       = another.LimitOverLayCount;
            this.StackingCount           = another.StackingCount;
            this.ActiveCount             = another.ActiveCount;
            this.TriggerIntervalTime     = another.TriggerIntervalTime;
            this.AttachUnit               = another.AttachUnit;
            this.CreateFromUnit = another.CreateFromUnit;
            this.Duration                = another.Duration;
            this.Config                  = Bridge.AbilityConfigManager.getConfig(this.Id);
            this.OverrideValue           = another.OverrideValue;
            this.RandomValue             = another.RandomValue;
            this.SkillId                 = another.SkillId;
            this.IsRefreshAttribute      = another.IsRefreshAttribute;
          //  this.ActionAddedAbilityIds   = new List<string>(another.ActionAddedAbilityIds);
            this.ActionAddedSkillUuidIds = new List<string>(another.ActionAddedSkillUuidIds);
            this.MaxValue = another.MaxValue;
            this.MinValue = another.MinValue;
            this.DurationForAddAbility = another.DurationForAddAbility;
            this.FromAbilityUuid = another.FromAbilityUuid;
        }

        public void Pack(ProtoBuf.IMsgBase m)
        {
            m = m != null ? m : new ProtoMsg.AbilityInstance();
            var msg = m as ProtoMsg.AbilityInstance;
            msg.Uuid                    = this.Uuid                               ;
            msg.Id                      = this.Id                                 ;
            msg.AbilityState            = (int)this.AbilityState                       ;
            msg.DurationRemain          = this.DurationRemain                     ;
            msg.ActiveTime              = this.ActiveTime                         ;
            msg.LimitOverLayCount       = this.LimitOverLayCount                  ;
            msg.StackingCount           = this.StackingCount                      ;
            msg.ActiveCount             = this.ActiveCount                        ;
            msg.TriggerIntervalTime     = this.TriggerIntervalTime                ;
            //msg.AttachUnit              = this.AttachUnit.Pack()                         ;
           // msg.FromUnit                = this.FromUnit                           ;
            msg.Duration                = this.Duration                           ;
            //msg.Config                  = this.Config                             ;
            msg.OverrideValue           = this.OverrideValue                      ;
            msg.RandomValue             = this.RandomValue                        ;
            msg.SkillId                 = this.SkillId                            ;
            msg.IsRefreshAttribute      = this.IsRefreshAttribute                 ;
           // msg.ActionAddedAbilityIds.AddRange(this.ActionAddedAbilityIds);
            msg.ActionAddedSkillUuidIds.AddRange(this.ActionAddedSkillUuidIds)            ;
            msg.MaxValue                = this.MaxValue                           ;
            msg.MinValue                = this.MinValue                           ;
        }

        public void Unpack(ProtoBuf.IMsgBase m)
        {
            var msg = m as ProtoMsg.AbilityInstance;
            this.Uuid                = msg.Uuid;
            this.Id                  = msg.Id;
            this.AbilityState        = (AbilityState)msg.AbilityState;
            this.DurationRemain      = msg.DurationRemain;
            this.ActiveTime          = msg.ActiveTime;
            this.LimitOverLayCount   = msg.LimitOverLayCount;
            this.StackingCount       = msg.StackingCount;
            this.ActiveCount         = msg.ActiveCount;
            this.TriggerIntervalTime = msg.TriggerIntervalTime;
            this.Duration            = msg.Duration;
            this.OverrideValue       = msg.OverrideValue;
            this.RandomValue         = msg.RandomValue;
            this.SkillId             = msg.SkillId;
            this.IsRefreshAttribute  = msg.IsRefreshAttribute;
        //    this.ActionAddedAbilityIds.AddRange(msg.ActionAddedAbilityIds);
            this.ActionAddedSkillUuidIds.AddRange(msg.ActionAddedSkillUuidIds);
            this.MaxValue            = msg.MaxValue;
            this.MinValue            = msg.MinValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attachUnit">能力附加到哪个角色</param>
        /// <param name="id"></param>
        /// <param name="overrideValue"></param>
        /// <param name="overrideDuration"></param>
        /// <param name="overrideRand"></param>
        /// <param name="overrideSkillId"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="from">能力怎么来的</param>
        /// <param name="overrideSubAbilityDuration"></param>
        public void CreateAbilityInstance(UnitInstance createFromUnit, UnitInstance attachUnit, string id,double overrideValue,float overrideDuration,float overrideRand,SkillType overrideSkillId, double minValue, double maxValue,AbilityCreateFrom from,float overrideSubAbilityDuration = 0)
        {
            this.CreateFromUnit = createFromUnit;
            this.AttachUnit = attachUnit;
            this.Uuid = Guid.NewGuid().ToString("N");
            this.Id = id;
            this.AbilityCreateFrom = from;
            this.Config = Bridge.AbilityConfigManager.getConfig(this.Id);

            this.ActiveTime = 0;
            this.StackingCount = 0;
            this.ActiveCount = 0;
            this.LimitOverLayCount = this.Config.Stacking;
            this.TriggerIntervalTime = Config.AbilityChanceData.TriggerTime;
            this.SetAbilityOverrideValues( overrideValue,  overrideDuration,  overrideRand,  overrideSkillId,  minValue,  maxValue, overrideSubAbilityDuration);

           
        }

        public void SetAbilityOverrideValues(double overrideValue, float overrideDuration, float overrideRand, SkillType overrideSkillId, double minValue, double maxValue, float overrideSubAbilityDuration = 0)
        {
            this.DurationForAddAbility = overrideSubAbilityDuration;
            this.OverrideValue  = overrideValue;
            this.Duration       = overrideDuration != 0 ? overrideDuration : Config.Duration;
            this.DurationRemain = Duration;
            this.RandomValue    = overrideRand != 0 ? overrideRand : Config.AbilityConditionData.Random;
            this.SkillId        = overrideSkillId != 0 ? (int)overrideSkillId : (int)Config.AbilityActionData.SkillActionSkillId;
            this.MinValue       = minValue != 0 ? minValue : Config.ItemMinValue;
            this.MaxValue       = maxValue != 0 ? maxValue : Config.ItemMaxValue;
            this.AbilityState   = AbilityState.WaitTrigger;
            this.IsRefreshAttribute = true;//单纯刷新属性
            this.TryRunNoChanceTypeAction();

        }

        public void TryRunNoChanceTypeAction()
        {
            var chanceType = this.Config.AbilityChanceData.ChanceType;
            if ((chanceType == AbilityChanceType.BaseAttr || chanceType == AbilityChanceType.None || chanceType == AbilityChanceType.Seconds))
            {
                CheckAndRunAction(this.Config.AbilityChanceData.ChanceType, (int)this.SkillId);
            }
        }


        //private void OnInitAndTryTrigger(UnitInstance unit,int skillId = -1,bool ifNoChanceToTrigger = true)
        //{
        //    this.Config = Bridge.AbilityConfigManager.getConfig(this.Id);
        //    this.AttachUnit = unit;
        //    this.DurationRemain = Duration;
        //    this.TriggerIntervalTime = Config.AbilityChanceData.TriggerTime;
        //    this.AbilityState = AbilityState.WaitTrigger;
        //    //无条件的直接执行
        //    if (ifNoChanceToTrigger &&  (this.Config.AbilityChanceData.ChanceType == AbilityChanceType.BaseAttr 
        //                                 || this.Config.AbilityChanceData.ChanceType == AbilityChanceType.None 
        //                                 || this.Config.AbilityChanceData.ChanceType == AbilityChanceType.Seconds))
        //    {
        //        CheckAndTrigger(this.Config.AbilityChanceData.ChanceType, skillId);
        //    }
        //}


        public void OnPoolObjectDeSpawn()
        {
            //this.OwnerUnit.AbilityContainer.Remove(ActionAddedAbilityIds);
            //this.OwnerUnit.SkillInstanceModifierContainer.Remove(ActionAddedSkillUuidIds);
            this.Uuid                    = string.Empty;
            this.Id                      = string.Empty;
            this.AbilityState            = AbilityState.WaitTrigger;
            this.DurationRemain          = 0;
            this.ActiveTime              = 0;
            this.LimitOverLayCount       = 0;
            this.StackingCount           = 0;
            this.ActiveCount             = 0;
            this.TriggerIntervalTime     = 0;
            this.AttachUnit              = null;
            this.CreateFromUnit                = null;
            this.TriggerFromUnit = null;
            this.Duration                = 0;
            this.Config                  = null;
            this.OverrideValue           = 0;
            this.SkillId                 = 0;
            this.DurationForAddAbility   = 0;
            this.IsRefreshAttribute      = false;
            this.FromAbilityUuid = string.Empty;
            this.ActionAddedSkillUuidIds.Clear();
            this.mCheckParams = default;
        }


        public void customDeserializeJson()
        {
            Bridge.AbilityConfigManager.tryGetConfig(this.Id,out this.Config);
            if (this.Config == null)
            {
                Debug.LogError("not found ability Id " + this.Id);
                this.AbilityState = AbilityState.Destory;
            }
        }

        public bool IsCreateFromAbility()
        {
            if (!string.IsNullOrEmpty(this.FromAbilityUuid))
            {
                return true;
            }
            return false;
        }

        public bool CheckChance(AbilityChanceType chanceType, UnitInstance selfUnit, int checkSkillType = -1)
        {
            if (IsCreateFromAbility())
            {
                return false;
            }

            if (chanceType == this.Config.AbilityChanceData.ChanceType)
            {
                if (chanceType == AbilityChanceType.AttackCount)
                {
                    if (selfUnit.AttackSourceCounter > 0 && selfUnit.AttackSourceCounter % this.Config.AbilityChanceData.ChanceParamToAttackCount == 0)
                    {
                        return true;
                    }
                }
                else if (chanceType == AbilityChanceType.SkillAttack)
                {
                    var convCheckSkillType = (SkillType) checkSkillType;
                    if ((SkillType)this.Config.AbilityChanceData.SkillType == SkillType.NONE) //任意技能
                    {
                        return true;
                    }
                    if (convCheckSkillType != SkillType.NONE && convCheckSkillType == this.Config.AbilityChanceData.SkillType)
                    {
                        return true;
                    }
                    else if (convCheckSkillType != this.Config.AbilityChanceData.SkillType || checkSkillType == -1)
                    {
                        return false;
                    }
                }
                else if (chanceType == AbilityChanceType.SkillRunning)
                {
                    if (this.Config.AbilityChanceData.SkillType > 0 && (SkillType)checkSkillType != this.Config.AbilityChanceData.SkillType)
                    {
                        return false;
                    }
                }
                else if (chanceType == AbilityChanceType.None || chanceType == AbilityChanceType.BaseAttr)
                {
                    return true;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }



        public void OnUpdate()
        {
            if (AbilityState == AbilityState.Active)//this.AbilityState != AbilityState.WaitExit && this.AbilityState != AbilityState.Destory)
            {
                if (this.Duration > 0)//is buff
                {
                    this.DurationRemain -= Time.deltaTime * this.AttachUnit.OwningDungeon.TimeScale;
                    if (this.AttachUnit != null)
                    {
                        this.UpdateTimeAction();
                    }

                    if (this.DurationRemain < 0)
                    {
                        this.DurationRemain = 0;
                        this.AbilityState = AbilityState.WaitExit;
                    }
                }
            }
            if (this.AttachUnit != null)
            {
                if (this.AbilityState == AbilityState.Active)
                {
                    this.ActiveTime += Time.deltaTime;
                }

                if (this.AbilityState == AbilityState.WaitExit)
                {
#if DEBUG_ABILITY

                    Debug.Log("销毁能力 Update-> OnDestroy " + this.GetDesc());
#endif
                    this.Destroy();
                }
            }
        }

        public void UpdateTimeAction()
        {

            if (this.TriggerIntervalTime != 0)
            {
                this.TriggerIntervalTime -= Time.deltaTime * this.AttachUnit.OwningDungeon.TimeScale;

                if (this.TriggerIntervalTime < 0)
                {
                    var config = this.Config;
                    if (config.AbilityChanceData.ChanceType == AbilityChanceType.Seconds)
                    {
                        this.TriggerIntervalTime = config.AbilityChanceData.TriggerTime + this.TriggerIntervalTime;
#if DEBUG_ABILITY
                        Log("时间触发:"+ GetDesc());
#endif
                        if (config.AbilityConditionData.AbilityCondtionType == AbilityCondtionType.None)//无条件
                        {
                            RepeatAction(this.AttachUnit);
                        }
                        else
                        {
                            this.CheckAndRunAction(AbilityChanceType.Seconds, -1);
                        }
                    }

                }
            }
        }

        

        private bool CheckCondition(UnitInstance unit)
        {
            var condition = this.Config.AbilityConditionData;
            if (condition.AbilityCondtionType == AbilityCondtionType.None)
            {
                return true;
            }
//#if DEBUG_ABILITY
//            Log("尝试检查能力 : " + this.GetDesc() + " 检查者: " + unit.ConfigId);
//#endif
            if (AbilityConditionBase.Instance.abilityConditionFunMapping.ContainsKey(condition.AbilityCondtionType))
            {
                var fun = AbilityConditionBase.Instance.abilityConditionFunMapping[condition.AbilityCondtionType];//获取函数
                
                if (!fun.Invoke(unit, this))
                {
                    return false;
                }
                
            }
            return true;
        }

        public void CheckAndRunAction(AbilityChanceType checkChanceType,int skillId, CheckParams checkParams = default)
        {
            this.mCheckParams = checkParams;
            if (this.AttachUnit == null || (this.AbilityState == AbilityState.Destory))
            {
                LogError(this.GetDesc() + " 单位不存在 跳过检查 或者状态为 :"+ AbilityState.ToString());
                return;
                //OnInit(selfUnit);
            }
            //if (this.AbilityState == AbilityState.WaitTrigger || this.AbilityState == AbilityState.Destory)
            //{
            //    if (tryInit)
            //    {
            //        this.OnInitAndTryTrigger(this.AttachUnit,skillId,false);
            //    }
            //}


            if (this.CheckChance(checkChanceType, this.AttachUnit, skillId))
            {
                var conditionTarget = GetConditionTargetUnit();
                bool statePass = this.AttachUnit != null;// (ability.AbilityState == AbilityState.WaitTrigger || ability.AbilityState == AbilityState.Active);
                if (conditionTarget != null && conditionTarget.IsDead == false && statePass)// && conditionTarget.CurrentHp > 0 &&
                {


                    if (CheckCondition(conditionTarget))
                    {
                        this.TriggerFromUnit = this.AttachUnit;
                        if (this.AbilityState == AbilityState.WaitTrigger || this.AbilityState == AbilityState.Destory)
                        {
                            this.StartAction();
                            this.AbilityState = AbilityState.Active;
                        }
                        else if (this.AbilityState == AbilityState.Active)
                        {
                            var target = this.Config.IsBuff ? this.AttachUnit : null;
                            RepeatAction(target);
                        }
                    }
                    else
                    {
                        if (this.AbilityState == AbilityState.Active)
                        {
                            if (this.Config.AbilityChanceData.ChanceType == AbilityChanceType.HpChange || this.Config.AbilityChanceData.ChanceType == AbilityChanceType.MpChange)
                            {
                                this.AbilityState = AbilityState.WaitExit;
                            }
                            //onEndExecute();
                            //ability.AbilityState = AbilityState.WaitTrigger;
                        }
                    }

                    if (this.Config.TriggerCount > 0 && this.ActiveCount == this.Config.TriggerCount)
                    {
#if DEBUG_ABILITY
                        Debug.Log(this.Config.Desc+ "WaitExit TriggerCount");
#endif
                        this.AbilityState = AbilityState.WaitExit;
                        this.EndAction();
                    }
                }
                else
                {
                    if (statePass == false)
                    {
                        Debug.LogError(this.AttachUnit.ConfigId +" 能力执行失败: 应该是状态 没有重置 state = " + this.AbilityState + " "+GetDesc());
                    }
                }
            }
        }


        public void StartAction()
        {
            this.ActiveTime = 0;
            this.ActiveCount = 1;
            this.DurationRemain = Duration;
            RunAction(null);
        }

        //已经再执行状态时,再次执行动作
        public void RepeatAction(UnitInstance targetOrNull)
        {
            this.ActiveTime = 0;
            ActiveCount++;
            RunAction(targetOrNull);
        }

        public void RunAction(UnitInstance targetOrNull)
        {
            FastList<UnitInstance> targetList = null;
            UnitInstance target = null;
            //属性
            target = targetOrNull != null ? targetOrNull : this.AttachUnit.GetAbilityTargetUnit(this.Config.AbilityActionData.AttributesSideType, out targetList);
            if (target != null)
            {
                if (targetList != null && targetList.Count > 0)
                {
                    for (int i = 0; i < targetList.Count; i++)
                    {
                        if (this.OverrideAttributeAction(targetList[i]))
                        {
                            SendTriggerEvent(target, true, true);
                        }
                    }
                }
                else
                {
                    if (this.OverrideAttributeAction(target))
                    {
                        SendTriggerEvent(target, true, true);
                    }
                }
            }

            target = targetOrNull != null ? targetOrNull : this.AttachUnit.GetAbilityTargetUnit(this.Config.AbilityActionData.AbilityReviseSide, out targetList);
            if (targetList != null && targetList.Count > 0)
            {
                for (int i = 0; i < targetList.Count; i++)
                {
                    CheckAndExceReviseSide(targetList[i]);
                }
            }
            else
            {
                CheckAndExceReviseSide(target);
            }

            target = targetOrNull != null ? targetOrNull : this.AttachUnit.GetAbilityTargetUnit(this.Config.AbilitySummorData.SummonSideType, out targetList);
            if (targetList != null && targetList.Count > 0)
            {
                for (int i = 0; i < targetList.Count; i++)
                {
                    CheckAndExceSummon(targetList[i]);
                }
            }
            else
            {
                CheckAndExceSummon(target);
            }

            UnitInstance from = this.CreateFromUnit != null ? this.CreateFromUnit : this.AttachUnit;
            target = targetOrNull != null ? targetOrNull : this.AttachUnit.GetAbilityTargetUnit(this.Config.AbilityActionData.DamageSideType, out targetList);
            if (targetList != null && targetList.Count > 0)
            {
                for (int i = 0; i < targetList.Count; i++)
                {
                    CheckAndExceDamage(from,targetList[i]);
                }
            }
            else
            {
                CheckAndExceDamage(from,target);
            }

            target = targetOrNull != null ? targetOrNull : this.AttachUnit.GetAbilityTargetUnit(this.Config.AbilityActionData.StateSideType, out targetList);
            if (targetList != null && targetList.Count > 0)
            {
                for (int i = 0; i < targetList.Count; i++)
                {
                    CheckAndExceUnitState(targetList[i]);
                }
            }
            else
            {
                CheckAndExceUnitState(target);
            }

            CheckAndExceSkill(this.AttachUnit);

            if (string.IsNullOrEmpty(this.Config.AbilityActionData.AddAbilityId) == false)
            {
                target = this.AttachUnit.GetAbilityTargetUnit(this.Config.AbilityActionData.AddAbilitySideType, out targetList);
                string addAbilityId = this.Config.AbilityActionData.AddAbilityId;
                double value = this.OverrideValue != 0 ? this.OverrideValue : this.Config.AbilityActionData.AddAbilityValue;
                if (targetList != null && targetList.Count > 0)
                {
                    for (int j = 0; j < targetList.Count; j++)
                    {
                        AddOrUpdateTempAbility(addAbilityId,value,targetList[j]);
                    }
                }
                else
                {
                    AddOrUpdateTempAbility(addAbilityId, value, target);
                }
            }
        }

        private void AddOrUpdateTempAbility(string addAbilityId, double value,UnitInstance target)
        {
            if (target != null && target.CurrentHp > 0)
            {
                float duration = DurationForAddAbility != 0 ? DurationForAddAbility :  this.Duration != 0 ? this.Duration : this.Config.AbilityActionData.AddAbilityOverrideDuration;
                float random = this.RandomValue != 0 ? this.RandomValue : this.Config.AbilityConditionData.Random;
                SkillType skillId = this.Config.AbilityChanceData.SkillType;// this.Config.AbilityActionData.SkillActionSkillId;

                if (string.IsNullOrEmpty(addAbilityId) == false)//每个能力产生的id,buff
                {
                    AbilityInstance ability;
                    ability = target.AbilityContainer.GetByFromAbilityUUID(this.Uuid);

                    if (ability == null)
                    {
                        ability = Bridge.AbilityInstancePool.Spawn() as AbilityInstance;
                        ability.TriggerFromUnit = this.AttachUnit;
                        ability.CreateAbilityInstance(this.AttachUnit, target, addAbilityId, value, duration, random, skillId, value, value, this.AbilityCreateFrom);

                        int instId = target.GetHashCode();
                        target.AbilityContainer.Add(ability);
                        ability.FromAbilityUuid = this.Uuid;
                    }
                    else
                    {
                        int instId = target.GetHashCode();
                        ability.TriggerFromUnit = this.AttachUnit;
                        ability.AttachUnit = target;
                        if (AbilityState == AbilityState.Active)
                        {
                            ability.OnActivatingOverrideAction(target);
                        }
                        else if (ability.AbilityState != AbilityState.Destory)
                        {
                            ability.StartAction();
                            ability.AbilityState = AbilityState.Active;
                        }
                    }
                    
#if DEBUG_ABILITY
                            Log("创建临时能力 " + ability.GetDesc());
#endif
                    
                }
            }
        }

        private UnitInstance CheckAndExceSkill(UnitInstance target)
        {
            if (this.Config.AbilityActionData.SkillActionType != SkillActionType.None)
            {
                SkillType skillId = this.SkillId != 0 ? (SkillType)this.SkillId : this.Config.AbilityActionData.SkillActionSkillId;
                double value = this.OverrideValue != 0 ? this.OverrideValue : this.Config.AbilityActionData.skillActionValue;
#if DEBUG_ABILITY
                Log("执行动作: 技能动作" + target.ConfigId + " SkillActionType=" + this.Config.AbilityActionData.SkillActionType+ " value="+ value);
#endif
                //修改全局技能属性
                switch (this.Config.AbilityActionData.SkillActionType)
                {
                    case SkillActionType.None:
                        break;
                    case SkillActionType.SkillCd:
                        {
                            {
                                SkillInstanceModifier modifier = target.SkillInstanceModifierContainer.GetModifierByAbilityUuid(this.Uuid);
                                if (modifier == null)
                                {
                                    modifier = new SkillInstanceModifier()
                                    {
                                        SkillId = skillId,
                                        CdRate = (float) value,
                                        CreaateFromAbilityUuid = this.Uuid,
                                    };
                                    target.SkillInstanceModifierContainer.AddModifier(modifier);
                                    this.ActionAddedSkillUuidIds.Add(modifier.Uuid);
                                }

                                SendTriggerEvent(target, true, false);
                            }
                        }
                        break;
                    case SkillActionType.SkillMp:
                        {

                            {
                                SkillInstanceModifier modifier = target.SkillInstanceModifierContainer.GetModifierByAbilityUuid(this.Uuid);
                                if (modifier == null)
                                {

                                    modifier = new SkillInstanceModifier()
                                    {
                                        SkillId = skillId,
                                        MpRate = (float)value,
                                        CreaateFromAbilityUuid = this.Uuid,
                                    };
                                    target.SkillInstanceModifierContainer.AddModifier(modifier);
                                    this.ActionAddedSkillUuidIds.Add(modifier.Uuid);
                                }
                                SendTriggerEvent(target, true, false);
                            }

                        }
                        break;
                    case SkillActionType.SkillDuration:
                        {
                            SkillInstanceModifier modifier = target.SkillInstanceModifierContainer.GetModifierByAbilityUuid(this.Uuid);
                            if (modifier == null)
                            {
                                modifier = new SkillInstanceModifier()
                                {
                                    SkillId = skillId,
                                    Duration = (float) value,
                                    CreaateFromAbilityUuid = this.Uuid,
                                };
                                target.SkillInstanceModifierContainer.AddModifier(modifier);
                                this.ActionAddedSkillUuidIds.Add(modifier.Uuid);
                            }

                            SendTriggerEvent(target, true, false);

                        }
                        break;
                    case SkillActionType.SkillDamage:
                        {
                            SkillInstanceModifier modifier = target.SkillInstanceModifierContainer.GetModifierByAbilityUuid(this.Uuid);
                            if (modifier == null)
                            {
                                modifier = new SkillInstanceModifier()
                                {
                                    SkillId = skillId,
                                    DamageValue = value,
                                    IsValueType = this.Config.AbilityActionData.DamageValueIsValueType,
                                    CreaateFromAbilityUuid =  this.Uuid,
                                };
                                target.SkillInstanceModifierContainer.AddModifier(modifier);
                                this.ActionAddedSkillUuidIds.Add(modifier.Uuid);
                            }

                            SendTriggerEvent(target, true, false);
                        }
                        break;
                    default:
                        break;
                }
            }

            return target;
        }


        private UnitInstance CheckAndExceSummon(UnitInstance target)
        {
            if (string.IsNullOrEmpty(this.Config.AbilitySummorData.SummonUnitId) == false)
            {
                //target = this.OwnerUnit.GetAbilityTargetUnit(this.Config.AbilityActionData.StateSideType);
                //if (target != null && target.CurrentHp > 0 && target.OwningDungeon != null)
                {
#if DEBUG_ABILITY
                    Log("执行动作: 召唤" + target.ConfigId + " "+GetDesc());
#endif

                    Vector3 position = this.AttachUnit.PhysicsBody.transform.position + this.AttachUnit.PhysicsBody.transform.forward * 2f;
                    int level = target.GetSkillLevel(this.Config.AbilityChanceData.SkillType);//target.Level +

                    // this.AttachUnit.SetAllAttributeDirt(true);

                    CmdSpawnCharacter.SpawningData sd = new CmdSpawnCharacter.SpawningData
                    {
                        ConfigId = this.Config.AbilitySummorData.SummonUnitId,
                        Level = this.Config.AbilitySummorData.SummonLevel <= 0 ? level : Mathf.Clamp(1, this.Config.AbilitySummorData.SummonLevel, int.MaxValue),
                        SpawnWorldPos = position,
                        SpawnWorlRot = this.AttachUnit.PhysicsBody.transform.localRotation,
                        IsBoss = false,
                        IsEliteMonster = false,
                        IsInSecretRoom = false,
                        Floor = this.AttachUnit.OwningDungeon.Chapter,
                        IsPlayerCharacter = this.AttachUnit.IsPlayerUnit,
                        IsPlayerSupportCharacter = this.AttachUnit.IsPlayerUnit,
                        RoomId = this.AttachUnit.OwningDungeon.CurRoomIndex,
                        OverrideItemSlots = new ItemSlots(this.AttachUnit.ItemSlots),
                        OverrideCollectedSuitHeroCId = this.AttachUnit.ConfigId,
                        OverrideCollectedSuitItemIds = new List<string>(this.AttachUnit.CollectedSuitItemIds),
                      //  OverrideFixedBaseAttribute = new Dictionary<BaseAttributeType, double>(this.AttachUnit.AttributeResultCache),
                    };
                    var summonUnit = GameLogic.CmdSpawnCharacter.ExecuteStatic(sd);
                    summonUnit.IsSummon = true;
                    summonUnit.SummonLifeTime = Config.AbilitySummorData.SummonDuration;
                    CmdUnitJoinDungeon.ExecuteStatic(summonUnit, AttachUnit.OwningDungeon);

                }
            }

            return target;
        }


        private UnitInstance CheckAndExceUnitState(UnitInstance target)
        {
            if (this.Config.AbilityActionData.StateSideType != AbilitySideType.None && string.IsNullOrEmpty(this.Config.AbilityActionData.UnitState) == false)
            {
                //target = this.OwnerUnit.GetAbilityTargetUnit(this.Config.AbilityActionData.StateSideType);
                if (target != null && target.CurrentHp > 0 && target.OwningDungeon != null)
                {
#if DEBUG_ABILITY
                    Log("执行动作: 改变状态" + target.ConfigId + " "+GetDesc());
#endif
                    if (this.Config.AbilityActionData.IsStun && target.Stunned == false)
                    {
                        CmdSetStunCondition.ExecuteStatic(target, true);
                        SendTriggerEvent(target, true, false);
                    }
                    if (this.Config.AbilityActionData.Injured && target.IsInjured == false)
                    {
                        CmdSetInjuredCondition.ExecuteStatic(target, true);
                        SendTriggerEvent(target, true, false);
                    }
                    if (this.Config.AbilityActionData.IsSilence && target.IsSilence == false)
                    {
                        CmdSetSilenceCondition.ExecuteStatic(target, true);
                        SendTriggerEvent(target, true, false);
                    }
                    if (this.Config.AbilityActionData.ResistDebuff && target.ResistDebuff == false)
                    {
                        CmdSetResistDebuffCondition.ExecuteStatic(target, true);
                        SendTriggerEvent(target, true, false);
                    }

                    if (this.Config.AbilityActionData.IsConfused && target.Confused == false)
                    {
                        CmdSetConfusedCondition.ExecuteStatic(target, true);
                        SendTriggerEvent(target, true, false);
                    }

                    if (this.Config.AbilityActionData.Charmed && target.Charmed == false)
                    {
                        CmdSetCharmCondition.ExecuteStatic(target, true);
                        SendTriggerEvent(target, true, false);
                    }
                }
            }

            return target;
        }

        private UnitInstance CheckAndExceDamage(UnitInstance from,UnitInstance target)
        {
            if (this.Config.AbilityActionData.DamageTypeValues.Count > 0)// && this.Config.AbilityActionData.DamageSideType != AbilitySideType.None
            {
                //target = this.OwnerUnit.GetAbilityTargetUnit(this.Config.AbilityActionData.DamageSideType);
                if (from != null && from.OwningDungeon != null && target != null && target.CurrentHp > 0 && target.OwningDungeon != null)
                {
                    SkillInstance skill = null;
                    SkillType skillId = SkillType.NONE;
                    bool isUseSkill = false;
                    if (this.Config.AbilityChanceData.ChanceType == AbilityChanceType.SkillAttack)
                    {
                        skillId = (SkillType)Enum.Parse(typeof(SkillType), this.Config.AbilityChanceData.ChanceParam);
                        if (skillId != SkillType.NONE)
                        {
                            skill = from.getSkillInstance(skillId);
                            isUseSkill = true;
                        }
                    }
                    Dictionary<DamageType, double> damages = CmdDealDamageToCharacter.NewDamageDictionary();
                    var ie = this.Config.AbilityActionData.DamageTypeValues;
                    int skillLevel = from.GetSkillLevel(skillId);
                    if (isUseSkill)
                    {//技能释放时候就算好  this.OverrideValue
                        for (int i = 0; i < ie.Count; i++)
                        {
                            var k = this.Config.AbilityActionData.DamageTypeValues.Keys[i];
                            var v = this.Config.AbilityActionData.DamageTypeValues.Values[i];
                            DamageType damageType = k;
                            // BaseAttributeType attributeType = AttributeUtil.DamageTypeAttributeMapping[damageType];
                            if (this.Config.AbilityActionData.DamageValueIsValueType)
                            {
                                damages[damageType] = this.OverrideValue + v;
                            }
                            else
                            {
                                var pct = v;
                                damages[damageType] = OverrideValue * v;

                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ie.Count; i++)
                        {
                            var k = this.Config.AbilityActionData.DamageTypeValues.Keys[i];
                            var v = this.Config.AbilityActionData.DamageTypeValues.Values[i];
                            DamageType damageType = k;
                            BaseAttributeType attributeType = AttributeUtil.DamageTypeAttributeMapping[damageType];

                            if (this.Config.AbilityActionData.DamageValueIsValueType)
                            {
                                var value = this.OverrideValue != 0 ? this.OverrideValue : v;
                                damages[damageType] = value;
                            }
                            else//百分比,
                            {
                                var pct = this.OverrideValue != 0 ? this.OverrideValue : v;
                                var baseAttrValue = from.GetCalculateAttribute(attributeType, false);
                                damages[damageType] = baseAttrValue * pct;
                            }
                        }
                    }

                    var resultDamages = CmdDealDamageToCharacter.ExecuteStabDamage(from, target, damages, DamageRangeType.Melee, skillId,true);
#if DEBUG_ABILITY
                    Log($"执行动作: 伤害{GetDesc()} 来自{from.ConfigId} 目标为: {target.ConfigId} ,物理 { resultDamages[DamageType.Physics] } 法术 { resultDamages[DamageType.Magic]} 总伤害 { resultDamages[DamageType.All]}");
#endif

                    SendTriggerEvent(target, true, false);
                }
            }

            return target;
        }





        private UnitInstance CheckAndExceReviseSide(UnitInstance target)
        {
            if (this.Config.AbilityActionData.AbilityReviseSide != AbilitySideType.None && this.Config.AbilityActionData.AbilityReviseType != AbilityReviseType.None)
            {
               // target = this.OwnerUnit.GetAbilityTargetUnit(this.Config.AbilityActionData.AbilityReviseSide);

                if (target != null && target.IsDead == false && target.OwningDungeon != null)
                {

                    //if (this.mCheckParams.TargetUnit != null)
                    //{
                    //    bool isResist = (this.mCheckParams.TargetUnit.AbilityContainer.Get("ResistHpsteal") != null); //对方抵免疫吸血
                    //    if (isResist)
                    //    {
                    //        return target;
                    //    }
                    //}


                    if (this.Config.AbilityActionData.AbilityReviseType == AbilityReviseType.Hp)
                    {
                        var max = target.MaxLife();
                        var revise = 0.0;
                        var value = (OverrideValue != 0 ? OverrideValue : this.Config.AbilityActionData.AbilityReviseValue);
                        if (value < 0)//直接扣除生命,秒杀,只能描述小怪或者召唤物,如果条件复杂了,应该配置表增加一列kill,小怪\boss\远程等.
                        {
                            if (target.IsBoss == false && (target.IsMonster || target.IsSummon))
                            {
                                if (this.Config.AbilityActionData.AbilityReviseIsValueType)
                                {
                                    revise = value;
                                }
                                else
                                {
                                    revise = max * value;
                                }
                                CmdGainHp.ExecuteStatic(target, revise, false, false);
                                if (target.CurrentHp <= 0.0)
                                {
                                    Bridge.DeathSystem.killCharacter(target, this.mCheckParams.TargetUnit, false, false, SkillType.NONE);
                                }
                            }
                        }
                        else
                        {
                            if (this.Config.AbilityActionData.AbilityReviseIsValueType)
                            {
                                revise = value;
                            }
                            else
                            {
                                revise = max * value;
                            }

                            if (this.mCheckParams.TargetUnit != null)
                            {
                                var targetLimitPct = this.mCheckParams.TargetUnit.GetCalculateAttribute(BaseAttributeType.HpRecoverLimit,true);
                                revise = revise - (revise * targetLimitPct);
                            }
                            //Unit.AddBaseAttributeUpper(BaseAttributeType.LifeRecover, revise);
                            CmdGainHp.ExecuteStatic(target, revise, false, false);
                        }

#if DEBUG_ABILITY
                        Log("执行动作: 按照最高生命百分比回复生命： " + revise + "," + this.StackingCount + "次" + GetDesc()+ target.ConfigId );
#endif
                        SendTriggerEvent(target, true, false);
                    }
                    else if (this.Config.AbilityActionData.AbilityReviseType == AbilityReviseType.PhysLife)
                    {
                        var max = target.GetCalculateAttribute(BaseAttributeType.PhysA, true);
                        double revise = 0;
                        double value = (OverrideValue != 0 ? OverrideValue : this.Config.AbilityActionData.AbilityReviseValue);
                        if (this.Config.AbilityActionData.AbilityReviseIsValueType)
                        {
                            revise = value;
                        }
                        else
                        {
                            revise = max * value;
                        }

                        if (this.mCheckParams.TargetUnit != null)
                        {
                            var targetLimitPct = this.mCheckParams.TargetUnit.GetCalculateAttribute(BaseAttributeType.HpRecoverLimit, true);
                            revise = revise - (revise * targetLimitPct);
                        }
                        //Unit.AddBaseAttributeUpper(BaseAttributeType.LifeRecover, revise);
                        CmdGainHp.ExecuteStatic(target, revise, false, false);
#if DEBUG_ABILITY
                        Log("执行动作: 按照物理伤害比例 回复生命： " + revise + "," + this.StackingCount + "次" + GetDesc() + target.ConfigId);
#endif
                        SendTriggerEvent(target, true, false);
                    }
                    else if (this.Config.AbilityActionData.AbilityReviseType == AbilityReviseType.SkillLife)
                    {
                        var max = target.GetCalculateAttribute(BaseAttributeType.SkillA, true);
                        double revise = 0;
                        double value = (OverrideValue != 0 ? OverrideValue : this.Config.AbilityActionData.AbilityReviseValue);
                        if (this.Config.AbilityActionData.AbilityReviseIsValueType)
                        {
                            revise = value;
                        }
                        else
                        {
                            revise = max * value;
                        }

                        if (this.mCheckParams.TargetUnit != null)
                        {
                            var targetLimitPct = this.mCheckParams.TargetUnit.GetCalculateAttribute(BaseAttributeType.HpRecoverLimit, true);
                            revise = revise - (revise * targetLimitPct);
                        }
                        //Unit.AddBaseAttributeUpper(BaseAttributeType.LifeRecover, revise);
                        CmdGainHp.ExecuteStatic(target, revise, false, false);
#if DEBUG_ABILITY
                        Log("执行动作: 按照法术伤害比例回复生命： " + revise + "," + this.StackingCount + "次" + GetDesc() + target.ConfigId);
#endif
                        SendTriggerEvent(target, true, false);
                    }
                    else if (this.Config.AbilityActionData.AbilityReviseType == AbilityReviseType.FireLife)
                    {
                        var max = target.GetCalculateAttribute(BaseAttributeType.FireA, true);
                        double revise = 0;
                        double value = (OverrideValue != 0 ? OverrideValue : this.Config.AbilityActionData.AbilityReviseValue);
                        if (this.Config.AbilityActionData.AbilityReviseIsValueType)
                        {
                            revise = value;
                        }
                        else
                        {
                            revise = max * value;
                        }

                        if (this.mCheckParams.TargetUnit != null)
                        {
                            var targetLimitPct = this.mCheckParams.TargetUnit.GetCalculateAttribute(BaseAttributeType.HpRecoverLimit, true);
                            revise = revise - (revise * targetLimitPct);
                        }
                        //Unit.AddBaseAttributeUpper(BaseAttributeType.LifeRecover, revise);
                        CmdGainHp.ExecuteStatic(target, revise, false, false);
#if DEBUG_ABILITY
                        Log("执行动作: 回复生命： " + revise + ", " + this.StackingCount + "次" + GetDesc() + target.ConfigId);
#endif
                        SendTriggerEvent(target, true, false);
                    }
                    else if (this.Config.AbilityActionData.AbilityReviseType == AbilityReviseType.LighLife)
                    {
                        var max = target.GetCalculateAttribute(BaseAttributeType.LighA, true);
                        double revise = 0;
                        double value = (OverrideValue != 0 ? OverrideValue : this.Config.AbilityActionData.AbilityReviseValue);
                        if (this.Config.AbilityActionData.AbilityReviseIsValueType)
                        {
                            revise = value;
                        }
                        else
                        {
                            revise = max * value;
                        }

                        if (this.mCheckParams.TargetUnit != null)
                        {
                            var targetLimitPct = this.mCheckParams.TargetUnit.GetCalculateAttribute(BaseAttributeType.HpRecoverLimit, true);
                            revise = revise - (revise * targetLimitPct);
                        }
                        //Unit.AddBaseAttributeUpper(BaseAttributeType.LifeRecover, revise);
                        CmdGainHp.ExecuteStatic(target, revise, false, false);
#if DEBUG_ABILITY
                        Log("执行动作: 回复生命： "+revise+ ", "+ this.StackingCount + "次" + GetDesc() + target.ConfigId);
#endif
                        SendTriggerEvent(target, true, false);
                    }
                    else if (this.Config.AbilityActionData.AbilityReviseType == AbilityReviseType.PoisLife)
                    {
                        var max = target.GetCalculateAttribute(BaseAttributeType.PoisA, true);
                        double revise = 0;
                        double value = (OverrideValue != 0 ? OverrideValue : this.Config.AbilityActionData.AbilityReviseValue);
                        if (this.Config.AbilityActionData.AbilityReviseIsValueType)
                        {
                            revise = value;
                        }
                        else
                        {
                            revise = max * value;
                        }

                        if (this.mCheckParams.TargetUnit != null)
                        {
                            var targetLimitPct = this.mCheckParams.TargetUnit.GetCalculateAttribute(BaseAttributeType.HpRecoverLimit, true);
                            revise = revise - (revise * targetLimitPct);
                        }
                        //Unit.AddBaseAttributeUpper(BaseAttributeType.LifeRecover, revise);
                        CmdGainHp.ExecuteStatic(target, revise, false, false);
#if DEBUG_ABILITY
                        Log("执行动作: 回复生命： "+revise+ "," + this.StackingCount + "次" + GetDesc() + target.ConfigId);
#endif
                        SendTriggerEvent(target, true, false);
                    }
                    else if (this.Config.AbilityActionData.AbilityReviseType == AbilityReviseType.Mp)
                    {
                        var max = target.GetMaxMp();
                        double revise = 0;
                        double value = (OverrideValue != 0 ? OverrideValue : this.Config.AbilityActionData.AbilityReviseValue);
                        if (this.Config.AbilityActionData.AbilityReviseIsValueType)
                        {
                            revise = value;
                        }
                        else
                        {
                            revise = max * value;
                        }

                        if (this.mCheckParams.TargetUnit != null)
                        {
                            var targetLimitPct = this.mCheckParams.TargetUnit.GetCalculateAttribute(BaseAttributeType.HpRecoverLimit, true);
                            revise = revise - (revise * targetLimitPct);
                        }
                        //Unit.AddBaseAttributeUpper(BaseAttributeType.LifeRecover, revise);
                        CmdGainMp.ExecuteStatic(target, revise, false);
#if DEBUG_ABILITY
                        Log("执行动作: 回复魔法： " + revise+","+ this.StackingCount + "次" + GetDesc() + target.ConfigId);
#endif
                        SendTriggerEvent(target, true, false);
                    }
                    else if (this.Config.AbilityActionData.AbilityReviseType == AbilityReviseType.HpShield)
                    {
                        var max = target.MaxLife();
                        double extraHp = 0;
                        double value = (OverrideValue != 0 ? OverrideValue : this.Config.AbilityActionData.AbilityReviseValue);
                        if (this.Config.AbilityActionData.AbilityReviseIsValueType)
                        {
                            extraHp = value;
                        }
                        else
                        {
                            extraHp = max * value;
                        }

                        //CmdGainHp.ExecuteStatic(target, revise, false, true);
                        CmdGainHpShield.Execute(target, 0, extraHp);

                        SendTriggerEvent(target, true, false);
#if DEBUG_ABILITY
                        Log("执行动作: 生命护盾： " + extraHp +","+ this.StackingCount + "次" + GetDesc() + target.ConfigId);
#endif
                    }
                    else if (this.Config.AbilityActionData.AbilityReviseType == AbilityReviseType.DamageHp)
                    {
                        var damageDict = this.mCheckParams.DamageDict;
                        var max = damageDict[DamageType.All];// target.MaxLife();
                        double extraHp = 0;
                        double value = (OverrideValue != 0 ? OverrideValue : this.Config.AbilityActionData.AbilityReviseValue);
                        if (this.Config.AbilityActionData.AbilityReviseIsValueType)
                        {
                            extraHp = value;
                        }
                        else
                        {
                            extraHp = max * value;
                        }

                        CmdGainHp.ExecuteStatic(target, extraHp, false, false);

                        SendTriggerEvent(target, true, false);
#if DEBUG_ABILITY
                        Log("执行动作: 生命护盾： " + extraHp +","+ this.StackingCount + "次" + GetDesc() + target.ConfigId);
#endif
                    }
                    else
                    {
                        LogError("功能未实现 Type :" + AbilityReviseType.HpShield.ToString());

                    }
                }
            }

            return target;
        }

        //能力正在执行中,再次被条件触发时
        public void OnActivatingOverrideAction(UnitInstance unit)
        {
            this.AttachUnit = unit;
            this.DurationRemain = Duration;
#if DEBUG_ABILITY
            Log("尝试叠加 " + this.Config.Id + " " + GetDesc() + " ->  能力正在执行中 重置CD时间");
#endif

            var target = this.AttachUnit.GetAbilityTargetUnit(this.Config.AbilityActionData.AttributesSideType, out var targetList);

            if (targetList != null && targetList.Count > 0)
            {
                for (int i = 0; i < targetList.Count; i++)
                {
                    if (this.OverrideAttributeAction(targetList[i]))
                    {
                        SendTriggerEvent(target, true, true);
                    }
                }
            }
            else
            {
                if (this.OverrideAttributeAction(target))
                {
                    SendTriggerEvent(target, true, true);
                }
            }
            //if (OverrideAttributeAction())
            //{
            //    SendTriggerEvent(this.AttachUnit, true, true);
            //}
        }

        public bool OverrideAttributeAction(UnitInstance target)
        {
            // if (this.execOverLayCount < this.LimitOverLayCount || this.LimitOverLayCount == -1)
            if (this.Config.AbilityActionData.AttributeValues.Count > 0
                && (this.AbilityState == AbilityState.Active || this.AbilityState == AbilityState.WaitTrigger)
                && (this.StackingCount < this.Config.Stacking || this.StackingCount < 1))
            {

                var list = this.Config.AbilityActionData.AttributeValues.Keys;
                for (int i = 0; i < list.Count; i++)
                {
                    var attributeType = list[i];
                    var value = this.Config.AbilityActionData.AttributeValues.Values[i];
                    var resultValue = this.OverrideValue != 0 ? this.OverrideValue : value;
                    bool isValueType = this.Config.AbilityActionData.AttributeIsValueType;
                    {
                        var modifier = target.AbilityAttributeModifierContainer.GetModifierByAbilityUuid(this.Uuid, attributeType);
                        if (modifier == null)
                        {
                            modifier = new AbilityAttributeModifier()
                            {
                                AttributeType = attributeType,
                                AbilityConfigId = this.Config.Id,
                                Value = resultValue,
                                CreaateFromAbilityUuid = this.Uuid,
                                StackingCount = Mathf.RoundToInt(this.StackingCount),
                                IsValueType = isValueType,
                            };
                            target.AbilityAttributeModifierContainer.AddModifier(modifier);
                        }
                        else
                        {
                            modifier.AttributeType = attributeType;
                            modifier.Value = resultValue;
                            modifier.StackingCount = Mathf.RoundToInt(this.StackingCount);
                            modifier.IsValueType = isValueType;
                        }
                    }
                    target.AttributeDirtFlag[attributeType] = true;
                }


                this.StackingCount++;
#if DEBUG_ABILITY
                    Log($"覆盖属性：目标  {target.ConfigId}" + this.StackingCount + "次" + GetDesc());

#endif
                return true;
            }
            else if (IsRefreshAttribute)
            {
                IsRefreshAttribute = false;

                var list = this.Config.AbilityActionData.AttributeValues.Keys;
                for (int i = 0; i < list.Count; i++)
                {
                    var attributeType = list[i];
                    bool isValueType = this.Config.AbilityActionData.AttributeIsValueType;
                    var value = this.Config.AbilityActionData.AttributeValues.Values[i];
                    var resultValue = this.OverrideValue != 0 ? this.OverrideValue : value;
                    {
                        var modifier = target.AbilityAttributeModifierContainer.GetModifierByAbilityUuid(this.Uuid, attributeType);
                        if (modifier != null)
                        {
                            modifier.AttributeType = attributeType;
                            modifier.Value = resultValue;
                            modifier.StackingCount = Mathf.RoundToInt(this.StackingCount);
                            modifier.IsValueType = isValueType;
                        }
                    }
                    target.AttributeDirtFlag[attributeType] = true;
                }
#if DEBUG_ABILITY
 
                    Log($"刷新属性：目标  {target.ConfigId}" + this.StackingCount + "次" + GetDesc() );
#endif
                return true;
            }
            return false;
        }

        public void EndAction()
        {
            this.StackingCount = 0;
            this.ActiveTime = 0;
            if (AttachUnit == null)
            {
              //  LogError("出错: " + GetDesc());
                return;
            }

            this.CreateFromUnit = null;
#if DEBUG_ABILITY
            Log("结束能力 : " + GetDesc() + this.AttachUnit.ConfigId);
#endif
            //属性变化还原
            AttachUnit.AbilityAttributeModifierContainer.RemoveModifierByAbilityUuid(this.Uuid);

            for (int i = 0; i < this.Config.AbilityActionData.AttributeValues.Count; i++)
            {
                var attributeType = this.Config.AbilityActionData.AttributeValues.Keys[i];
                this.AttachUnit.AttributeDirtFlag[attributeType] = true;
            }


            if (AttachUnit != null && AttachUnit.CurrentHp > 0)
            {
                if (this.Config.AbilityActionData.IsStun)
                {
                    CmdSetStunCondition.ExecuteStatic(AttachUnit, false);
                    SendTriggerEvent(AttachUnit, false, false);

                }
                if (this.Config.AbilityActionData.Injured)
                {
                    CmdSetInjuredCondition.ExecuteStatic(AttachUnit, false);
                    SendTriggerEvent(AttachUnit, false, false);
                }
                if (this.Config.AbilityActionData.IsSilence)
                {
                    CmdSetSilenceCondition.ExecuteStatic(AttachUnit, false);
                    SendTriggerEvent(AttachUnit, false, false);
                }

                if (this.Config.AbilityActionData.IsConfused)
                {
                    CmdSetConfusedCondition.ExecuteStatic(AttachUnit, false);
                    SendTriggerEvent(AttachUnit, false, false);
                }
                
                if (this.Config.AbilityActionData.ResistDebuff)
                {
                    CmdSetResistDebuffCondition.ExecuteStatic(AttachUnit, false);
                    SendTriggerEvent(AttachUnit, false, false);
                }

                if (this.Config.AbilityActionData.Charmed)
                {
                    CmdSetCharmCondition.ExecuteStatic(AttachUnit, false);
                    SendTriggerEvent(AttachUnit, false, false);
                }

                
            }

            AttachUnit.SkillInstanceModifierContainer.RemoveModifierByAbilityUuid(this.Uuid);

            //for (int i = ActionAddedAbilityIds.Count - 1; i >= 0; i--)
            //{
            //    var uuid = ActionAddedAbilityIds.Values[i];
            //    var abilityInstance = this.AttachUnit.AbilityContainer.GetByUUID(uuid);
            //    if (abilityInstance != null)
            //    {
            //        abilityInstance.Destroy();
            //    }
            //}
            //ActionAddedAbilityIds.Clear();
            SendTriggerEvent(this.AttachUnit, false, false);
        }

        public void Destroy()
        {
            EndAction();
            //if (ActionAddedAbilityIds.Contains(this.Uuid))
            //{
            //    this.AttachUnit = null;
            //    this.AbilityState = AbilityState.Destory;
            //}
            //else
            //{
            //    this.AbilityState = AbilityState.WaitTrigger;
            //}
            this.AbilityState = AbilityState.WaitTrigger;
#if DEBUG_ABILITY
            Log("销毁能力 : " + GetDesc());
#endif
        }


        //public float GetAttributeModifierPct(BaseAttributeType attributeType)
        //{
        //    if (this.AbilityState != AbilityState.Active)
        //    {
        //        return 0;
        //    }
        //    for (int i = 0; i < ActionAttributeRateList.Count; i++)
        //    {
        //        var attributes = ActionAttributeRateList[i];
        //        if (attributes.TryGetValue(attributeType, out double value))
        //        {
        //            return this.OverrideValue != 0 ? (float)this.OverrideValue : (float)value;
        //        }
        //    }
        //    return 0f;
        //}

        //public double GetAttributeModifierValue(BaseAttributeType attributeType)
        //{
        //    if (this.AbilityState != AbilityState.Active)
        //    {
        //        return 0;
        //    }
        //    for (int i = 0; i < ActionAttributeValueList.Count; i++)
        //    {
        //        var attributes = ActionAttributeValueList[i];
        //        if (attributes.TryGetValue(attributeType, out double value))
        //        {
        //            return this.OverrideValue != 0 ? this.OverrideValue : (double)value;
        //        }
        //    }
        //    return 0d;
        //}

        public float getNormalizedProgress(float time)
        {
            if (string.IsNullOrEmpty(this.Config.IconPath))
            {
                return 0f;
            }
            return (float)(1.0 - Mathf.Clamp01(this.getSecondsRemaining() / this.Duration));
        }

        public float getSecondsRemaining()
        {
            return Mathf.Clamp(this.DurationRemain, 0f, float.MaxValue);
        }

        public void SendTriggerEvent(UnitInstance unit,bool isTrigger,bool isResetDuration)
        {
            if (unit != null)
            {
#if DEBUG_ABILITY
                Log("发送事件给:"+unit.ConfigId + (isTrigger ? "激活 :" : "销毁 :") + GetDesc() + (isResetDuration ? "重置时间" : ""));
#endif
                GameLogic.Bridge.EventBus.OnAbilityTrigger?.Invoke(unit, this, isTrigger);
                if (unit.OwningDungeon != null)
                {
                    unit.OwningDungeon.Evenes.OnAbilityTrigger?.Invoke(unit, this, isTrigger, isResetDuration);
                }
            }
            else
            {
#if DEBUG_ABILITY
                LogError("能力触发事件失败");
#endif

            }
        }

        public UnitInstance GetConditionTargetUnit()
        {
            var data = this.Config.AbilityConditionData;

            if (data.AbilitySideType == AbilitySideType.None || data.AbilitySideType == AbilitySideType.Self || data.AbilitySideType == AbilitySideType.SelfAll)
            {
                return this.AttachUnit;
            }
            else if (data.AbilitySideType == AbilitySideType.Target
                || data.AbilitySideType == AbilitySideType.Target)
            {
                var list = this.AttachUnit.OwningDungeon.GetHostilityUnitsSlow(this.AttachUnit);
                if (list.Count > 0)
                {
                    return list[0];
                }
            }

            return null;
        }

        StringBuilder decs = new StringBuilder(255);

        public string GetDesc()
        {
            decs.Clear();
            if (this.AttachUnit != null)
            {
                decs.Append(this.AttachUnit.ConfigId);
                decs.Append(_.L(this.Config.Id));
                decs.Append(":");
            }

            decs.Append(" value:");
            decs.Append(this.OverrideValue.ToString("0.00#"));

            decs.Append(" duration:");
            decs.Append(this.Duration.ToString("0.#"));

            decs.Append(" random:");
            decs.Append(this.RandomValue.ToString("0.#"));


            //decs.Append(" sub random:");
            //decs.Append(this.Config.AbilityConditionData.Random.ToString("0.0#"));
            //decs.Append(" sub Duration:");
            //decs.Append( this.AddAbilityDuration);

            return decs.ToString();
        }

        private void Log(string s)
        {
            Debug.Log(s);
        }

        private void LogError(string s)
        {
            Debug.LogError(s);
        }
    }

}