// Converting data parts from an incoming byte array to type dependent arrays which we return.

using System;
namespace F12019New
{
    public class DataConverter
    {
        public static T[] ConvArray<T>(byte[] rawData, int idx, int count, Func<byte[], int, T> convFunc)
        {
            T[] a = new T[count];

            for (int i = 0; i < count; i++)
            {
                a[i] = convFunc(rawData, idx);
            }
            return a;
        }
    }
}
