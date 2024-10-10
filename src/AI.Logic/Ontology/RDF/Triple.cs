using System;

namespace AI.Logic.Ontology.RDF
{
    /// <summary>
    /// Представляет один RDF-триплет.
    /// </summary>
    [Serializable]
    public class Triple
    {
        /// <summary>
        /// Субъект триплета.
        /// </summary>
        public string Subject { get; }

        /// <summary>
        /// Предикат триплета.
        /// </summary>
        public string Predicate { get; }

        /// <summary>
        /// Объект триплета.
        /// </summary>
        public string Object { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса Triple.
        /// </summary>
        /// <param name="subject">Субъект триплета.</param>
        /// <param name="predicate">Предикат триплета.</param>
        /// <param name="obj">Объект триплета.</param>
        public Triple(string subject, string predicate, string obj)
        {
            Subject = subject;
            Predicate = predicate;
            Object = obj;
        }

        /// <summary>
        /// Возвращает строковое представление триплета.
        /// </summary>
        /// <returns>Строковое представление триплета.</returns>
        public override string ToString()
        {
            return $"{Subject} {Predicate} {Object} .";
        }
    }
}
