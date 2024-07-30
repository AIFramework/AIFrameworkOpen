using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.Logic.Ontology.RDF
{

    /// <summary>
    /// Класс для управления RDF-онтологией.
    /// </summary>
    [Serializable]
    public class Ontology
    {
        private List<Triple> triples;
        private List<Triple> subclassRelationships;
        private List<Triple> subpropertyRelationships;

        /// <summary>
        /// Инициализирует новый экземпляр класса Ontology.
        /// </summary>
        public Ontology()
        {
            triples = new List<Triple>();
            subclassRelationships = new List<Triple>();
            subpropertyRelationships = new List<Triple>();
        }

        /// <summary>
        /// Добавляет триплет в онтологию.
        /// </summary>
        /// <param name="triple">Добавляемый триплет.</param>
        public void AddTriple(Triple triple)
        {
            triples.Add(triple);

            if (triple.Predicate == "rdfs:subClassOf")
            {
                subclassRelationships.Add(triple);
            }
            else if (triple.Predicate == "rdfs:subPropertyOf")
            {
                subpropertyRelationships.Add(triple);
            }
        }

        /// <summary>
        /// Возвращает все триплеты в онтологии.
        /// </summary>
        /// <returns>Список всех триплетов.</returns>
        public List<Triple> GetTriples()
        {
            return triples;
        }

        /// <summary>
        /// Возвращает триплеты по субъекту.
        /// </summary>
        /// <param name="subject">Субъект для поиска.</param>
        /// <returns>Список триплетов с заданным субъектом.</returns>
        public List<Triple> GetTriplesBySubject(string subject)
        {
            return triples.Where(t => t.Subject == subject).ToList();
        }

        /// <summary>
        /// Возвращает триплеты по предикату.
        /// </summary>
        /// <param name="predicate">Предикат для поиска.</param>
        /// <returns>Список триплетов с заданным предикатом.</returns>
        public List<Triple> GetTriplesByPredicate(string predicate)
        {
            return triples.Where(t => t.Predicate == predicate).ToList();
        }

        /// <summary>
        /// Возвращает триплеты по объекту.
        /// </summary>
        /// <param name="obj">Объект для поиска.</param>
        /// <returns>Список триплетов с заданным объектом.</returns>
        public List<Triple> GetTriplesByObject(string obj)
        {
            return triples.Where(t => t.Object == obj).ToList();
        }

        /// <summary>
        /// Возвращает все подклассы заданного класса.
        /// </summary>
        /// <param name="cls">Класс для поиска подклассов.</param>
        /// <returns>Множество подклассов заданного класса.</returns>
        public HashSet<string> GetSubclasses(string cls)
        {
            HashSet<string> subclasses = new HashSet<string>();
            foreach (var triple in subclassRelationships)
            {
                if (triple.Object == cls)
                {
                    subclasses.Add(triple.Subject);
                    subclasses.UnionWith(GetSubclasses(triple.Subject));
                }
            }
            return subclasses;
        }

        /// <summary>
        /// Возвращает все под-свойства заданного свойства.
        /// </summary>
        /// <param name="prop">Свойство для поиска под-свойств.</param>
        /// <returns>Множество под-свойств заданного свойства.</returns>
        public HashSet<string> GetSubproperties(string prop)
        {
            HashSet<string> subproperties = new HashSet<string>();
            foreach (var triple in subpropertyRelationships)
            {
                if (triple.Object == prop)
                {
                    subproperties.Add(triple.Subject);
                    subproperties.UnionWith(GetSubproperties(triple.Subject));
                }
            }
            return subproperties;
        }
    }
}
