using ProperVersion;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace HudClient
{
    public class GuiDialogClient : ModSystem
    {
        public static GuiDialogClient guiDialogClient;
        private ICoreClientAPI capi;
        private OverlayTask overlayTask;
        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            capi = api;
            overlayTask = new OverlayTask(api);
            CommandArgumentParsers parsers = api.ChatCommands.Parsers;
            var setwaymarker = api.ChatCommands.Create("setwaymarker").RequiresPlayer()
                .WithDescription("[name] save curret position to ui marker")
                .WithArgs(new ICommandArgumentParser[]
                {
                    parsers.Word("name"),
                })
                .HandleWith(new OnCommandDelegate(savemarker));
            var delwaymarker = api.ChatCommands.Create("delwaymarker").RequiresPlayer()
                .WithDescription("[name] delete curret position from ui marker")
                .WithArgs(new ICommandArgumentParser[]
                {
                    parsers.Word("name"),
                })
                .HandleWith(new OnCommandDelegate(delmarker));
            OverlayTaskLoad(); 
        }

        private TextCommandResult delmarker(TextCommandCallingArgs args)
        {
            if (args.ArgCount != 1)
                return TextCommandResult.Error("Error delete location, use [name-marker]");
            overlayTask.RemoveMarker((args[0] as string));
            return TextCommandResult.Success("Succses delete location.");
        }

        private TextCommandResult savemarker(TextCommandCallingArgs args)
        {
            if (args.ArgCount != 1)
                return TextCommandResult.Error("Error save location, use [name-marker] at pos");
            overlayTask.AddMarker((args[0] as string), capi.World.Player.Entity.Pos.XYZ);
            return TextCommandResult.Success("Succses save location.");
        }

        private void OverlayTaskLoad()
        {
            overlayTask.Compose();
            overlayTask.TryOpen();
        }
    }
}
