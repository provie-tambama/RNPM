using System.Collections.Generic;
using System.Linq;

namespace RNPM.Common.Extensions
{
    public static class MathExtensions
    {
        public static IEnumerable<int> BatchInteger(this int total, int batchSize)
        {
            if (batchSize == 0)
            {
                yield return 0;
            }

            if (batchSize >= total)
            {
                yield return total;
            }
            else
            {
                int rest = total % batchSize;
                int divided = total / batchSize;
                if (rest > 0)
                {
                    divided += 1;
                }

                for (int i = 0; i < divided; i++)
                {
                    if (rest > 0 && i == divided - 1)
                    {
                        yield return rest;
                    }
                    else
                    {
                        yield return batchSize;
                    }
                }
            }
        }

        public static IEnumerable<decimal> BatchDecimal(this decimal total, decimal batchSize)
        {
            if (batchSize == 0)
            {
                yield return 0;
            }

            if (batchSize >= total)
            {
                yield return total;
            }
            else
            {
                decimal rest = total % batchSize;
                int divided = (int)(total / batchSize);
                if (rest > 0)
                {
                    divided += 1;
                }

                for (int i = 0; i < divided; i++)
                {
                    if (rest > 0 && i == divided - 1)
                    {
                        yield return rest;
                    }
                    else
                    {
                        yield return batchSize;
                    }
                }
            }
        }

        public static List<decimal> MinSubsetSums(this decimal targetSum, List<decimal> numbers)
        {
            List<List<decimal>> result = new List<List<decimal>>();
            List<decimal> current = new List<decimal>();
            AllSubsetSumsHelper(numbers, targetSum, current, result);
            return result.OrderBy(r => r.Count).First();
        }

        public static void AllSubsetSumsHelper(List<decimal> numbers, decimal targetSum, List<decimal> current, List<List<decimal>> result)
        {
            if (targetSum == 0)
            {
                result.Add(current.ToList());
                return;
            }

            if (targetSum < 0 || numbers.Count == 0)
            {
                return;
            }

            decimal num = numbers[0];
            List<decimal> rest = numbers.GetRange(1, numbers.Count - 1);

            AllSubsetSumsHelper(rest, targetSum, current, result);

            decimal count = targetSum / num;
            for (int i = 1; i <= count; i++)
            {
                current.Add(num);
                AllSubsetSumsHelper(rest, targetSum - i * num, current, result);
            }

            for (int i = 1; i <= count; i++)
            {
                current.RemoveAt(current.Count - 1);
            }
        }
    }
}
