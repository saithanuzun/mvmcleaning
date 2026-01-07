import React, {useState} from 'react';
import {useNavigate} from 'react-router-dom';
import api from '../services/api';
import Logo from '../assets/Logo.png'; 

const PostcodePage = ({bookingData, updateBookingData}) => {
    const [postcode, setPostcode] = useState(bookingData.postcode || '');
    const [phone, setPhone] = useState(bookingData.phone || '');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const validatePostcode = (code) => {
        const postcodeRegex = /^[A-Z]{1,2}[0-9][A-Z0-9]? ?[0-9][A-Z]{2}$/i;
        return postcodeRegex.test(code);
    };

    const validatePhone = (phoneNumber) => {
        const phoneRegex = /^[\+]?[0-9\s\-\(\)]{10,}$/;
        return phoneRegex.test(phoneNumber);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        if (!validatePostcode(postcode)) {
            setError('Please enter a valid UK postcode');
            return;
        }

        if (!validatePhone(phone)) {
            setError('Please enter a valid phone number');
            return;
        }

        setLoading(true);
        try {
            const response = await api.postcode.validate(postcode);

            if (response.success && response.data.isValid) {
                if (response.data.isCovered) {
                    updateBookingData({postcode: postcode.toUpperCase(), phone});
                    navigate('/services');
                } else {
                    setError('Sorry, we don\'t currently service this postcode area. Please check back soon!');
                }
            } else {
                setError('Please enter a valid UK postcode');
            }
        } catch (err) {
            setError('Error checking postcode. Please try again.');
            console.error('Postcode validation error:', err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-gradient-to-br from-gray-50 via-white to-blue-50/30 py-8 px-4">
            <div className="max-w-6xl mx-auto">
                <div className="bg-white rounded-2xl shadow-xl overflow-hidden flex flex-col lg:flex-row">

                    <div className="lg:w-1/2">
                        <div
                            className="px-8 pt-8 pb-6 relative overflow-hidden"
                            style={{
                                background: '#194376'
                            }}
                        >
                            
                            <div className="relative z-10">
                                <div className="inline-block px-3 py-1 bg-white/30 rounded-full mb-3">
                                    <span className="text-white text-sm font-semibold">Step 1 of 4</span>
                                </div>
                                <h1 className="text-3xl font-bold text-white mb-2">
                                    Get Started
                                </h1>
                                <p className="text-white/95 text-base">
                                    Enter your details to check service availability in your area
                                </p>
                            </div>
                        </div>

                        <div className="p-8">
                            <form onSubmit={handleSubmit} className="space-y-6">
                                {/* Postcode Input */}
                                <div>
                                    <label className="block text-gray-800 text-sm font-bold mb-2 flex items-center">
                                        <svg className="w-4 h-4 mr-2 text-[#46C6CE]" fill="none" stroke="currentColor"
                                             viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                                                  d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"/>
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                                                  d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"/>
                                        </svg>
                                        Postcode *
                                    </label>
                                    <div className="relative">
                                        <input
                                            type="text"
                                            value={postcode}
                                            onChange={(e) => setPostcode(e.target.value.toUpperCase())}
                                            placeholder="e.g., SW1A 1AA"
                                            className="w-full px-4 py-3.5 border-2 border-gray-300 rounded-xl focus:border-[#46C6CE] focus:ring-2 focus:ring-[#46C6CE]/20 outline-none transition-all duration-200 text-gray-800 font-medium"
                                            required
                                            maxLength="8"
                                        />
                                    </div>
                                    <p className="text-xs text-gray-500 mt-2 ml-1 flex items-center">
                                        <svg className="w-3 h-3 mr-1 text-[#46C6CE]" fill="currentColor" viewBox="0 0 20 20">
                                            <path fillRule="evenodd"
                                                  d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
                                                  clipRule="evenodd"/>
                                        </svg>
                                        Enter your full UK postcode (e.g., SW1A 1AA)
                                    </p>
                                </div>

                                {/* Phone Input */}
                                <div>
                                    <label className="block text-gray-800 text-sm font-bold mb-2 flex items-center">
                                        <svg className="w-4 h-4 mr-2 text-[#46C6CE]" fill="none" stroke="currentColor"
                                             viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                                                  d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"/>
                                        </svg>
                                        Phone Number *
                                    </label>
                                    <div className="relative">
                                        <input
                                            type="tel"
                                            value={phone}
                                            onChange={(e) => setPhone(e.target.value)}
                                            placeholder="e.g., 07123 456789"
                                            className="w-full px-4 py-3.5 border-2 border-gray-300 rounded-xl focus:border-[#46C6CE] focus:ring-2 focus:ring-[#46C6CE]/20 outline-none transition-all duration-200 text-gray-800 font-medium"
                                            required
                                        />
                                    </div>
                                    <p className="text-xs text-gray-500 mt-2 ml-1 flex items-center">
                                        <svg className="w-3 h-3 mr-1 text-[#46C6CE]" fill="currentColor" viewBox="0 0 20 20">
                                            <path fillRule="evenodd"
                                                  d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
                                                  clipRule="evenodd"/>
                                        </svg>
                                        We'll send booking confirmations to this number
                                    </p>
                                </div>

                                {/* Error Message */}
                                {error && (
                                    <div
                                        className="bg-red-50 border-l-4 border-red-500 p-4 rounded-lg animate-in fade-in slide-in-from-top duration-300"
                                    >
                                        <div className="flex items-start">
                                            <svg className="h-5 w-5 text-red-500 mr-3 mt-0.5 flex-shrink-0" fill="none"
                                                 stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                                                      d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                                            </svg>
                                            <p className="text-red-700 font-medium">{error}</p>
                                        </div>
                                    </div>
                                )}

                                {/* Submit Button - Using Your Primary Blue */}
                                <button
                                    type="submit"
                                    disabled={loading}
                                    className={`
                                        w-full text-white font-bold py-4 px-6 rounded-xl transition-all duration-300 ease-in-out 
                                        shadow-lg disabled:opacity-60 disabled:cursor-not-allowed
                                        ${!loading && 'hover:shadow-xl hover:scale-[1.02] active:scale-[0.98]'}
                                    `}
                                    style={{
                                        background: loading
                                            ? '#9ca3af'
                                            : 'linear-gradient(135deg, #194376 0%, #14325e 100%)'
                                    }}
                                >
                                    {loading ? (
                                        <span className="flex items-center justify-center">
                                            <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
                                                 xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                                <circle className="opacity-25" cx="12" cy="12" r="10"
                                                        stroke="currentColor" strokeWidth="4"></circle>
                                                <path className="opacity-75" fill="currentColor"
                                                      d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                            </svg>
                                            Checking Availability...
                                        </span>
                                    ) : (
                                        <span className="flex items-center justify-center">
                                            Check Availability
                                            <svg className="ml-2 w-5 h-5" fill="none" stroke="currentColor"
                                                 viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2.5"
                                                      d="M13 7l5 5m0 0l-5 5m5-5H6"/>
                                            </svg>
                                        </span>
                                    )}
                                </button>

                                <div
                                    className="bg-gradient-to-r from-[#46C6CE]/10 to-blue-50 border-l-4 p-4 rounded-lg"
                                    style={{borderColor: '#46C6CE'}}
                                >
                                    <div className="flex items-start">
                                        <div
                                            className="w-5 h-5 rounded-full flex items-center justify-center mr-3 mt-0.5 flex-shrink-0"
                                            style={{backgroundColor: '#46C6CE'}}
                                        >
                                            <svg className="w-3 h-3 text-white" fill="none" stroke="currentColor"
                                                 viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="3"
                                                      d="M5 13l4 4L19 7"/>
                                            </svg>
                                        </div>
                                        <div>
                                            <p className="text-sm font-semibold text-[#194376] mb-1">
                                                What happens next?
                                            </p>
                                            <p className="text-sm text-gray-700">
                                                We'll instantly check if we service your area and show you our available
                                                cleaning options tailored to your location.
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            </form>
                        </div>
                    </div>

                    {/* Right Side - Primary Blue Color with Logo */}
                    <div
                        className="lg:w-1/2 p-6 lg:p-10 flex flex-col justify-center items-center relative"
                        style={{
                            background: '#194376'
                        }}
                    >
                        {/* Background Pattern */}
                        <div className="absolute inset-0 opacity-5">
                            <div className="absolute top-0 left-0 w-full h-full bg-gradient-to-br from-white/10 to-transparent"></div>
                            <div className="absolute bottom-0 right-0 w-64 h-64 bg-white/5 rounded-full -mr-20 -mb-20"></div>
                        </div>

                        <div className="relative z-10 w-full max-w-lg">
                            {/* Logo Section */}
                            <div className="text-center mb-12">
                                <div className="mb-8">
                                    <img
                                        src={Logo}
                                        alt="Company Logo"
                                        className="w-80 h-80 mx-auto object-contain drop-shadow-2xl"
                                    />
                                </div>

                                <div className="space-y-4">
                                    <h1 className="text-3xl font-bold text-white tracking-tight">
                                        Welcome to Our Shop
                                    </h1>
                                    <div className="w-20 h-1 bg-gradient-to-r from-cyan-400 to-blue-300 mx-auto rounded-full"></div>
                                    <p className="text-lg text-white/90 font-light max-w-md mx-auto">
                                        Your trusted partner for premium services in your area
                                    </p>
                                </div>
                            </div>

                            {/* Trust Badge */}
                            <div className="mt-12 pt-8 border-t border-white/20">
                                <div className="flex items-center justify-center space-x-8 text-white/80">
                                    <div className="text-center">
                                        <div className="text-2xl font-bold text-white mb-1">4.9/5</div>
                                        <div className="text-sm font-medium">Rating</div>
                                    </div>
                                    <div className="h-10 w-px bg-white/30"></div>
                                    <div className="text-center">
                                        <div className="text-2xl font-bold text-white mb-1">2K+</div>
                                        <div className="text-sm font-medium">Clients</div>
                                    </div>
                                    <div className="h-10 w-px bg-white/30"></div>
                                    <div className="text-center">
                                        <div className="text-2xl font-bold text-white mb-1">99%</div>
                                        <div className="text-sm font-medium">Satisfied</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default PostcodePage;