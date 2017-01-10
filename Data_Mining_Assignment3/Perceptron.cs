using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Dynamic;


namespace Data_Mining_Assignment3
{
    class Perceptron
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        //Main method to run all the main functions of the algrithm.
        public static double[] runAlgorithm(string location)
        {
            var headers = File.ReadLines(location).First();
            var names = headers.Split(',');
            var numberOfFields = names.Length;
            var classifier = numberOfFields;
            List<string> types = new List<string>();        //Array of data names 
            List<Array> data = new List<Array>();
            List<List<string>> newValue = new List<List<string>>();
            var isClassifier = false;
            foreach (var line in File.ReadAllLines(location))       // Reads in the the csv file stores it as List of arrays
            {
                var colums = line.Split(',');

                string[] obj = new string[colums.Length];
                for (var x = 0; x < names.Length; x++)
                {

                    double isNum = 0;
                    List<string> cat = new List<string>();
                    if (newValue.Count() < numberOfFields - 1)
                        newValue.Add(cat);
                    if (!double.TryParse(colums[x], out isNum) && !(x == colums.Length - 1) && !newValue.ElementAt(x).Contains(colums[x]))
                    {
                        isClassifier = true;
                        newValue.ElementAt(x).Add(colums[x]);

                    }
                    if (x == colums.Length - 1 && !types.Contains(colums[x]))
                    {
                        types.Add(colums[x]);
                    }

                    obj[x] = colums[x];

                }
                data.Add(obj);

            }
            if (isClassifier)
                data = alterData(data, newValue);

            List<Array> testData = new List<Array>();
            List<Array> trainData = new List<Array>();
            var train = data.Count() * 2 / 3;
            var test = data.Count() * 1 / 3;
            shuffle(data);                                  // This section shuffles the data and then splits the data into training and testing data.
            for (var el = 0; el < data.Count(); el++)
            {

                if (el < train)
                {
                    trainData.Add(data[el]);
                }
                if (el >= train)
                {
                    testData.Add(data[el]);
                }
            }
            List<int> expectedResults = new List<int>();
            List<double[]> formulae = new List<double[]>();
            if (types.Count > 2)
            {
                foreach (string type in types)              //This part of the code gatheres the expected results for the array which 
                {
                    expectedResults.Clear();                //One vs all if the count is gratter than 2
                    for (var a = 0; a < trainData.Count; a++)
                    {
                        expectedResults.Add(trainData.ElementAt(a).GetValue(classifier - 1).Equals(type) ? 1 : 0);

                    }
                    double[] we = new double[numberOfFields];

                    formulae.Add(clacWeights(trainData, expectedResults));

                }
            }
            else {
                expectedResults.Clear();                // if there is only two types of classes
                for (var a = 0; a < trainData.Count; a++)
                {
                    expectedResults.Add(trainData.ElementAt(a).GetValue(classifier - 1).Equals(types.ElementAt(0)) ? 1 : 0);

                }
                double[] we = new double[numberOfFields];

                formulae.Add(clacWeights(trainData, expectedResults));
            }

            return runTest(formulae, testData, types);
        }

        public static double[] clacWeights(List<Array> data, List<int> expected)
        {

            var rand = new Random();
            var numFields = data.ElementAt(3).Length;
            double[] weights = new double[numFields];       // Main percptron algoithm 
            for (var i = 0; i < weights.Length; i++)
            {
                double val = rand.NextDouble();
                weights.SetValue(val, i);               //Randomise the weights for 
            }
            var count = 0;
            double previousError = 0;
            double totalError = 0;
            var limit = false;
            double learningRate = (double)data.Count() / 100;
            do
            {
                count++;
                totalError = 0;
                for (var i = 0; i < data.Count; i++)
                {
                    double result = findOutput(data[i], weights, false);

                    double error = expected[i] - result;
                    for (var y = 0; y < data[i].Length; y++)
                    {
                        if (y != numFields - 1)
                        {
                            double value = double.Parse(data.ElementAt(i).GetValue(y).ToString());  //This adjust the weights dependinng on the results 
                            weights[y] += learningRate * error * value;
                        }
                        else
                        {
                            weights[y] += learningRate * error;
                        }

                    }
                    totalError += Math.Abs(error);
                }

                learningRate = learningRate / count;
                previousError = totalError;
                limit = totalError == 0 || count >= 100 ? true : false;
            }
            while (!limit);     //Breaks the run when ceratin criteria is meet. 
            return weights;
        }

        public static double findOutput(Array item, double[] weights, bool realScore)
        {
            double sum = 0;
            for (var y = 0; y < weights.Length; y++)
            {
                if (y != item.Length - 1)
                {
                    double value = double.Parse(item.GetValue(y).ToString());       //Calculates the outputs  
                    sum += weights[y] * value;
                }
                else
                {
                    sum += weights[y];
                }
            }
            if (realScore)
                return sum;
            else
            {
                return sum > 0 ? 1 : 0;
            }
        }

        public static double[] runTest(List<double[]> forms, List<Array> data, List<string> types)
        {
            // double[] weights = new double[];
            var correctGuess = 0;
            var wrongGuess = 0;
            foreach (var o in data)
            {
                double bestScore = -100;
                var bestGuess = "";                     // This process test the algoithm accuracy.
                for (var x = 0; x < forms.Count; x++)
                {
                    // weights = forms.ElementAt(x);
                    double sum = findOutput(o, forms.ElementAt(x), true);

                    if (forms.Count > 2)
                    {
                        if (sum > bestScore)
                        {
                            bestScore = sum;
                            bestGuess = types[x].ToString();
                        }
                    }
                    else
                    {
                        if (sum > 0)
                        {
                            bestGuess = types[0].ToString();
                        }
                        else
                        {
                            bestGuess = types[1].ToString();
                        }
                    }
                }
                if (bestGuess.Equals(o.GetValue(o.Length - 1)))
                {
                    correctGuess++;
                }
                else
                {
                    wrongGuess++;
                }


            }

            double result = ((double)correctGuess / data.Count) * 100;

            double[] resArray = new double[3];
            resArray[0] = result;
            resArray[1] = correctGuess;
            resArray[2] = wrongGuess;
            return resArray;
        }
        public static List<Array> alterData(List<Array> data, List<List<string>> newData)
        {


            foreach (Array d in data)
            {

                for (var c = 0; c < d.Length - 1; c++)
                {
                    List<string> select = new List<string>();
                    select = newData.ElementAt(c);                 //This function changes the discreste data to contionous data if needed 
                    for (var n = 0; n < select.Count; n++)
                    {
                        double ik = findOcc(data, c, select.ElementAt(n));
                        if (d.GetValue(c).Equals(select.ElementAt(n)))
                            d.SetValue(ik.ToString(), c);
                    }
                }
            }


            return data;
        }

        public static double findOcc(List<Array> data, int col, string prop)
        {
            // List<Array> select = new List<Array>();
            //var total = 0;
            var ofType = 0;
            foreach (var i in data)
            {
                if (i.GetValue(col).Equals(prop))
                {
                    ofType++;
                }
            }
            return (double)ofType / data.Count;
        }




        public static void shuffle(List<Array> list)
        {
            Random rnd = new Random();
            int n = list.Count;
            while (n > 1)                   //Shuffles the data  to randomise the order. 
            {
                n--;
                int k = rnd.Next(0, n + 1);
                dynamic value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}

