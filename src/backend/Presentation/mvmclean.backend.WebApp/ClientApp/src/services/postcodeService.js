// Postcode Validation Service - Using external API
import axios from 'axios';

const POSTCODE_API_URL = 'https://api.postcodes.io';

const postcodeService = {
    /**
     * Validate postcode using external postcode validation API
     * @param {string} postcode - The postcode to validate
     * @returns {Promise<{valid: boolean, postcode: string, area: string, district: string}>}
     */
    validate: async (postcode) => {
        try {
            const response = await axios.get(
                `${POSTCODE_API_URL}/postcodes/${encodeURIComponent(postcode.trim())}/validate`
            );
            return {
                valid: response.data.result,
                postcode: postcode.trim(),
                message: response.data.result ? 'Valid postcode' : 'Invalid postcode'
            };
        } catch (error) {
            return {
                valid: false,
                postcode: postcode.trim(),
                message: 'Unable to validate postcode',
                error: error.message
            };
        }
    },

    /**
     * Get postcode details (area, district, etc.)
     * @param {string} postcode - The postcode
     * @returns {Promise<object>}
     */
    getDetails: async (postcode) => {
        try {
            const response = await axios.get(
                `${POSTCODE_API_URL}/postcodes/${encodeURIComponent(postcode.trim())}`
            );
            if (response.data.status === 200) {
                return {
                    postcode: response.data.result.postcode,
                    area: response.data.result.admin_district,
                    district: response.data.result.admin_county,
                    region: response.data.result.region,
                    latitude: response.data.result.latitude,
                    longitude: response.data.result.longitude
                };
            }
            return null;
        } catch (error) {
            console.error('Error fetching postcode details:', error);
            return null;
        }
    }
};

export default postcodeService;
