using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace WayMarker
{
    internal static class ClientStorage
    {
        public static Dictionary<string, Dictionary<string, (bool enabled,(double[] markerColour, double[] markerOutlineColour)color, Vec3d vec3)>> location = 
            new Dictionary<string, Dictionary<string, (bool, (double[], double[]), Vec3d)>>();

        public static (double[], double[]) GetColorFromName(string name)
        {
            ColorPicker picker;
            if (!Enum.TryParse(name, true, out picker))
                return Default;
            switch (picker)
            {
                case ColorPicker.Default: return Default;
                case ColorPicker.Purple: return Purple;
                case ColorPicker.Blue: return Blue;
                case ColorPicker.Green: return Green;
                case ColorPicker.Red: return Red;
                case ColorPicker.White: return White;
                case ColorPicker.Yellow: return Yellow;
                default: return Default;
            }
        }
        public static (double[], double[]) Default = (new[] { 1.0, 1.0, 0.25, 0.5 }, new[] { 1.0, 1.0, 0, 0.5 });
        public static (double[], double[]) Red = (new[] { 1.0, 0.0, 0.0, 0.5 }, new[] { 1.0, 0.0, 0.0, 1.0 });
        public static (double[], double[]) Green = (new[] { 0.0, 1.0, 0.0, 0.5 }, new[] { 0.0, 1.0, 0.0, 1.0 });
        public static (double[], double[]) White = (new[] { 1.0, 1.0, 1.0, 0.5 }, new[] { 1.0, 1.0, 1.0, 1.0 });
        public static (double[], double[]) Purple = (new[] { 1.0, 0.0, 0.8, 0.5 }, new[] { 1.0, 0.0, 0.8, 1.0 });
        public static (double[], double[]) Yellow = (new[] { 1.0, 1.0, 0.0, 0.5 }, new[] { 1.0, 1.0, 0.0, 1.0 });
        public static (double[], double[]) Blue = (new[] { 0.0, 0.0, 1.0, 0.5 }, new[] { 0.0, 0.0, 1.0, 1.0 });
        public enum ColorPicker
        {
            Default,
            Red,
            Green,
            White,
            Purple,
            Yellow,
            Blue
        }
    }
}
