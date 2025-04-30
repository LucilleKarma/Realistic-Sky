using RealisticSky.Common.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace RealisticSky.Content.Atmosphere;

public sealed class AtmosphereRenderer : ModSystem
{
    /// <summary>
    /// The render target that holds the contents of the atmosphere.
    /// </summary>
    internal static AtmosphereTargetContent? AtmosphereTarget;

    public override void Load()
    {
        // Initialize the atmosphere target.
        AtmosphereTarget = new();
        Main.ContentThatNeedsRenderTargets.Add(AtmosphereTarget);
    }

    public override void Unload()
    {
        // Dispose the atmosphere target
        if (AtmosphereTarget != null)
        {
            Main.ContentThatNeedsRenderTargets.Remove(AtmosphereTarget);
            AtmosphereTarget = null;
        }
    }

    public static void RenderFromTarget() => 
        Main.spriteBatch.RequestAndDrawRenderTarget(AtmosphereTarget);
}
