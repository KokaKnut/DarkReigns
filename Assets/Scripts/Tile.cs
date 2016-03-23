using System;
using System.Collections;
using System.Collections.Generic;

public class Tile : IComparable {

	public enum TYPE
    {
        ground,
        air
    }

    private TYPE _type;
    private int _x;
    private int _y;

    public TYPE type
    {
        get
        {
            return _type;
        }
        set
        {
            _type = value;
        }
    }

    public int x
    {
        get
        {
            return _x;
        }
    }

    public int y
    {
        get
        {
            return _y;
        }
    }

    public int[] coordinates
    {
        get
        {
            return new int[] { x, y };
        }
    }

    public Tile(int x, int y, TYPE type)
    {
        _x = x;
        _y = y;
        this.type = type;
    }

    public int CompareTo(Object other)
    {
        if (other.GetType() != this.GetType())
            return 1;
        Tile otherTile = other as Tile;
        if (this.x == otherTile.x)
            return this.y.CompareTo(otherTile.y);
        return this.x.CompareTo(otherTile.x);
    }

    public override string ToString()
    {
        return type + " at: " + x + ", " + y;
    }
}