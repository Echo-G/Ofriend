using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Ofriend.Projectiles
{
    public class DogLaserProjectile : ModProjectile
    {
        private const float LaserLength = 1200f;
        private const float LaserWidth = 18f;
        private const int FadeFrames = 12;

        public override string Texture => "Ofriend/Items/dogdogbark";

        public int InitialTimeLeft => System.Math.Max(1, (int)Projectile.ai[0]);

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.ai[0] > 0f && Projectile.localAI[0] == 0f)
            {
                Projectile.timeLeft = (int)Projectile.ai[0];
                Projectile.localAI[0] = 1f;
            }

            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity.SafeNormalize(Vector2.UnitX) * LaserLength;

            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                start,
                end,
                LaserWidth,
                ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Vector2 start = Projectile.Center - Main.screenPosition;
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            float rotation = direction.ToRotation();
            float opacity = GetOpacity();

            DrawLaser(pixel, start, rotation, LaserWidth * 1.55f, Color.DeepSkyBlue * (0.22f * opacity));
            DrawLaser(pixel, start, rotation, LaserWidth, Color.Cyan * (0.55f * opacity));
            DrawLaser(pixel, start, rotation, LaserWidth * 0.42f, Color.White * (0.90f * opacity));

            return false;
        }

        private float GetOpacity()
        {
            int elapsed = InitialTimeLeft - Projectile.timeLeft;
            float fadeIn = MathHelper.Clamp(elapsed / (float)FadeFrames, 0f, 1f);
            float fadeOut = MathHelper.Clamp(Projectile.timeLeft / (float)FadeFrames, 0f, 1f);
            return System.Math.Min(fadeIn, fadeOut);
        }

        private static void DrawLaser(Texture2D pixel, Vector2 start, float rotation, float width, Color color)
        {
            Main.EntitySpriteDraw(
                pixel,
                start,
                new Rectangle(0, 0, 1, 1),
                color,
                rotation,
                new Vector2(0f, 0.5f),
                new Vector2(LaserLength, width),
                SpriteEffects.None,
                0);
        }
    }
}
