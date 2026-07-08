using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Ofriend.Projectiles
{
    public class DogHeadProjectile : ModProjectile
    {
        private const float BackDistance = 96f;
        private const float SpreadSpacing = 34f;
        private const float MoveLerp = 0.25f;
        private const float RecoilDamping = 0.82f;
        private const float RecoilStopSpeed = 0.08f;

        public override string Texture => "Ofriend/Projectiles/DogHeadCharge";

        public bool IsFiring => Projectile.ai[0] >= 1f;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
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
            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }

            Projectile.rotation = 0f;

            if (IsFiring)
            {
                Projectile.velocity *= RecoilDamping;
                if (Projectile.velocity.LengthSquared() < RecoilStopSpeed * RecoilStopSpeed)
                {
                    Projectile.velocity = Vector2.Zero;
                }

                return;
            }

            int index = (int)Projectile.ai[1];
            int count = System.Math.Max(1, (int)Projectile.localAI[0]);
            Vector2 targetPosition = GetChargePosition(player, index, count);
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetPosition, MoveLerp);
            Projectile.timeLeft = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            string texturePath = IsFiring
                ? "Ofriend/Projectiles/DogHeadFire"
                : "Ofriend/Projectiles/DogHeadCharge";

            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;

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

        public static Vector2 GetChargePosition(Player player, int index, int count)
        {
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
            Vector2 away = (playerCenter - Main.MouseWorld).SafeNormalize(-Vector2.UnitY);
            Vector2 perpendicular = away.RotatedBy(MathHelper.PiOver2);
            float centeredIndex = index - (count - 1) * 0.5f;
            float arcOffset = -System.Math.Abs(centeredIndex) * 5f;

            return playerCenter + away * (BackDistance + arcOffset) + perpendicular * centeredIndex * SpreadSpacing;
        }
    }
}
