using System;
using System.IO;
namespace RadialNetworkTrain
{
    class RadialTrainProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nBegin radial basis function (RBF) network training\n");
    
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
            Console.WriteLine("\nFirst four and last line of normalized, encoded input data:\n");
            Helpers.ShowMatrix(allData, 4, 3, true, true);

            Console.WriteLine("\nSplitting data into 80%-20% train and test sets");
            double[][] trainData = null;
            double[][] testData = null;
            int seed = 8; 
            GetTrainTest(allData, seed, out trainData, out testData); 

            Console.WriteLine("\nTraining data: \n");
            Helpers.ShowMatrix(trainData, trainData.Length, 3, true, false);
            Console.WriteLine("\nTest data:\n");
            Helpers.ShowMatrix(testData, testData.Length, 3, true, false);

            Console.WriteLine("\nCreating a 30-5-1 radial basis function network");
            int numInput = 30;
            int numHidden = 5;
            int numOutput = 1;
            RadialNetwork rn = new RadialNetwork(numInput, numHidden, numOutput);

            Console.WriteLine("\nBeginning RBF training\n");
            int maxIterations = 10; 
            double[] bestWeights = rn.Train(trainData, maxIterations);

            Console.WriteLine("\nEvaluating result RBF classification accuracy on the test data");
            rn.SetWeights(bestWeights);

            double acc = rn.Accuracy(testData);
            Console.WriteLine("Classification accuracy = " + acc.ToString("F4"));

            Console.WriteLine("\nEnd RBF network training\n");
            Console.ReadLine();

        } 

        static void GetTrainTest(double[][] allData, int seed, out double[][] trainData, out double[][] testData)

        {
            
            int[] allIndices = new int[allData.Length];
            for (int i = 0; i < allIndices.Length; ++i)
                allIndices[i] = i;

            Random rnd = new Random(seed);
            for (int i = 0; i < allIndices.Length; ++i) 
            {
                int r = rnd.Next(i, allIndices.Length);
                int tmp = allIndices[r];
                allIndices[r] = allIndices[i];
                allIndices[i] = tmp;
            }

            int numTrain = (int)(0.80 * allData.Length);
            int numTest = allData.Length - numTrain;

            trainData = new double[numTrain][];
            testData = new double[numTest][];

            int j = 0;
            for (int i = 0; i < numTrain; ++i)
                trainData[i] = allData[allIndices[j++]];
            for (int i = 0; i < numTest; ++i)
                testData[i] = allData[allIndices[j++]];

        } 
    } 


    public class RadialNetwork
    {
        private static Random rnd = null;
        private int numInput;
        private int numHidden;
        private int numOutput;
        private double[] inputs;
        private double[][] centroids;
        private double[] widths;
        private double[][] hoWeights;
        private double[] oBiases;
        private double[] outputs;

        public RadialNetwork(int numInput, int numHidden, int numOutput)
        {
            rnd = new Random(0);
            this.numInput = numInput;
            this.numHidden = numHidden;
            this.numOutput = numOutput;
            this.inputs = new double[numInput];
            this.centroids = MakeMatrix(numHidden, numInput);
            this.widths = new double[numHidden];
            this.hoWeights = MakeMatrix(numHidden, numOutput);
            this.oBiases = new double[numOutput];
            this.outputs = new double[numOutput];
        } 

        private static double[][] MakeMatrix(int rows, int cols) 
        {
            double[][] result = new double[rows][];
            for (int r = 0; r < rows; ++r)
                result[r] = new double[cols];
            return result;
        }

        

        public void SetWeights(double[] weights)
        {
            
            
            if (weights.Length != (numHidden * numOutput) + numOutput)
                throw new Exception("Bad weights length in SetWeights");
            int k = 0; 
            for (int i = 0; i < numHidden; ++i)
                for (int j = 0; j < numOutput; ++j)
                    this.hoWeights[i][j] = weights[k++];
            for (int i = 0; i < numOutput; ++i)
                this.oBiases[i] = weights[k++];
        }

        public double[] GetWeights()
        {
            double[] result = new double[numHidden * numOutput + numOutput];
            int k = 0;
            for (int i = 0; i < numHidden; ++i)
                for (int j = 0; j < numOutput; ++j)
                    result[k++] = this.hoWeights[i][j];
            for (int i = 0; i < numOutput; ++i)
                result[k++] = this.oBiases[i];
            return result;
        }
        private double MeanSquaredError(double[][] trainData, double[] weights)
        {
            
            this.SetWeights(weights); 

            double[] xValues = new double[numInput]; 
            double[] tValues = new double[numOutput]; 
            double sumSquaredError = 0.0;
            for (int i = 0; i < trainData.Length; ++i) 
            {
                
                Array.Copy(trainData[i], xValues, numInput); 
                Array.Copy(trainData[i], numInput, tValues, 0, numOutput); 
                double[] yValues = this.ComputeOutputs(xValues); 
                for (int j = 0; j < yValues.Length; ++j)
                    sumSquaredError += ((yValues[j] - tValues[j]) * (yValues[j] - tValues[j]));
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

        

        public double[] ComputeOutputs(double[] xValues)
        {
            
            Array.Copy(xValues, this.inputs, xValues.Length); 

            double[] hOutputs = new double[numHidden]; 
            for (int j = 0; j < numHidden; ++j) 
            {
                double d = EuclideanDist(inputs, centroids[j], inputs.Length); 
                                                                               
                double r = -1.0 * (d * d) / (2 * widths[j] * widths[j]);
                double g = Math.Exp(r);
                
                hOutputs[j] = g;
            }

            double[] tempResults = new double[numOutput];

            for (int k = 0; k < numOutput; ++k)
                for (int j = 0; j < numHidden; ++j)
                    tempResults[k] += (hOutputs[j] * hoWeights[j][k]); 

            for (int k = 0; k < numOutput; ++k)
                tempResults[k] += oBiases[k]; 

            double[] finalOutputs = Softmax(tempResults); 
            Array.Copy(finalOutputs, this.outputs, finalOutputs.Length); 

            double[] returnResult = new double[numOutput]; 
            Array.Copy(finalOutputs, returnResult, outputs.Length);
            return returnResult;
        } 



        private static double[] Softmax(double[] rawOutputs)
        {
            
            
            
            double max = rawOutputs[0];
            for (int i = 0; i < rawOutputs.Length; ++i)
                if (rawOutputs[i] > max) max = rawOutputs[i];

            
            double scale = 0.0;
            for (int i = 0; i < rawOutputs.Length; ++i)
                scale += Math.Exp(rawOutputs[i] - max);

            double[] result = new double[rawOutputs.Length];
            for (int i = 0; i < rawOutputs.Length; ++i)
                result[i] = Math.Exp(rawOutputs[i] - max) / scale;

            return result; 
        }

        
        

        private void DoCentroids(double[][] trainData)
        {
            
            
            
            int numAttempts = trainData.Length;
            int[] goodIndices = new int[numHidden];  
            double maxAvgDistance = double.MinValue; 
            for (int i = 0; i < numAttempts; ++i)
            {
                int[] randomIndices = DistinctIndices(numHidden, trainData.Length); 
                double sumDists = 0.0; 
                for (int j = 0; j < randomIndices.Length - 1; ++j) 
                {
                    int firstIndex = randomIndices[j];
                    int secondIndex = randomIndices[j + 1];
                    sumDists += AvgAbsDist(trainData[firstIndex], trainData[secondIndex], numInput); 
                }

                double estAvgDist = sumDists / numInput; 
                if (estAvgDist > maxAvgDistance) 
                {
                    maxAvgDistance = estAvgDist;
                    Array.Copy(randomIndices, goodIndices, randomIndices.Length); 
                }
            } 

            Console.WriteLine("The indices (into training data) of the centroids are:");
            Helpers.ShowVector(goodIndices, goodIndices.Length, true);

            
            for (int i = 0; i < numHidden; ++i)
            {
                int idx = goodIndices[i]; 
                for (int j = 0; j < numInput; ++j)
                {
                    this.centroids[i][j] = trainData[idx][j]; 
                }
            }
        } 

        private static double AvgAbsDist(double[] v1, double[] v2, int numTerms)
        {
            
            
            if (v1.Length != v2.Length)
                throw new Exception("Vector lengths not equal in AvgAbsDist()");
            double sum = 0.0;
            for (int i = 0; i < numTerms; ++i)
            {
                double delta = Math.Abs(v1[i] - v2[i]);
                sum += delta;
            }
            return sum / numTerms;
        }

        private int[] DistinctIndices(int n, int range)
        {
            
            
            
            int[] result = new int[n];
            for (int i = 0; i < n; ++i)
                result[i] = i;

            for (int t = n; t < range; ++t)
            {
                int m = rnd.Next(0, t + 1);
                if (m < n) result[m] = t;
            }
            return result;
        }

        private void DoWidths(double[][] centroids)
        {
            
            
            
            double sumOfDists = 0.0;
            int ct = 0; 
            for (int i = 0; i < centroids.Length - 1; ++i)
            {
                for (int j = i + 1; j < centroids.Length; ++j)
                {
                    double dist = EuclideanDist(centroids[i], centroids[j], centroids[i].Length);
                    sumOfDists += dist;
                    ++ct;
                }
            }
            double avgDist = sumOfDists / ct;
            double width = avgDist;

            Console.WriteLine("The common width is: " + width.ToString("F4"));

            for (int i = 0; i < this.widths.Length; ++i) 
                this.widths[i] = width;
        }

        private double[] DoWeights(double[][] trainData, int maxIterations)
        {


            int numberParticles = trainData.Length / 1;
            //int numberParticles = 30;
            int Dim = (numHidden * numOutput) + numOutput; 
            double minX = -10.0; 
            double maxX = 10.0;
            double minV = minX;
            double maxV = maxX;
            Particle[] swarm = new Particle[numberParticles];
            double[] bestGlobalPosition = new double[Dim]; 
            double smallesttGlobalError = double.MaxValue; 

            
            for (int i = 0; i < swarm.Length; ++i) 
            {
                double[] randomPosition = new double[Dim];
                for (int j = 0; j < randomPosition.Length; ++j)
                {
                    double lo = minX;
                    double hi = maxX;
                    randomPosition[j] = (hi - lo) * rnd.NextDouble() + lo; 
                }

                double err = MeanSquaredError(trainData, randomPosition); 
                double[] randomVelocity = new double[Dim];

                for (int j = 0; j < randomVelocity.Length; ++j)
                {
                    double lo = -1.0 * Math.Abs(maxV - minV);
                    double hi = Math.Abs(maxV - minV);
                    randomVelocity[j] = (hi - lo) * rnd.NextDouble() + lo;
                }
                swarm[i] = new Particle(randomPosition, err, randomVelocity, randomPosition, err);

                
                if (swarm[i].error < smallesttGlobalError)
                {
                    smallesttGlobalError = swarm[i].error;
                    swarm[i].position.CopyTo(bestGlobalPosition, 0);
                }
            } 

            
            

            double w = 0.729; 
            double c1 = 1.49445; 
            double c2 = 1.49445; 
            double r1, r2; 

            int[] sequence = new int[numberParticles]; 
            for (int i = 0; i < sequence.Length; ++i)
                sequence[i] = i;

            int iteration = 0;
            while (iteration < maxIterations)
            {
                Console.WriteLine(iteration);
                if (smallesttGlobalError < 0.060) break; 

                double[] newVelocity = new double[Dim]; 
                double[] newPosition = new double[Dim]; 
                double newError; 

                Shuffle(sequence); 

                for (int pi = 0; pi < swarm.Length; ++pi) 
                {
                    int i = sequence[pi];
                    Particle currP = swarm[i]; 

                    
                    for (int j = 0; j < currP.velocity.Length; ++j) 
                    {
                        r1 = rnd.NextDouble();
                        r2 = rnd.NextDouble();

                        
                        
                        newVelocity[j] = (w * currP.velocity[j]) +
                          (c1 * r1 * (currP.bestPosition[j] - currP.position[j])) +
                          (c2 * r2 * (bestGlobalPosition[j] - currP.position[j]));

                        if (newVelocity[j] < minV)
                            newVelocity[j] = minV;
                        else if (newVelocity[j] > maxV)
                            newVelocity[j] = maxV;     
                    }

                    newVelocity.CopyTo(currP.velocity, 0);

                    
                    for (int j = 0; j < currP.position.Length; ++j)
                    {
                        newPosition[j] = currP.position[j] + newVelocity[j];  
                        if (newPosition[j] < minX)
                            newPosition[j] = minX;
                        else if (newPosition[j] > maxX)
                            newPosition[j] = maxX;
                    }

                    newPosition.CopyTo(currP.position, 0);

                    
                    
                    newError = MeanSquaredError(trainData, newPosition); 
                    currP.error = newError;

                    if (newError < currP.smallestError) 
                    {
                        newPosition.CopyTo(currP.bestPosition, 0);
                        currP.smallestError = newError;
                    }

                    if (newError < smallesttGlobalError) 
                    {
                        newPosition.CopyTo(bestGlobalPosition, 0);
                        smallesttGlobalError = newError;
                    }

                } 

                ++iteration;

            } 
        this.SetWeights(bestGlobalPosition);
            double[] returnResult = new double[(numHidden * numOutput) + numOutput];
            Array.Copy(bestGlobalPosition, returnResult, bestGlobalPosition.Length);

            Console.WriteLine("The best weights and bias values found are:\n");
            Helpers.ShowVector(bestGlobalPosition, 3, 10, true);
            return returnResult;
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

        public double[] Train(double[][] trainData, int maxIterations)
        {
            Console.WriteLine("\n1. Computing " + numHidden + " centroids");
            DoCentroids(trainData); 

            Console.WriteLine("\n2. Computing a common width for each hidden node");
            DoWidths(this.centroids); 

            int numWts = (numHidden * numOutput) + numOutput;
            Console.WriteLine("\n3. Determining " + numWts + " weights and bias values using PSO algorithm");
            double[] bestWeights =
              DoWeights(trainData, maxIterations); 

            return bestWeights;
        } 

        

        private static double EuclideanDist(double[] v1, double[] v2, int numTerms)
        {
            
            
            if (v1.Length != v2.Length)
                throw new Exception("Vector lengths not equal in EuclideanDist()");
            double sum = 0.0;
            for (int i = 0; i < numTerms; ++i)
            {
                double delta = (v1[i] - v2[i]) * (v1[i] - v2[i]);
                sum += delta;
            }
            return Math.Sqrt(sum);
        }


    } 

    

    public class Particle
    {
        public double[] position; 
        public double error; 
        public double[] velocity;

        public double[] bestPosition; 
        public double smallestError;

        public Particle(double[] position, double error, double[] velocity, double[] bestPosition, double smallestError)
        {
            this.position = new double[position.Length];
            position.CopyTo(this.position, 0);
            this.error = error;
            this.velocity = new double[velocity.Length];
            velocity.CopyTo(this.velocity, 0);
            this.bestPosition = new double[bestPosition.Length];
            bestPosition.CopyTo(this.bestPosition, 0);
            this.smallestError = smallestError;
        }

    } 

    

    public class Helpers
    {
        public static double[][] MakeMatrix(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }

        public static void ShowVector(double[] vector, int decimals, int valsPerLine, bool blankLine)
        {
            for (int i = 0; i < vector.Length; ++i)
            {
                if (i > 0 && i % valsPerLine == 0) 
                    Console.WriteLine("");
                if (vector[i] >= 0.0) Console.Write(" ");
                Console.Write(vector[i].ToString("F" + decimals) + " "); 
            }
            if (blankLine) Console.WriteLine("\n");
        }

        public static void ShowVector(int[] vector, int valsPerLine, bool blankLine)
        {
            for (int i = 0; i < vector.Length; ++i)
            {
                if (i > 0 && i % valsPerLine == 0) 
                    Console.WriteLine("");
                if (vector[i] >= 0.0) Console.Write(" ");
                Console.Write(vector[i] + " ");
            }
            if (blankLine) Console.WriteLine("\n");
        }

        public static void ShowMatrix(double[][] matrix, int numRows, int decimals, bool lineNumbering, bool showLastLine)
        {
            int ct = 0;
            if (numRows == -1) numRows = int.MaxValue; 
            for (int i = 0; i < matrix.Length && ct < numRows; ++i)
            {
                if (lineNumbering == true)
                    Console.Write(i.ToString().PadLeft(3) + ": ");
                for (int j = 0; j < matrix[0].Length; ++j)
                {
                    if (matrix[i][j] >= 0.0) Console.Write(" "); 
                    Console.Write(matrix[i][j].ToString("F" + decimals) + " ");
                }
                Console.WriteLine("");
                ++ct;
            }
            if (showLastLine == true && numRows < matrix.Length)
            {
                Console.WriteLine("      ........\n ");
                int i = matrix.Length - 1;
                Console.Write(i.ToString().PadLeft(3) + ": ");
                for (int j = 0; j < matrix[0].Length; ++j)
                {
                    if (matrix[i][j] >= 0.0) Console.Write(" "); 
                    Console.Write(matrix[i][j].ToString("F" + decimals) + " ");
                }
            }
            Console.WriteLine("");
        }

    } 
