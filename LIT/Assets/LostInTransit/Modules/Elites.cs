﻿using LostInTransit.Equipments;
using LostInTransit.ScriptableObjects;
using LostInTransit.Utils;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LostInTransit.Modules
{
    public static class Elites
    {
        public static EliteDef[] loadedEliteDefs
        {
            get
            {
                return LITContent.serializableContentPack.eliteDefs;
            }
        }

        public static List<LITEliteDef> elites = new List<LITEliteDef>();

        public static readonly int EliteRampPropertyID = Shader.PropertyToID("_EliteRamp");

        public static void Initialize()
        {
            LITLogger.LogI("Initializing Elites.");
            var eliteDefs = Assets.LITAssets.LoadAllAssets<LITEliteDef>();
            foreach(var def in eliteDefs)
            {
                EliteEquipment equipment;
                bool flag = Pickups.EliteEquipments.TryGetValue(def.eliteEquipmentDef, out equipment);
                bool flag1 = LITMain.config.Bind<bool>("Lost in Transit Elites", def.name, true, "Enable/disable this Elite Type.").Value;
                if(flag && flag1)
                {
                    HG.ArrayUtils.ArrayAppend(ref LITContent.serializableContentPack.equipmentDefs, equipment.EquipmentDef);
                    Debug.Log(equipment);
                    equipment.Initialize();
                    Pickups.Equipments.Add(equipment.EquipmentDef, equipment);

                    HG.ArrayUtils.ArrayAppend(ref LITContent.serializableContentPack.eliteDefs, def);
                    elites.Add(def);
                    if (def.overlay)
                        Buffs.Buffs.overlayMaterials.Add(equipment.EquipmentDef.passiveBuffDef, def.overlay);
                    LITLogger.LogD($"Added Elite {def.name}");
                }
            }
            IL.RoR2.CharacterModel.UpdateMaterials += AddEliteMaterial;
            On.RoR2.CombatDirector.Init += AddLITElites;
        }

        private static void AddLITElites(On.RoR2.CombatDirector.orig_Init orig)
        {
            orig();
            foreach (var eliteDef in elites)
            {
                switch (eliteDef.eliteTier)
                {
                    case EliteTiers.Basic:
                        HG.ArrayUtils.ArrayAppend(ref CombatDirector.eliteTiers[1].eliteTypes, eliteDef);
                        HG.ArrayUtils.ArrayAppend(ref CombatDirector.eliteTiers[2].eliteTypes, eliteDef);
                        LITLogger.LogD($"Added Elite {eliteDef.name} to Combat Director's Tier 1 & 2's Elites.");
                        break;
                    case EliteTiers.PostLoop:
                        HG.ArrayUtils.ArrayAppend(ref CombatDirector.eliteTiers[3].eliteTypes, eliteDef);
                        LITLogger.LogD($"Added Elite {eliteDef.name} to Combat Director's Tier 3 Elites.");
                        break;
                    case EliteTiers.Other:
                        break;
                }
            }
        }

        //TYVM Mystic!
        private static void AddEliteMaterial(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                MoveType.After,
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<CharacterModel>("propertyStorage"),
                x => x.MatchLdsfld(typeof(CommonShaderProperties), "_EliteIndex")
            );
            c.GotoNext(
                MoveType.After,
                x => x.MatchCallOrCallvirt<MaterialPropertyBlock>("SetFloat")
            );
            c.Emit(OpCodes.Ldarg, 0);
            c.EmitDelegate<Action<CharacterModel>>((model) =>
            {
                if (!elites.Contains(EliteCatalog.GetEliteDef(model.myEliteIndex) as LITEliteDef))
                    return;
                LITEliteDef eliteDef = EliteCatalog.GetEliteDef(model.myEliteIndex) as LITEliteDef;
                model.propertyStorage.SetTexture(EliteRampPropertyID, eliteDef.eliteRamp);
            });
        }
    }
}