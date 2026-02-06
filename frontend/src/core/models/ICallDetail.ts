interface ICallDetail {
  id: number;
  originalCalledPartyNumber: string;
  callingPartyNumber: string;
  dateTimeOrigination: string;
  dateTimeConnect: string;
  callAwaitDuration: number;
  duration: number;
  hasRedirected: string;
  callEndedReason: string;
  callType: string;
  callDirection: number;
}
