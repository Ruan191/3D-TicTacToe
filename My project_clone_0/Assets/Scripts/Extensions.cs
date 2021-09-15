using System.Collections;
using System.Collections.Generic;

public static class Extensions
{
    //Bool Extentions
    public static void Switch(this bool b)
    {
        if (b == true)
            _ = false;
        else
            _ = true;
    }
}
