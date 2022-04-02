//Side container class
public class Cube
{

    public Side[] HorizontalSides { get; set; }
    public Side[] VerticalSides { get; set; }

    public Cube(Side[] horizontal, Side[] vertical)
    {
        HorizontalSides = horizontal;
        VerticalSides = vertical;
    }
}