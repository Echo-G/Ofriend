using Microsoft.Xna.Framework;
using Ofriend.Players;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ofriend.Projectiles
{
    public class DogDogBarkHoldout : ModProjectile
    {
        public const float HoldoutDistance = 24f;
        public const int MaxDogHeads = 8;

        private const int DogdogBaseFrames = 52;
        private const int DogdogOctaveFrames = 29;
        private const int SoundGapFrames = 12;
        private const int BarkBaseFrames = 180;
        private const int BarkExtraFrames = 15;
        private const int LaserMinDurationFrames = 60;
        private const int LaserMaxDurationFrames = 180;
        private const int LaserFullChargeFrames = 720;
        private const int FireShakeFrames = 18;
        private const float FireShakeIntensity = 9f;

        private static readonly float[] SpawnPitches = new float[]
        {
            0.00f, 0.08f, 0.16f, 0.24f, 0.32f, 0.40f, 0.48f, 0.56f
        };

        private readonly int[] dogHeadIds = new int[MaxDogHeads];
        private int chargeFrames;
        private int dogHeadCount;
        private int nextDogSoundTimer;
        private int overflowSoundCount;
        private bool fired;

        public ref float HoldTimer => ref Projectile.ai[0];

        public override string Texture => "Ofriend/Items/dogdogbark";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!CanKeepHolding(player))
            {
                if (!fired && player.active && !player.dead)
                {
                    FireDogHeads(player);
                }

                Projectile.Kill();
                return;
            }

            HoldTimer++;
            chargeFrames++;
            UpdateHeldProjectile(player);

            if (Main.myPlayer == Projectile.owner)
            {
                UpdateCharge(player);
            }

            Projectile.timeLeft = 2;
        }

        public override void OnKill(int timeLeft)
        {
            if (fired || Main.myPlayer != Projectile.owner)
            {
                return;
            }

            for (int i = 0; i < dogHeadCount; i++)
            {
                Projectile dogHead = GetDogHead(i);
                if (dogHead != null)
                {
                    dogHead.Kill();
                }
            }
        }

        private bool CanKeepHolding(Player player)
        {
            return player.active &&
                !player.dead &&
                player.channel &&
                !player.noItems &&
                !player.CCed &&
                player.HeldItem?.ModItem is global::Ofriend.Items.dogdogbark;
        }

        private void UpdateHeldProjectile(Player player)
        {
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
            Vector2 direction = (Main.MouseWorld - playerCenter).SafeNormalize(Vector2.UnitX);
            Vector2 holdoutOffset = direction * HoldoutDistance;

            if (holdoutOffset != Projectile.velocity)
            {
                Projectile.netUpdate = true;
            }

            Projectile.velocity = holdoutOffset;
            Projectile.direction = Projectile.velocity.X < 0f ? -1 : 1;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.Center = playerCenter;
            Projectile.rotation = Projectile.velocity.ToRotation();
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }

        private void UpdateCharge(Player player)
        {
            if (nextDogSoundTimer > 0)
            {
                nextDogSoundTimer--;
                return;
            }

            if (dogHeadCount < MaxDogHeads)
            {
                SpawnDogHead(player, dogHeadCount);
                PlayDogdogSound(player.Center, SpawnPitches[dogHeadCount], false);
                nextDogSoundTimer = GetDogdogInterval(DogdogBaseFrames, SpawnPitches[dogHeadCount]);
                dogHeadCount++;
                UpdateDogHeadCounts();
                return;
            }

            PlayOverflowDogdogSound(player.Center);
        }

        private void SpawnDogHead(Player player, int index)
        {
            IEntitySource source = Projectile.GetSource_FromThis();
            Vector2 spawnPosition = DogHeadProjectile.GetChargePosition(player, index, MaxDogHeads);

            int dogHeadId = Projectile.NewProjectile(
                source,
                spawnPosition,
                Vector2.Zero,
                ModContent.ProjectileType<DogHeadProjectile>(),
                0,
                0f,
                Projectile.owner,
                0f,
                index);

            dogHeadIds[index] = dogHeadId;
            Main.projectile[dogHeadId].localAI[0] = MaxDogHeads;
        }

        private void UpdateDogHeadCounts()
        {
            for (int i = 0; i < dogHeadCount; i++)
            {
                Projectile dogHead = GetDogHead(i);
                if (dogHead != null)
                {
                    dogHead.localAI[0] = dogHeadCount;
                }
            }
        }

        private Projectile GetDogHead(int index)
        {
            if (index < 0 || index >= dogHeadIds.Length)
            {
                return null;
            }

            int id = dogHeadIds[index];
            if (id < 0 || id >= Main.maxProjectiles)
            {
                return null;
            }

            Projectile projectile = Main.projectile[id];
            if (!projectile.active || projectile.owner != Projectile.owner || projectile.type != ModContent.ProjectileType<DogHeadProjectile>())
            {
                return null;
            }

            return projectile;
        }

        private void FireDogHeads(Player player)
        {
            fired = true;

            if (Main.myPlayer != Projectile.owner || dogHeadCount <= 0)
            {
                return;
            }

            Vector2 target = Main.MouseWorld;
            int laserDuration = GetLaserDuration();
            int laserDamage = System.Math.Max(1, Projectile.damage / 2);
            float barkPitch = GetBarkPitch(laserDuration);

            SoundEngine.PlaySound(new SoundStyle("Ofriend/Assets/Sounds/bark")
            {
                Pitch = barkPitch,
                Volume = 1f,
                MaxInstances = 3
            }, player.Center);

            player.GetModPlayer<GoheiPlayer>().RequestScreenShake(FireShakeFrames, FireShakeIntensity);

            for (int i = 0; i < dogHeadCount; i++)
            {
                Projectile dogHead = GetDogHead(i);
                if (dogHead == null)
                {
                    continue;
                }

                Vector2 direction = (target - dogHead.Center).SafeNormalize(Vector2.UnitX);
                dogHead.ai[0] = 1f;
                dogHead.velocity = Vector2.Zero;
                dogHead.timeLeft = laserDuration;
                dogHead.netUpdate = true;

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    dogHead.Center,
                    direction,
                    ModContent.ProjectileType<DogLaserProjectile>(),
                    laserDamage,
                    Projectile.knockBack,
                    Projectile.owner,
                    laserDuration);
            }
        }

        private int GetLaserDuration()
        {
            float chargeProgress = MathHelper.Clamp(chargeFrames / (float)LaserFullChargeFrames, 0f, 1f);
            return (int)MathHelper.Lerp(LaserMinDurationFrames, LaserMaxDurationFrames, chargeProgress);
        }

        private static int GetDogdogInterval(int baseFrames, float pitch)
        {
            float speed = 1f + pitch;
            return (int)System.Math.Ceiling(baseFrames / speed) + SoundGapFrames;
        }

        private static float GetBarkPitch(int laserDurationFrames)
        {
            float targetFrames = laserDurationFrames + BarkExtraFrames;
            float speed = BarkBaseFrames / targetFrames;
            return MathHelper.Clamp(speed - 1f, -1f, 1f);
        }

        private void PlayOverflowDogdogSound(Vector2 position)
        {
            overflowSoundCount++;

            if (overflowSoundCount <= 5)
            {
                float pitch = MathHelper.Clamp(0.56f + overflowSoundCount * 0.068f, 0.56f, 0.90f);
                PlayDogdogSound(position, pitch, false);
                nextDogSoundTimer = GetDogdogInterval(DogdogBaseFrames, pitch);
                return;
            }

            float octavePitch = MathHelper.Clamp(-0.20f + (overflowSoundCount - 6) * 0.08f, -0.20f, 0.20f);
            PlayDogdogSound(position, octavePitch, true);
            nextDogSoundTimer = GetDogdogInterval(DogdogOctaveFrames, octavePitch);
        }

        private static void PlayDogdogSound(Vector2 position, float pitch, bool octave)
        {
            string soundPath = octave
                ? "Ofriend/Assets/Sounds/dogdog_octave"
                : "Ofriend/Assets/Sounds/dogdog";

            SoundEngine.PlaySound(new SoundStyle(soundPath)
            {
                Pitch = pitch,
                Volume = 0.9f,
                MaxInstances = 4
            }, position);
        }
    }
}
