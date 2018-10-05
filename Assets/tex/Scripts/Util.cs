namespace tex
{
    public class Util
    {
        public static int NearestPowerOfTwo(int n)
        {
            int v = n; 

            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++; // next power of 2

            int x = v >> 1; // previous power of 2

            return (v - n) > (n - x) ? x : v;
        }
    }
}