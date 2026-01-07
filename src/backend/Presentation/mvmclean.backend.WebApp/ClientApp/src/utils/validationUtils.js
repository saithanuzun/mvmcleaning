// Validation utilities for booking form
export const validationRules = {
    /**
     * Validate UK postcode format
     */
    isValidPostcode: (postcode) => {
        const postcodeRegex = /^[A-Z]{1,2}[0-9]{1,2}[A-Z]{0,1} ?[0-9][A-Z]{2}$/i;
        return postcodeRegex.test(postcode.trim());
    },

    /**
     * Validate telephone number (various formats)
     */
    isValidTelephone: (telephone) => {
        // Accepts various formats: +44, 020, 0123, etc. (10-15 digits)
        const telephoneRegex = /^[\+]?[0-9\s\-\(\)]{10,}$/;
        return telephoneRegex.test(telephone.trim());
    },

    /**
     * Validate phone number (mobile)
     */
    isValidPhoneNumber: (phoneNumber) => {
        // Similar to telephone but specifically for mobile
        const phoneRegex = /^[\+]?[0-9\s\-\(\)]{10,}$/;
        return phoneRegex.test(phoneNumber.trim());
    },

    /**
     * Validate email address
     */
    isValidEmail: (email) => {
        if (!email) return true; // Email is optional
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email.trim());
    },

    /**
     * Validate date is in future
     */
    isValidFutureDate: (dateTime) => {
        const selectedDate = new Date(dateTime);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        return selectedDate > today;
    }
};

/**
 * Validate entire booking form
 */
export const validateBookingForm = (formData) => {
    const errors = {};

    if (!formData.telephone?.trim()) {
        errors.telephone = 'Telephone number is required';
    } else if (!validationRules.isValidTelephone(formData.telephone)) {
        errors.telephone = 'Please enter a valid telephone number';
    }

    if (!formData.phoneNumber?.trim()) {
        errors.phoneNumber = 'Phone number is required';
    } else if (!validationRules.isValidPhoneNumber(formData.phoneNumber)) {
        errors.phoneNumber = 'Please enter a valid phone number';
    }

    if (formData.email && !validationRules.isValidEmail(formData.email)) {
        errors.email = 'Please enter a valid email address';
    }

    if (!formData.dateTime) {
        errors.dateTime = 'Booking date and time is required';
    } else if (!validationRules.isValidFutureDate(formData.dateTime)) {
        errors.dateTime = 'Booking date must be in the future';
    }

    if (!formData.services || formData.services.length === 0) {
        errors.services = 'At least one service must be selected';
    }

    return {
        isValid: Object.keys(errors).length === 0,
        errors
    };
};
