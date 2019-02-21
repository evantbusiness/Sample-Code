using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thivierge_FinalGA
{
    static class GA
    {
        private static int popSize = 50;
        private static float eliteRate = 0.1f;
        private static float mutationRate = 0.01f;
        private static float mutation = Int32.MaxValue * mutationRate;

        private static bool[,] target;
        private static bool[,] bestResult;

        private static int width = 15;
        private static int height = 15;

        private static List<GA_Class> population = new List<GA_Class>();
        private static List<GA_Class> buffer = new List<GA_Class>();

        private static Random rand = new Random();

        public static int generation = 0;
        public static bool complete = false;

        class GA_Class
        {
            public bool[,] units;
            public int fitness;
        }

        public static void InitPopulation()
        {
            population.Clear();
            buffer.Clear();
            complete = false;

            for (int i = 0; i < popSize; i++)
            {
                GA_Class citizen = new GA_Class();

                citizen.fitness = 0;
                citizen.units = new bool[width,height];

                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        int randSelect = rand.Next(0,2);

                        if(randSelect == 1)
                        {
                            citizen.units[x, y] = true;
                        }
                        else if(randSelect == 2)
                        {
                            citizen.units[x, y] = false;
                        }
                    }
                }

                population.Add(citizen);
                buffer.Add(citizen);
            }
            bestResult = population[0].units;
        }

        private static void CalcFitness()
        {
            int fitness;

            for(int i = 0; i < popSize; i++)
            {
                fitness = 0;
                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        if (population[i].units[x, y] != target[x, y])
                            fitness++;
                    }
                }

                population[i].fitness = fitness;
            }
        }

        private static void SortByFitness()
        {
            List<GA_Class> sortedList = new List<GA_Class>();
            sortedList = population.OrderBy(o => o.fitness).ToList();
            for(int i = 0; i < popSize; i++)
            {
                population[i] = sortedList[i];
            }
        }

        private static void Mate()
        {
            float esize = popSize * eliteRate;
            int matePos, i1, i2;

            Elitism(esize);

            for(int i = (int)esize; i < popSize; i++)
            {
                i1 = rand.Next(0, popSize / 2);
                i2 = rand.Next(0, popSize / 2);
                matePos = rand.Next() % (width * height);

                GA_Class child = new GA_Class();
                child.units = new bool[width, height];

                int m = 0;

                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        if (rand.Next() < mutation)
                        {
                            int randBool = rand.Next() % 2;
                            if(randBool == 0)
                            {
                                child.units[x, y] = false;
                            }
                            else
                            {
                                child.units[x, y] = true;
                            }
                        }
                        
                        else if(m <= matePos)
                            child.units[x, y] = population[i1].units[x, y];
                        
                        else
                            child.units[x, y] = population[i2].units[x, y];

                        m++;     
                    }
                }

                buffer[i] = child;                
            }
        }

        private static void Elitism(float esize)
        {
            for(int i = 0; i < (int)esize; i++)
            {
                buffer[i] = population[i];
            }
        }

        private static void Swap()
        {
            List<GA_Class> temp = new List<GA_Class>();
            temp = population;
            population = buffer;
            buffer = temp;
        }

        public static void Update()
        {
            CalcFitness();
            SortByFitness();

            bestResult = population[0].units;

            if (population.ElementAt(0).fitness == 0)
            {
                complete = true;
                
            }

            Mate();
            Swap();

            generation++;
            
        }

        public static void SetTarget(bool[,] inputTarget)
        {
            target = inputTarget;
        }


        public static bool[,] GetBest()
        {
            return bestResult;
        }
    }
}
