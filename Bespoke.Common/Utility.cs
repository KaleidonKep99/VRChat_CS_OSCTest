using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bespoke.Common
{
    /// <summary>
	/// General-Purpose utility functions.
	/// </summary>
	public static class Utility
	{
        /// <summary>
        /// Convert a byte array of ASCII characters to a string.
        /// </summary>
        /// <param name="source">The source array containing the ASCII characters.</param>
        /// <returns>A string with the same characters in the byte array.</returns>
        public static string ASCIIByteArrayToString(byte[] source)
        {
            if (source == null)
            {
                return string.Empty;
            }
            else
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                return encoding.GetString(source);
            }
        }

        /// <summary>
        /// Convert a byte array of ASCII characters to a string.
        /// </summary>
        /// <param name="source">The source array containing the ASCII characters.</param>
        /// <returns>A string with the same characters in the byte array.</returns>
        public static string UnicodeByteArrayToString(byte[] source)
        {
            if (source == null)
            {
                return string.Empty;
            }
            else
            {
                UnicodeEncoding encoding = new UnicodeEncoding();
                return encoding.GetString(source);
            }
        }

		/// <summary>
		/// Determine if two values are near each other.
		/// </summary>
		/// <param name="value1">The first value.</param>
		/// <param name="value2">The second value.</param>
		/// <param name="proximityThreshold">The proximity threshold.</param>
		/// <returns>true if the difference between the values is less than or equal to <paramref name="proximityThreshold"/>; otherwise, false.</returns>
        public static bool ValuesInProximity(int value1, int value2, int proximityThreshold)
        {
            return Math.Abs(value1 - value2) <= proximityThreshold;
        }

        /// <summary>
        /// Determine if two values are near each other.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="proximityThreshold">The proximity threshold.</param>
        /// <returns>true if the difference between the values is less than or equal to <paramref name="proximityThreshold"/>; otherwise, false.</returns>
        public static bool ValuesInProximity(double value1, double value2, double proximityThreshold)
        {
            return Math.Abs(value1 - value2) <= proximityThreshold;
        }

        /// <summary>
        /// Determine if two values are near each other.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="proximityThreshold">The proximity threshold.</param>
        /// <returns>true if the difference between the values is less than or equal to <paramref name="proximityThreshold"/>; otherwise, false.</returns>
        public static bool ValuesInProximity(DateTime value1, DateTime value2, TimeSpan proximityThreshold)
        {
            TimeSpan difference;

            if (value1 > value2)
            {
                difference = value1.Subtract(value2);
            }
            else
            {
                difference = value2.Subtract(value1);
            }

            return difference <= proximityThreshold;
        }

        /// <summary>
        /// Get the values of an enumeration.
        /// </summary>
        /// <param name="enumType">The enumeration.</param>
        /// <returns>An array of <see cref="Enum"/> values present within the enumeration.</returns>
        /// <exception cref="ArgumentException"><paramref name="enumType"/> is not an enumeration.</exception>
        public static Enum[] GetEnumValues(Type enumType)
        {
            Assert.IsTrue(enumType.IsEnum);

            FieldInfo[] info = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
            Enum[] values = new Enum[info.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = (Enum)info[i].GetValue(null);
            }

            return values;
        }

        /// <summary>
        /// Get the maximum integer value associated with an enumeration.
        /// </summary>
        /// <param name="enumType">The enumeration.</param>
        /// <returns>The maximum integer value associated with the specified enumeration.</returns>
        /// <exception cref="ArgumentException"><paramref name="enumType"/> is not an enumeration.</exception>
        public static int GetEnumMaxValue(Type enumType)
        {
            Assert.IsTrue(enumType.IsEnum);

            int maxValue = int.MinValue;

            foreach (int value in Enum.GetValues(enumType))
            {
                if (value > maxValue)
                {
                    maxValue = value;
                }
            }

            return maxValue;
        }

        /// <summary>
        /// Copy a subset of an array.
        /// </summary>
        /// <typeparam name="T">The array type.</typeparam>
        /// <param name="source">The source array to copy from.</param>
        /// <param name="start">The index into the source array at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <returns>A copy of the subset of the source array.</returns>
        public static T[] CopySubArray<T>(this T[] source, int start, int length)
        {
            T[] result = new T[length];
            Array.Copy(source, start, result, 0, length);

            return result;
        }

        /// <summary>
        /// Swap byte order.
        /// </summary>
        /// <param name="data">The source data.</param>
        /// <returns>The swapped data source.</returns>
        public static byte[] SwapEndian(byte[] data)
        {
            byte[] swapped = new byte[data.Length];
            for (int i = data.Length - 1, j = 0; i >= 0; i--, j++)
            {
                swapped[j] = data[i];
            }

            return swapped;
        }

        /// <summary>
        /// Bitwise test for flag.
        /// </summary>
        /// <param name="input">Enumerated value to test.</param>
        /// <param name="flagToMatch">Flag to match against.</param>
        /// <returns>true if the flag is set; otherwise, false.</returns>
        public static bool IsFlagSet(Enum input, Enum flagToMatch)
        {
            return (Convert.ToUInt32(input) & Convert.ToUInt32(flagToMatch)) != 0;
        }

        /// <summary>
        /// Build a row for a latin square.
        /// </summary>
        /// <param name="seed">The seed (starting point) of the row.</param>
        /// <param name="minValue">The minimum value that can reside within the square.</param>
        /// <param name="conditionCount">The number of conditions within the square.</param>
        /// <returns>A row for a latin square.</returns>
        /// <exception cref="ArgumentException"><paramref name="minValue"/> is greater than or equal to <paramref name="conditionCount"/>. -or- <paramref name="seed"/> is less
        /// than <paramref name="minValue"/>. -or- <paramref name="seed"/> is greater than or equal to <paramref name="conditionCount"/></exception>
        public static int[] BuildLatinSquareRow(int seed, int minValue, int conditionCount)
        {
            Assert.IsTrue(minValue < conditionCount);
            Assert.IsTrue(seed >= minValue);
            Assert.IsTrue(seed < conditionCount);

            List<int> row = new List<int>();

            for (int i = seed; i < conditionCount; i++)
            {
                row.Add(i);
            }

            if (row.Count < conditionCount)
            {
                for (int i = minValue; i < seed; i++)
                {
                    row.Add(i);
                }
            }

            return row.ToArray();
        }

        /// <summary>
        /// Extension method for resetting a <seealso cref="System.Timers.Timer"/> object.
        /// </summary>
        /// <param name="timer">The timer to reset.</param>
        public static void Reset(this System.Timers.Timer timer)
        {
            timer.Stop();
            timer.Start();
        }
	}
}
