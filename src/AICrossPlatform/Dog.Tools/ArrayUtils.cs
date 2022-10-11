using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI.Dog.Tools
{
    /// <summary>
    /// Операции над массивами
    /// </summary>
    public static class ArrayUtils<T> where T : IComparable<T>
    {
        /// <summary>
        /// Удаление первого вхождения под массива
        /// </summary>
        /// <param name="array">Массив данных</param>
        /// <param name="subArrayForRemove">Удаляемый массив</param>
        public static T[] DeleteSubArray(T[] array, T[] subArrayForRemove)
        {
            var indexis = SearchSubArray(array, subArrayForRemove); // Поиск подмассива

            if (indexis == null) return array; // Если подмассив не найден

            T[] returnObj = new T[array.Length-subArrayForRemove.Length]; // Создание нового

            // Заполнение массива
            for (int i = 0, j = 0; i < array.Length; i++)
            {
                if (i < indexis.Item1 || i > indexis.Item2)
                    returnObj[j++] = array[i];
            }

            return returnObj;
        }


        /// <summary>
        /// Поиск первого вхождения под массива
        /// </summary>
        /// <param name="array">Массив данных</param>
        /// <param name="subArray">Подмассив</param>
        public static Tuple<int, int> SearchSubArray(T[] array, T[] subArray) 
        {
            if(subArray.Length > array.Length) return null;


            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].CompareTo(subArray[0]) == 0) 
                {
                    int start = i;
                    int end = i;
                    bool isF = true; // Найдена ли под последовательность
                    if (start + subArray.Length > array.Length) return null;

                    // Проверка подпоследовательности
                    for (int j = 1; j < subArray.Length; j++)
                    {
                        if (array[i + j].CompareTo(subArray[j]) != 0)
                        {
                            isF = false;
                            break;
                        }
                        end = i + j;
                    }
                    if (isF) return new Tuple<int, int>(start, end);
                }
            }


            return null;
        }
    }
}
