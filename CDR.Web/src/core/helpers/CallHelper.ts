/**
 * CallHelper Utility
 * CDR field formatters and transformations
 * Reference: design/03-data-integrity.md §6
 */

/**
 * Format call duration from seconds to human-readable format
 * @param seconds Duration in seconds
 * @returns Formatted string like "1h 2m 3s"
 */
export function formatDuration(seconds: number): string {
  if (!seconds || seconds < 0) return '0s';

  const hours = Math.floor(seconds / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  const secs = seconds % 60;

  const parts: string[] = [];
  if (hours > 0) parts.push(`${hours}h`);
  if (minutes > 0) parts.push(`${minutes}m`);
  if (secs > 0 || parts.length === 0) parts.push(`${secs}s`);

  return parts.join(' ');
}

/**
 * Map call direction number to Turkish text
 */
export const CALL_DIRECTION_MAP: Record<number, string> = {
  1: 'Gelen',
  2: 'Giden',
};

/**
 * Format call direction (1=Inbound, 2=Outbound)
 * @param direction Direction code (1 or 2)
 * @returns Turkish direction label
 */
export function formatCallDirection(direction: number): string {
  return CALL_DIRECTION_MAP[direction] || 'Bilinmiyor';
}

/**
 * Map yes/no values to Turkish
 */
export const YES_NO_MAP: Record<string, string> = {
  Yes: 'Evet',
  No: 'Hayır',
};

/**
 * Format yes/no value to Turkish
 * @param value "Yes" or "No"
 * @returns Turkish text or original if not mapped
 */
export function formatYesNo(value: string): string {
  return YES_NO_MAP[value] || value;
}
