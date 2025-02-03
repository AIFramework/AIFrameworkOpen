using System;
using System.Text;

namespace AI.DataPrepaire.Backends.BertTokenizers
{
    /// <summary>
    /// Структура TokenizeResult представляет результат токенизации текста.
    /// </summary>
    [Serializable]
    public struct TokenizeResult
    {
        /// <summary>
        /// Массив идентификаторов входных токенов.
        /// </summary>
        public int[] InputIds { get; set; }

        /// <summary>
        /// Массив масок внимания (attention mask).
        /// </summary>
        public int[] AttentionMask { get; set; }

        /// <summary>
        /// Массив идентификаторов типов токенов.
        /// </summary>
        public int[] TypeIds { get; set; }


        /// <summary>
        /// Строковое представление
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("InputIds:");

            for (int i = 0; i < InputIds.Length; i++)
            {
                stringBuilder.Append(InputIds[i]);
                stringBuilder.Append("  ");
            }


            stringBuilder.Append("\n\nAttentionMask:\n");

            for (int i = 0; i < AttentionMask.Length; i++)
            {
                stringBuilder.Append(AttentionMask[i]);
                stringBuilder.Append("  ");
            }

            return stringBuilder.ToString();
        }
    }

}
