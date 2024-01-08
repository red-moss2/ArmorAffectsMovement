using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;
using System;

namespace ArmorAffectsMovementMod
{
    public class ArmorAffectsMovement : MonoBehaviour
    {
        private static Mod mod;
        string walkSpeedId;
        string runSpeedId;
        bool debugMode = false;
        PlayerEntity player;
        PlayerSpeedChanger speedChanger;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            go.AddComponent<ArmorAffectsMovement>();



            mod.IsReady = true;
        }

        void Start()
        {
            debugMode = true;
            speedChanger = GameManager.Instance.SpeedChanger;
            player = GameManager.Instance.PlayerEntity;

            SaveLoadManager.OnLoad += InitArmorSpeed;
            StartGameBehaviour.OnStartGame += InitArmorSpeed;
            DaggerfallUI.UIManager.OnWindowChange += RecalculateArmorSpeed;
        }

        public void InitArmorSpeed(object sender, EventArgs e)
        {
            modifyMovementFromArmor();
        }

        public void InitArmorSpeed(SaveData_v1 saveData)
        {
            modifyMovementFromArmor();
        }

        public void RecalculateArmorSpeed(object sender, EventArgs e)
        {
            // Clear the previous modifiers before recalculating.
            if (walkSpeedId != null)
                speedChanger.RemoveSpeedMod(walkSpeedId, false);

            if (runSpeedId != null)
                speedChanger.RemoveSpeedMod(runSpeedId, true);

            modifyMovementFromArmor();
        }

        void modifyMovementFromArmor()
        {
            var equipment = player.ItemEquipTable.EquipTable;
            float totalWeight = 0f;

            // Iterate through the equipment slots and calculate weight total of all equipped items.
            for (int i = 0; i < equipment.Length; i++)
            {
                if (equipment[i] == null)
                    continue;

                totalWeight += equipment[i].weightInKg;
            }

            var armorPenalty = calculateArmorMovementPenalty(totalWeight);

            speedChanger.AddWalkSpeedMod(out string walkSpeedUID, armorPenalty);
            speedChanger.AddRunSpeedMod(out string runSpeedUID, armorPenalty);

            // Cache the ids of the modifiers so we can clear them for recalculation.
            walkSpeedId = walkSpeedUID;
            runSpeedId = runSpeedUID;
        }

        // How much to modify speed (e.g. 75% of normal speed: 0.75, No change: 1)
        float calculateArmorMovementPenalty(float totalWeight)
        {
            var settings = mod.GetSettings();
            float strength = player.Stats.LiveStrength;

            // Power of the effect of weight, from 1 to 3. 1 is severe, 3 is weak. Default is 1.4.
            float overallEffect = settings.GetValue<float>("Overall", "overallEffect");

            // Impact that strength has, from 1000 to 7000. 1500 is strong, 10000 is weak.
            float strengthEffect = 6000f;

            float weightModifier = (100f - (totalWeight / overallEffect)) / 100f;
            float strengthBonus = weightModifier * ((strength * (strength / 5)) / strengthEffect);
            float modifier = Mathf.Clamp(weightModifier + strengthBonus, 0f, 1f);

            if (debugMode)
            {
                Debug.Log("ArmorAffectsMovement | Overall effect: " + overallEffect);
                Debug.Log("ArmorAffectsMovement | Total Weight: " + totalWeight);
                Debug.Log("ArmorAffectsMovement | Weight modifier: " + weightModifier);
                Debug.Log("ArmorAffectsMovement | Strength bonus: " + strengthBonus);
                Debug.Log("ArmorAffectsMovement | Speed modifier: " + modifier);
            }

            return modifier;
        }
    }
}
