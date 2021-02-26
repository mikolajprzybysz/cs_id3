using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3
{
    /// <summary>
    ///Data structure for storing info about attributes.
    ///Contains name, outcomes of an attribute and constructor.
    /// </summary>
    public class Attribute
    {
        private String attributeName;

        public String AttributeName
        {
            get { return attributeName; }
            set { attributeName = value; }
        }
        private List<String> attributeOutcomes = new List<String>();

        public List<String> AttributeOutcomes
        {
            get { return attributeOutcomes; }
            set { attributeOutcomes = value; }
        }

        public Attribute(String name, List<String> outcomes)
        {
            attributeName = name;
            attributeOutcomes.AddRange(outcomes);
        }
    }
}
