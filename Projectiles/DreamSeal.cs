using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ofriend.Projectiles
{
    public class DreamSeal : ModProjectile
    {
        private Player player => Main.player[Projectile.owner];
        private float timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.scale = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 400;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            timer++;

            NPC target = FindClosestNPC(900f);
            if (target != null)
            {
                if (Projectile.localAI[0] == 0)
                {
                    Vector2 burstDir = Vector2.Normalize(target.Center - Projectile.Center);
                    Projectile.velocity = burstDir * 22f;
                    Projectile.localAI[0] = 1;
                }
                MoveTowardsTarget(target.Center, maxSpeed: 18f, acceleration: 0.6f);
            }
            else
            {
                Projectile.localAI[0] = 0;
                OrbitAroundPlayer(radius: 80f, angularSpeed: 0.05f, moveSpeed: 14f);
            }
        }

        // 绘制：彩虹发光 + 光晕
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;

            // 彩虹色（原版 Disco 颜色每帧自动变化）
            Color rainbowColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);

            // 1. 绘制光晕（放大半透明）
            Color auraColor = rainbowColor * 0.35f;
            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                auraColor,
                Projectile.rotation,
                origin,
                Projectile.scale * 1.4f,   // 放大 1.4 倍
                SpriteEffects.None,
                0
            );

            // 2. 绘制弹幕本体（彩虹色叠加）
            Color mainColor = rainbowColor * 0.9f;
            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                mainColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            return false; // 阻止默认绘制
        }

        private void MoveTowardsTarget(Vector2 targetPos, float maxSpeed, float acceleration)
        {
            Vector2 desiredDirection = targetPos - Projectile.Center;
            float distance = desiredDirection.Length();

            if (distance > 5f)
            {
                desiredDirection /= distance;
                Projectile.velocity += desiredDirection * acceleration;

                float currentSpeed = Projectile.velocity.Length();
                if (currentSpeed > maxSpeed)
                {
                    Projectile.velocity *= maxSpeed / currentSpeed;
                }
            }
        }

        private void OrbitAroundPlayer(float radius, float angularSpeed, float moveSpeed)
        {
            Vector2 playerCenter = player.Center;

            float angle = timer * angularSpeed;
            Vector2 orbitOffset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
            Vector2 desiredPosition = playerCenter + orbitOffset;

            MoveTowardsTarget(desiredPosition, maxSpeed: moveSpeed, acceleration: 0.4f);

            Vector2 toPlayer = playerCenter - Projectile.Center;
            float distToPlayer = toPlayer.Length();
            if (distToPlayer > radius * 1.5f)
            {
                Projectile.velocity += Vector2.Normalize(toPlayer) * 0.3f;
            }
        }

        private NPC FindClosestNPC(float maxRange)
        {
            NPC closest = null;
            float minDist = maxRange;
            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || npc.friendly || npc.dontTakeDamage) continue;
                float dist = Vector2.Distance(Projectile.Center, npc.Center);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = npc;
                }
            }
            return closest;
        }
    }
}