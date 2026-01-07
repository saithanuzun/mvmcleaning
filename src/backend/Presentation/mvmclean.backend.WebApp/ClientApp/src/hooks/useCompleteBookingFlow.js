// Booking Flow State Management - combines all booking steps
import { useState, useCallback } from 'react';
import { usePostcodeValidation, useContractorsByPostcode } from './useBookingFlow';
import { useBookingCreation } from './useBookingCreation';

export const useCompleteBookingFlow = () => {
    // Step 1: Postcode Validation & Contractor Selection
    const postcode = usePostcodeValidation();
    const contractors = useContractorsByPostcode();
    
    // Step 2: Booking Creation
    const booking = useBookingCreation();

    // Booking form data
    const [bookingFormData, setBookingFormData] = useState({
        telephone: '',
        phoneNumber: '',
        email: '',
        services: [],
        dateTime: '',
        notes: ''
    });

    // Current step in the flow
    const [currentStep, setCurrentStep] = useState(1); // 1: Postcode, 2: Contractors, 3: Services, 4: Booking Details, 5: Confirmation

    /**
     * Step 1: Validate postcode and get contractors
     */
    const proceedWithPostcode = useCallback(async (postcodeValue) => {
        postcode.setPostcode(postcodeValue);
        const isValid = await postcode.validate(postcodeValue);
        
        if (isValid) {
            const contractorsList = await contractors.fetch(postcodeValue);
            if (contractorsList.length > 0) {
                setCurrentStep(2);
                return true;
            } else {
                postcode.setError('No contractors available in this postcode area');
                return false;
            }
        }
        return false;
    }, [postcode, contractors]);

    /**
     * Step 2: Select contractor and proceed
     */
    const selectContractorAndProceed = useCallback((contractorId) => {
        const selected = contractors.selectContractor(contractorId);
        if (selected) {
            setCurrentStep(3);
            return true;
        }
        return false;
    }, [contractors]);

    /**
     * Step 3: Update booking form data
     */
    const updateBookingData = useCallback((field, value) => {
        setBookingFormData(prev => ({
            ...prev,
            [field]: value
        }));
    }, []);

    const updateServices = useCallback((services) => {
        setBookingFormData(prev => ({
            ...prev,
            services
        }));
    }, []);

    /**
     * Step 4: Create the booking
     */
    const submitBooking = useCallback(async () => {
        // API only requires phoneNumber and postcode
        const bookingPayload = {
            phoneNumber: bookingFormData.phoneNumber,
            postcode: postcode.postcode
            // Additional data can be stored in React state for later use
            // contractorId: contractors.selectedContractorId,
            // dateTime: bookingFormData.dateTime,
            // services: bookingFormData.services,
        };

        const result = await booking.create(bookingPayload);
        
        if (result) {
            setCurrentStep(5); // Success confirmation page
            return true;
        }
        return false;
    }, [bookingFormData, postcode.postcode, booking]);

    /**
     * Reset entire flow
     */
    const resetFlow = useCallback(() => {
        postcode.reset();
        contractors.reset();
        booking.reset();
        setBookingFormData({
            telephone: '',
            phoneNumber: '',
            email: '',
            services: [],
            dateTime: '',
            notes: ''
        });
        setCurrentStep(1);
    }, [postcode, contractors, booking]);

    /**
     * Go to previous step
     */
    const previousStep = useCallback(() => {
        setCurrentStep(prev => Math.max(1, prev - 1));
    }, []);

    return {
        // Current state
        currentStep,
        
        // Postcode validation state
        postcode: {
            value: postcode.postcode,
            isValidating: postcode.isValidating,
            isValid: postcode.isValid,
            details: postcode.postcodeDetails,
            error: postcode.error
        },

        // Contractors state
        contractors: {
            ids: contractors.contractorIds,      // Array of contractor ID strings
            selectedId: contractors.selectedContractorId,
            isFetching: contractors.isFetching,
            error: contractors.error,
            bookingId: contractors.bookingId     // Booking ID from contractor fetch
        },

        // Booking form state
        bookingForm: bookingFormData,

        // Booking creation state
        booking: {
            isCreating: booking.isCreating,
            error: booking.error,
            result: booking.bookingResult
        },

        // Actions
        actions: {
            proceedWithPostcode,
            selectContractorAndProceed,
            updateBookingData,
            updateServices,
            submitBooking,
            previousStep,
            resetFlow
        }
    };
};
