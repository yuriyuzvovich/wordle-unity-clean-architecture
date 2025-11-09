using UnityEngine;

namespace Wordle.Presentation.UI.SafeArea
{
    /// <summary>
    /// Pure C# logic for calculating safe area anchor points.
    /// Separated from MonoBehaviour for testability.
    /// </summary>
    public static class SafeAreaCalculator
    {
        /// <summary>
        /// Calculates anchor min/max values based on safe area and screen dimensions.
        /// </summary>
        /// <param name="safeArea">The device's safe area rectangle</param>
        /// <param name="screenWidth">Screen width in pixels</param>
        /// <param name="screenHeight">Screen height in pixels</param>
        /// <param name="applyLeft">Whether to apply left safe area inset</param>
        /// <param name="applyRight">Whether to apply right safe area inset</param>
        /// <param name="applyBottom">Whether to apply bottom safe area inset</param>
        /// <param name="applyTop">Whether to apply top safe area inset</param>
        /// <param name="additionalLeftPadding">Additional left padding in pixels</param>
        /// <param name="additionalRightPadding">Additional right padding in pixels</param>
        /// <param name="additionalBottomPadding">Additional bottom padding in pixels</param>
        /// <param name="additionalTopPadding">Additional top padding in pixels</param>
        /// <returns>Tuple of (anchorMin, anchorMax)</returns>
        public static (Vector2 anchorMin, Vector2 anchorMax) Calculate(
            Rect safeArea,
            int screenWidth,
            int screenHeight,
            bool applyLeft,
            bool applyRight,
            bool applyBottom,
            bool applyTop,
            float additionalLeftPadding = 0f,
            float additionalRightPadding = 0f,
            float additionalBottomPadding = 0f,
            float additionalTopPadding = 0f)
        {
            if (screenWidth == 0 || screenHeight == 0)
            {
                return (Vector2.zero, Vector2.one);
            }

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= screenWidth;
            anchorMin.y /= screenHeight;
            anchorMax.x /= screenWidth;
            anchorMax.y /= screenHeight;

            if (!applyLeft)
            {
                anchorMin.x = 0f;
            }
            else if (additionalLeftPadding > 0f)
            {
                anchorMin.x += additionalLeftPadding / screenWidth;
            }

            if (!applyRight)
            {
                anchorMax.x = 1f;
            }
            else if (additionalRightPadding > 0f)
            {
                anchorMax.x -= additionalRightPadding / screenWidth;
            }

            if (!applyBottom)
            {
                anchorMin.y = 0f;
            }
            else if (additionalBottomPadding > 0f)
            {
                anchorMin.y += additionalBottomPadding / screenHeight;
            }

            if (!applyTop)
            {
                anchorMax.y = 1f;
            }
            else if (additionalTopPadding > 0f)
            {
                anchorMax.y -= additionalTopPadding / screenHeight;
            }

            return (anchorMin, anchorMax);
        }
    }
}
