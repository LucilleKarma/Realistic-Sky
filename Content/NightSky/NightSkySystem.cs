using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RealisticSky.Common.DataStructures;
using Terraria;
using Terraria.ModLoader;
using RealisticSky.Common.Utilities;

namespace RealisticSky.Content.NightSky;

public sealed class NightSkySystem : ModSystem
{
    public override void Load()
    {
        On_Main.DrawStarsInBackground += DrawStars;
    }

    private void DrawStars(On_Main.orig_DrawStarsInBackground orig, Main self, Main.SceneArea sceneArea, bool artificial)
    {
        if (RealisticSkyConfig.Instance is null || RealisticSkyManager.TemporarilyDisabled || !RealisticSkyManager.CanRender)
        {
            orig(self, sceneArea, artificial);
            return;
        }

        // Calculate the background draw matrix in advance.
        Vector3 translationDirection = new(1f, Main.BackgroundViewMatrix.Effects.HasFlag(SpriteEffects.FlipVertically) ? -1f : 1f, 1f);
        Matrix backgroundMatrix = Main.BackgroundViewMatrix.ZoomMatrix * Matrix.CreateScale(translationDirection);

        // Draw stars and the galaxy.
        Main.spriteBatch.End(out SpriteBatchSnapshot snapshot);
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, Main.BackgroundViewMatrix.EffectMatrix);
        GalaxyRenderer.Render();

        Main.spriteBatch.End();
        StarsRenderer.Render(RealisticSkyManager.Opacity, backgroundMatrix);
        Main.spriteBatch.Begin(in snapshot);
    }
}
