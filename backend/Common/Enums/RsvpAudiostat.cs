using System.ComponentModel;

namespace Common.Enums;

public enum RsvpStat
{
    [Description("No Reservation")]
    NoReservation = 0,

    [Description("Reservation Failure")]
    ReservationFailure = 1,

    [Description("Reservation Success")]
    ReservationSuccess = 2,

    [Description("Reservation No Response")]
    ReservationNoResponse = 3,

    [Description("Mid Call Failure Preempted")]
    MidCallFailurePreempted = 4,

    [Description("Mid Call Failure Lost Bandwidth")]
    MidCallFailureLostBandwidth = 5,
}