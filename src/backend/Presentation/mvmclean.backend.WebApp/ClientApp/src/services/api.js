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
            return response.data;
        },
        validateAndBook: async (postcode, phone) => {
            const response = await apiClient.post('/postcode/validate-and-book', { postcode, phone });
            return response.data;
        }
    },

    // Services API
    services: {
        getByPostcode: async (postcode) => {
            const response = await apiClient.get(`/services/bypostcode/${postcode}`);
            return response.data;
        },
        getById: async (serviceId) => {
            const response = await apiClient.get(`/services/${serviceId}`);
            return response.data;
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
            const response = await apiClient.post('/basket/add', {
                bookingId: bookingId,
                serviceItemId: serviceItemId,
                serviceName: serviceName,
                price: parseFloat(price),
                quantity: 1,
                durationMinutes: parseInt(durationMinutes)
            });
            return response.data;
        },
        remove: async (bookingId, serviceItemId) => {
            const response = await apiClient.post('/basket/remove', {
                bookingId: bookingId,
                serviceItemId: serviceItemId
            });
            return response.data;
        },
        updateQuantity: async (bookingId, serviceItemId, quantity) => {
            const response = await apiClient.post('/basket/add', {
                bookingId: bookingId,
                serviceItemId: serviceItemId,
                quantity: parseInt(quantity)
            });
            return response.data;
        },
        clear: async () => {
            const sessionId = getSessionId();
            const response = await apiClient.post(`/basket/clear/${sessionId}`);
            return response.data;
        }
    },

    // Availability API
    availability: {
        getSlots: async (postcode, date, durationMinutes, contractorIds = []) => {
            const response = await apiClient.get('/availability/date', {
                params: { postcode, date, durationMinutes, contractorIds: contractorIds.join(',') }
            });
            return response.data;
        }
    },

    // Booking API
    booking: {
        create: async (bookingData) => {
            const response = await apiClient.post('/booking/create', bookingData);
            return response.data;
        },
        getById: async (bookingId) => {
            const response = await apiClient.get(`/booking/${bookingId}`);
            return response.data;
        },
        verifyPayment: async (bookingId, sessionId) => {
            const response = await apiClient.post('/booking/verify-payment', {
                bookingId,
                sessionId
            });
            return response.data;
        }
    }
};

export default api;
export { getSessionId, apiClient };
