using System;
using System.Collections.Generic;
using System.Linq;

namespace Solitary.Core
{

    public static class StackExtension
    {
        private static Random rnd = new Random();

        public static List<T> PopRange<T>(this Stack<T> stack, int amount)
        {
            var result = new List<T>(amount);
            while (amount-- > 0 && stack.Count > 0)
            {
                result.Add(stack.Pop());
            }
            return result;
        }

        public static void PushRange<T>(this Stack<T> source, IEnumerable<T> collection)
        {
            foreach (var item in collection)
                source.Push(item);
        }

        public static void Shuffle<T>(this Stack<T> stack)
        {
            var values = stack.ToArray();
            stack.Clear();
            foreach (var value in values.OrderBy(x => rnd.Next()))
                stack.Push(value);
        }
    }

}