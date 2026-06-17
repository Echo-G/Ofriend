using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ofriend.Projectiles
{
    public class HomingAmulet : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            // 寻找目标
            NPC target = FindClosestNPC(500f);
            if (target != null)
            {
                // 追踪目标
                Vector2 direction = target.Center - Projectile.Center;
                direction.Normalize();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 6f, 0.1f);
            }
            else
            {
                // 无目标时，保持当前速度
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 6f;
            }
            
            // 旋转效果
            Projectile.rotation += 0.1f;
        }

        private NPC FindClosestNPC(float maxDistance)
        {
            NPC closestNPC = null;
            float closestDistance = maxDistance;
            
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.lifeMax > 5)
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPC = npc;
                    }
                }
            }
            
            return closestNPC;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 命中NPC时的处理
        }
    }
}
