using System.Xml;
using UnityEngine;

public class OSMNode : BaseOSM
{
    public ulong ID { get; private set; }
    public float Latitude { get; private set; }
    public float Longitude { get; private set; }
    public float X { get; private set; }
    public float Y { get; private set; }

    public static implicit operator Vector3 (OSMNode node) {
        return new Vector3(node.X, 0, node.Y);
    }

    public OSMNode(XmlNode node) {
        ID = GetAttribute<ulong>("id", node.Attributes);
        Latitude = GetAttribute<float>("lat", node.Attributes);
        Longitude = GetAttribute<float>("lon", node.Attributes);

        X = (float)MercatorProjection.lonToX(Longitude);
        Y = (float)MercatorProjection.latToY(Latitude);
    }
}