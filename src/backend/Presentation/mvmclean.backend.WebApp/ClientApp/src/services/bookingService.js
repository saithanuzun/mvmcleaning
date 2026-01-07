// Booking Service - API calls for booking operations
import { getSessionId, apiClient } from './api';

const bookingService = {
    /**
     * Create a new booking with postcode and phone number
     * @param {object} bookingData - Booking data
     * @param {string} bookingData.phoneNumber - Customer phone number (required by API)
     * @param {string} bookingData.postcode - Service postcode (required by API)
     * @returns {Promise<object>} Created booking details with BookingId
     */
    create: async (bookingData) => {
        try {
            const payload = {
                phoneNumber: bookingData.phoneNumber,
                postcode: bookingData.postcode
            };

            const response = await apiClient.post('/api/booking/create', payload);
            return {
                success: true,
                bookingId: response.data.data?.BookingId || response.data.BookingId,
                booking: response.data.data || response.data,
                message: response.data.message || ''
            };
        } catch (error) {
            console.error('Error creating booking:', error);
            return {
                success: false,
                error: error.response?.data?.message || error.message
            };
        }
    },

    /**
     * Get booking by ID
     * @param {string} bookingId - The booking ID
     * @returns {Promise<object>}
     */
    getById: async (bookingId) => {
        try {
            const response = await apiClient.get(`/api/booking/${bookingId}`);
            return response.data;
        } catch (error) {
            console.error('Error fetching booking:', error);
            throw error;
        }
    },

    /**
     * Verify payment
     * @param {string} bookingId - The booking ID
     * @param {string} sessionId - Stripe session ID
     * @returns {Promise<object>}
     */
    verifyPayment: async (bookingId, sessionId) => {
        try {
            const response = await apiClient.post('/api/booking/verify-payment', { bookingId, sessionId });
            return response.data;
        } catch (error) {
            console.error('Error verifying payment:', error);
            throw error;
        }
    }
};

export default bookingService;
