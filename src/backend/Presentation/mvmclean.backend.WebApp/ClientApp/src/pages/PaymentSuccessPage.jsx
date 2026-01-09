// src/pages/PaymentSuccessPage.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const PaymentSuccessPage = () => {
    const navigate = useNavigate();
    const [bookingDetails, setBookingDetails] = useState(null);

    useEffect(() => {
        // Get booking details from localStorage
        const bookingId = localStorage.getItem('pending_booking_id');
        const bookingData = localStorage.getItem('booking_data');
        
        if (bookingId && bookingData) {
            setBookingDetails(JSON.parse(bookingData));
            
            // Clear booking session after 5 seconds
            const timer = setTimeout(() => {
                localStorage.removeItem('pending_booking_id');
                localStorage.removeItem('booking_session_id');
                localStorage.removeItem('booking_data');
            }, 5000);
            
            return () => clearTimeout(timer);
        }
    }, []);

    useEffect(() => {
        if (!bookingDetails) return;

        if (window.__purchaseTracked) return;
        window.__purchaseTracked = true;

        if (window.gtag) {
            window.gtag('event', 'conversion', {
                send_to: 'AW-17424866501/uEZiCM-QzP0aEMW56fRA',
                value: bookingDetails.totalAmount, 
                currency: 'GBP',
                transaction_id: bookingDetails.bookingId,
            });
        }
    }, [bookingDetails]);


    const handleBookAnother = () => {
        // Reset localStorage
        localStorage.removeItem('pending_booking_id');
        localStorage.removeItem('booking_session_id');
        localStorage.removeItem('booking_data');
        
        // Navigate to shop
        window.location.href = '/shop';
    };

    const handleViewDetails = () => {
        if (bookingDetails?.bookingId) {
            // Navigate to MVC booking details page
            window.location.href = `/booking/find`;
        }
    };


    return (
        <div className="min-h-screen bg-gradient-to-br from-gray-50 via-white to-blue-50/30 py-12 px-4">
            <div className="max-w-3xl mx-auto">
                {/* Success Animation */}
                <div className="text-center mb-8">
                    <div className="inline-flex items-center justify-center w-20 h-20 rounded-full mb-6 animate-in zoom-in" style={{ backgroundColor: '#46C6CE' }}>
                        <svg className="w-10 h-10 text-white" fill="currentColor" viewBox="0 0 20 20">
                            <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                        </svg>
                    </div>
                    <h1 className="text-4xl font-bold mb-2" style={{ color: '#194376' }}>
                        Booking Confirmed!
                    </h1>
                    <p className="text-gray-600 text-lg">
                        Your booking has been confirmed successfully
                    </p>
                </div>

                {/* Booking Details Card */}
                <div className="bg-white rounded-2xl shadow-xl p-8 mb-6">
                    <div className="border-b-2 border-gray-100 pb-6 mb-6">
                        <h2 className="text-2xl font-bold text-gray-800 mb-2">Booking Details</h2>
                        <p className="text-gray-600">
                            A confirmation email has been sent to your email address
                        </p>
                    </div>

                    {bookingDetails && (
                        <div className="space-y-6">
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                                <div className="bg-gray-50 p-4 rounded-xl">
                                    <p className="text-xs text-gray-500 mb-1 font-semibold uppercase tracking-wide">
                                        Booking Reference
                                    </p>
                                    <p className="font-bold text-xl" style={{ color: '#194376' }}>
                                        {bookingDetails.bookingId?.substring(0, 8).toUpperCase() || 'N/A'}
                                    </p>
                                </div>

                                <div className="bg-gray-50 p-4 rounded-xl">
                                    <p className="text-xs text-gray-500 mb-1 font-semibold uppercase tracking-wide">
                                        Status
                                    </p>
                                    <div className="flex items-center">
                                        <div className="w-2 h-2 rounded-full bg-green-500 mr-2"></div>
                                        <p className="font-bold text-lg text-green-600">
                                            {bookingDetails.status || 'Confirmed'}
                                        </p>
                                    </div>
                                </div>
                            </div>

                            <div className="bg-gradient-to-r from-blue-50 to-cyan-50 p-6 rounded-xl border-2" style={{ borderColor: '#46C6CE40' }}>
                                <h3 className="font-bold text-lg text-gray-800 mb-4">What happens next?</h3>
                                <ul className="space-y-3">
                                    <li className="flex items-start">
                                        <div className="w-6 h-6 rounded-full flex items-center justify-center mr-3 flex-shrink-0 mt-0.5" style={{ backgroundColor: '#46C6CE' }}>
                                            <span className="text-white text-xs font-bold">1</span>
                                        </div>
                                        <p className="text-gray-700">
                                            You will receive a confirmation email with all booking details
                                        </p>
                                    </li>
                                    <li className="flex items-start">
                                        <div className="w-6 h-6 rounded-full flex items-center justify-center mr-3 flex-shrink-0 mt-0.5" style={{ backgroundColor: '#46C6CE' }}>
                                            <span className="text-white text-xs font-bold">2</span>
                                        </div>
                                        <p className="text-gray-700">
                                            Your assigned contractor will contact you before the appointment
                                        </p>
                                    </li>
                                    <li className="flex items-start">
                                        <div className="w-6 h-6 rounded-full flex items-center justify-center mr-3 flex-shrink-0 mt-0.5" style={{ backgroundColor: '#46C6CE' }}>
                                            <span className="text-white text-xs font-bold">3</span>
                                        </div>
                                        <p className="text-gray-700">
                                            We'll send you a reminder 24 hours before your scheduled service
                                        </p>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    )}
                </div>

                {/* Action Buttons */}
                <div className="flex flex-col sm:flex-row gap-4 justify-center">
                    <button
                        onClick={handleViewDetails}
                        className="px-8 py-4 bg-white border-2 text-gray-700 font-bold rounded-xl hover:bg-gray-50 transition-all flex items-center justify-center gap-2"
                        style={{ borderColor: '#194376', color: '#194376' }}
                    >
                        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                        View Booking Details
                    </button>
                    <button
                        onClick={handleBookAnother}
                        className="px-8 py-4 bg-[#194376] text-white font-bold rounded-xl hover:shadow-xl transition-all transform hover:scale-105 flex items-center justify-center gap-2"
                    >
                        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                        </svg>
                        Book Another Service
                    </button>
                </div>

                {/* Support Info */}
                <div className="mt-8 text-center">
                    <p className="text-gray-600 text-sm">
                        Need help? Contact us at{' '}
                        <a href="mailto:support@mvmclean.com" className="font-semibold" style={{ color: '#194376' }}>
                            support@mvmclean.com
                        </a>
                    </p>
                </div>
            </div>
        </div>
    );
};

export default PaymentSuccessPage;
