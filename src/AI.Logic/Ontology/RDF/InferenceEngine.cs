using System;
using System.Collections.Generic;

namespace AI.Logic.Ontology.RDF
{
    /// <summary>
    /// Класс для выполнения логического вывода в RDF-онтологии.
    /// </summary>
    [Serializable]
    public class InferenceEngine
    {
        private Ontology ontology;

        /// <summary>
        /// Инициализирует новый экземпляр класса InferenceEngine.
        /// </summary>
        /// <param name="ontology">Онтология для выполнения логического вывода.</param>
        public InferenceEngine(Ontology ontology)
        {
            this.ontology = ontology;
        }

        /// <summary>
        /// Выполняет логический вывод на основе триплетов в онтологии.
        /// </summary>
        /// <returns>Список выведенных триплетов.</returns>
        public List<Triple> Infer()
        {
            List<Triple> inferredTriples = new List<Triple>();
            var triples = ontology.GetTriples();

            // Иерархический вывод классов
            foreach (var triple in triples)
            {
                if (triple.Predicate == "rdf:type")
                {
                    var subclasses = ontology.GetSubclasses(triple.Object);
                    foreach (var subclass in subclasses)
                    {
                        inferredTriples.Add(new Triple(triple.Subject, "rdf:type", subclass));
                    }
                }
            }

            // Иерархический вывод свойств
            foreach (var triple in triples)
            {
                var subproperties = ontology.GetSubproperties(triple.Predicate);
                foreach (var subproperty in subproperties)
                {
                    inferredTriples.Add(new Triple(triple.Subject, subproperty, triple.Object));
                }
            }

            // Универсальный вывод
            foreach (var triple1 in triples)
            {
                foreach (var triple2 in triples)
                {
                    if (triple1.Object == triple2.Subject)
                    {
                        inferredTriples.Add(new Triple(triple1.Subject, triple2.Predicate, triple2.Object));
                    }
                }
            }

            return inferredTriples;
        }
    }
}
