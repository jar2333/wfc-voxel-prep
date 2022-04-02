using System.Collections.Generic;

public class SocketGenerator
{
    
    
    //The mapping from Sides to sockets (string)
    private Dictionary<Side, string> horizontalSocketDict;
    private Dictionary<Side, string> verticalSocketDict;
    
    //counter that keeps track of different sockets
    private int topSocket = 1;

    public SocketGenerator()
    {
        var comparer = Side.CreateSetComparer();
        horizontalSocketDict = new Dictionary<Side, string>(comparer);
        verticalSocketDict   = new Dictionary<Side, string>(comparer);
    }
    
    private string GetVerticalSocket(Side side) {
        //ABSTRACT AWAY TO GETROTATIONALSYMMETRY METHOD IN SIDE (ENUMS)
        Side rotatedSide1 = side.RotateSide();
        Side rotatedSide2 = rotatedSide1.RotateSide();
        Side rotatedSide3 = rotatedSide2.RotateSide();
        
        // symmetries: A: 1234 (4-way), B: 12 (2-way), C: 1 (None)
        bool isSymmetricA = side.SetEquals(rotatedSide1); //4-way symmetry
        bool isSymmetricB = side.SetEquals(rotatedSide2) && rotatedSide1.SetEquals(rotatedSide3); //2-way symmetry

        string sock;

        if(verticalSocketDict.ContainsKey(side)) {
            sock = verticalSocketDict[side];
            return sock;
        }

        if (isSymmetricA) {
            sock = topSocket.ToString() + "I";
            verticalSocketDict.Add(side, sock);
        }
        else if (isSymmetricB) {
            sock = topSocket.ToString();
            verticalSocketDict.Add(side,         sock + "-0");
            verticalSocketDict.Add(rotatedSide1, sock + "-1");
            sock += "-0";
        }
        else {
            sock = topSocket.ToString();
            verticalSocketDict.Add(side        , sock + "_0");
            verticalSocketDict.Add(rotatedSide1, sock + "_1");
            verticalSocketDict.Add(rotatedSide2, sock + "_2");
            verticalSocketDict.Add(rotatedSide3, sock + "_3");
            sock += "_0";
        }
        topSocket += 1;
        return sock;
    }

    private string GetHorizontalSocket(Side side) {
        //ABSTRACT AWAY TO GETMIRRORSYMMETRY METHOD IN SIDE (ENUMS)
        Side mirroredSide = side.MirrorSide();

        bool isSymmetric = side.SetEquals(mirroredSide);

        string sock;

        if (horizontalSocketDict.ContainsKey(side)) {
            sock = horizontalSocketDict[side];
            return sock;
        }

        if (isSymmetric) {
            sock = topSocket.ToString() + "S";
            horizontalSocketDict.Add(side, sock);
        }
        else {
            sock = topSocket.ToString();
            horizontalSocketDict.Add(side, sock);
            horizontalSocketDict.Add(mirroredSide, sock + "F");
        }
        topSocket += 1;
        return sock;

    }
}