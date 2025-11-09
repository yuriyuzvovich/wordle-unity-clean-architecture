using NUnit.Framework;
using UnityEngine;
using Wordle.Presentation.UI.SafeArea;

namespace Wordle.Presentation.Tests
{
    [TestFixture]
    public class SafeAreaCalculatorTests
    {
        [Test]
        public void Calculate_FullScreenNoInsets_ReturnsZeroToOne()
        {
            Rect safeArea = new Rect(0, 0, 1080, 1920);
            int screenWidth = 1080;
            int screenHeight = 1920;

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea, screenWidth, screenHeight,
                true, true, true, true);

            Assert.AreEqual(Vector2.zero, anchorMin);
            Assert.AreEqual(Vector2.one, anchorMax);
        }

        [Test]
        public void Calculate_WithTopNotch_AdjustsTopAnchor()
        {
            Rect safeArea = new Rect(0, 0, 1080, 1820);
            int screenWidth = 1080;
            int screenHeight = 1920;

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea, screenWidth, screenHeight,
                true, true, true, true);

            Assert.AreEqual(0f, anchorMin.x);
            Assert.AreEqual(0f, anchorMin.y);
            Assert.AreEqual(1f, anchorMax.x);
            Assert.AreEqual(1820f / 1920f, anchorMax.y, 0.0001f);
        }

        [Test]
        public void Calculate_WithBottomHomeIndicator_AdjustsBottomAnchor()
        {
            Rect safeArea = new Rect(0, 100, 1080, 1820);
            int screenWidth = 1080;
            int screenHeight = 1920;

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea, screenWidth, screenHeight,
                true, true, true, true);

            Assert.AreEqual(0f, anchorMin.x);
            Assert.AreEqual(100f / 1920f, anchorMin.y, 0.0001f);
            Assert.AreEqual(1f, anchorMax.x);
            Assert.AreEqual(1920f / 1920f, anchorMax.y, 0.0001f);
        }

        [Test]
        public void Calculate_ApplyTopFalse_IgnoresTopInset()
        {
            Rect safeArea = new Rect(0, 0, 1080, 1820);
            int screenWidth = 1080;
            int screenHeight = 1920;

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea, screenWidth, screenHeight,
                true, true, true, false);

            Assert.AreEqual(Vector2.zero, anchorMin);
            Assert.AreEqual(Vector2.one, anchorMax);
        }

        [Test]
        public void Calculate_ApplyBottomFalse_IgnoresBottomInset()
        {
            Rect safeArea = new Rect(0, 100, 1080, 1820);
            int screenWidth = 1080;
            int screenHeight = 1920;

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea, screenWidth, screenHeight,
                true, true, false, true);

            Assert.AreEqual(0f, anchorMin.x);
            Assert.AreEqual(0f, anchorMin.y);
            Assert.AreEqual(1f, anchorMax.x);
            Assert.AreEqual(1920f / 1920f, anchorMax.y);
        }

        [Test]
        public void Calculate_ApplyLeftFalse_IgnoresLeftInset()
        {
            Rect safeArea = new Rect(50, 0, 1030, 1920);
            int screenWidth = 1080;
            int screenHeight = 1920;

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea, screenWidth, screenHeight,
                false, true, true, true);

            Assert.AreEqual(0f, anchorMin.x);
            Assert.AreEqual(0f, anchorMin.y);
            Assert.AreEqual(1080f / 1080f, anchorMax.x);
            Assert.AreEqual(1f, anchorMax.y);
        }

        [Test]
        public void Calculate_ApplyRightFalse_IgnoresRightInset()
        {
            Rect safeArea = new Rect(50, 0, 1030, 1920);
            int screenWidth = 1080;
            int screenHeight = 1920;

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea, screenWidth, screenHeight,
                true, false, true, true);

            Assert.AreEqual(50f / 1080f, anchorMin.x, 0.0001f);
            Assert.AreEqual(0f, anchorMin.y);
            Assert.AreEqual(1f, anchorMax.x);
            Assert.AreEqual(1f, anchorMax.y);
        }

        [Test]
        public void Calculate_WithAdditionalTopPadding_AddsExtraPadding()
        {
            Rect safeArea = new Rect(0, 0, 1080, 1920);
            int screenWidth = 1080;
            int screenHeight = 1920;
            float additionalTopPadding = 50f;

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea, screenWidth, screenHeight,
                true, true, true, true,
                0f, 0f, 0f, additionalTopPadding);

            Assert.AreEqual(Vector2.zero, anchorMin);
            Assert.AreEqual(1f, anchorMax.x);
            Assert.AreEqual(1f - (50f / 1920f), anchorMax.y, 0.0001f);
        }

        [Test]
        public void Calculate_WithAdditionalBottomPadding_AddsExtraPadding()
        {
            Rect safeArea = new Rect(0, 0, 1080, 1920);
            int screenWidth = 1080;
            int screenHeight = 1920;
            float additionalBottomPadding = 50f;

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea, screenWidth, screenHeight,
                true, true, true, true,
                0f, 0f, additionalBottomPadding, 0f);

            Assert.AreEqual(0f, anchorMin.x);
            Assert.AreEqual(50f / 1920f, anchorMin.y, 0.0001f);
            Assert.AreEqual(Vector2.one, anchorMax);
        }

        [Test]
        public void Calculate_WithAdditionalLeftPadding_AddsExtraPadding()
        {
            Rect safeArea = new Rect(0, 0, 1080, 1920);
            int screenWidth = 1080;
            int screenHeight = 1920;
            float additionalLeftPadding = 30f;

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea, screenWidth, screenHeight,
                true, true, true, true,
                additionalLeftPadding, 0f, 0f, 0f);

            Assert.AreEqual(30f / 1080f, anchorMin.x, 0.0001f);
            Assert.AreEqual(0f, anchorMin.y);
            Assert.AreEqual(Vector2.one, anchorMax);
        }

        [Test]
        public void Calculate_WithAdditionalRightPadding_AddsExtraPadding()
        {
            Rect safeArea = new Rect(0, 0, 1080, 1920);
            int screenWidth = 1080;
            int screenHeight = 1920;
            float additionalRightPadding = 30f;

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea, screenWidth, screenHeight,
                true, true, true, true,
                0f, additionalRightPadding, 0f, 0f);

            Assert.AreEqual(Vector2.zero, anchorMin);
            Assert.AreEqual(1f - (30f / 1080f), anchorMax.x, 0.0001f);
            Assert.AreEqual(1f, anchorMax.y);
        }

        [Test]
        public void Calculate_ZeroScreenDimensions_ReturnsDefaultAnchors()
        {
            Rect safeArea = new Rect(0, 0, 1080, 1920);

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea, 0, 0,
                true, true, true, true);

            Assert.AreEqual(Vector2.zero, anchorMin);
            Assert.AreEqual(Vector2.one, anchorMax);
        }

        [Test]
        public void Calculate_iPhoneXNotch_CorrectAnchors()
        {
            Rect safeArea = new Rect(0, 102, 1125, 2334);
            int screenWidth = 1125;
            int screenHeight = 2436;

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea, screenWidth, screenHeight,
                true, true, true, true);

            Assert.AreEqual(0f, anchorMin.x);
            Assert.AreEqual(102f / 2436f, anchorMin.y, 0.0001f);
            Assert.AreEqual(1f, anchorMax.x);
            Assert.AreEqual((102f + 2334f) / 2436f, anchorMax.y, 0.0001f);
        }

        [Test]
        public void Calculate_AllEdgesWithNotch_HandlesCorrectly()
        {
            Rect safeArea = new Rect(50, 100, 980, 1720);
            int screenWidth = 1080;
            int screenHeight = 1920;

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea, screenWidth, screenHeight,
                true, true, true, true);

            Assert.AreEqual(50f / 1080f, anchorMin.x, 0.0001f);
            Assert.AreEqual(100f / 1920f, anchorMin.y, 0.0001f);
            Assert.AreEqual(1030f / 1080f, anchorMax.x, 0.0001f);
            Assert.AreEqual(1820f / 1920f, anchorMax.y, 0.0001f);
        }
    }
}
