using System.Collections.Generic;

public class WeightedItem
{
    public object item;
    public float rangeMin;
    public float rangeMax;
    public float weight;

    public WeightedItem()
    {
        item = null;
        rangeMin = 0;
        rangeMax = 0;
        weight = 0;
    }

    public WeightedItem(object o, float w, float rMin, float rMax)
    {
        item = o;
        rangeMin = rMin;
        rangeMax = rMax;
        weight = w;
    }
}

public class WeightedShuffler {

    private List<WeightedItem> items;
    private float weightSum;
    private float weightMin;

	//constructor
    public WeightedShuffler()
    {
        items = new List<WeightedItem>();
        weightSum = 0;
        weightMin = float.MaxValue;
    }

    //adds an item to current list with its probabillity
    public void Add(object o, float weight)
    {
        items.Add(new WeightedItem(o, weight, weightSum, weightSum + weight));

        if (weightMin > weight)
            weightMin = weight;

        weightSum += weight;
    }
    
    //shuffle the list
    public List<object> GetShufledList()
    {
        List<object> list = new List<object>();

        //copy the original list so we can remove items without losing them
        List<WeightedItem> itemsP = new List<WeightedItem>();
        foreach (WeightedItem item in items)
        {
            itemsP.Add(item);
        }

        //loop until empty
        //get random number in range
        //search down items looking at range for appropiate hit
        //if range is missing, linear probe for new hit
        //

        return list;
    }
}