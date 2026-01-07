// Contractor Service - API calls for contractor operations
import { apiClient } from './api';

const contractorService = {
    /**
     * Get contractors by postcode
     * @param {string} postcode - The postcode to search contractors
     * @param {string} bookingId - Optional booking ID
     * @returns {Promise<{contractorIds: string[], bookingId: string}>} Response with contractor IDs
     */
    getByPostcode: async (postcode, bookingId = '') => {
        try {
            const response = await apiClient.get(`/api/contractor/bypostcode/${encodeURIComponent(postcode)}`, {
                params: { bookingId }
            });
            // API returns: { ContractorIds: [...], BookingId: "...", success: true, message: "..." }
            return {
                contractorIds: response.data.data?.ContractorIds || response.data.ContractorIds || [],
                bookingId: response.data.data?.BookingId || response.data.BookingId || bookingId,
                message: response.data.message || ''
            };
        } catch (error) {
            console.error('Error fetching contractors by postcode:', error);
            throw error;
        }
    },

    /**
     * Get contractor availability by day
     * @param {string} contractorId - The contractor ID
     * @param {string} date - Date for availability check (ISO format: YYYY-MM-DD)
     * @returns {Promise<object>}
     */
    getAvailabilityByDay: async (contractorId, date) => {
        try {
            const response = await apiClient.get(`/api/contractor/availability/day`, {
                params: { contractorId, date }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching contractor availability:', error);
            throw error;
        }
    }
};

export default contractorService;
