interface BreakListItem {
  id: string;
  userId: string;
  startTime: string;
  endTime: string | null;
  reason: string;
  breakType?: string; // "Break" or "EndOfShift"
}
