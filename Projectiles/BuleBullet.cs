using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Ofriend.Projectiles
{
    public class BuleBullet : ModProjectile
    {
        private const float SearchRange = 700f;
        private const float HomingSpeed = 18f;
        private const float HomingAcceleration = 1.05f;
        private const float MinimumNoTargetSpeed = 8f;

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            NPC target = FindClosestNPC(SearchRange);
            if (target != null)
            {
                Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * HomingSpeed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, HomingAcceleration / HomingSpeed);
            }
            else if (Projectile.velocity.Length() < MinimumNoTargetSpeed)
            {
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * MinimumNoTargetSpeed;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        private NPC FindClosestNPC(float maxDistance)
        {
            NPC closestNPC = null;
            float closestDistance = maxDistance;

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
                    closestNPC = npc;
                }
            }

            return closestNPC;
        }
    }
}
