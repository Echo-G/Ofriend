using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ofriend.Players;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Ofriend.Projectiles
{
    public class DreamSeal : ModProjectile
    {
        private const float SearchRange = 1200f;
        private const float HomingSpeed = 32f;
        private const float HomingAcceleration = 1.55f;
        private const float OrbitRadius = 80f;
        private const float OrbitAngularSpeed = 0.055f;
        private const float OrbitMoveSpeed = 18f;
        private const int HitShakeFrames = 8;
        private const float HitShakeIntensity = 5f;

        private static Texture2D glowTexture;

        private Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.scale = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 420;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.aiStyle = -1;
        }

        public override void Unload()
        {
            glowTexture?.Dispose();
            glowTexture = null;
        }

        public override void AI()
        {
            Projectile.rotation += 0.14f;
            ClearTouchedHostileProjectiles();

            NPC target = FindClosestNPC(SearchRange);
            if (target != null)
            {
                MoveTowards(target.Center, HomingSpeed, HomingAcceleration);
            }
            else
            {
                OrbitAroundPlayer();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = GetGlowTexture();
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 textureOrigin = texture.Size() * 0.5f;
            Vector2 glowOrigin = glow.Size() * 0.5f;
            float colorTime = Main.GlobalTimeWrappedHourly * 0.45f + Projectile.whoAmI * 0.11f;

            for (int i = 0; i < 3; i++)
            {
                Color glowColor = Main.hslToRgb((colorTime + i * 0.18f) % 1f, 0.95f, 0.60f) * (0.36f - i * 0.08f);
                float glowScale = Projectile.scale * (1.75f + i * 0.35f);

                Main.EntitySpriteDraw(
                    glow,
                    drawPosition,
                    null,
                    glowColor,
                    0f,
                    glowOrigin,
                    glowScale,
                    SpriteEffects.None,
                    0);
            }

            for (int i = 0; i < 8; i++)
            {
                float angle = MathHelper.TwoPi * i / 8f + Main.GlobalTimeWrappedHourly * 0.55f;
                Vector2 offset = angle.ToRotationVector2() * 4f;
                Color edgeColor = Main.hslToRgb((colorTime + i / 8f) % 1f, 1f, 0.62f) * 0.22f;

                Main.EntitySpriteDraw(
                    texture,
                    drawPosition + offset,
                    null,
                    edgeColor,
                    Projectile.rotation,
                    textureOrigin,
                    Projectile.scale * 1.08f,
                    SpriteEffects.None,
                    0);
            }

            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                Color.White,
                Projectile.rotation,
                textureOrigin,
                Projectile.scale,
                SpriteEffects.None,
                0);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner >= 0 && Projectile.owner < Main.maxPlayers)
            {
                Main.player[Projectile.owner].GetModPlayer<GoheiPlayer>().RequestScreenShake(HitShakeFrames, HitShakeIntensity);
            }
        }

        private void MoveTowards(Vector2 targetPosition, float maxSpeed, float acceleration)
        {
            Vector2 desiredVelocity = (targetPosition - Projectile.Center).SafeNormalize(Vector2.Zero) * maxSpeed;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, acceleration / maxSpeed);

            if (Projectile.velocity.Length() > maxSpeed)
            {
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * maxSpeed;
            }
        }

        private void OrbitAroundPlayer()
        {
            if (!Owner.active || Owner.dead)
            {
                Projectile.Kill();
                return;
            }

            float angle = Main.GameUpdateCount * OrbitAngularSpeed + Projectile.whoAmI * 0.71f;
            Vector2 targetPosition = Owner.Center + angle.ToRotationVector2() * OrbitRadius;
            MoveTowards(targetPosition, OrbitMoveSpeed, 0.75f);
        }

        private NPC FindClosestNPC(float maxRange)
        {
            NPC closest = null;
            float closestDistance = maxRange;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.lifeMax <= 5)
                {
                    continue;
                }

                float distance = Vector2.Distance(Projectile.Center, npc.Center);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = npc;
                }
            }

            return closest;
        }

        private void ClearTouchedHostileProjectiles()
        {
            foreach (Projectile other in Main.projectile)
            {
                if (!other.active || other.whoAmI == Projectile.whoAmI || !other.hostile || other.friendly || other.damage <= 0)
                {
                    continue;
                }

                if (Projectile.Hitbox.Intersects(other.Hitbox))
                {
                    other.Kill();
                }
            }
        }

        private static Texture2D GetGlowTexture()
        {
            if (glowTexture != null && !glowTexture.IsDisposed)
            {
                return glowTexture;
            }

            const int size = 96;
            Color[] data = new Color[size * size];
            Vector2 center = new Vector2((size - 1) * 0.5f);
            float radius = size * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float progress = MathHelper.Clamp(distance / radius, 0f, 1f);
                    float alpha = 1f - progress;
                    alpha *= alpha;
                    data[y * size + x] = Color.White * alpha;
                }
            }

            glowTexture = new Texture2D(Main.graphics.GraphicsDevice, size, size);
            glowTexture.SetData(data);
            return glowTexture;
        }
    }
}
