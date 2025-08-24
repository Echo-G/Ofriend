using Terraria.ModLoader;
using Ofriend.Systems;

namespace Ofriend
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class Ofriend : Mod
	{
		public static Ofriend Instance { get; private set; }

		public Ofriend()
		{
			Instance = this;
		}

		public override void Load()
		{
			// 系统加载时的初始化
		}

		public override void Unload()
		{
			// 系统卸载时的清理
			Instance = null;
		}
	}
}
