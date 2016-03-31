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
        _CompareDel = CompareToX;
    }

    //Allow for the IComparable interface to sort on a different axis
    private delegate int _Comparison(Tile one, Tile other);
    private _Comparison _CompareDel;

    public int CompareTo(Object other)
    {
        return _CompareDel(this, (Tile)other);
    }

    public void SwapCompare()
    {
        if (_CompareDel == CompareToX)
            _CompareDel = CompareToY;
        else
            _CompareDel = CompareToX;
    }

    public static int CompareToX(Tile one, Tile other)
    {
        if (other.GetType() != one.GetType())
            return 1;
        Tile otherTile = other as Tile;
        if (one.x == otherTile.x)
            return one.y.CompareTo(otherTile.y);
        return one.x.CompareTo(otherTile.x);
    }

    public static int CompareToY(Tile one, Tile other)
    {
        if (other.GetType() != one.GetType())
            return 1;
        Tile otherTile = other as Tile;
        if (one.y == otherTile.y)
            return one.x.CompareTo(otherTile.x);
        return one.y.CompareTo(otherTile.y);
    }

    public override string ToString()
    {
        return type + " at: " + x + ", " + y;
    }
}