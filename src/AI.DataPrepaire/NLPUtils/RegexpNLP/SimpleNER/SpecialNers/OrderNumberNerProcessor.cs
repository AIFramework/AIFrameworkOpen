using System;
using System.Collections.Generic;
using System.Text;

namespace AI.DataPrepaire.NLPUtils.RegexpNLP.SimpleNER.SpecialNers
{
    /// <summary>
    /// Процессор обрабатывающий порядковые номера в т.ч. и буквенные
    /// </summary>
    public class OrderNumberNerProcessor : RegexNer
    {
        //const string orderNumberPatten =
        //    @"\b\d+\.(?:\d\b|\s.*)?";

        const string orderNumberPatten =
            @"\b(\d+\.)+\d*";

        /// <summary>
        /// Процессор обрабатывающий порядковые номера в т.ч. и буквенные
        /// </summary>
        public OrderNumberNerProcessor() 
            : base(orderNumberPatten, "orderNumber") { }
    }
}
