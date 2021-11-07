﻿using Moonstorm;
using RoR2;

namespace LostInTransit.Buffs
{
    public class DiceCrit : BuffBase
    {
        public override BuffDef BuffDef { get; set; } = Assets.LITAssets.LoadAsset<BuffDef>("DiceCrit");
        public static BuffDef buff;

        public override void Initialize()
        {
            buff = BuffDef;
        }

        public override void AddBehavior(ref CharacterBody body, int stack)
        {
            body.AddItemBehavior<DiceCritBehavior>(stack);
        }

        public class DiceCritBehavior : CharacterBody.ItemBehavior, IStatItemBehavior
        {
            private bool statsDirty = false;
            public void RecalculateStatsStart() { }
            public void RecalculateStatsEnd()
            {
                statsDirty = true;
                body.crit += (Items.BlessedDice.critAmount);
            }
            public void OnDestroy()
            {
                if (statsDirty)
                {
                    body.crit -= (Items.BlessedDice.critAmount);
                }
            }
        }
    }
}