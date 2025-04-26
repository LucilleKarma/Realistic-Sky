using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RealisticSky.Assets;
using RealisticSky.Common.DataStructures;
using RealisticSky.Common.Utilities;
using RealisticSky.Content.Sun;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace RealisticSky.Content.Clouds;

public sealed class CloudsRenderer : ModSystem
{
    /// <summary>
    /// The render target that holds the contents of the clouds.
    /// </summary>
    internal static CloudsTargetContent? CloudTarget;

    /// <summary>
    /// The horizontal offset of clouds.
    /// </summary>
    public static float CloudHorizontalOffset
    {
        get;
        set;
    }

    public override void Load()
    {
        // Initialize the cloud target.
        CloudTarget = new();
        Main.ContentThatNeedsRenderTargets.Add(CloudTarget);
    }

    public override void Unload()
    {
        // Dispose the cloud target
        if (CloudTarget != null)
        {
            Main.ContentThatNeedsRenderTargets.Remove(CloudTarget);
            CloudTarget = null;
        }
    }

    public static void Render()
    {
        // Don't do anything if the realistic clouds config is disabled.
        if (!RealisticSkyConfig.Instance.RealisticClouds)
            return;

        // Disable normal clouds.
        Main.cloudBGAlpha = 0f;
        for (int i = 0; i < Main.maxClouds; i++)
            Main.cloud[i].active = false;

        CloudHorizontalOffset -= Main.windSpeedCurrent * 0.3f;

        GraphicsDevice gd = Main.instance.GraphicsDevice;
        Vector2 screenSize = new(gd.Viewport.Width, gd.Viewport.Height);

        // Request and draw our render target.
        Main.spriteBatch.RequestAndDrawRenderTarget(CloudTarget, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y));
    }
}
