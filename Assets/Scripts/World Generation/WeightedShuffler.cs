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

public class WeightedShuffler<T> {

    private List<T> items;

	//constructor
    public WeightedShuffler()
    {
        items = new List<T>();
    }

    //adds an item to current list with its probabillity
    
    //shuffle the list
}
