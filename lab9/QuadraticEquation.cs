using System;

namespace lab9
{
    public class QuadraticEquation
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public QuadraticEquation() : this(1, 0, 0) { }

        public QuadraticEquation(double a, double b, double c)
        {
            if (a == 0)
                throw new ArgumentException("Коэффициент 'a' не может быть равен 0.");
            A = a;
            B = b;
            C = c;
        }

        public QuadraticEquation(QuadraticEquation other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            A = other.A;
            B = other.B;
            C = other.C;
        }

        public double Discriminant => B * B - 4 * A * C;

        public double[] GetRoots()
        {
            double d = Discriminant;
            if (d < 0) return Array.Empty<double>();
            if (d == 0) return new[] { -B / (2 * A) };

            double sqrtD = Math.Sqrt(d);
            return new[] { (-B - sqrtD) / (2 * A), (-B + sqrtD) / (2 * A) };
        }

        public override string ToString() => $"{A}x² + ({B})x + ({C}) = 0";

        public static QuadraticEquation operator ++(QuadraticEquation eq)
        {
            if (eq == null) throw new ArgumentNullException(nameof(eq));
            return new QuadraticEquation(eq.A + 1, eq.B + 1, eq.C + 1);
        }

        public static QuadraticEquation operator --(QuadraticEquation eq)
        {
            if (eq == null) throw new ArgumentNullException(nameof(eq));
            return new QuadraticEquation(eq.A - 1, eq.B - 1, eq.C - 1);
        }

        public static implicit operator double(QuadraticEquation eq)
        {
            if (eq == null) throw new ArgumentNullException(nameof(eq));
            return eq.Discriminant;
        }

        public static explicit operator bool(QuadraticEquation eq)
        {
            if (eq == null) throw new ArgumentNullException(nameof(eq));
            return eq.Discriminant >= 0;
        }

        public static bool operator ==(QuadraticEquation eq1, QuadraticEquation eq2)
        {
            if (ReferenceEquals(eq1, eq2)) return true;
            if (eq1 is null || eq2 is null) return false;
            return eq1.A == eq2.A && eq1.B == eq2.B && eq1.C == eq2.C;
        }

        public static bool operator !=(QuadraticEquation eq1, QuadraticEquation eq2) => !(eq1 == eq2);

        public override bool Equals(object obj) => obj is QuadraticEquation other && this == other;

        public override int GetHashCode() => HashCode.Combine(A, B, C);
    }
}