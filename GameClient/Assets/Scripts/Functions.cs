using System.Collections;
using System.Collections.Generic;

public static class Functions
{

    public static string GenUUID()
    {
        return System.Guid.NewGuid().ToString();
    }

}
