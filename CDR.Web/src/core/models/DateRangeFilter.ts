/**
 * DateRangeFilter Interface
 * Date range parameters for API queries
 * Reference: design/03-data-integrity.md §2
 */
interface DateRangeFilter {
  start: string; // ISO8601: "2026-01-10T00:00:00Z"
  end: string; // ISO8601: "2026-01-10T23:59:59Z"
}

export default DateRangeFilter;
