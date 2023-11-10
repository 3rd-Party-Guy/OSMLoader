using System.Collections.Generic;
using System.Xml;

public class OSMWay : BaseOSM
{
    public ulong ID { get; private set; }
    public bool Visible { get; private set; }
    public List<ulong> NodeIDs { get; private set; }
    public bool IsBoundary { get; private set; }
    public bool IsBuilding { get; private set; }
    public bool IsRoad { get; private set; }
    public float Height { get; private set; }
    public string Name { get; private set; }
    public int Lanes { get; private set; }
    public string Color { get; private set; }

    public OSMWay(XmlNode node) {
        NodeIDs = new List<ulong>();
        Height = 3.0f;
        Lanes = 1;
        Name = "";
        Color = "";

        ID = GetAttribute<ulong>("id", node.Attributes);
        Visible = GetAttribute<bool>("visible", node.Attributes);

        XmlNodeList nds = node.SelectNodes("nd");
        foreach(XmlNode n in nds) {
            ulong refNo = GetAttribute<ulong>("ref", n.Attributes);
            NodeIDs.Add(refNo);
        }

        if (NodeIDs.Count > 1)
            IsBoundary = NodeIDs[0] == NodeIDs[NodeIDs.Count - 1];

        XmlNodeList tags = node.SelectNodes("tag");
        foreach (XmlNode t in tags) {
            string key = GetAttribute<string>("k", t.Attributes);

            switch (key) {
                case "building:levels":
                    Height = 3.0f * GetAttribute<float>("v", t.Attributes);
                    break;
                case "height":
                    Height = 0.3048f * GetAttribute<float>("v", t.Attributes);
                    break;
                case "building":
                    IsBuilding = true;
                    break;
                case "highway":
                    IsRoad = true;
                    break;
                case "lanes":
                    Lanes = GetAttribute<int>("v", t.Attributes);
                    break;
                case "name":
                    Name = GetAttribute<string>("v", t.Attributes);
                    break;
                case "building:colour":
                    Color = GetAttribute<string>("v", t.Attributes);
                    break;
                default:
                    break;
            }
        }
    }
}