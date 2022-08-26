//
// Автор кода стемера: SergeiGalkovskii
// Ссылка на репозиторий: https://github.com/SergeiGalkovskii/Porter-s-algorithm-for-stemming-for-russian-language-csharp
//Ссылка на оригинальный проект стемера:

//Лицензия на стемер


//MIT License

//Copyright(c) 2017 SergeiGalkovskii

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.



using System;
using System.Text.RegularExpressions;


namespace AI.NLP.Stemmers
{
    /// <summary>
    /// Стемер русского языка
    /// </summary>
    [Serializable]
    public static class StemmerRus
    {
        private static readonly Regex PERFECTIVEGROUND = new Regex("((ив|ивши|ившись|ыв|ывши|ывшись)|((<;=[ая])(в|вши|вшись)))$");
        private static readonly Regex REFLEXIVE = new Regex("(с[яь])$");
        private static readonly Regex ADJECTIVE = new Regex("(ее|ие|ые|ое|ими|ыми|ей|ий|ый|ой|ем|им|ым|ом|его|ого|ему|ому|их|ых|ую|юю|ая|яя|ою|ею)$");
        private static readonly Regex PARTICIPLE = new Regex("((ивш|ывш|ующ)|((?<=[ая])(ем|нн|вш|ющ|щ)))$");
        private static readonly Regex VERB = new Regex("((ила|ыла|ена|ейте|уйте|ите|или|ыли|ей|уй|ил|ыл|им|ым|ен|ило|ыло|ено|ят|ует|уют|ит|ыт|ены|ить|ыть|ишь|ую|ю)|((?<=[ая])(ла|на|ете|йте|ли|й|л|ем|н|ло|но|ет|ют|ны|ть|ешь|нно)))$");
        private static readonly Regex NOUN = new Regex("(а|ев|ов|ие|ье|е|иями|ями|ами|еи|ии|и|ией|ей|ой|ий|й|иям|ям|ием|ем|ам|ом|о|у|ах|иях|ях|ы|ь|ию|ью|ю|ия|ья|я)$");
        private static readonly Regex RVRE = new Regex("^(.*?[аеиоуыэюя])(.*)$");
        private static readonly Regex DERIVATIONAL = new Regex(".*[^аеиоуыэюя]+[аеиоуыэюя].*ость?$");
        private static readonly Regex DER = new Regex("ость?$");
        private static readonly Regex SUPERLATIVE = new Regex("(ейше|ейш)$");
        private static readonly Regex I = new Regex("и$");
        private static readonly Regex P = new Regex("ь$");
        private static readonly Regex NN = new Regex("нн$");

        /// <summary>
        /// Стемминг массива слов
        /// </summary>
        /// <param name="words">Массив слов</param>
        public static string[] TransformingWordsArray(string[] words)
        {
            string[] strs = new string[words.Length];

            for (int i = 0; i < words.Length; i++)
            {
                strs[i] = TransformingWord(words[i]);
            }

            return strs;
        }
        /// <summary>
        /// стемминг
        /// </summary>
        /// <param name="word">слово</param>
        /// <returns>приставка+корень</returns>
        public static string TransformingWord(string word)
        {
            word = word.ToLower();
            word = word.Replace('ё', 'е');
            MatchCollection m = RVRE.Matches(word);
            if (m.Count > 0)
            {
                Match match = m[0]; // only one match in this case 
                GroupCollection groupCollection = match.Groups;
                string pre = groupCollection[1].ToString();
                string rv = groupCollection[2].ToString();

                MatchCollection temp = PERFECTIVEGROUND.Matches(rv);
                string StringTemp = ReplaceFirst(temp, rv);


                if (StringTemp.Equals(rv))
                {
                    MatchCollection tempRV = REFLEXIVE.Matches(rv);
                    rv = ReplaceFirst(tempRV, rv);
                    temp = ADJECTIVE.Matches(rv);
                    StringTemp = ReplaceFirst(temp, rv);
                    if (!StringTemp.Equals(rv))
                    {
                        rv = StringTemp;
                        tempRV = PARTICIPLE.Matches(rv);
                        rv = ReplaceFirst(tempRV, rv);
                    }
                    else
                    {
                        temp = VERB.Matches(rv);
                        StringTemp = ReplaceFirst(temp, rv);
                        if (StringTemp.Equals(rv))
                        {
                            tempRV = NOUN.Matches(rv);
                            rv = ReplaceFirst(tempRV, rv);
                        }
                        else
                        {
                            rv = StringTemp;
                        }
                    }

                }
                else
                {
                    rv = StringTemp;
                }

                MatchCollection tempRv = I.Matches(rv);
                rv = ReplaceFirst(tempRv, rv);
                if (DERIVATIONAL.Matches(rv).Count > 0)
                {
                    tempRv = DER.Matches(rv);
                    rv = ReplaceFirst(tempRv, rv);
                }

                temp = P.Matches(rv);
                StringTemp = ReplaceFirst(temp, rv);
                if (StringTemp.Equals(rv))
                {
                    tempRv = SUPERLATIVE.Matches(rv);
                    rv = ReplaceFirst(tempRv, rv);
                    tempRv = NN.Matches(rv);
                    rv = ReplaceFirst(tempRv, rv);
                }
                else
                {
                    rv = StringTemp;
                }
                word = pre + rv;

            }

            return word;
        }

        private static string ReplaceFirst(MatchCollection collection, string part)
        {
            string StringTemp = "";
            if (collection.Count == 0)
            {
                return part;
            }

            else
            {
                StringTemp = part;
                for (int i = 0; i < collection.Count; i++)
                {
                    GroupCollection GroupCollection = collection[i].Groups;
                    if (StringTemp.Contains(GroupCollection[i].ToString()))
                    {
                        string deletePart = GroupCollection[i].ToString();
                        StringTemp = StringTemp.Replace(deletePart, "");
                    }

                }
            }
            return StringTemp;
        }
    }
}