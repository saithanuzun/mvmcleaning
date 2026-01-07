// Custom hook for postcode validation and contractor fetching
import { useState, useCallback } from 'react';
import postcodeService from '../services/postcodeService';
import contractorService from '../services/contractorService';

export const usePostcodeValidation = () => {
    const [postcode, setPostcode] = useState('');
    const [isValidating, setIsValidating] = useState(false);
    const [isValid, setIsValid] = useState(null);
    const [postcodeDetails, setPostcodeDetails] = useState(null);
    const [error, setError] = useState('');

    const validate = useCallback(async (code) => {
        if (!code || code.trim().length === 0) {
            setError('Postcode is required');
            setIsValid(false);
            return false;
        }

        setIsValidating(true);
        setError('');
        
        try {
            const result = await postcodeService.validate(code);
            setIsValid(result.valid);
            
            if (result.valid) {
                const details = await postcodeService.getDetails(code);
                setPostcodeDetails(details);
            } else {
                setError(result.message || 'Invalid postcode');
                setPostcodeDetails(null);
            }
            
            return result.valid;
        } catch (err) {
            setError('Error validating postcode: ' + err.message);
            setIsValid(false);
            setPostcodeDetails(null);
            return false;
        } finally {
            setIsValidating(false);
        }
    }, []);

    const reset = useCallback(() => {
        setPostcode('');
        setIsValid(null);
        setPostcodeDetails(null);
        setError('');
    }, []);

    return {
        postcode,
        setPostcode,
        isValidating,
        isValid,
        postcodeDetails,
        error,
        validate,
        reset
    };
};

export const useContractorsByPostcode = () => {
    const [contractorIds, setContractorIds] = useState([]); // Array of contractor ID strings
    const [selectedContractorId, setSelectedContractorId] = useState(null);
    const [isFetching, setIsFetching] = useState(false);
    const [error, setError] = useState('');
    const [bookingId, setBookingId] = useState(''); // Store booking ID from response

    const fetch = useCallback(async (postcode, bookingIdParam = '') => {
        if (!postcode || postcode.trim().length === 0) {
            setError('Postcode is required');
            return [];
        }

        setIsFetching(true);
        setError('');
        setContractorIds([]);
        setSelectedContractorId(null);

        try {
            const result = await contractorService.getByPostcode(postcode, bookingIdParam);
            // API returns { contractorIds: ["id1", "id2"], bookingId: "...", message: "..." }
            setContractorIds(result.contractorIds || []);
            setBookingId(result.bookingId);
            return result.contractorIds || [];
        } catch (err) {
            const errorMessage = err.response?.data?.message || err.message || 'Error fetching contractors';
            setError(errorMessage);
            setContractorIds([]);
            return [];
        } finally {
            setIsFetching(false);
        }
    }, []);

    const selectContractor = useCallback((contractorId) => {
        // Check if contractor ID exists in the list
        if (contractorIds.includes(contractorId)) {
            setSelectedContractorId(contractorId);
            return { id: contractorId };
        }
        setError('Contractor not found');
        return null;
    }, [contractorIds]);

    const reset = useCallback(() => {
        setContractorIds([]);
        setSelectedContractorId(null);
        setBookingId('');
        setError('');
    }, []);

    return {
        contractorIds,      // Array of contractor ID strings
        selectedContractorId,
        isFetching,
        error,
        bookingId,          // Booking ID from API response
        fetch,
        selectContractor,
        reset
    };
};
