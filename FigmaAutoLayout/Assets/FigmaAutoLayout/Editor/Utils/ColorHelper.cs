using System.Collections.Generic;
using System.Linq;
using Blobler.Objects;
using UnityEngine;

namespace Blobler.Utils
{
    internal static class ColorHelper 
    {
        internal static bool NeedAddImage(IReadOnlyList<FigmaFill> fills)
        {
            return fills.Any(x => !x.visible.HasValue || x.visible.Value); 
        }
        
        internal static Color CalculateColor(IReadOnlyList<FigmaFill> fills)
        {
            var color = new Color();
            
            if (fills == null || fills.Count == 0)
                return color;

            var firstFill = fills[0];
            if (!firstFill.visible.HasValue || firstFill.visible.Value)
            {
                switch (firstFill.type)
                {
                    case "SOLID":
                        color = new Color(firstFill.color.r, firstFill.color.g, firstFill.color.b,
                            firstFill.opacity.GetValueOrDefault(firstFill.color.a));
                        break;
                    case "IMAGE":
                        color = Color.white;
                        break;
                }
            }

            if (fills.Count <= 1) 
                return color;
            
            for (var i = 1; i < fills.Count; i++)
            {
                var fill = fills[i];

                if (fill.visible.HasValue && !fill.visible.Value)
                    continue;
                
                switch (fill.type)
                {
                    case "SOLID":
                        switch (fill.blendMode)
                        {
                            case FigmaBlendMode.NORMAL:
                            {
                                var opacity = fill.opacity.GetValueOrDefault(fill.color.a);
                                var red = AlphaBlending(color.r, fill.color.r, opacity);
                                var green = AlphaBlending(color.g, fill.color.g, opacity);
                                var blue = AlphaBlending(color.b, fill.color.b, opacity);
                                var alpha = color.a + (1f - color.a) * opacity; 
                                color = new Color(red, green, blue, alpha);
                                break;
                            }
                            case FigmaBlendMode.MULTIPLY:
                            {
                                var opacity = fill.opacity.GetValueOrDefault(fill.color.a);
                                var red = Multiply(color.r, fill.color.r, opacity);
                                var green = Multiply(color.g, fill.color.g, opacity);
                                var blue = Multiply(color.b, fill.color.b, opacity);
                                var alpha = color.a + (1f - color.a) * opacity; 
                                color = new Color(red, green, blue, alpha); 
                                break;
                            }
                            case FigmaBlendMode.OVERLAY:
                            {
                                var opacity = fill.opacity.GetValueOrDefault(fill.color.a);
                                var red = color.a < 0.5f ? Multiply(color.r, fill.color.r, opacity) : Screen(color.r, fill.color.r, opacity);
                                var green = color.g < 0.5f ? Multiply(color.g, fill.color.g, opacity) : Screen(color.g, fill.color.g, opacity);
                                var blue = color.b < 0.5f ? Multiply(color.b, fill.color.b, opacity) : Screen(color.b, fill.color.b, opacity);
                                var alpha = color.a + (1f - color.a) * opacity; 
                                color = new Color(red, green, blue, alpha); 
                                break;
                            }
                            case FigmaBlendMode.SCREEN:
                            {
                                var opacity = fill.opacity.GetValueOrDefault(fill.color.a);
                                var red = Screen(color.r, fill.color.r, opacity);
                                var green = Screen(color.g, fill.color.g, opacity);
                                var blue = Screen(color.b, fill.color.b, opacity);
                                var alpha = color.a + (1f - color.a) * opacity; 
                                color = new Color(red, green, blue, alpha); 
                                break;
                            }
                            default:
                            {
                                Debug.Log($"Blend mode {fill.blendMode} in development yet. It will be normal blend mode."); 
                                var opacity = fill.opacity.GetValueOrDefault(fill.color.a);
                                var red = AlphaBlending(color.r, fill.color.r, opacity);
                                var green = AlphaBlending(color.g, fill.color.g, opacity);
                                var blue = AlphaBlending(color.b, fill.color.b, opacity);
                                var alpha = color.a + (1f - color.a) * opacity; 
                                color = new Color(red, green, blue, alpha);
                                break;
                            }
                        }

                        break;
                    default:
                        Debug.LogWarning($"Unity base UI tools does not support gradients like {fill.type}. Try ask your UI designer fix it :)");
                        break;
                }
            }

            return color;
        }

        private static float Multiply(float a, float b, float alpha)
        {
            return AlphaBlending(a, a * b, alpha);
        }

        private static float Screen(float a, float b, float alpha)
        {
            return AlphaBlending(a, 1f - (1f - a) * (1f - b), alpha);
        }

        private static float AlphaBlending(float a, float b, float alpha)
        {
            return b * alpha + a * (1f - alpha);
        }
    }
}
