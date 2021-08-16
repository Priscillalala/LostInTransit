﻿using RoR2;
using LostInTransit.ScriptableObjects;
using System.Linq;
using UnityEngine;

namespace LostInTransit.Components
{
    [RequireComponent(typeof(CharacterModel))]
    public class LITIdrsHelper : MonoBehaviour
    {
        private CharacterModel model;
        private ItemDisplayRuleSet modelIDRS;

        void Start()
        {
            model = GetComponent<CharacterModel>();
            modelIDRS = model.itemDisplayRuleSet;
            for (int i = 0; i < modelIDRS.runtimeEquipmentRuleGroups.Length; i++)
            {
                DisplayRuleGroup ruleGroup = modelIDRS.runtimeEquipmentRuleGroups[i];
                if (!ruleGroup.isEmpty)
                {
                    for (int j = 0; j < ruleGroup.rules.Length; j++)
                    {
                        ItemDisplayRule currentRule = ruleGroup.rules[j];
                        if (currentRule.childName == LITIDRS.ItemDisplayRule.constant)
                        {
                            ChildLocator childLocator = model.childLocator;
                            var firstChild = childLocator.transformPairs.FirstOrDefault().name;
                            currentRule.childName = firstChild;
                            ruleGroup.rules[j] = currentRule;
                        }
                    }
                }
            }
            for (int i = 0; i < modelIDRS.runtimeItemRuleGroups.Length; i++)
            {
                DisplayRuleGroup ruleGroup = modelIDRS.runtimeItemRuleGroups[i];
                if (!ruleGroup.isEmpty)
                {
                    for (int j = 0; j < ruleGroup.rules.Length; j++)
                    {
                        ItemDisplayRule currentRule = ruleGroup.rules[j];
                        if (currentRule.childName == LITIDRS.ItemDisplayRule.constant)
                        {
                            ChildLocator childLocator = model.childLocator;
                            var firstChild = childLocator.transformPairs.FirstOrDefault().name;
                            currentRule.childName = firstChild;
                            ruleGroup.rules[j] = currentRule;
                        }
                    }
                }
            }
        }
    }
}