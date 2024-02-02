using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace HudClient
{
    internal static class CoreAPIExtensions
    {
        // Token: 0x06000083 RID: 131 RVA: 0x00005224 File Offset: 0x00003424
        public static Vec3d GetPlayerPosition(this ICoreClientAPI api)
        {
            return api.World.Player.Entity.Pos.XYZ;
        }

        // Token: 0x06000084 RID: 132 RVA: 0x00005240 File Offset: 0x00003440
        public static Vec3i GetPlayerPositioni(this ICoreClientAPI api)
        {
            Vec3d xyz = api.World.Player.Entity.Pos.XYZ;
            return new Vec3i((int)xyz.X, (int)xyz.Y, (int)xyz.Z);
        }
    }
}
