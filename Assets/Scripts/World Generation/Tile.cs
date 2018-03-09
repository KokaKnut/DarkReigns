using UnityEngine;
using System;

[System.Serializable]
public class Tile : IComparable {

	public enum TYPE
    {
        none,
        ground,
        air,
        rope,
        spawn,
        shrine
    }

    [SerializeField]
    private TYPE _type;
    [SerializeField]
    private int _x;
    [SerializeField]
    private int _y;

    private bool _linkageTile = false;

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

    public bool linkageTile
    {
        get
        {
            return _linkageTile;
        }
        set
        {
            _linkageTile = value;
        }
    }

    public Tile(int x, int y, TYPE type)
    {
        _x = x;
        _y = y;
        this.type = type;
        _CompareDel = CompareToX;
    }

    public Tile(int x, int y, TYPE type, Char c)
    {
        _x = x;
        _y = y;
        this.type = type;
        SetCompare(c);
    }

    //Allow for the IComparable interface to sort on a different axis
    public delegate int _Comparison(Tile one, Tile other);
    [SerializeField]
    private _Comparison _CompareDel;

    public int CompareTo(System.Object other)
    {
        return _CompareDel(this, (Tile)other);
    }

    public void SetCompare(Char c)
    {
        if (c == 'Y')
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
        return type + " at: " + x + ", " + y + ", " + _CompareDel.Method.Name;
    }

    public string ToUnityString()
    {
        return type + " at: " + x + ", " + y;
    }
}