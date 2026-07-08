using Microsoft.Xna.Framework;
using Ofriend.Items;
using Ofriend.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Ofriend.Players
{
    public class GoheiPlayer : ModPlayer
    {
        private static readonly int[] DreamSealIntervals = new int[]
        {
            24, 22, 20, 18, 16, 14, 12, 10, 8, 7, 6, 5
        };

        private int bombSelectedItem = -1;
        private int bombTimer;
        private int dreamSealsFired;
        private int screenShakeTimer;
        private int screenShakeDuration;
        private float screenShakeIntensity;

        public bool IsBombActive => dreamSealsFired < DreamSealIntervals.Length && bombTimer > 0;

        public void StartBomb()
        {
            bombSelectedItem = Player.selectedItem;
            bombTimer = DreamSealIntervals[0];
            dreamSealsFired = 0;
        }

        public void RequestScreenShake(int frames, float intensity)
        {
            screenShakeTimer = System.Math.Max(screenShakeTimer, frames);
            screenShakeDuration = System.Math.Max(screenShakeDuration, screenShakeTimer);
            screenShakeIntensity = System.Math.Max(screenShakeIntensity, intensity);
        }

        public override void PreUpdate()
        {
            if (IsBombActive && bombSelectedItem >= 0 && bombSelectedItem < 10)
            {
                Player.selectedItem = bombSelectedItem;
            }
        }

        public override void PostUpdate()
        {
            UpdateBomb();
        }

        public override void ModifyScreenPosition()
        {
            if (screenShakeTimer <= 0 || screenShakeIntensity <= 0f)
            {
                screenShakeTimer = 0;
                screenShakeDuration = 0;
                screenShakeIntensity = 0f;
                return;
            }

            int fadeFrames = System.Math.Max(1, System.Math.Min(20, screenShakeDuration / 4));
            float fade = screenShakeTimer < fadeFrames
                ? screenShakeTimer / (float)fadeFrames
                : 1f;

            Main.screenPosition += Main.rand.NextVector2Circular(
                screenShakeIntensity * fade,
                screenShakeIntensity * fade);

            screenShakeTimer--;

            if (screenShakeTimer <= 0)
            {
                screenShakeDuration = 0;
                screenShakeIntensity = 0f;
            }
        }

        private void UpdateBomb()
        {
            if (!IsBombActive)
            {
                return;
            }

            bombTimer--;
            if (bombTimer > 0)
            {
                return;
            }

            SpawnDreamSeal();
            dreamSealsFired++;

            if (dreamSealsFired >= DreamSealIntervals.Length)
            {
                bombTimer = 0;
                bombSelectedItem = -1;
                return;
            }

            bombTimer = DreamSealIntervals[dreamSealsFired];
        }

        private void SpawnDreamSeal()
        {
            IEntitySource source = Player.GetSource_FromThis();
            Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Gohei.DreamSealInitialSpeed;

            Projectile.NewProjectile(
                source,
                Player.Center,
                velocity,
                ModContent.ProjectileType<DreamSeal>(),
                Gohei.GetDreamSealDamage(Player.HeldItem.damage),
                Gohei.DreamSealKnockBack,
                Player.whoAmI);
        }
    }
}
