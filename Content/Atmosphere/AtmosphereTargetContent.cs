using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RealisticSky.Assets;
using RealisticSky.Common.DataStructures;
using RealisticSky.Content.Sun;
using Terraria;
using Terraria.GameContent;

namespace RealisticSky.Content.Atmosphere;

public sealed class AtmosphereTargetContent : ARenderTargetContentByRequest
{
    protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        // Initialize the underlying render target if necessary.
        Vector2 size = new(device.Viewport.Width, device.Viewport.Height);
        PrepareARenderTarget_WithoutListeningToEvents(ref _target, Main.instance.GraphicsDevice, (int)size.X, (int)size.Y, RenderTargetUsage.PreserveContents);

        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);

        // Draw the host's contents to the render target.
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.identity);
        RenderToTarget();
        spriteBatch.End();

        device.SetRenderTarget(null);

        // Mark preparations as completed.
        _wasPrepared = true;
    }

    private static void RenderToTarget()
    {
        // Since this can render on the mod screen it's important that the shader be checked for if it's disposed or not.
        if (RealisticSkyConfig.Instance is null)
            return;

        Effect shader = EffectsRegistry.AtmosphereShader.Value;
        if (shader?.IsDisposed ?? true)
            return;

        SkyPlayerSnapshot player = SkyPlayerSnapshot.TakeSnapshot();
        float spaceInterpolant = RealisticSkyManager.SpaceHeightInterpolant;

        // Calculate the true screen size.
        Vector2 screenSize = new(Main.instance.GraphicsDevice.Viewport.Width, Main.instance.GraphicsDevice.Viewport.Height);

        // Calculate opacity and brightness values based on a combination of how far in space the player is and what the general sky brightness is.
        float worldYInterpolant = player.Center.Y / player.MaxTilesY / 16f;
        float upperSurfaceRatioStart = (float)(player.WorldSurface / player.MaxTilesY) * 0.5f;
        float surfaceInterpolant = Utils.GetLerpValue(RealisticSkyManager.SpaceYRatioStart, upperSurfaceRatioStart, worldYInterpolant, true);

        float radius = MathHelper.Lerp(17000f, 6400f, spaceInterpolant);
        float yOffset = (spaceInterpolant * 600f + 250f) * screenSize.Y / 1440f;
        float baseSkyBrightness = RealisticSkyManager.SkyBrightness;
        float atmosphereOpacity = Utils.GetLerpValue(0.08f, 0.2f, baseSkyBrightness + spaceInterpolant * 0.4f, true) * MathHelper.Lerp(1f, 0.5f, surfaceInterpolant) * Utils.Remap(baseSkyBrightness, 0.078f, 0.16f, 0.9f, 1f);

        // Calculate the exponential sunlight exposure coefficient.
        float sunlightExposure = Utils.Remap(RealisticSkyConfig.Instance.SunlightExposure, RealisticSkyConfig.MinSunlightExposure, RealisticSkyConfig.MaxSunlightExposure, 0.4f, 1.6f);

        // Prepare the sky shader.
        RealisticSkyConfig config = RealisticSkyConfig.Instance;
        Vector3 lightWavelengths = new(config.RedWavelength, config.GreenWavelength, config.BlueWavelength);

        shader.Parameters["globalTime"]?.SetValue(Main.GlobalTimeWrappedHourly);
        shader.Parameters["atmosphereRadius"]?.SetValue(radius);
        shader.Parameters["planetRadius"]?.SetValue(radius * 0.8f);
        shader.Parameters["performanceMode"]?.SetValue(RealisticSkyConfig.Instance.PerformanceMode);
        shader.Parameters["screenHeight"]?.SetValue(screenSize.Y);
        shader.Parameters["sunPosition"]?.SetValue(new Vector3(SunPositionSaver.SunPosition - Vector2.UnitY * Main.sunModY * RealisticSkyManager.SpaceHeightInterpolant, -500f));
        shader.Parameters["planetPosition"]?.SetValue(new Vector3(screenSize.X * 0.4f, radius + yOffset, 0f));
        shader.Parameters["rgbLightWavelengths"]?.SetValue(lightWavelengths);
        shader.Parameters["sunlightExposure"]?.SetValue(sunlightExposure);
        shader.CurrentTechnique.Passes[0].Apply();

        // Draw the atmosphere.
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Vector2 drawPosition = screenSize * 0.5f;
        Vector2 skyScale = screenSize / pixel.Size();
        Main.spriteBatch.Draw(pixel, drawPosition, null, Color.White * atmosphereOpacity, 0f, pixel.Size() * 0.5f, skyScale, 0, 0f);
    }
}
