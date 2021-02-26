using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ID3
{
    public partial class ID3 : Form
    {
        public ID3()
        {
            InitializeComponent();

            //Dane sa wprowadzane manualnie, musisz to zmienic.
            //Wysle ci plik csv z takimi danymi jak tu wprowadzam, zreszta sa na stronie w linku
            //Algorytm wyglada na dzialajacy juz, chociaz bugow specjalnie nie szukalem.
            //Dane o drzewie masz w root, po skonczeniu algorytmu.
            //Jesli na ostatim poziomie w lisciu masz nazwe kolumny decyzyjnej (np. Decision), a nie decyzje (np. Yes No)
            //To znaczy, ze parentOutcome nie zostalo uzyte.Nie rysujesz go w dzrewie.

            //Odpal sobie tabele dla trzech pierwszych. Zauwaz,ze wystarczy miec Sunny lub Overcast by stwierdzic Yes lub No dla decyzji. I zobacz co jest lisciu dla rain. O to mi chodzi

            //Jak powinno wygladac drzewo - tez w linku ponizej.
            //Zacznij sprawdzac na innych danych, dopiero gdy zrobisz wczytywanie na 100% z pliku dobrze, bo inaczej algorytm bedzie sie sypal
            //No i GUI zrob
            //Co do dokumentacji ustalimy pozniej, cos sie napisze, zreszta ja robie komentaz w kodzie do funkcji
            //Jak skonczys daj znac, termin jest na 24, ale podobno Brys przesunal troche, ale starajmy sie miec do srody, a tam sie dokumentacje zrobi

            //Data taken from, common exampel
            //http://www.cise.ufl.edu/~ddd/cap6635/Fall-97/Short-papers/2.htm

            #region Creation of a data table. Notice that decision attribute is not added to the set of tests

            List<String> test = new List<string>();

            test.Add("Sunny");
            test.Add("Overcast");
            test.Add("Rain");
            Attribute outlook = new Attribute("Outlook", test);
            test.Clear();

            test.Add("Hot");
            test.Add("Mild");
            test.Add("Cool");
            Attribute temperature = new Attribute("Temperature", test);
            test.Clear();

            test.Add("High");
            test.Add("Normal");
            Attribute humidity = new Attribute("Humidity", test);
            test.Clear();

            test.Add("Weak");
            test.Add("Strong");
            Attribute wind = new Attribute("Wind", test);
            test.Clear();

            test.Add("Yes");
            test.Add("No");
            Attribute decision = new Attribute("Decision", test);
            test.Clear();

            List<Attribute> attributes = new List<Attribute>();
            attributes.Add(outlook);
            attributes.Add(temperature);
            attributes.Add(humidity);
            attributes.Add(wind);

            DataTable result = new DataTable("samples");

            DataColumn column = result.Columns.Add("Outlook");
            column.DataType = typeof(String);
            column = result.Columns.Add("Temperature");
            column.DataType = typeof(String);
            column = result.Columns.Add("Humidity");
            column.DataType = typeof(String);
            column = result.Columns.Add("Wind");
            column.DataType = typeof(String);
            column = result.Columns.Add("Decision");
            column.DataType = typeof(String);

            #endregion

            #region Filling the table with data. Data taken from the website presented above
            //#1
            result.Rows.Add(new object[] { "Sunny", "Hot", "High", "Weak", "No" });
            //#2
            result.Rows.Add(new object[] { "Sunny", "Hot", "High", "Strong", "No" });
            //#3
            result.Rows.Add(new object[] { "Overcast", "Hot", "High", "Weak", "Yes" });
            //#4
            result.Rows.Add(new object[] { "Rain", "Mild", "High", "Weak", "Yes" });
            //#5
            result.Rows.Add(new object[] { "Rain", "Cool", "Normal", "Weak", "Yes" });
            //#6
            result.Rows.Add(new object[] { "Rain", "Cool", "Normal", "Strong", "No" });
            //#7
            result.Rows.Add(new object[] { "Overcast", "Cool", "Normal", "Strong", "Yes" });
            //#8
            result.Rows.Add(new object[] { "Sunny", "Mild", "High", "Weak", "No" });
            //#9
            result.Rows.Add(new object[] { "Sunny", "Cool", "Normal", "Weak", "Yes" });
            //#10
            result.Rows.Add(new object[] { "Rain", "Mild", "Normal", "Weak", "Yes" });
            //#11
            result.Rows.Add(new object[] { "Sunny", "Mild", "Normal", "Strong", "Yes" });
            //#12
            result.Rows.Add(new object[] { "Overcast", "Mild", "High", "Strong", "Yes" });
            //#13
            result.Rows.Add(new object[] { "Overcast", "Hot", "Normal", "Weak", "Yes" });
            //#14
            result.Rows.Add(new object[] { "Rain", "Mild", "High", "Strong", "No" });
            
            #endregion

            //Prepare the root for the tree
            TreeNode root = new TreeNode();
 
            //Main function
            buildtree(result, decision, attributes, root, null);
        }

        /// <summary>
        ///Look for the index index of decision column
        /// </summary>
        /// <param name="cases">Table containing data</param>
        /// <param name="decision">Object representing decision</param>
        public int getDecisionColumn(DataTable cases, Attribute decision)
        {
            int decisionColumnNumber = -1;

            for (int i = 0; i < cases.Columns.Count; i++)
            {
                if (cases.Columns[i].ColumnName == decision.AttributeName)
                {
                    decisionColumnNumber = i;
                    break;
                }
            }

            if (decisionColumnNumber == -1)
            {
                MessageBox.Show("Error. Decision column" + decision.AttributeName + " not found in DataTable", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            return decisionColumnNumber;
        }

        /// <summary>
        ///Recursive function that build a decision tree according to ID3 algorithm
        /// </summary>
        /// <param name="cases">Table containing data</param>
        /// <param name="decision">Object representing decision</param>
        /// <param name="tests">Test that are left to divide a dataset</param>
        /// <param name="parent">Ancestor of the attribute. New test is connected with tha ancestor</param>
        /// <param name="parentOutcome">Represents class of the parent attribute. Label for the node</param>
        public void buildtree(DataTable cases, Attribute decision, List<Attribute> tests, TreeNode parent, String parentOutcome)
        {
            TreeNode node = new TreeNode();

            if (stop(cases, decision, tests))
            {
                String leaf = chooseClass(cases, decision, tests);
                TreeNode temp = new TreeNode(leaf);
                temp.ParentOutcome = parentOutcome;
                parent.Nodes.Add(temp);
                
                return;
            }

            Attribute test = chooseTest(cases, decision, tests);

                node.copyContent(test);
                node.ParentOutcome = parentOutcome;
                parent.Nodes.Add(node);
            
            Int32 testColumnNumber = getDecisionColumn(cases, test);

            foreach (String s in test.AttributeOutcomes)
            {
                DataTable tempCases = new DataTable(s);

                foreach (DataColumn dc in cases.Columns)
                {
                    tempCases.Columns.Add(new DataColumn(dc.ColumnName));
                }

                for (int i = 0; i < cases.Rows.Count; i++)
                {
                    DataRow toinsert = cases.Rows[i];

                    if ((cases.Rows[i][testColumnNumber].ToString()) == tempCases.TableName)
                    {
                        tempCases.ImportRow(cases.Rows[i]);
                    }
                }

                tests.Remove(test);
                List<Attribute> tempTests = new List<Attribute>();
                tempTests.AddRange(tests);

                buildtree(tempCases, decision, tempTests, node, s);
                
            }
        }

        /// <summary>
        ///Checks if the algorithm should be stopped.
        ///Conditions:
        ///-Set of cases is empty
        ///-Set of tests is empty
        ///-All cases belong to the same class
        /// </summary>
        /// <param name="cases">Table containing data</param>
        /// <param name="decision">Object representing decision</param>
        /// <param name="tests">Test that are left to divide a dataset</param>
        public Boolean stop(DataTable cases, Attribute decision, List<Attribute> tests)
        {
            if (cases.Rows.Count == 0)
            {
                return true;
            }

            if (tests.Count == 0)
            {
                return true;
            }

            Int32 decisionColumnNumber = getDecisionColumn(cases, decision);

            List<String> numberOfItems = new List<String>();

            for (int i = 0; i < cases.Rows.Count; i++)
            {
                Boolean outcomeFound = false;

                foreach (String s in numberOfItems)
                {
                    if ((cases.Rows[i][decisionColumnNumber].ToString()) == s)
                    {
                        outcomeFound = true;
                        break;
                    }
                }

                if (!outcomeFound)
                {
                    numberOfItems.Add(cases.Rows[i][decisionColumnNumber].ToString());
                    if (numberOfItems.Count == 2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        ///The most frequent class in the set of cases is returned if cases are present or attribute name if cases are empty 
        /// </summary>
        /// <param name="cases">Table containing data</param>
        /// <param name="decision">Object representing decision</param>
        /// <param name="tests">Test that are left to divide a dataset</param>
        public String chooseClass(DataTable cases, Attribute decision, List<Attribute> tests)
        {
            if (cases.Rows.Count == 0) return decision.AttributeName;

            Int32 decisionColumnNumber = getDecisionColumn(cases, decision);

            List<ClassCounter> classCounters = new List<ClassCounter>();

            for (int i = 0; i < cases.Rows.Count; i++)
            {
                Boolean outcomeFound = false;

                foreach (ClassCounter cs in classCounters)
                {
                    if ((cases.Rows[i][decisionColumnNumber].ToString()) == cs.Value)
                    {
                        outcomeFound = true;
                        cs.Counter++;
                        break;
                    }
                }

                if (!outcomeFound)
                {
                    classCounters.Add(new ClassCounter(cases.Rows[i][decisionColumnNumber].ToString()));
                }
            }

            int mostFrequent = -1;
            int maxValue = 0;

            foreach (ClassCounter cs in classCounters)
            {
                if (cs.Counter > maxValue)
                {
                    maxValue = cs.Counter;
                    mostFrequent = classCounters.IndexOf(cs);
                }
            }

            if (mostFrequent == -1)
            {
                MessageBox.Show("Error. Can't find any class in cases.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            return classCounters[mostFrequent].Value;

            MessageBox.Show("Error. Attribute not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
            return null;
        }

        /// <summary>
        ///Function choses test from available
        /// </summary>
        /// <param name="cases">Table containing data</param>
        /// <param name="decision">Object representing decision</param>
        /// <param name="tests">Test that are left to divide a dataset</param>
        public Attribute chooseTest(DataTable cases, Attribute decision, List<Attribute> tests)
        {
            Double expectedInformation = calculateExpectedInformation(cases, decision);
            double maxGain = 0.0;

            Attribute test = null;

            foreach (Attribute a in tests)
            {
                Double entropy = calculateEntropy(cases, decision, a);
                Double tempGain = expectedInformation - entropy;
                if (tempGain > maxGain)
                {
                    maxGain = tempGain;
                    test = a;
                }
            }

            return test;
        }

        /// <summary>
        ///Function calculates expected information of a set
        /// </summary>
        /// <param name="cases">Table containing data</param>
        /// <param name="decision">Object representing decision</param>
        public double calculateExpectedInformation(DataTable cases, Attribute decision)
        {

            Double information = 0.0;
            Double total = cases.Rows.Count;
            List<ClassCounter> classCounters = new List<ClassCounter>();

            Int32 decisionColumnNumber = getDecisionColumn(cases, decision);

            foreach (String s in decision.AttributeOutcomes)
            {
                classCounters.Add(new ClassCounter(s, 0));
            }

            for (int i = 0; i < total; i++)
            {
                foreach (ClassCounter cs in classCounters)
                {
                    if ((cases.Rows[i][decisionColumnNumber].ToString()) == cs.Value)
                    {
                        cs.Counter++;
                        break;
                    }
                }

            }

            foreach (ClassCounter cs in classCounters)
            {
                information += (cs.Counter / total) * (-Math.Log((cs.Counter / total), 2));
            }

            return information;
        }

        /// <summary>
        ///Function calculates entropy for given decision attribute, test attribute and dataset
        /// </summary>
        /// <param name="cases">Table containing data</param>
        /// <param name="decision">Object representing decision</param>
        /// <param name="test">Test for which entropy is calculated</param>
        public double calculateEntropy(DataTable cases, Attribute decision, Attribute test)
        {
            Double entropy = 0.0;
            Double total = cases.Rows.Count;

            List<ClassCounter> classCounters = new List<ClassCounter>();

            Int32 testColumnNumber = getDecisionColumn(cases, test);

            List<DataTable> newDataTables = new List<DataTable>();

            foreach (String s in test.AttributeOutcomes)
            {
                newDataTables.Add(new DataTable(s));
                foreach (DataColumn dc in cases.Columns)
                {
                    newDataTables[newDataTables.Count - 1].Columns.Add(new DataColumn(dc.ColumnName));
                }
            }

            for (int i = 0; i < cases.Rows.Count; i++)
            {
                DataRow toinsert = cases.Rows[i];
                foreach (DataTable dt in newDataTables)
                {
                    if ((cases.Rows[i][testColumnNumber].ToString()) == dt.TableName)
                    {
                        dt.ImportRow(cases.Rows[i]);
                        break;
                    }
                }
            }


            Int32 decisionColumnNumber = getDecisionColumn(cases, decision);

            List<Double> tempEntropyInner = new List<Double>();

            foreach (DataTable dt in newDataTables)
            {
                List<ClassCounter> classCountersInner = new List<ClassCounter>();

                foreach (String s in decision.AttributeOutcomes)
                {
                    classCountersInner.Add(new ClassCounter(s, 0));
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    foreach (ClassCounter cs in classCountersInner)
                    {
                        if ((dt.Rows[i][decisionColumnNumber].ToString()) == cs.Value)
                        {
                            cs.Counter++;
                            break;
                        }
                    }
                }

                Double innerEntropy = 0.0;

                foreach (ClassCounter cs in classCountersInner)
                {
                    Double ratio = (double)cs.Counter / (double)dt.Rows.Count;
                    if (Double.IsNaN(ratio))
                    {
                        ratio = 0.0;
                    }
                    Double logarithm = -(Math.Log(ratio, 2));
                    if (logarithm != Double.NaN && logarithm != Double.PositiveInfinity)
                    {
                        innerEntropy += ratio * logarithm;
                    }
                }

                tempEntropyInner.Add(innerEntropy);
            }
            foreach (DataTable dt in newDataTables)
            {
                entropy += ((double)dt.Rows.Count / (double)total) * tempEntropyInner.First();
                tempEntropyInner.RemoveAt(0);
            }

            return entropy;
        }
    }
}
