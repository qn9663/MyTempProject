using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class AbilityAttributeModifier
    {

        public string Uuid;
        public string AbilityConfigId;
        public BaseAttributeType AttributeType;
        public double Value;
        public bool IsValueType;
        public int StackingCount;
        public string CreaateFromAbilityUuid;

        public AbilityAttributeModifier()
        {
            this.Uuid = System.Guid.NewGuid().ToString("N");
        }

        public AbilityAttributeModifier(AbilityAttributeModifier from)
        {
            this.Uuid = System.Guid.NewGuid().ToString("N");
            this.AttributeType = from.AttributeType;
            this.Value = from.Value;
            this.IsValueType = from.IsValueType;
            this.StackingCount = from.StackingCount;
        }
    }
}
