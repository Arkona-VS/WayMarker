using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace WayMarker
{
    public class OverlayTask : HudElement
    {
        private GuiElementCustomDraw marker;
        private IRenderAPI _rapi;
        private double[] screenCenter = { 0, 0 };
        public override bool ShouldReceiveMouseEvents()
        {
            return false;
        }
        public OverlayTask(ICoreClientAPI capi) : base(capi)
        {
            _rapi = capi.Render;
            capi.Event.RegisterGameTickListener((float dt) => {
                   marker.Redraw();
            }, 16);
            capi.Event.RegisterGameTickListener((float dt) =>
            {
                screenCenter[0] = (_rapi.FrameWidth / 2);
                screenCenter[1] = (_rapi.FrameHeight / 2);
            }, 1000);
            var loc = this.capi.LoadModConfig<Dictionary<string, Dictionary<string, (bool, (double[], double[]), Vec3d)>>> ("waymarker.json");
            if (loc != null)
                ClientStorage.location = loc;
            return;
        }
        public void AddMarker(string marker,string color, Vec3d position)
        {
            goto Inspect;
        CreateLoc:
            {
                ClientStorage.location.Add(this.capi.World.SavegameIdentifier, new Dictionary<string, (bool, (double[], double[]), Vec3d)>());
                goto Inspect;
            }

        Inspect:
            {
                if (ClientStorage.location.ContainsKey(this.capi.World.SavegameIdentifier))
                {
                    if (!ClientStorage.location[this.capi.World.SavegameIdentifier].ContainsKey(marker))
                        ClientStorage.location[this.capi.World.SavegameIdentifier].Add(marker, (true, ClientStorage.GetColorFromName(color), position));
                }
                else
                    goto CreateLoc;
            }
            this.capi.StoreModConfig(ClientStorage.location, "waymarker.json");
        }
        public void SetColorMarker(string markername, string color)
        {
            if (!ClientStorage.location.ContainsKey(this.capi.World.SavegameIdentifier))
                return;

            if (!ClientStorage.location[this.capi.World.SavegameIdentifier].ContainsKey(markername))
                return;
            var dict = ClientStorage.location[this.capi.World.SavegameIdentifier];
            var marker = dict[markername];
            marker.color = ClientStorage.GetColorFromName(color);
            dict[markername] = marker;
            ClientStorage.location[this.capi.World.SavegameIdentifier] = dict;
            this.capi.StoreModConfig(ClientStorage.location, "waymarker.json");
        }
        public void StateMarker(string markername, bool state)
        {
            if (!ClientStorage.location.ContainsKey(this.capi.World.SavegameIdentifier))
                return;

            if (!ClientStorage.location[this.capi.World.SavegameIdentifier].ContainsKey(markername))
                return;
            var dict = ClientStorage.location[this.capi.World.SavegameIdentifier];
            var marker = dict[markername];
            marker.enabled = state;
            dict[markername] = marker;
            ClientStorage.location[this.capi.World.SavegameIdentifier] = dict;
            this.capi.StoreModConfig(ClientStorage.location, "waymarker.json");
        }
        public void RemoveMarker(string marker)
        {
            if (ClientStorage.location.ContainsKey(this.capi.World.SavegameIdentifier))
                ClientStorage.location[this.capi.World.SavegameIdentifier].Remove(marker);
            this.capi.StoreModConfig(ClientStorage.location, "waymarker.json");
        }
        public void Compose()
        {
            ElementBounds textBounds = ElementBounds.Fixed(EnumDialogArea.RightFixed, 40.0, 10.0, (double)this._rapi.FrameWidth, (double)this._rapi.FrameHeight);
            ElementBounds bounds = textBounds.CopyOnlySize();
            ElementBounds hudbounds = ElementBounds.Fixed(EnumDialogArea.CenterMiddle, 0.0, 0.0, 0.0, 0.0);
            base.SingleComposer = this.capi.Gui.CreateCompo("waymarker", hudbounds)
                .AddDynamicCustomDraw(bounds, new DrawDelegateWithBounds(HandleDraw), "marker")
                .EndChildElements()
                .Compose(true);
            marker = base.SingleComposer.GetCustomDraw("marker");
        }
        private float[] GetTagPos(Vec3d blockPos)
        {
            Vec3d pos = blockPos.AddCopy(0.5, 0.5, 0.5);
            Vec3d player = capi.GetPlayerPosition();
            Vec3d vec3d = MatrixToolsd.Project(pos, this._rapi.PerspectiveProjectionMat, this._rapi.PerspectiveViewMat, this._rapi.FrameWidth, this._rapi.FrameHeight);
            {

                if(vec3d.X > (double)this._rapi.FrameWidth)
                    vec3d.X = (double)this._rapi.FrameWidth;
                if(vec3d.Y > (double)this._rapi.FrameHeight)
                    vec3d.Y = (double)this._rapi.FrameHeight;
                if (vec3d.X < 0.0)
                    vec3d.X = 0;
                if (vec3d.Y < 0.0)
                    vec3d.Y = 0;

                if (vec3d.Z < 0.0) // Если игрок смотрит в противоположную сторону
                { 
                    vec3d.Y = 0;
                    if (player.Y > blockPos.Y)
                        vec3d.Y = 0;
                    else
                        vec3d.Y = _rapi.FrameHeight;
                    vec3d.X = _rapi.FrameWidth - vec3d.X;
                }
            }
            return new float[]
            {
                (float)vec3d.X,
                (float)this._rapi.FrameHeight - (float)vec3d.Y
            };
        }
        private void DrawLabelAt(Context ctx, float[] screenPos, string label, bool fullSize = false)
        {
            ctx.SetFontSize((double)(fullSize ? 14f : Math.Max(8f, 30f * 0.75)));
            int stepCount = 0;
            foreach (var lab in label.Split('\n'))
            {
                TextExtents textExtents = ctx.TextExtents(lab);
                double num = textExtents.XBearing + textExtents.Width / 1.5;
                double num2 = textExtents.YBearing + textExtents.Height / 2.0 - stepCount;
                double num3 = (double)screenPos[0] - textExtents.Width / 1.5 - 4.0;
                double num4 = (double)screenPos[1] - textExtents.Height / 2.0 - 4.0 + stepCount;
                double num5 = textExtents.Width + 8.0;
                double num6 = textExtents.Height + 8.0;
                bool flag = fullSize || ((double)(_rapi.FrameWidth / 2) >= num3 && (double)(_rapi.FrameWidth / 2) <= num3 + num5 && (double)(_rapi.FrameHeight / 2) >= num4 && (double)(_rapi.FrameHeight / 2) <= num4 + num6);
                ctx.SetSourceRGBA(GuiStyle.DialogStrongBgColor[0], GuiStyle.DialogStrongBgColor[1], GuiStyle.DialogStrongBgColor[2], 0.75);
                ctx.Rectangle(num3, num4, num5, num6);
                ctx.Fill();
                ctx.SetSourceRGBA(GuiStyle.DialogDefaultTextColor);
                ctx.MoveTo((double)screenPos[0] - num, (double)screenPos[1] - num2);
                ctx.ShowText(lab);
                stepCount += (int)num6;
            }
        }
        private static float DrawCircleAt(Context ctx, float[] screenPos, double[] colorRgba, double[] outlineRgba,float size)
        {
            float num = Math.Max(1.8f, 10f * size);
            ctx.NewPath();
            ctx.SetSourceRGBA(colorRgba);
/*            ctx.Arc((double)screenPos[0], (double)screenPos[1], (double)num, 0.0, 6.283185307179586);*/
            DrawCustomMarker(ctx, screenPos, size);
            ctx.FillPreserve();
            ctx.SetSourceRGBA(outlineRgba);
            ctx.Stroke();
            return num;
        }

        private static void DrawCustomMarker(Context ctx, float[] screenPos, float size)
        {
            double x = 0;
            double y = 0;
            if (screenPos[0] == 0)
                x = screenPos[0] + 15;
            else
                x = screenPos[0] - 10;
            if (screenPos[1] == 0)
                y = screenPos[1] + 5;
            else
                y = screenPos[1] - 40;
            ctx.LineTo(x, y);
            ctx.RelLineTo(-15, 15);
            ctx.RelLineTo(10, 15);
            ctx.RelLineTo(10, -15);
            ctx.RelLineTo(-15, -15);
        }

        private void HandleDraw(Context ctx, ImageSurface surface, ElementBounds currentBounds)
        {
            ctx.Save();
            ctx.LineWidth = 3;
            if(ClientStorage.location.Count > 0 && capi.World.Player != null)
            {
                foreach (var loc in ClientStorage.location[this.capi.World.SavegameIdentifier])
                {
                    if (!loc.Value.enabled)
                        continue;
                    float[] tagPos = GetTagPos(loc.Value.vec3);
                    if (tagPos != null)
                    {
                        DrawCircleAt(ctx, tagPos, loc.Value.color.markerColour, loc.Value.color.markerOutlineColour, 1);
                        if (tagPos[0] <= screenCenter[0] + 25 && tagPos[0] >= screenCenter[0])
                        {
                            if (tagPos[1] <= screenCenter[1] + 40 && tagPos[1] >= screenCenter[1])
                            {
                                int num = (int)capi.GetPlayerPosition().DistanceTo(loc.Value.vec3);
                                DrawLabelAt(ctx, tagPos, loc.Key + "\n" + num.ToString() + " m.");
                            }
                        }
                    }
                }
            }
            ctx.Restore();
        }
    }
}
