using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RealisticSky.Common.DataStructures;
using Terraria.GameContent;

namespace RealisticSky.Common.Utilities;

public static class DrawingUtils
{
    /// <summary>
    /// Calls <see cref="SpriteBatch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect?, Matrix)"/> with the data on <paramref name="snapshot"/>.
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="snapshot"></param>
    public static void Begin(this SpriteBatch spriteBatch, in SpriteBatchSnapshot snapshot) =>
        spriteBatch.Begin(snapshot.sortMode, snapshot.blendState, snapshot.samplerState, snapshot.depthStencilState, snapshot.rasterizerState, snapshot.effect, snapshot.transformationMatrix);

    /// <summary>
    /// Calls <see cref="SpriteBatch.End()"/> and outs <paramref name="spriteBatch"/>'s data as <paramref name="snapshot"/>.
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="snapshot"></param>
    public static void End(this SpriteBatch spriteBatch, out SpriteBatchSnapshot snapshot)
    {
        snapshot = SpriteBatchSnapshot.Capture(spriteBatch);
        spriteBatch.End();
    }

    /// <summary>
    /// Requests the <paramref name="renderTarget"/> and draws it if its ready.
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="renderTarget"></param>
    public static void RequestAndDrawRenderTarget(this SpriteBatch spriteBatch, ARenderTargetContentByRequest? renderTarget)
    {
        renderTarget?.Request();
        if (renderTarget?.IsReady is true)
            spriteBatch.Draw(renderTarget.GetTarget(), Vector2.Zero, Color.White);
    }

    /// <summary>
    /// Requests the <paramref name="renderTarget"/> and draws it if its ready.
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="renderTarget"></param>
    public static void RequestAndDrawRenderTarget(this SpriteBatch spriteBatch, ARenderTargetContentByRequest? renderTarget, Rectangle destination)
    {
        renderTarget?.Request();
        if (renderTarget?.IsReady is true)
            spriteBatch.Draw(renderTarget.GetTarget(), destination, Color.White);
    }
}
