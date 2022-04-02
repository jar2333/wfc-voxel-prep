//Side container class
public class Cube {
    public Side Front  { get; set; }
    public Side Right  { get; set; }
    public Side Back   { get; set; }
    public Side Left   { get; set; }
    public Side Top    { get; set; }
    public Side Bottom { get; set; }

    public Cube(Side f, Side r, Side b, Side l, Side t, Side bm)
    {
        Front  = f;
        Right  = r;
        Back   = b;
        Left   = l;
        Top    = t;
        Bottom = bm;
    }
}