using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3
{
    /// <summary>
    ///Data structure for a tree
    /// </summary>
    public class TreeNode
    {
        //Label of a node. Test
        private String treeAttributeName;

        public String TreeAttributeName
        {
            get { return treeAttributeName; }
            set { treeAttributeName = value; }
        }

        //Outcome of a parent, by which we get to this node. Empty for the root
        private String parentOutcome;

        public String ParentOutcome
        {
            get { return parentOutcome; }
            set { parentOutcome = value; }
        }

        //Possible outcomes of the attribute
        private List<String> treeAttributeOutcomes = new List<String>();

        public List<String> TreeAttributeOutcomes
        {
            get { return treeAttributeOutcomes; }
            set { treeAttributeOutcomes = value; }
        }

        //Childs of the attribute. May be leafs with decisions
        List<TreeNode> nodes = new List<TreeNode>();

        public List<TreeNode> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }

        public TreeNode(Attribute attribute)
        {
            if (attribute.AttributeOutcomes.Count != 0)
            {
                for (int i = 0; i < attribute.AttributeOutcomes.Count; i++)
                {
                    treeAttributeOutcomes.Add(attribute.AttributeOutcomes[i]);
                }
                treeAttributeName = attribute.AttributeName;
            }
            else 
            {
                treeAttributeName = attribute.AttributeName;
            }
        }

        public TreeNode(String treeAttributeName)
        {
            this.treeAttributeName = treeAttributeName;
            this.treeAttributeOutcomes = null;
        }

        public TreeNode()
        {
          
        }

        /// <summary>
        ///Function used to copy values
        /// </summary>
        /// <param name="attribute">Values that are copied</param>
        public void copyContent(Attribute attribute)
        {
            treeAttributeName = attribute.AttributeName;
            treeAttributeOutcomes = attribute.AttributeOutcomes;
        }
    }
}
