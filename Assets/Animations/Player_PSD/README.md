# PSD player animation setup

Open `Assets/Prefabs/Player_PSD.prefab` to work on the new character. This is a separate visual/animation prefab; the gameplay player in `Assets/Resources/Player.prefab` is unchanged.

1. Select `Assets/Sprites/Player/Player.psd`, open Sprite Editor, and use Skinning Editor to create your bones, geometry, and weights. Apply the changes. The prefab inherits the PSD hierarchy.
2. Open the prefab and select its root Animator when recording animations. The 15 clips in `Clips` are blank placeholders for you to animate. Their loop settings and durations follow the existing animations.
3. `Player_PSD_Override.overrideController` already assigns every placeholder to the corresponding slot of the existing player state machine. All its parameters and transitions are reused. Edit these new clips without modifying the old player's clips or override controller.

The PSD uses separate layer sprites, Character Rig import, and 600 pixels per unit to match the previous character's import scale. No bones or animation keyframes have been generated. Keep hierarchy names stable after recording animations, since clip bindings use those paths.
