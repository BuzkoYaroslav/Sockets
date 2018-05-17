using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace MatrixGenerator
{

    public partial class MatrixGenerator : Form
    {
        public MatrixGenerator()
        {
            InitializeComponent();
        }

        public delegate void MatrixGeneratorMatrixGeneratorHandler(object sender, MatrixGeneratorEventArgs e);
        public event MatrixGeneratorMatrixGeneratorHandler MatrixGenerated;

        public delegate void Method();

        Random rnd = new Random();

        private const int minValue = 0;
        private const int maxValue = 100000;

        private bool isCanceled = false;

        private string matrixTemplate = "";
        private string vectorFileName = "";

        object lockObj = new object();
        private bool IsCanceled
        {
            get
            {
                lock (lockObj)
                {
                    return isCanceled;
                }
            }
            set
            {
                lock (lockObj)
                {
                    isCanceled = value;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            matrixTemplate = textBox1.Text;
            vectorFileName = textBox2.Text;

            Stopwatch wtch = new Stopwatch();

            try
            {
                int size = Convert.ToInt32(textBox3.Text);
                progressIndicator.Value = 0;
                progressIndicator.Maximum = size + 1;

                Thread thrd = new Thread(() =>
                {
                    long averageTime = 0;
                    for (int i = 0; i <= size; i++)
                    {
                        if (IsCanceled)
                        {
                            DeleteFiles(vectorFileName, matrixTemplate, i - 1);
                            break;
                        }

                        wtch.Reset();
                        wtch.Start();

                        string fileName = i == 0 ? vectorFileName : matrixTemplate.Replace("#", i.ToString());

                        if (i == 0) GenerateIntSequence(fileName, size);
                        else GenerateIntOneSequence(fileName, size, i - 1);

                        wtch.Stop();
                        averageTime = (averageTime * i + wtch.ElapsedTicks) / (i + 1);

                        ExecuteOnMainThread(() =>
                        {
                            progressIndicator.Value += 1;
                            timeLabel.Text = new TimeSpan(averageTime * (size - i)).ToString();
                        });
                    }
                });

                thrd.Start();
            } catch (FormatException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void GenerateIntSequence(string fileName, int size)
        {
            StreamWriter writer = new StreamWriter(fileName);

            writer.WriteLine(size);

            for (int i = 0; i < size; i++)
            {
                writer.Write(rnd.Next(minValue, maxValue));
                if (i != size - 1)
                    writer.Write(" ");
            }

            writer.Close();
        }
        private void GenerateIntOneSequence(string fileName, int size, int index)
        {
            StreamWriter writer = new StreamWriter(fileName);

            writer.WriteLine(size);

            for (int i = 0; i < size; i++)
            {
                if (i == index) writer.Write(1); else writer.Write(0);
                if (i != size - 1)
                    writer.Write(" ");
            }

            writer.Close();
        }
        private void DeleteFiles(string vectorFileName, string matrixTemplate, int number)
        {
            ExecuteOnMainThread(() =>
            {
                Enabled = false;
            });

            for (int i = 0; i <= number; i++)
                if (i == 0)
                    File.Delete(vectorFileName);
                else
                    File.Delete(matrixTemplate.Replace("#", i.ToString()));

            matrixTemplate = "";
            vectorFileName = "";

            ExecuteOnMainThread(() =>
            {
                Enabled = true;
            });
        }
        private void ExecuteOnMainThread(Method method)
        {
            Invoke(method);
        }

        private void MatrixGenerator_FormClosed(object sender, FormClosedEventArgs e)
        {
            MatrixGenerated(this, new MatrixGeneratorEventArgs(matrixTemplate, vectorFileName));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IsCanceled = true;
        }
    }

    public class MatrixGeneratorEventArgs
    {
        public string VectorFileName { get; }
        public string MatrixTemplateFileName { get; }

        public MatrixGeneratorEventArgs(string matrixFileName, string vectorFileName)
        {
            VectorFileName = vectorFileName;
            MatrixTemplateFileName = matrixFileName;
        }
    }
}
