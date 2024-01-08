# Armor Affects Movement

## Author

redmoss, with huge thanks to Magicono43

## Description

This mod applies the burden of armor and weapons on the player by restricting their movement speed. The heavier your equipment loadout, the slower you become; but characters with high strength can offset this to a certain extent.

This is an RP-flavoured mod, essentially. The goal is to put a downside on wearing heavy armor, to make it not only more realistic, but to give an actual advantage for wearing light armor. The vanilla Daggerfall experience makes it a no-brainer to go full plate on every character since there is no apparent downside; armor is effectively a dodge suit. Thus, rogue/assassin/burglar characters will benefit from full freedom of movement with this, allowing them to freely climb and leap around, while heavier armoured characters will have to stick to the ground, or hasten themselves with magical means.

Horse and cart travel is not affected, all players should be able to get around the world at a good pace, otherwise it becomes tedious.

## Installation

Place the `.dfmod` file into `StreamingAssets/Mods` folder in your DFU installation.

## Formula

The current formula should be:

```
totalWeight is the total weight of the player's equipped armor in kilograms.

overallEffect: 1 to 3 (1 strong, 3 weak)
strengthEffect: 200 to 700 (200 strong, 700 weak)

weightModifier = (100f - (totalWeight / overallEffect)) / 100f
strengthModifier = Strength / strengthEffect
strengthBonus = weightModifier * strengthModifier
modifier = Mathf.Clamp(weightModifier + strengthBonus, 0f, 1f)
```

Feedback is always appreciated, this mod needs testing for sure!

## TODO

- Restrict recalc code to only trigger on Inventory window, not every window
- Weight affects climbing (the player will slip more)
- Weight affects jump height
- Running uses more endurance as a function of weight
- Ensure horse and cart speeds are not affected (for ease of gameplay)
- Feedback and messages: "Your armor weighs you down"
- Holding a shield affects climbing? I think Roleplay and Realism forces you to sheath weapon to climb.
