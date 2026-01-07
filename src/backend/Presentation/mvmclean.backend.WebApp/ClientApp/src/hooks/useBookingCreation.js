// Custom hook for booking creation
import { useState, useCallback } from 'react';
import bookingService from '../services/bookingService';

export const useBookingCreation = () => {
    const [isCreating, setIsCreating] = useState(false);
    const [error, setError] = useState('');
    const [bookingResult, setBookingResult] = useState(null);

    const create = useCallback(async (bookingData) => {
        // API only requires: phoneNumber and postcode
        if (!bookingData.phoneNumber) {
            setError('Phone number is required');
            return null;
        }
        if (!bookingData.postcode) {
            setError('Postcode is required');
            return null;
        }

        setIsCreating(true);
        setError('');
        setBookingResult(null);

        try {
            const result = await bookingService.create(bookingData);
            
            if (result.success) {
                setBookingResult(result);
                return result;
            } else {
                setError(result.error || 'Failed to create booking');
                return null;
            }
        } catch (err) {
            const errorMessage = err.response?.data?.message || err.message || 'Error creating booking';
            setError(errorMessage);
            return null;
        } finally {
            setIsCreating(false);
        }
    }, []);

    const reset = useCallback(() => {
        setIsCreating(false);
        setError('');
        setBookingResult(null);
    }, []);

    return {
        isCreating,
        error,
        bookingResult,
        create,
        reset
    };
};
