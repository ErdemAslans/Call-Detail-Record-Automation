/**
 * LocationStatistics Interface
 * Chart data for location-based call statistics
 * Reference: design/01-api-contracts.md §3.1
 */
interface LocationStatistics {
  locations: string[]; // ["Ankara", "Esenyurt", "İstanbul", ...]
  inbound: number[]; // [270, 199, 175, 272, ...] call counts per location
  outbound: number[]; // [180, 120, 95, 150, ...] call counts per location
}

export default LocationStatistics;
