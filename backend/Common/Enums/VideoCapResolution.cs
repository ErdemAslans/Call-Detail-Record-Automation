using System.ComponentModel;

namespace Common.Enums;

public enum VideoCapResolution
{
    [Description("Not Established")]
    NotEstablished = 0,

    [Description("SQCIF")]
    SQCIF = 1,

    [Description("QCIF")]
    QCIF = 2,

    [Description("CIF")]
    CIF = 3,

    [Description("CIF4")]
    CIF4 = 4,

    [Description("CIF16")]
    CIF16 = 5,

    [Description("H263 Custom Resolution")]
    H263CustomResolution = 6,

    [Description("W360P")]
    V360P = 7,

    [Description("VGA")]
    VGA = 8,

    [Description("W448P")]
    V448P = 9,

    [Description("HD720P")]
    HD720P = 10,

    [Description("HD1080P")]
    HD1080P = 11,

    [Description("CIF2")]
    CIF2 = 12,
}