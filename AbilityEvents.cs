using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic
{

    public partial class Events
    {

        public delegate void OnAbilityTrigger(UnitInstance unit, AbilityInstance ability,bool isTrigger);

    }


    public partial interface IEventBus
    {

        Events.OnAbilityTrigger OnAbilityTrigger { get; set; }

    }


    public partial class EventBus //: MonoBehaviour, GameLogic.IEventBus
    {
        public Events.OnAbilityTrigger OnAbilityTrigger { get; set; }

    }
}
