public class Tile {

	public enum TYPE
    {
        ground,
        air
    }

    TYPE _type;

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

    public Tile(TYPE type)
    {
        this.type = type;
    }
}