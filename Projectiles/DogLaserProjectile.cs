using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Ofriend.Projectiles
{
    public class DogLaserProjectile : ModProjectile
    {
        private const float LaserLength = 1200f;
        private const float LaserStartOffset = 18f;
        private const float LaserWidth = 32f;
        private const int FadeFrames = 12;
        private const float RecoilDamping = 0.82f;
        private const float RecoilStopSpeed = 0.08f;

        public override string Texture => "Ofriend/Projectiles/DogLaser";

        public int InitialTimeLeft => System.Math.Max(1, (int)Projectile.ai[0]);
        private Vector2 LaserDirection => Projectile.ai[1].ToRotationVector2();

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

            Projectile.velocity *= RecoilDamping;
            if (Projectile.velocity.LengthSquared() < RecoilStopSpeed * RecoilStopSpeed)
            {
                Projectile.velocity = Vector2.Zero;
            }

            Projectile.rotation = Projectile.ai[1];
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            Vector2 start = GetLaserStart();
            Vector2 end = start + LaserDirection * LaserLength;

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
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 start = GetLaserStart() - Main.screenPosition;
            float opacity = GetOpacity();
            float widthScale = LaserWidth / texture.Height;
            float lengthScale = LaserLength / texture.Width;

            Main.EntitySpriteDraw(
                texture,
                start,
                null,
                Color.White * opacity,
                Projectile.rotation,
                new Vector2(0f, texture.Height * 0.5f),
                new Vector2(lengthScale, widthScale),
                SpriteEffects.None,
                0);

            return false;
        }

        private Vector2 GetLaserStart()
        {
            return Projectile.Center + LaserDirection * LaserStartOffset;
        }

        private float GetOpacity()
        {
            int elapsed = InitialTimeLeft - Projectile.timeLeft;
            float fadeIn = MathHelper.Clamp(elapsed / (float)FadeFrames, 0f, 1f);
            float fadeOut = MathHelper.Clamp(Projectile.timeLeft / (float)FadeFrames, 0f, 1f);
            return System.Math.Min(fadeIn, fadeOut);
        }
    }
}
