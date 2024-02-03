using ProperVersion;
using System;
using System.Runtime.InteropServices;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace WayMarker
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
            var savewaymarker = api.ChatCommands.Create("savewaymarker").RequiresPlayer()
                .WithDescription("[name] [color] save curret position to ui marker")
                .WithArgs(new ICommandArgumentParser[]
                {
                    parsers.Word("name"),
                    parsers.Word("color"),
                })
                .HandleWith(new OnCommandDelegate(savemarker));
            var delwaymarker = api.ChatCommands.Create("delwaymarker").RequiresPlayer()
                .WithDescription("[name] delete curret position from ui marker")
                .WithArgs(new ICommandArgumentParser[]
                {
                    parsers.Word("name"),
                })
                .HandleWith(new OnCommandDelegate(delmarker));
            var statewaymarker = api.ChatCommands.Create("statewaymarker").RequiresPlayer()
                .WithDescription("[name] [enabled/disabled] activate or disable ui marker")
                .WithArgs(new ICommandArgumentParser[]
                {
                    parsers.Word("name"),
                    parsers.Bool("enabled"),
                })
                .HandleWith(new OnCommandDelegate(statemarker));
            var setwaymarker = api.ChatCommands.Create("setcolorwaymarker").RequiresPlayer()
                .WithDescription("[name] [color] set color our ui marker")
                .WithArgs(new ICommandArgumentParser[]
                {
                                parsers.Word("name"),
                                parsers.Word("color"),
                })
                .HandleWith(new OnCommandDelegate(setcolorwaymarker));
            var colorwaymarker = api.ChatCommands.Create("colorswaymarker").RequiresPlayer()
                .WithDescription("list of all colors")
                .HandleWith(new OnCommandDelegate(colorswaymarker));
            OverlayTaskLoad(); 
        }

        private TextCommandResult colorswaymarker(TextCommandCallingArgs args)
        {
            string message = "Way marker colors:";
            foreach (var arg in Enum.GetNames(typeof(ClientStorage.ColorPicker))) {
                message += arg.ToString() + "\n";
            }
            return TextCommandResult.Success(message);
        }

        private TextCommandResult setcolorwaymarker(TextCommandCallingArgs args)
        {
            if (args.ArgCount != 2)
                return TextCommandResult.Error("Error set color marker, use [name-marker] [color]");
            overlayTask.SetColorMarker(args[0] as string, args[1] as string);
            return TextCommandResult.Success("Succses set color to:" + args[1]);
        }

        private TextCommandResult statemarker(TextCommandCallingArgs args)
        {
            if (args.ArgCount != 2)
                return TextCommandResult.Error("Error state location, use [name-marker] [enabled/disabled]");
            overlayTask.StateMarker(args[0] as string, (bool)args[1]);
            return TextCommandResult.Success("Succses state location:" + args[1]);
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
            if (args.ArgCount != 2)
                return TextCommandResult.Error("Error save location, use [name-marker] at pos");
            overlayTask.AddMarker((args[0] as string),args[1] as string, capi.World.Player.Entity.Pos.XYZ);
            return TextCommandResult.Success("Succses save location.");
        }

        private void OverlayTaskLoad()
        {
            overlayTask.Compose();
            overlayTask.TryOpen();
        }
    }
}
