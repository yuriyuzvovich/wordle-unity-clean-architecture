using System;

namespace Wordle.Core.Theme
{
    public readonly struct ColorRgba
    {
        public readonly float R;
        public readonly float G;
        public readonly float B;
        public readonly float A;

        public ColorRgba(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public ColorRgba(float r, float g, float b) : this(r, g, b, 1f)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj is ColorRgba other)
            {
                return Math.Abs(R - other.R) < Single.Epsilon &&
                    Math.Abs(G - other.G) < Single.Epsilon &&
                    Math.Abs(B - other.B) < Single.Epsilon &&
                    Math.Abs(A - other.A) < Single.Epsilon;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + R.GetHashCode();
                hash = hash * 31 + G.GetHashCode();
                hash = hash * 31 + B.GetHashCode();
                hash = hash * 31 + A.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(ColorRgba left, ColorRgba right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ColorRgba left, ColorRgba right)
        {
            return !left.Equals(right);
        }
    }
}