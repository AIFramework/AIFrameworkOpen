using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.Logic.Fuzzy
{
    /// <summary>
    /// Генератор нечеткого множества.
    /// </summary>
    /// <typeparam name="ElType">Тип элемента.</typeparam>
    /// <typeparam name="ImgType">Тип образа элемента.</typeparam>
    [Serializable]
    public class FuzzySetGenerator<ElType, ImgType>
    {
        /// <summary>
        /// Имя множества
        /// </summary>
        public string Name { get; set; }    

        /// <summary>
        /// Набор образов элементов.
        /// </summary>
        public IImageSet<ElType, ImgType> Image { get; set; }

        /// <summary>
        /// Интерфейс для получения коэффициентов принадлежности.
        /// </summary>
        public IMu<ElType, ImgType> Mu { get; set; }

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public FuzzySetGenerator() { }

        /// <summary>
        /// Конструктор с параметрами.
        /// </summary>
        /// <param name="image">Набор образов элементов.</param>
        /// <param name="mu">Интерфейс для получения коэффициентов принадлежности.</param>
        public FuzzySetGenerator(IImageSet<ElType, ImgType> image, IMu<ElType, ImgType> mu)
        {
            Image = image;
            Mu = mu;
        }

        /// <summary>
        /// Получение элемента нечеткого множества.
        /// </summary>
        /// <param name="element">Элемент.</param>
        /// <returns>Элемент нечеткого множества.</returns>
        public FuzzySetElement<ElType, ImgType> GetElement(ElType element)
        {
            var im = Image.GetImage(element);

            return new FuzzySetElement<ElType, ImgType>()
            {
                Mu = Mu.GetCoef(element, im),
                Image = im,
                ElementValue = element
            };
        }

        /// <summary>
        /// Генерация нечеткого множества.
        /// </summary>
        /// <param name="elements">Коллекция элементов.</param>
        /// <returns>Нечеткое множество.</returns>
        public FuzzySet<ElType, ImgType> SetGenerate(IEnumerable<ElType> elements)
        {
            var set = new List<FuzzySetElement<ElType, ImgType>>();

            foreach (var item in elements)
            {
                set.Add(GetElement(item));
            }

            return new FuzzySet<ElType, ImgType>(set);
        }

        /// <summary>
        /// "И" (пересечение) над нечеткими множествами, формула Яргена
        /// </summary>
        public static FuzzySet<ElType, ImgType> And(IEnumerable<ElType> elements, IEnumerable<FuzzySetGenerator<ElType, ImgType>> fuzzySets, double w = 1)
        {
            var image = fuzzySets.First().Image;
            double root = 1.0 / w;
            var result = new FuzzySet<ElType, ImgType>();

            foreach (var el in elements)
            {
                double mu = 1, sum = 0;
                var im = image.GetImage(el);

                foreach (var set in fuzzySets)
                    sum += Math.Pow(1 - set.Mu.GetCoef(el, im), w);

                mu -= Math.Min(1, Math.Pow(sum, root));

                result.Add(new FuzzySetElement<ElType, ImgType>()
                {
                    ElementValue = el,
                    Image = im,
                    Mu = mu
                });
            }

            return result;
        }

        /// <summary>
        /// "ИЛИ" (объединение) над нечеткими множествами, формула Яргена
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="fuzzySets"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static FuzzySet<ElType, ImgType> Or(IEnumerable<ElType> elements, IEnumerable<FuzzySetGenerator<ElType, ImgType>> fuzzySets, double w = 1)
        {
            var image = fuzzySets.First().Image;
            double root = 1.0 / w;
            var result = new FuzzySet<ElType, ImgType>();

            foreach (var el in elements)
            {
                double mu = 0, sum = 0;
                var im = image.GetImage(el);

                foreach (var set in fuzzySets)
                    sum += Math.Pow(set.Mu.GetCoef(el, im), w);

                mu = Math.Min(1, Math.Pow(sum, root));

                result.Add(new FuzzySetElement<ElType, ImgType>()
                {
                    ElementValue = el,
                    Image = im,
                    Mu = mu
                });
            }

            return result;
        }

    }

}
