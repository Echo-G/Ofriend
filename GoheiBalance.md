# Gohei balance notes

This file lists the Gohei values that are meant to be adjusted while balancing.

## Power level rules

Power level is shared through `PowerSystem.GetPowerLevel(int power)` and
`PowerPlayer.PowerLevel`.

```text
0-999     = level 1
1000-1999 = level 2
2000-2999 = level 3
3000-4000 = level 4
```

## Gohei damage values

Edit these constants in `Items/Gohei.cs`:

```csharp
public const int PanelDamage = 20;
public const float OfudaDamageMultiplier = 0.40f;
public const float HomingAmuletDamageMultiplier = 0.55f;
public const float DreamSealDamageMultiplier = 1.50f;
public const float HomingAmuletKnockBack = 2f;
public const float DreamSealKnockBack = 2f;
```

With the current defaults:

```text
Weapon panel damage: 20
Ofuda damage:        20 * 0.40 = 8
HomingAmulet damage: 20 * 0.55 = 11
DreamSeal damage:    20 * 1.50 = 30
```

## Gohei power scaling

The helper methods in `Items/Gohei.cs` control projectile counts:

```text
Level 1: 2 Ofuda, 0 YinYangOrb
Level 2: 2 Ofuda, 2 YinYangOrb
Level 3: 4 Ofuda, 2 YinYangOrb
Level 4: 4 Ofuda, 4 YinYangOrb
```

Ofuda is always fired in parallel lanes:

```text
2 Ofuda: offset -8, +8 pixels
4 Ofuda: offset -18, -6, +6, +18 pixels
```

## Bomb timing

Bomb fires 12 DreamSeal projectiles with these frame intervals:

```text
24, 22, 20, 18, 16, 14, 12, 10, 8, 7, 6, 5
```

Gohei left click is disabled during the sequence, and the player's selected
hotbar slot is locked until the 12th DreamSeal is fired.

## YinYangOrb texture

`Projectiles/YinYangOrb.png` is the 28x28 texture used by the orbiting orbs.
`Projectiles/YinYangOrb.cs` uses the default ModProjectile texture path, so no
extra code change is needed when replacing the png.
