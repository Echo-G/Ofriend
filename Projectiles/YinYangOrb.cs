using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ofriend.Items;
using Ofriend.Players;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Ofriend.Projectiles
{
    public class YinYangOrb : ModProjectile
    {
        private const float OrbitRadius = 72f;
        private const float OrbitAngularSpeed = 0.045f;
        private const float MoveSpeed = 24f;
        private const float MoveLerp = 0.28f;

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!ShouldExist(player))
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;
            Projectile.rotation += 0.06f;

            int orbIndex = (int)Projectile.ai[0];
            int orbCount = System.Math.Max(1, (int)Projectile.ai[1]);
            float angle = Main.GameUpdateCount * OrbitAngularSpeed + MathHelper.TwoPi * orbIndex / orbCount;
            Vector2 targetPosition = player.Center + angle.ToRotationVector2() * OrbitRadius;
            Vector2 desiredVelocity = targetPosition - Projectile.Center;

            if (desiredVelocity.Length() > MoveSpeed)
            {
                desiredVelocity = desiredVelocity.SafeNormalize(Vector2.Zero) * MoveSpeed;
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, MoveLerp);
            UpdateFireRequest(player);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;

            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                Color.Cyan * 0.16f,
                Projectile.rotation,
                origin,
                Projectile.scale * 1.45f,
                SpriteEffects.None,
                0);

            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                Color.White,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0);

            return false;
        }

        public static void RequestFire(Projectile orb, int delayFrames, int damage)
        {
            orb.localAI[0] = delayFrames;
            orb.localAI[1] = damage;
        }

        private void UpdateFireRequest(Player player)
        {
            if (Projectile.localAI[0] <= 0f)
            {
                return;
            }

            Projectile.localAI[0]--;
            if (Projectile.localAI[0] > 0f)
            {
                return;
            }

            Vector2 direction = (Projectile.Center - player.Center).SafeNormalize(Vector2.UnitY);
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                direction * Gohei.HomingAmuletInitialSpeed,
                ModContent.ProjectileType<HomingAmulet>(),
                (int)Projectile.localAI[1],
                Gohei.HomingAmuletKnockBack,
                Projectile.owner);
        }

        private static bool ShouldExist(Player player)
        {
            if (!player.active || player.dead)
            {
                return false;
            }

            if (player.HeldItem?.ModItem is not Gohei)
            {
                return false;
            }

            int desiredCount = Gohei.GetOrbCount(player.GetModPlayer<PowerPlayer>().PowerLevel);
            return desiredCount > 0;
        }
    }
}
