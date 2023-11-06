using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;

internal sealed class MapReader
{
    [HideInInspector] public Dictionary<ulong, OSMNode> nodes;
    [HideInInspector] public List<OSMWay> ways;
    [HideInInspector] public OSMBounds bounds;

    public void InitializeMap(string osmDataFile) {
        nodes = new Dictionary<ulong, OSMNode>();
        ways = new List<OSMWay>();

        string osmText = File.ReadAllText(osmDataFile);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(osmText);

        SetBounds(doc.SelectSingleNode("/osm/bounds"));
        GetNodes(doc.SelectNodes("/osm/node"));
        GetWays(doc.SelectNodes("/osm/way"));

        float minx = (float)MercatorProjection.lonToX(bounds.MinLon);
        float maxx = (float)MercatorProjection.lonToX(bounds.MaxLon);
        float miny = (float)MercatorProjection.latToY(bounds.MinLat);
        float maxy = (float)MercatorProjection.latToY(bounds.MaxLat);
    }

    private void GetWays(XmlNodeList xmlNodeList) {
        foreach (XmlNode node in xmlNodeList) {
            OSMWay way = new OSMWay(node);
            ways.Add(way);
        }
    }

    private void GetNodes(XmlNodeList xmlNodeList) {
        foreach (XmlNode node in xmlNodeList) {
            OSMNode n = new OSMNode(node);
            nodes[n.ID] = n;
        }
    }

    private void SetBounds(XmlNode xmlNode) {
        bounds = new OSMBounds(xmlNode);
    }
}