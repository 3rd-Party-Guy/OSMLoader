using System;

public class MercatorProjection
{
    private static readonly double R_MAJOR = 6378137.0;
    private static readonly double R_MINOR = 6356752.3142;
    private static readonly double RATIO = R_MINOR / R_MAJOR;
    private static readonly double ECCENT = Math.Sqrt(1.0 - (RATIO * RATIO));
    private static readonly double COM = 0.5 * ECCENT;
    private static readonly double HALF_PI = Math.PI / 2.0;

    private static readonly double DEG2RAD = Math.PI / 180.0;
    private static readonly double RAD2DEG = 180.0 / Math.PI;

    public static double[] toPixel(double lon, double lat) => new double[] { lonToX(lon), latToY(lat) };
    public static double[] toGeoCoord(double x, double y) => new double[] { xToLon(x), yToLat(y) };

    private static double RadToDeg(double rad) => rad * RAD2DEG;
    private static double DegToRad(double deg) => deg * DEG2RAD;

    public static double lonToX(double lon) => R_MAJOR * DegToRad(lon);
    public static double xToLon(double x) => RadToDeg(x) / R_MAJOR;

    public static double latToY(double lat) {
        lat = Math.Min(89.5, Math.Max(lat , -89.5));
        double phi = DegToRad(lat);
        double sinphi = Math.Sin(phi);
        double con = ECCENT * sinphi;
        con = Math.Pow(((1.0 - con) / (1.0 + con)), COM);
        double ts = Math.Tan(0.5 * ((Math.PI * 0.5) - phi)) / con;

        return 0 - R_MAJOR * Math.Log(ts);
    }

    public static double yToLat(double y) {
        double ts = Math.Exp(-y / R_MAJOR);
        double phi = HALF_PI - 2 * Math.Atan(ts);
        double dphi = 1.0;
        int i = 0;
        while((Math.Abs(dphi) > 0.0000000001) && (i < 15)) {
            double con = ECCENT * Math.Sin(phi);
            dphi = HALF_PI - 2 * Math.Atan(ts * Math.Pow((1.0 - con) / (1.0 + con), COM)) - phi;
            phi += dphi;
            i++;
        }

        return RadToDeg(phi);
    }
}