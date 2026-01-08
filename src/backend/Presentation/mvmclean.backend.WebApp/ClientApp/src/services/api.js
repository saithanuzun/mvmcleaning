// API Service Configuration
import axios from 'axios';

// Base API URL - use relative path so it works with .NET server
const API_BASE_URL = '/api';

// Create axios instance with default config
const apiClient = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json'
    }
});

// Generate or get session ID for basket
const getSessionId = () => {
    let sessionId = localStorage.getItem('booking_session_id');
    if (!sessionId) {
        sessionId = `session_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
        localStorage.setItem('booking_session_id', sessionId);
    }
    return sessionId;
};

// API Service
const api = {
    // Postcode API
    postcode: {
        validate: async (postcode) => {
            const response = await apiClient.post('/postcode/validate', { postcode });
            return {
                success: response.data?.success || false,
                message: response.data?.message,
                data: response.data?.data
            };
        },
        validateAndBook: async (postcode, phone) => {
            const response = await apiClient.post('/postcode/validate-and-book', { postcode, phone });
            return {
                success: response.data?.success || false,
                message: response.data?.message,
                data: response.data?.data
            };
        }
    },

    // Services API
    services: {
        getByPostcode: async (postcode) => {
            const response = await apiClient.get(`/services/bypostcode/${postcode}`);
            return {
                success: response.data?.success || false,
                message: response.data?.message,
                data: response.data?.data
            };
        },
        getById: async (serviceId) => {
            const response = await apiClient.get(`/services/${serviceId}`);
            return {
                success: response.data?.success || false,
                message: response.data?.message,
                data: response.data?.data
            };
        }
    },

    // Basket API
    basket: {
        get: async () => {
            const sessionId = getSessionId();
            const response = await apiClient.get(`/basket/${sessionId}`);
            return response.data;
        },
        add: async (bookingId, serviceItemId, serviceName, price, durationMinutes) => {
            try {
                const response = await apiClient.post('/basket/add', {
                    bookingId: bookingId,
                    serviceItemId: serviceItemId,
                    serviceName: serviceName,
                    price: parseFloat(price),
                    quantity: 1,
                    durationMinutes: parseInt(durationMinutes)
                });
                // Unwrap ApiResponse and return with normalized success field
                return {
                    success: response.data?.success || false,
                    message: response.data?.message,
                    data: response.data?.data
                };
            } catch (error) {
                console.error('Add to cart API error:', error.response?.data || error.message);
                return {
                    success: false,
                    message: error.response?.data?.message || error.message || 'Failed to add to cart',
                    data: null
                };
            }
        },
        remove: async (bookingId, serviceItemId) => {
            try {
                const response = await apiClient.post('/basket/remove', {
                    bookingId: bookingId,
                    serviceItemId: serviceItemId
                });
                // Unwrap ApiResponse and return with normalized success field
                return {
                    success: response.data?.success || false,
                    message: response.data?.message,
                    data: response.data?.data
                };
            } catch (error) {
                console.error('Remove from cart API error:', error.response?.data || error.message);
                return {
                    success: false,
                    message: error.response?.data?.message || error.message || 'Failed to remove from cart',
                    data: null
                };
            }
        },
        updateQuantity: async (bookingId, serviceItemId, quantity) => {
            const response = await apiClient.post('/basket/add', {
                bookingId: bookingId,
                serviceItemId: serviceItemId,
                quantity: parseInt(quantity)
            });
            return {
                success: response.data?.success || false,
                message: response.data?.message,
                data: response.data?.data
            };
        },
        clear: async () => {
            const sessionId = getSessionId();
            const response = await apiClient.post(`/basket/clear/${sessionId}`);
            return {
                success: response.data?.success || false,
                message: response.data?.message,
                data: response.data?.data
            };
        }
    },

    // Availability API
    availability: {
        getSlots: async (postcode, date, durationMinutes, contractorIds = []) => {
            const response = await apiClient.get('/availability/date', {
                params: { postcode, date, durationMinutes, contractorIds: contractorIds.join(',') }
            });
            return {
                success: response.data?.success || false,
                message: response.data?.message,
                data: response.data?.data
            };
        }
    },

    // Booking API
    booking: {
        create: async (bookingData) => {
            const response = await apiClient.post('/booking/create', bookingData);
            return {
                success: response.data?.success || false,
                message: response.data?.message,
                data: response.data?.data
            };
        },
        getById: async (bookingId) => {
            const response = await apiClient.get(`/booking/${bookingId}`);
            return {
                success: response.data?.success || false,
                message: response.data?.message,
                data: response.data?.data
            };
        },
        verifyPayment: async (bookingId, sessionId) => {
            const response = await apiClient.post('/booking/verify-payment', {
                bookingId,
                sessionId
            });
            return {
                success: response.data?.success || false,
                message: response.data?.message,
                data: response.data?.data
            };
        }
    }
};

export default api;
export { getSessionId, apiClient };
