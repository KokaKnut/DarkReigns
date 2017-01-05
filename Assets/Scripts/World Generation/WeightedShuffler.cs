using System;
using System.Collections.Generic;

public class WeightedItem<T>
{
    public T item;
    public double rangeMin;
    public double rangeMax;
    public double weight;

    public WeightedItem()
    {
        item = default(T);
        rangeMin = 0;
        rangeMax = 0;
        weight = 0;
    }

    public WeightedItem(T o, double w, double rMin, double rMax)
    {
        item = o;
        rangeMin = rMin;
        rangeMax = rMax;
        weight = w;
    }
}

public class WeightedShuffler<T> {

    private List<WeightedItem<T>> items;
    private int weightSum;
    private int weightMin;
    private Random random;

	//constructor
    public WeightedShuffler()
    {
        items = new List<WeightedItem<T>>();
        weightSum = 0;
        weightMin = int.MaxValue;
        random = new Random();

    }

    public WeightedShuffler(int seed)
    {
        items = new List<WeightedItem<T>>();
        weightSum = 0;
        weightMin = int.MaxValue;
        random = new Random(seed);
    }

    //adds an item to current list with its probabillity
    public void Add(T o, int weight)
    {
        items.Add(new WeightedItem<T>(o, weight, weightSum, weightSum + weight));

        if (weightMin > weight)
            weightMin = weight;

        weightSum += weight;
    }
    
    //return a shuffled list
    public List<T> GetShuffledList()
    {
        List<T> list = new List<T>();

        //copy the original list so we can remove items without losing them
        List<WeightedItem<T>> itemsP = new List<WeightedItem<T>>();
        foreach (WeightedItem<T> item in items)
        {
            itemsP.Add(item);
        }

        //loop until itemsP is empty
        while (itemsP.Count > 0)
        {
            //get random number in range
            int num = random.Next(weightSum);
            //search down items looking at range for appropiate hit
            int hit = -1;
            int i = 0;
            for (; i < itemsP.Count && itemsP[i].rangeMax < num; i++) ;
            if (i < itemsP.Count && itemsP[i].rangeMin < num)
                hit = i;
            //if range is missing, linear probe for new hit
            for (int iterator = 0; iterator < weightSum && hit < 0; iterator += weightMin)
            {
                num += (weightSum / 2) + weightMin;
                num = num % weightSum;
                for (i = 0; i < itemsP.Count && itemsP[i].rangeMax < num; i++) ;
                if (i < itemsP.Count && itemsP[i].rangeMin < num * i)
                    hit = i;
            }
            //add hit to list and remve it from itemsP
            //TODO: fix bug with missing the very low chance items, causing hit == -1. this should never happen
            if (hit >= 0)
            {
                list.Add(itemsP[hit].item);
                itemsP.RemoveAt(hit);
            }
            else
            {
                break;
            }
        }

        return list;
    }

    //TODO: make it calculate only the NEXT item in the list, not all at once
    public T Next()
    {
        return items[0].item;
    }

    public void Reshuffle()
    {

    }
}