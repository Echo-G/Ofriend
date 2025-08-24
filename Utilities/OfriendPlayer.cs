using Ofriend.Items.Armor;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Ofriend.Utilities
{
    public class OfriendPlayer : ModPlayer
    {
        //Boki transformation bools
        public bool BokiHide;
        public bool BokiForce;
        public bool BokiPrevious;
        public bool BokiPower;
        public bool BokiTrans;
        //Nanami transformation bools
        public bool NanamiHide;
        public bool NanamiForce;
        public bool NanamiPrevious;
        public bool NanamiPower;
        public bool NanamiTrans;
        public override void ResetEffects()
        {
            BokiPrevious = BokiTrans;
            BokiTrans = BokiHide = BokiForce = BokiPower = false;
            NanamiPrevious = NanamiTrans;
            NanamiTrans = NanamiHide = NanamiForce = NanamiPower = false;
        }
        public override void UpdateVisibleVanityAccessories()
        {
            for (int n = 13; n < 18 + Player.extraAccessorySlots; n++)
            {
                Item item = Player.armor[n];
                if (item.type == ItemType<Boki>())
                {
                    BokiHide = false;
                    BokiForce = true;
                }
                else if (item.type == ItemType<Nanatra>())
                {
                    NanamiHide = false;
                    NanamiForce = true;
                }
            }
        }
        public override void FrameEffects()
        {
            if ((BokiTrans || BokiForce) && !BokiHide)
            {
                var costume = GetInstance<Boki>();
                Player.head = EquipLoader.GetEquipSlot(Mod, "Boki", EquipType.Head);
                Player.body = EquipLoader.GetEquipSlot(Mod, "Boki", EquipType.Body);
                Player.legs = EquipLoader.GetEquipSlot(Mod, "Boki", EquipType.Legs);
            }
            else if ((NanamiTrans || NanamiForce) && !NanamiHide)
            {
                var cosrume = GetInstance<Nanatra>();
                Player.head = EquipLoader.GetEquipSlot(Mod, "Nanatra", EquipType.Head);
                Player.body = EquipLoader.GetEquipSlot(Mod, "Nanatra", EquipType.Body);
                Player.legs = EquipLoader.GetEquipSlot(Mod, "Nanatra", EquipType.Legs);
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (NanamiTrans)
            {
                SoundStyle soundpowerup = new SoundStyle("ofriend/Assets/Sounds/NanamiOnhit");
                SoundEngine.PlaySound(soundpowerup, Player.position);
            }
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (NanamiTrans) modifiers.DisableSound();
            return;
        }
    }
}

