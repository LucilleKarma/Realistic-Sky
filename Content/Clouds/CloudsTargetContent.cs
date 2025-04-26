using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RealisticSky.Assets;
using RealisticSky.Common.Utilities;
using RealisticSky.Content.Sun;
using Terraria;
using Terraria.GameContent;

namespace RealisticSky.Content.Clouds;

public sealed class CloudsTargetContent : ARenderTargetContentByRequest
{
    protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        // Initialize the underlying render target if necessary.
        Vector2 size = new Vector2(device.Viewport.Width, device.Viewport.Height) * 0.5f;
        PrepareARenderTarget_WithoutListeningToEvents(ref _target, device, (int)size.X, (int)size.Y, RenderTargetUsage.PreserveContents);

        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);

        // Draw the host's contents to the render target.
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
        RenderToTarget();
        spriteBatch.End();

        device.SetRenderTarget(null);

        // Mark preparations as completed.
        _wasPrepared = true;
    }

    private static void RenderToTarget()
    {
        if (RealisticSkyConfig.Instance is null)
            return;

        Effect shader = EffectsRegistry.CloudShader.Value;
        if (shader?.IsDisposed ?? true)
            return;

        GraphicsDevice gd = Main.instance.GraphicsDevice;
        Vector2 screenSize = new(gd.Viewport.Width, gd.Viewport.Height);

        Matrix backgroundMatrix = Main.BackgroundViewMatrix.TransformationMatrix;

        Vector2 sunPosition = Main.dayTime ? SunPositionSaver.SunPosition : SunPositionSaver.MoonPosition;
        sunPosition *= 0.5f;
        sunPosition = Vector2.Transform(sunPosition, Matrix.Invert(backgroundMatrix));

        float windDensityInterpolant = MathUtils.Saturate(Main.cloudAlpha + MathF.Abs(Main.windSpeedCurrent) * 0.84f);

        shader.Parameters["globalTime"]?.SetValue(Main.GlobalTimeWrappedHourly);
        shader.Parameters["screenSize"]?.SetValue(screenSize);
        shader.Parameters["worldPosition"]?.SetValue(Main.screenPosition);
        shader.Parameters["sunPosition"]?.SetValue(new Vector3(sunPosition, 5f));
        shader.Parameters["sunColor"]?.SetValue(Main.ColorOfTheSkies.ToVector4());
        shader.Parameters["cloudColor"]?.SetValue(Color.Lerp(Color.Wheat, Color.LightGray, 0.85f).ToVector4());
        shader.Parameters["densityFactor"]?.SetValue(MathHelper.Lerp(10f, 0.3f, MathF.Pow(windDensityInterpolant, 0.48f)));
        shader.Parameters["cloudHorizontalOffset"]?.SetValue(CloudsRenderer.CloudHorizontalOffset);
        shader.CurrentTechnique.Passes[0].Apply();

        Texture2D cloud = TexturesRegistry.CloudDensityMap.Value;
        Main.spriteBatch.Draw(cloud, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
    }
}
