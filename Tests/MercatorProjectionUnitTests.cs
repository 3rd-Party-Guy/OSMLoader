using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MercatorProjectionUnitTests
{
    [Test]
    public void ToPixel()
    {
        double[] toPixelTestOne = MercatorProjection.toPixel(74, 859);
        double[] toPixelTestOneAns = { 8237642, 34619289 };

        double[] toPixelTestTwo = MercatorProjection.toPixel(96, 300);
        double[] toPixelTestTwoAns = { 10686671, 34619289 };

        Assert.AreEqual(Math.Round(toPixelTestOne[0]), toPixelTestOneAns[0]);
        Assert.AreEqual(Math.Round(toPixelTestOne[1]), toPixelTestOneAns[1]);

        Assert.AreEqual(Math.Round(toPixelTestTwo[0]), toPixelTestTwoAns[0]);
        Assert.AreEqual(Math.Round(toPixelTestTwo[1]), toPixelTestTwoAns[1]);
    }

    [Test]
    public void ToGeoCoords()
    {
        double[] toGeoTestOne = MercatorProjection.toGeoCoord(1356300, 54600000);
        double[] toGeoTestOneAns = { 12, 90 };

        double[] toGetTestTwo = MercatorProjection.toGeoCoord(98723165, 9999999);
        double[] toGeoTestTwoAns = { 887, 67 };

        Assert.AreEqual(Math.Round(toGeoTestOne[0]), toGeoTestOneAns[0]);
        Assert.AreEqual(Math.Round(toGeoTestOne[1]), toGeoTestOneAns[1]);

        Assert.AreEqual(Math.Round(toGetTestTwo[0]), toGeoTestTwoAns[0]);
        Assert.AreEqual(Math.Round(toGetTestTwo[1]), toGeoTestTwoAns[1]);
    }
}
