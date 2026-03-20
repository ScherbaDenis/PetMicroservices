/**
 * API Configuration
 * Centralized configuration for API endpoints
 */
/**
 * Base URL for the API Gateway
 * Change this value to point to different environments
 */
export const API_GATEWAY_URL = 'http://localhost:5000';
/**
 * Helper function to build full API URLs
 * @param path - The API path (e.g., '/user', '/template')
 * @returns Full URL to the API endpoint
 */
export function buildApiUrl(path) {
    // Ensure path starts with /
    const normalizedPath = path.startsWith('/') ? path : `/${path}`;
    return `${API_GATEWAY_URL}${normalizedPath}`;
}
