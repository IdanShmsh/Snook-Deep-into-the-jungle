First, let me introduce some of the tools I made myself for making life easier when developing the game.

In addition to the built in vector opperation methods, inside the Vector class of Unity, I found myself making some vector opperations by hand repetedly, 
so instead I just shoved them into a separated class to keep the code neat.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class VectorHandeler
{
    //return the mid point between two vector3s
    public static Vector3 middle(Vector3 v1, Vector3 v2)
    {
        return new Vector3((v1.x + v2.x) / 2, (v1.y + v2.y) / 2, (v1.z + v2.z) / 2);
    }

    //return the point between two vector3s with the ratio of distances between these is the ratio between the two given
    public static Vector3 between(Vector3 v1, Vector3 v2, float ratio1, float ratio2)
    {
        return new Vector3(((ratio2 * v1.x) + (ratio1 * v2.x)) / (ratio1 + ratio2), ((ratio2 * v1.y) + (ratio1 * v2.y)) / (ratio1 + ratio2), ((ratio2 * v1.z) + (ratio1 * v2.z)) / (ratio1 + ratio2));
    }

    //return the mid point between two vector2s
    public static Vector2 middle(Vector2 v1, Vector2 v2)
    {
        return new Vector2((v1.x + v2.x) / 2, (v1.y + v2.y) / 2);
    }

    //return the point between two vector2s with the ratio of distances between these is the ratio between the two given
    public static Vector2 between(Vector2 v1, Vector2 v2, float ratio1, float ratio2)
    {
        return new Vector2(((ratio2 * v1.x) + (ratio1 * v2.x)) / (ratio1 + ratio2), ((ratio2 * v1.y) + (ratio1 * v2.y)) / (ratio1 + ratio2));
    }

    //return the scalar distance between two vector3s
    public static float distance(Vector3 v1, Vector3 v2)
    {
        return Mathf.Sqrt(((v1.x - v2.x) * (v1.x - v2.x)) + ((v1.y - v2.y) * (v1.y - v2.y)) + ((v1.z - v2.z) * (v1.z - v2.z)));
    }

    //return the distance between two vector2s
    public static float distance(Vector2 v1, Vector2 v2)
    {
        return Mathf.Sqrt(((v1.x - v2.x) * (v1.x - v2.x)) + ((v1.y - v2.y) * (v1.y - v2.y)));
    }

    //return the quanternion that a the point 'from' needs to have in order to be looking at the point 'to'
    public static Quaternion lookAt(Vector3 from, Vector3 to)
    {
        return Quaternion.LookRotation(to - from);
    }

    //return the direction vector of 'from' and 'to' - a normalized vector (magnitude 1) that when plotted on 'from', points at 'to'
    public static Vector3 getDirectionVector(Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }

    //return the direction vector of 'from' and 'to' - a normalized vector (magnitude 1) that when plotted on 'from', points at 'to'
    public static Vector2 getDirectionVector(Vector2 from, Vector2 to)
    {
        return (to - from).normalized;
    }

    //return a list of 'pointAmnt' vectors distributed in equal distances from each other between two points
    public static Vector3[] destribute(Vector3 from, Vector3 to, int pointAmnt)
    {
        Vector3[] points = new Vector3[pointAmnt];

        points[0] = from;

        for (float i = 1; i < points.Length; i++)
        {
            points[(int)i] = between(from, to, ((i) / (points.Length - 1)), 1 - ((i) / (points.Length - 1)));
        }

        return points;
    }

    //return a normalized vector3 with a random direction
    public static Vector3 randomDirection3()
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    //return a normalized vector2 with a random direction
    public static Vector2 randomDirection2()
    {
        return new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }
}

===================================================================================================================================================

In addition to that I designed another(, much cooler) class providing me with mathematical tools that I used throughout my code.

Throught the project I found myself needing a random generator that spits out a value (not only numbers) based on some probablility that it
will be picked. The default Random.range(a, b) spits out a random number between a and b with close to an even chance of every number being chosen.
I needed a random generator that gives me values with specified chances of each to be picked.
 
I did that using two methods. 
- The first can be used with any form of value (not only numbers). To use it I give it each value with
its probability to be chosen, where this probability can be any number, the actual probability of it being chosen is relative to the other's.
If I tell it 'the probability to pick "meow" is 5' and 'the probability to pick "woof" is 10' the probability of the randomizer picking 'woof' is
twice as much as the probability of it picking 'meow', same for the probabilities 60 and 120 - it doesn't matter.
- The second one is much more complicated wich makes it very interesting.
This one can return only numbers but it's much more useful. The information about the probability of each number being picked is given using a
ValueProbabilityFunction, which is a structure in the code that takes a method with float input and a float output, representing a mathematical 
function that describes the probability of each value to be picked, and makes some mathematical opperations on that function so that it can make 
a random pick based on the probability function it was given.
In addition to the function, it requests a number that represents the interval between each option, and the range of options. (if the range is 5
, 10 and the interval is 0.5 the options will be 5, 5.5, 6,..., 9.5, 10)
What it does is calculating the area under the curve of the probability function by calculating the integral of it, in the range specified, where
dx is the interval specified. When picking a number, I pick a random number beteen 0 and 1 (in a regular fassion), I multiply it by the previously
calculated area and it gives me a certain area which is a fruction of the whole area under the curve. I re-calculate the integral in the same way
but each itteration of the summation I check if the area passed the calculated fruction, if it did I return the number it was on when it got to 
this area.
The logic and math behind that are a thing I'm very proud of myself for coming up with alone. When I calculate the integral I add up the localized 
area of each value - the value of the function at that value times the interval. This is the localized area it acupies out of the entire area under 
the curve. Now, Because is interval is constant, the bigger the value of the function for that value, the bigger the area it ocupies, and so, when
generating a fruction of the whole area, the bigger the chance it will fall inside of the personal area of that specific value.

This class is independent from the Unity engine and can be used in any c# project

using System;

/// <summary>
/// This class takes care of complex randomised picking of values using a probability of each to be selected
/// </summary>
/// <typeparam name="T">Is the type of value that the class will randomly selecting</typeparam>
public class SmartRandomizer<T>
{
    //a "Random" instace for randomisations
    static Random random = new Random(); 

    //the array of valueProbabilityPair(s) (each contains a value and the probability of it being selected)
    //for the use of an instance of this class.
    private valueProbabilityPair<T>[] pairArray; 

    public SmartRandomizer(valueProbabilityPair<T>[] pairArray)
    {
        this.pairArray = pairArray;
    }

    /// <summary>
    /// this function uses the array of valueProbabilityPair(s) (each contains a value and the probability of it being selected)
    /// that was assigned by the constructor and returns a selected value (values a generic)
    /// </summary>
    /// <returns>the selected value</returns>
    public T randomizeValue()
    {
        return randomizeValue(pairArray);
    }

    /// <summary>
    /// this function gets an array of valueProbabilityPair(s) (each contains a value and the probability of it being selected)
    /// and returns a selected value (values a generic)
    /// </summary>
    /// <param name="pairArray">the array of valueProbabilityPair(s)</param>
    /// <returns>the selected value</returns>
    public static T randomizeValue(valueProbabilityPair<T>[] pairArray)
    {
        float currentSum = 0; //first, current sum is used to get the sum of all the probabilities
                              //to determain the range in which the randomised number will vbe selected

        for(int i = 0; i < pairArray.Length; i++)
        {
            currentSum += pairArray[i].getProbability();
        }

        double number = random.NextDouble() * currentSum;//assign the randomised number to a random numer between 0 - currentSum

        currentSum = 0; //now use the current sum variable to select the right value 

        for (int i = 0; i < pairArray.Length; i++)
        {
            currentSum += pairArray[i].getProbability();

            if(currentSum >= number) //when the current sum goes beyond the generated number it means that the number is in
                                     //the probability range of the current value - so return it.
            {
                return pairArray[i].getValue();
            }
        }

        return pairArray[random.Next(0, pairArray.Length)].getValue(); //if somehow failed to get a value from the process above just return a random value
    }

    /// <summary>
    /// Set the valueProbabilityPair array of this instance.
    /// </summary>
    /// <param name="pairArray">The valueProbabilityPair array to be set</param>
    public void setValueProbabilityPairArray(valueProbabilityPair<T>[] pairArray)
    {
        this.pairArray = pairArray;
    }
}

/// <summary>
/// This class uses the class valueProbabilityFunction to generate random numbers
/// based on their probability which is given by a probability/value function.
/// </summary>
public class SmartRandomizer
{
    //a "Random" instace for randomisations
    static Random random = new Random();

    //An instance of valueProbabilityFunction for the use of instances of this class;
    valueProbabilityFunction vpf;

    /// <summary>
    /// Create a new incetance of the smart randomzer.
    /// Instances of this class allow saving the valueProbabilityFunction instance for a repeated use.
    /// </summary>
    /// <param name="vpf">a valueProbabilityFunction that will be used to generate random values</param>
    public SmartRandomizer(valueProbabilityFunction vpf)
    {
        this.vpf = vpf;
    }

    /// <summary>
    /// Generate a random number using the valueProbabilityFunction instance of this instance
    /// </summary>
    /// <returns>A random number</returns>
    public double randomizeValue()
    {
        return this.vpf.getARandomizedNumberInRange(random.NextDouble());
    }

    /// <summary>
    /// Generate a random number using a custom valueProbabilityFunction instance.
    /// </summary>
    /// <param name="vpf">A custom valueProbabilityFunction instance to generate the random number</param>
    /// <returns>A random number</returns>
    public static double randomizeValue(valueProbabilityFunction vpf)
    {
        return vpf.getARandomizedNumberInRange(random.NextDouble());
    }

    /// <summary>
    /// Set the valueProbabilityFunction of this class.
    /// </summary>
    /// <param name="vpf">the valueProbabilityFunction to be set</param>
    public void setValueProbabilityFunction(valueProbabilityFunction vpf)
    {
        this.vpf = vpf;
    }

    public static bool randomBoolean()
    {
        return (random.Next(2) == 1);
    }
}

/// <summary>
/// This class represents a pair of an object of type T and its probability to be selected by the
/// smart randomizer.
/// Used only for the smart randomizer class's asctions, for randomized selection of values when the probability of
/// each value being selected is given.
/// </summary>
/// <typeparam name="T"></typeparam>
public class valueProbabilityPair<T>
{
    private T value;
    private float probability;

    /// <summary>
    /// Create a pair of a value of type T and a probability.
    /// </summary>
    /// <param name="value">the value with the oribability to be selected</param>
    /// <param name="prob">the probability of the value to be selected **0 - 1**</param>
    public valueProbabilityPair(T value, float prob)
    {
        this.value = value;
        this.probability = prob;
    }

    public T getValue()
    {
        return value;
    }

    public float getProbability()
    {
        return probability;
    }
}

/// <summary>
/// This class is used only for the smart randomizer class's asctions, for randomized selection of numbers
/// using a probability funcion.
/// </summary>
public class valueProbabilityFunction
{
    //the probability function
    Func<float, float> probabilityFunction;
    //precition of calculation
    float precision = 1;

    //the start of the range in which the values will be picked from the function
    float rangeStart;
    //the end of the range in which the values will be picked from the function
    float rangeEnd;

    //the area ander the function's curve in the specified range.
    float rangeArea;

    /// <summary>
    /// Create a new value probability function calculation instance
    /// </summary>
    /// <param name="probabilityFunction">A pointer to a function that requires a float and returns a float. This is the probability function.</param>
    /// <param name="precision">A float that specifies the precition of the calculations performed by this instance. (The lower the precission the
    /// greater the impact on the system. Very small precission values may cause slowing of the system's performance.)</param>
    /// <param name="rangeStart">The start of the range in which the values will be picked from the function</param>
    /// <param name="rangeEnd">The end of the range in which the values will be picked from the function</param>
    public valueProbabilityFunction(Func<float, float> probabilityFunction, float precision, float rangeStart, float rangeEnd)
    {
        this.probabilityFunction = probabilityFunction;
        this.precision = precision;
        if (this.precision <= 0) throw (new Exception("Precision value must be higher than 0."));
        this.rangeStart = rangeStart;
        this.rangeEnd = rangeEnd;
        if((this.rangeEnd - this.rangeStart) < this.precision) throw (new Exception("The end of the range must be after the range start by at least the precision value once."));

        calculateRangeArea();
    }

    /// <summary>
    /// in this function the area under the probability function that was given is calculated
    /// using an integral between rangeStart and rangeEnd where precision is used as the dx of the integral.
    /// (calculation will consider function values smaller than 0 as 0).
    /// </summary>
    private void calculateRangeArea()
    {
        for(float i = this.rangeStart; i < this.rangeEnd; i += precision)
        {
            float value = probabilityFunction(i);
            if(value > 0)
            {
                rangeArea += value * precision;
            }
        }
    }

    /// <summary>
    /// This function uses a random double given (0 - 1) and returns a new number, out of the given range.
    /// This number is the smart generated number using the probability function.
    /// </summary>
    /// <param name="randomizedNumberInRange">A random double (0 - 1)</param>
    /// <returns>The smart generated number using the probability function.</returns>
    public double getARandomizedNumberInRange(double randomizedNumberInRange)
    {
        //calculate the fraction of the given number out of the whole function area.
        double number = (randomizedNumberInRange * rangeArea);

        //integrate the function until passing the area calculated.
        //return the number at which is happened (i);
        double temp = 0;
        for (float i = this.rangeStart; i < this.rangeEnd; i += precision)
        {
            float value = probabilityFunction(i);
            if (value > 0)
            {
                temp += value * precision;
            }

            if(temp >= number)
            {
                return i;
            }
        }

        //if for some reason the oppertaion was not completed successfully - return the fraction out of the range.
        return (randomizedNumberInRange * (this.rangeEnd - this.rangeStart)) + this.rangeStart;
    }
}



