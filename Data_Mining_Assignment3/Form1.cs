using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Data_Mining_Assignment3
{

    //This class handles the Gui and the output of the results.
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            TextBox myText = new TextBox();
            myText.Location = new Point(25, 25);

        }
        OpenFileDialog ofd = new OpenFileDialog();
        public string fileName = "";
        private void button1_Click(object sender, EventArgs e)
        {
            ofd.Filter = " CSV files .csv | *.csv";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
                fileName = ofd.FileName;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

            List<double[]> results = new List<double[]>();
            if (fileName != "")
            {
                for(var a = 0; a < 10; a++)
                {
                    results.Add(Perceptron.runAlgorithm(fileName));
                }   
            }
            else if (!fileName.Contains(".csv"))
            {
                MessageBox.Show("Please enter a valid file adress");
            }
            else
            {
                MessageBox.Show("Please enter a valid file adress");
            }
            double overall = 0;
            for(var r = 0;r < results.Count; r++)
            {
                double[] res = new double[3];
                res = results[r];
                MessageBox.Show("The Results for test "+ (r+1)+"\n" +
            "Accuracy: " + Math.Round(res[0], 2) + "%\nWith " + res[1] + " true positives \nAnd " + res[2] + " flase positives \nFrom " +(res[1]+res[2])+" test cases");
                overall += res[0];
            }

            MessageBox.Show("The Average Reuslt of the ten runs  "+ Math.Round(overall / results.Count,2) +"%");
        }
    }
}
