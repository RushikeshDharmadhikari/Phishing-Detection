using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Phishing
{
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nNeural network training\n");
            

            
            
            
            int counter = 0;
            var reader1 = new StreamReader(File.OpenRead(@"C:\Users\Rushikesh.Dharmadhik\Downloads\Training Dataset.txt"));
            
            
            Console.WriteLine("Reading Files.....");
            double[][] allData = new double[1500][];
            
            while (!reader1.EndOfStream && counter < 1500)
            {
                
                string line = reader1.ReadLine();
                string[] values = line.Split(',');
                allData[counter] = new double[values.Length];

                for (int i = 0; i < values.Length; i++)
                {
                    allData[counter][i] = Double.Parse(values[i]);
                }
                counter++;
               
            }

            reader1.Close();
            ShowMatrix(allData, 1500, 1, true);
            Console.Read();
            Console.WriteLine("\nFirst 6 rows of entire 2000-item data set:");
            ShowMatrix(allData, 6, 1, true);

            Console.WriteLine("Creating 80% training and 20% test data matrices");
            double[][] trainData = null;
            double[][] testData = null;
            MakeTrainTest(allData, out trainData, out testData);

            Console.WriteLine("\nFirst 5 rows of training data:");
            ShowMatrix(trainData, 5, 1, true);
            Console.WriteLine("First 3 rows of test data:");
            ShowMatrix(testData, 3, 1, true);

         
          

            Console.WriteLine("\nFirst 5 rows of normalized training data:");
            ShowMatrix(trainData, 5, 1, true);
            Console.WriteLine("First 3 rows of normalized test data:");
            ShowMatrix(testData, 3, 1, true);

            Console.WriteLine("\nCreating a 5-input, 7-hidden, 2-output neural network");
            Console.Write("Hard-coded tanh function for input-to-hidden and softmax for ");
            Console.WriteLine("hidden-to-output activations");
            const int numInput = 30;
            const int numHidden = 15;
            const int numOutput = 1;
            NeuralNetwork nn = new NeuralNetwork(numInput, numHidden, numOutput);

            Console.WriteLine("\nInitializing weights and bias to small random values");
            nn.InitializeWeights();

            int maxEpochs = 500;
            double learnRate = 0.05;
            double momentum = 0.01;
            double weightDecay = 0.0001;
            Console.WriteLine("Setting maxEpochs = 500, learnRate = 0.05, momentum = 0.01, weightDecay = 0.0001");
            Console.WriteLine("Mean squared error < 0.020 stopping condition");

            Console.WriteLine("\nTraining using incremental back-propagation\n");
            nn.Train(trainData, maxEpochs, learnRate, momentum, weightDecay);
            Console.WriteLine("Training complete");

            double[] weights = nn.GetWeights();
            Console.WriteLine("Final neural network weights and bias values:");
            /* for (int i = 0; i < weights.Length; i++)
             {
                 Console.WriteLine(weights[i].ToString("N11"));
             }*/
            ShowVector(weights, 10, 3, true);

            double trainAcc = nn.Accuracy(trainData);
            Console.WriteLine("\nAccuracy on training data = " + trainAcc.ToString("F4"));

            double testAcc = nn.Accuracy(testData);
            Console.WriteLine("\nAccuracy on test data = " + testAcc.ToString("F4"));

            Console.WriteLine("\nEnd");
            Console.ReadLine();         
            Console.Read();
        }
        static void ShowVector(double[] vector, int valsPerRow, int decimals, bool newLine)
        {
            for (int i = 0; i < vector.Length; ++i)
            {
                if (i % valsPerRow == 0) Console.WriteLine("");
                Console.Write(vector[i].ToString("F" + decimals).PadLeft(decimals + 4) + " ");
            }
            if (newLine == true) Console.WriteLine("");
        }

        static void MakeTrainTest(double[][] allData, out double[][] trainData, out double[][] testData)
        {
            
            Random rnd = new Random(0);
            int totRows = allData.Length;
            int numCols = allData[0].Length;

            int trainRows = (int)(totRows * 0.80); 
            int testRows = totRows - trainRows;

            trainData = new double[trainRows][];
            testData = new double[testRows][];

            int[] sequence = new int[totRows];
            for (int i = 0; i < sequence.Length; ++i)
                sequence[i] = i;

            for (int i = 0; i < sequence.Length; ++i)
            {
                int r = rnd.Next(i, sequence.Length);
                int tmp = sequence[r];
                sequence[r] = sequence[i];
                sequence[i] = tmp;
            }

            int si = 0;
            int j = 0;

            for (; si < trainRows; ++si) 
            {
                trainData[j] = new double[numCols];
                int idx = sequence[si];
                Array.Copy(allData[idx], trainData[j], numCols);
                ++j;
            }

            j = 0; 
            for (; si < totRows; ++si) 
            {
                testData[j] = new double[numCols];
                int idx = sequence[si];
                Array.Copy(allData[idx], testData[j], numCols);
                ++j;
            }
        } 
        static void Normalize(double[][] dataMatrix, int[] cols)
        {
            
            foreach (int col in cols)
            {
                double sum = 0.0;
                for (int i = 0; i < dataMatrix.Length; ++i)
                    sum += dataMatrix[i][col];
                double mean = sum / dataMatrix.Length;
                sum = 0.0;
                for (int i = 0; i < dataMatrix.Length; ++i)
                    sum += (dataMatrix[i][col] - mean) * (dataMatrix[i][col] - mean);
                
                double sd = Math.Sqrt(sum / (dataMatrix.Length - 1));
                for (int i = 0; i < dataMatrix.Length; ++i)
                    dataMatrix[i][col] = (dataMatrix[i][col] - mean) / sd;
            }
        }
        static void ShowMatrix(double[][] matrix, int numRows, int decimals, bool newLine)
        {
            for (int i = 0; i < numRows; ++i)
            {
                Console.Write(i.ToString().PadLeft(3) + ": ");
                for (int j = 0; j < matrix[i].Length; ++j)
                {
                    if (matrix[i][j] >= 0.0) Console.Write(" "); else Console.Write("-");
                    Console.Write(Math.Abs(matrix[i][j]).ToString("F" + decimals) + " ");
                }
                Console.WriteLine("");
            }
            if (newLine == true) Console.WriteLine("");
        }
    }
    public class NeuralNetwork
    {
        private static Random rnd;

        private int numInput;
        private int numHidden;
        private int numOutput;

        private double[] inputs;

        private double[][] ihWeights; 
        private double[] hBiases;
        private double[] hOutputs;

        private double[][] hoWeights; 
        private double[] oBiases;

        private double[] outputs;

        private double[] oGrads; 
        private double[] hGrads; 

        private double[][] ihPrevWeightsDelta;  
        private double[] hPrevBiasesDelta;
        private double[][] hoPrevWeightsDelta;
        private double[] oPrevBiasesDelta;


        public NeuralNetwork(int numInput, int numHidden, int numOutput)
        {
            rnd = new Random(0); 

            this.numInput = numInput;
            this.numHidden = numHidden;
            this.numOutput = numOutput;

            this.inputs = new double[numInput];

            this.ihWeights = MakeMatrix(numInput, numHidden);
            this.hBiases = new double[numHidden];
            this.hOutputs = new double[numHidden];

            this.hoWeights = MakeMatrix(numHidden, numOutput);
            this.oBiases = new double[numOutput];

            this.outputs = new double[numOutput];

            this.hGrads = new double[numHidden];
            this.oGrads = new double[numOutput];

            this.ihPrevWeightsDelta = MakeMatrix(numInput, numHidden);
            this.hPrevBiasesDelta = new double[numHidden];
            this.hoPrevWeightsDelta = MakeMatrix(numHidden, numOutput);
            this.oPrevBiasesDelta = new double[numOutput];
        }

        private static double[][] MakeMatrix(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int r = 0; r < result.Length; ++r)
                result[r] = new double[cols];
            return result;
        }

        public override string ToString() 
        {
            string s = "";
            s += "===============================\n";
            s += "numInput = " + numInput + " numHidden = " + numHidden + " numOutput = " + numOutput + "\n\n";

            s += "inputs: \n";
            for (int i = 0; i < inputs.Length; ++i)
                s += inputs[i].ToString("F2") + " ";
            s += "\n\n";

            s += "ihWeights: \n";
            for (int i = 0; i < ihWeights.Length; ++i)
            {
                for (int j = 0; j < ihWeights[i].Length; ++j)
                {
                    s += ihWeights[i][j].ToString("F4") + " ";
                }
                s += "\n";
            }
            s += "\n";

            s += "hBiases: \n";
            for (int i = 0; i < hBiases.Length; ++i)
                s += hBiases[i].ToString("F4") + " ";
            s += "\n\n";

            s += "hOutputs: \n";
            for (int i = 0; i < hOutputs.Length; ++i)
                s += hOutputs[i].ToString("F4") + " ";
            s += "\n\n";

            s += "hoWeights: \n";
            for (int i = 0; i < hoWeights.Length; ++i)
            {
                for (int j = 0; j < hoWeights[i].Length; ++j)
                {
                    s += hoWeights[i][j].ToString("F4") + " ";
                }
                s += "\n";
            }
            s += "\n";

            s += "oBiases: \n";
            for (int i = 0; i < oBiases.Length; ++i)
                s += oBiases[i].ToString("F4") + " ";
            s += "\n\n";

            s += "hGrads: \n";
            for (int i = 0; i < hGrads.Length; ++i)
                s += hGrads[i].ToString("F4") + " ";
            s += "\n\n";

            s += "oGrads: \n";
            for (int i = 0; i < oGrads.Length; ++i)
                s += oGrads[i].ToString("F4") + " ";
            s += "\n\n";

            s += "ihPrevWeightsDelta: \n";
            for (int i = 0; i < ihPrevWeightsDelta.Length; ++i)
            {
                for (int j = 0; j < ihPrevWeightsDelta[i].Length; ++j)
                {
                    s += ihPrevWeightsDelta[i][j].ToString("F4") + " ";
                }
                s += "\n";
            }
            s += "\n";

            s += "hPrevBiasesDelta: \n";
            for (int i = 0; i < hPrevBiasesDelta.Length; ++i)
                s += hPrevBiasesDelta[i].ToString("F4") + " ";
            s += "\n\n";

            s += "hoPrevWeightsDelta: \n";
            for (int i = 0; i < hoPrevWeightsDelta.Length; ++i)
            {
                for (int j = 0; j < hoPrevWeightsDelta[i].Length; ++j)
                {
                    s += hoPrevWeightsDelta[i][j].ToString("F4") + " ";
                }
                s += "\n";
            }
            s += "\n";

            s += "oPrevBiasesDelta: \n";
            for (int i = 0; i < oPrevBiasesDelta.Length; ++i)
                s += oPrevBiasesDelta[i].ToString("F4") + " ";
            s += "\n\n";

            s += "outputs: \n";
            for (int i = 0; i < outputs.Length; ++i)
                s += outputs[i].ToString("F2") + " ";
            s += "\n\n";

            s += "===============================\n";
            return s;
        }

        

        public void SetWeights(double[] weights)
        {
            int numWeights = (numInput * numHidden) + (numHidden * numOutput) + numHidden + numOutput;
            if (weights.Length != numWeights)
                throw new Exception("Bad weights array length: ");

            int k = 0;

            for (int i = 0; i < numInput; ++i)
                for (int j = 0; j < numHidden; ++j)
                    ihWeights[i][j] = weights[k++];
            for (int i = 0; i < numHidden; ++i)
                hBiases[i] = weights[k++];
            for (int i = 0; i < numHidden; ++i)
                for (int j = 0; j < numOutput; ++j)
                    hoWeights[i][j] = weights[k++];
            for (int i = 0; i < numOutput; ++i)
                oBiases[i] = weights[k++];
        }

        public void InitializeWeights()
        {
            
            int numWeights = (numInput * numHidden) + (numHidden * numOutput) + numHidden + numOutput;
            double[] initialWeights = new double[numWeights];
            double lo = -0.01;
            double hi = 0.01;
            for (int i = 0; i < initialWeights.Length; ++i)
                initialWeights[i] = (hi - lo) * rnd.NextDouble() + lo;
            this.SetWeights(initialWeights);

            /*for (int i = 0; i < numWeights; i++)
                Console.WriteLine(initialWeights[i]);*/
            Console.Read();
        }

        public double[] GetWeights()
        {
            
            int numWeights = (numInput * numHidden) + (numHidden * numOutput) + numHidden + numOutput;
            double[] result = new double[numWeights];
            int k = 0;
            for (int i = 0; i < ihWeights.Length; ++i)
                for (int j = 0; j < ihWeights[0].Length; ++j)
                    result[k++] = ihWeights[i][j];
            for (int i = 0; i < hBiases.Length; ++i)
                result[k++] = hBiases[i];
            for (int i = 0; i < hoWeights.Length; ++i)
                for (int j = 0; j < hoWeights[0].Length; ++j)
                    result[k++] = hoWeights[i][j];
            for (int i = 0; i < oBiases.Length; ++i)
                result[k++] = oBiases[i];
            return result;
        }

        
        private double[] ComputeOutputs(double[] xValues)
        {
            if (xValues.Length != numInput)
                throw new Exception("Bad xValues array length");

            double[] hSums = new double[numHidden]; 
            double[] oSums = new double[numOutput]; 
            for (int i = 0; i < xValues.Length; ++i) 
                this.inputs[i] = xValues[i];

            for (int j = 0; j < numHidden; ++j)  
                for (int i = 0; i < numInput; ++i)
                    hSums[j] += this.inputs[i] * this.ihWeights[i][j]; 
            for (int i = 0; i < numHidden; ++i)  
                hSums[i] += this.hBiases[i];

            for (int i = 0; i < numHidden; ++i)  
                this.hOutputs[i] = HyperTanFunction(hSums[i]); 
            for (int j = 0; j < numOutput; ++j)   
                for (int i = 0; i < numHidden; ++i)
                    oSums[j] += hOutputs[i] * hoWeights[i][j];

            for (int i = 0; i < numOutput; ++i)  
                oSums[i] += oBiases[i];

            for (int i = 0; i < oSums.Length; i++)
            {
                oSums[i] = LogSigmoid(oSums[i]);
            }
            
            Array.Copy(oSums, outputs, oSums.Length);

            double[] retResult = new double[numOutput]; 
            Array.Copy(this.outputs, retResult, retResult.Length);
            return retResult;
        }

        private static double HyperTanFunction(double x)
        {
            if (x < -20.0) return -1.0; 
            else if (x > 20.0) return 1.0;
            else return Math.Tanh(x);
        }

        private static double LogSigmoid(double x) 
        { 
            if (x < -45.0) return 0.0; 
            else if (x > 45.0) return 1.0; 
            else return 1.0 / (1.0 + Math.Exp(-x)); 
        }
        private static double[] Softmax(double[] oSums)
        {
            
            double max = oSums[0];
            for (int i = 0; i < oSums.Length; ++i)
                if (oSums[i] > max) max = oSums[i];

            
            double scale = 0.0;
            for (int i = 0; i < oSums.Length; ++i)
                scale += Math.Exp(oSums[i] - max);

            double[] result = new double[oSums.Length];
            for (int i = 0; i < oSums.Length; ++i)
                result[i] = Math.Exp(oSums[i] - max) / scale;

            return result; 
        }

        

        private void UpdateWeights(double[] tValues, double learnRate, double momentum, double weightDecay)
        {
        
            if (tValues.Length != numOutput)
                throw new Exception("target values not same Length as output in UpdateWeights");

        
            for (int i = 0; i < oGrads.Length; ++i)
            {
                double derivative = (1 - outputs[i]) * outputs[i];
                oGrads[i] = derivative * (tValues[i] - outputs[i]);
            }

            
            for (int i = 0; i < hGrads.Length; ++i)
            {
            
                double derivative = (1 - hOutputs[i]) * (1 + hOutputs[i]);
                double sum = 0.0;
                for (int j = 0; j < numOutput; ++j)
                {
                    double x = oGrads[j] * hoWeights[i][j];
                    sum += x;
                }
                hGrads[i] = derivative * sum;
            }

            for (int i = 0; i < ihWeights.Length; ++i) 
            {
                for (int j = 0; j < ihWeights[0].Length; ++j) 
                {
                    double delta = learnRate * hGrads[j] * inputs[i]; 
                    ihWeights[i][j] += delta; 
                    
                    ihWeights[i][j] += momentum * ihPrevWeightsDelta[i][j];
                    ihWeights[i][j] -= (weightDecay * ihWeights[i][j]); 
                    ihPrevWeightsDelta[i][j] = delta;  
                }
            }

            
            for (int i = 0; i < hBiases.Length; ++i)
            {
                double delta = learnRate * hGrads[i] * 1.0; 
                hBiases[i] += delta;
                hBiases[i] += momentum * hPrevBiasesDelta[i];
                hBiases[i] -= (weightDecay * hBiases[i]);
                hPrevBiasesDelta[i] = delta; 
            }

            
            for (int i = 0; i < hoWeights.Length; ++i)
            {
                for (int j = 0; j < hoWeights[0].Length; ++j)
                {
                
                    double delta = learnRate * oGrads[j] * hOutputs[i];
                    hoWeights[i][j] += delta;
                    hoWeights[i][j] += momentum * hoPrevWeightsDelta[i][j]; 
                    hoWeights[i][j] -= (weightDecay * hoWeights[i][j]); 
                    hoPrevWeightsDelta[i][j] = delta; 
                }
            }

            for (int i = 0; i < oBiases.Length; ++i)
            {
                double delta = learnRate * oGrads[i] * 1.0;
                oBiases[i] += delta;
                oBiases[i] += momentum * oPrevBiasesDelta[i];
                oBiases[i] -= (weightDecay * oBiases[i]);
                oPrevBiasesDelta[i] = delta;
            }

        } 
       
        public void Train(double[][] trainData, int maxEprochs, double learnRate, double momentum,
          double weightDecay)
        {
            
            int epoch = 0;
            double[] xValues = new double[numInput];
            double[] tValues = new double[numOutput]; 

            int[] sequence = new int[trainData.Length];
            for (int i = 0; i < sequence.Length; ++i)
                sequence[i] = i;
            FileStream fout = null;
            try
            {
                fout = new FileStream(@"C:\result1.txt", FileMode.Create);
            }
            catch (IOException exception)
            {
                Console.WriteLine("Error Opening File");
            }
            StreamWriter fstr_out = new StreamWriter(fout);

            while (epoch < maxEprochs)
            {
                double mse = MeanSquaredError(trainData);
                fstr_out.WriteLine(mse);
               
                if (mse < 0.002) break; 
                
                Shuffle(sequence);
                for (int i = 0; i < trainData.Length; ++i)
                {
                    int idx = sequence[i];
                    Array.Copy(trainData[idx], xValues, numInput);
                    Array.Copy(trainData[idx], numInput, tValues, 0, numOutput);
                    ComputeOutputs(xValues); 
                    UpdateWeights(tValues, learnRate, momentum, weightDecay); 
                }
                ++epoch;
            }
            Console.WriteLine(epoch);
            Console.Read();
            fstr_out.Close();
        } 

        private static void Shuffle(int[] sequence)
        {
            for (int i = 0; i < sequence.Length; ++i)
            {
                int r = rnd.Next(i, sequence.Length);
                int tmp = sequence[r];
                sequence[r] = sequence[i];
                sequence[i] = tmp;
            }
        }

        private double MeanSquaredError(double[][] trainData) 
        {
            double sumSquaredError = 0.0;
            double[] xValues = new double[numInput]; 
            double[] tValues = new double[numOutput];
            
            for (int i = 0; i < trainData.Length; ++i)
            {
                Array.Copy(trainData[i], xValues, numInput);
                Array.Copy(trainData[i], numInput, tValues, 0, numOutput); 
                double[] yValues = this.ComputeOutputs(xValues); 
                for (int j = 0; j < numOutput; ++j)
                {
                    double err = tValues[j] - yValues[j];
                    sumSquaredError += err * err;
                }
            }
            return sumSquaredError / trainData.Length;
        }


        public double Accuracy(double[][] testData)
        {
            
            int numCorrect = 0;
            int numWrong = 0;
            double[] xValues = new double[numInput]; 
            double[] tValues = new double[numOutput];
            double[] yValues; 

            for (int i = 0; i < testData.Length; ++i)
            {
                Array.Copy(testData[i], xValues, numInput); 
                Array.Copy(testData[i], numInput, tValues, 0, numOutput);
                yValues = this.ComputeOutputs(xValues);
                int maxIndex = MaxIndex(yValues); 

                if (tValues[maxIndex] == 1.0) 
                    ++numCorrect;
                else
                    ++numWrong;
            }
            return (numCorrect * 1.0) / (numCorrect + numWrong); 
        }

        private static int MaxIndex(double[] vector) 
        {
            int bigIndex = 0;
            double biggestVal = vector[0];
            for (int i = 0; i < vector.Length; ++i)
            {
                if (vector[i] > biggestVal)
                {
                    biggestVal = vector[i]; bigIndex = i;
                }
            }
            return bigIndex;
        }

    }
}
