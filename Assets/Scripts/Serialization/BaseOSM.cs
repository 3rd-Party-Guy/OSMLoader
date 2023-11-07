using System;
using System.Xml;

public class BaseOSM
{
    protected T GetAttribute<T>(string attributeName, XmlAttributeCollection attributes) {
        // if (attributes[attributeName] == null)
        //     return default(T);
        string value = attributes[attributeName].Value;
        return (T)Convert.ChangeType(value, typeof(T));
    }
}